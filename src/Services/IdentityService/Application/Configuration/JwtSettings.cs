namespace Whatsapp.Flow.Services.Identity.Application.Configuration
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        public string Key { get; init; }
        public string Issuer { get; init; }
        public string Audience { get; init; }
        public double DurationInMinutes { get; init; }
    }
} 