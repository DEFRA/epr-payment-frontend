using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Options
{
    [ExcludeFromCodeCoverage]
    public class RedisOptions
    {
        public const string ConfigSection = "Redis";

        public required string ConnectionString { get; set; }

        public required string InstanceName { get; set; }
    }
}