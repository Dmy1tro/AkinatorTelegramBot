using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class RegionUrlResponse
    {
        [JsonProperty("translated_theme_name")]
        public string TranslatedThemeName { get; set; }

        [JsonProperty("urlWs")]
        public string UrlWs { get; set; }

        [JsonProperty("subject_id")]
        public string SubjectId { get; set; }
    }
}
