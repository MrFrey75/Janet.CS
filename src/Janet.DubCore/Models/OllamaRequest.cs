using System.Text.Json.Serialization;


namespace Janet.DubCore.Models;

    public class OllamaRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("system")]
        public string System { get; set; }

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }

        [JsonPropertyName("format")]
        public string Format { get; set; }

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
    }