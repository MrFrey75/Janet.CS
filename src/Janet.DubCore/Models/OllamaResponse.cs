using System.Text.Json.Serialization;

namespace Janet.DubCore.Services;

    public class OllamaResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; }

        [JsonPropertyName("done")]
        public bool Done { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }