namespace VaultNetAPIv2.Results
{
    public class ServiceResult
    {

        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public object? Data { get; set; }

    }
}
