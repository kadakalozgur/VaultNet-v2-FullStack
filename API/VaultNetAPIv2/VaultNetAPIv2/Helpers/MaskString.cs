namespace VaultNetAPIv2.Helpers
{
    public static class MaskString
    {
        public static string MaskEmail(this string email)
        {

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                return "*****";
            }
                
            var parts = email.Split('@');
            var localPart = parts[0];
            var domainPart = parts[1];

            if (localPart.Length <= 2)
            {
                return $"***@{domainPart}";
            }

            var maskedLocal = $"{localPart[0]}***{localPart[^1]}";

            return $"{maskedLocal}@{domainPart}";

        }
    }
}
