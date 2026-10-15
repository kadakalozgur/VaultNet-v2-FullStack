using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using VaultNetAPIv2.Data;
using VaultNetAPIv2.Services;
using VaultNetAPIv2.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<AppDbContext>()
    .SetApplicationName("VaultNetAPI");

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IStorageService, SupabaseStorageService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddSingleton<IEncryptionService, DataProtectionEncryptionService>();
builder.Services.AddSingleton<IPasswordHashService, BCryptPasswordHashService>();
builder.Services.AddScoped<IVaultItemService, VaultItemService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Supabase Konfigürasyonu
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];
var options = new Supabase.SupabaseOptions { AutoConnectRealtime = false };
builder.Services.AddSingleton(new Supabase.Client(supabaseUrl, supabaseKey, options));

// JWT için bölüm. Hazýr olarak alýndý.
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
    };

    options.Events = new JwtBearerEvents
    {

        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("accessToken"))
            {
                context.Token = context.Request.Cookies["accessToken"];
            }
            return Task.CompletedTask;
        },

        OnTokenValidated = async context =>
        {

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

            var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier);
            var securityStampClaim = context.Principal?.FindFirst("SecurityStamp");

            if (userIdClaim == null || securityStampClaim == null)
            {
                context.Fail("Unauthorized: Security claims are missing.");
                return;
            }

            if (Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                var user = await dbContext.Users.FindAsync(userId);

                if (user == null || user.SecurityStamp.ToString() != securityStampClaim.Value)
                {
                    context.Fail("Unauthorized: Security stamp mismatch. Token revoked.");
                }
            }
        }

    };
});

// Rate Limiter
builder.Services.AddRateLimiter(options =>
{

    // Veritabanýnda ayný anda iþlenecek iþlem sayýsýný sýnýrladým. (100 adet, kuyruk limiti 50 adet)
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Return þeklinin kendi yazdýðým ServiceResult þeklinde dönmesi için kýsým
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var rateLimitError = new
        {
            isSuccess = false,
            statusCode = 429,
            message = "Too many requests. Please wait a moment and try again."
        };

        await context.HttpContext.Response.WriteAsJsonAsync(rateLimitError, cancellationToken: token);

    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetConcurrencyLimiter(
            partitionKey: "global",
            factory: partition => new ConcurrencyLimiterOptions
            {
                PermitLimit = 100, 
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 50    
            }));

    // Login-Register için limit. IP adresi üzerinden bakýlýyor.
    options.AddPolicy("AuthLimit", httpContext =>
    {

        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.TraceIdentifier;

        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: clientIp,
            factory: partition => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 3,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });

    });

    // Profil iþlemleri için limit. UserId üzerinden bakýlýyor.
    options.AddPolicy("UserLimit", httpContext =>
    {

        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? httpContext.Connection.RemoteIpAddress?.ToString()
                     ?? "anonymous";

        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: userId, 
            factory: partition => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 5,               
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 4,         
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0                 
            });

    });

    options.AddPolicy("ReadLimit", httpContext =>
    {

        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? httpContext.Connection.RemoteIpAddress?.ToString()
                     ?? "anonymous";

        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: userId,
            factory: partition => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 60,             
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 4,         
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });

    });

    options.AddPolicy("WriteLimit", httpContext =>
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? httpContext.Connection.RemoteIpAddress?.ToString()
                     ?? "anonymous";

        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: userId,
            factory: partition => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 4,
                QueueLimit = 0
            });

    });

});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseExceptionHandler("/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Canlýda Hsts kullanmak için.
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.ToString());
        diagnosticContext.Set("UserId", httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    };
});

app.Use(async (context, next) =>
{
    using (Serilog.Context.LogContext.PushProperty("ClientIp", context.Connection.RemoteIpAddress?.ToString()))
    using (Serilog.Context.LogContext.PushProperty("TraceId", context.TraceIdentifier))
    {
        await next();
    }
});

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
