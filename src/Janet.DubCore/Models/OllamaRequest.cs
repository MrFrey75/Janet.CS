using System.Text.Json.Serialization;


namespace Janet.DubCore.Models;

    public class OllamaRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "phi3";

        [JsonPropertyName("system")]
        public string System { get; set; } = string.Empty;

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonPropertyName("format")]
        public string Format { get; set; } = string.Empty;

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;
    }