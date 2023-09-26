using System.Text.Json.Serialization;

namespace LogCentralizer.Backend.Worker
{
    public record FiveMLogs
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("room")]
        public string? Room { get; set; }

        [JsonPropertyName("user_id")]
        public object UserId { get; set; }

        [JsonPropertyName("target_id")]
        public object TargetId { get; set; }

        [JsonPropertyName("time")]
        public long Time { get; set; }
    }
}