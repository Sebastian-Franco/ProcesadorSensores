using System.Text.Json.Serialization;

namespace ProcesadorStreamSensores.Infrastructure.Json
{
    public class SensorInputDto
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("zone")]
        public string Zone { get; set; } = string.Empty;

        // En el JSON viene como string
        [JsonPropertyName("value")]
        public string ValueRaw { get; set; } = string.Empty;
    }
}
