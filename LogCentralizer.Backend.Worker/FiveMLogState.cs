using MassTransit;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LogCentralizer.Backend.Worker
{
    public class FiveMLogState : SagaStateMachineInstance
    {

        [BsonId]
        public Guid CorrelationId { get; set; }

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
