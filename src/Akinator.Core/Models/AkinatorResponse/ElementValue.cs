using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class ElementValue
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id_base")]
        public string IdBase { get; set; }

        [JsonProperty("proba")]
        public string Proba { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("valide_contrainte")]
        public string ValideContrainte { get; set; }

        [JsonProperty("ranking")]
        public string Ranking { get; set; }

        [JsonProperty("pseudo")]
        public string Pseudo { get; set; }

        [JsonProperty("picture_path")]
        public string PicturePath { get; set; }

        [JsonProperty("corrupt")]
        public string Corrupt { get; set; }

        [JsonProperty("relative")]
        public string Relative { get; set; }

        [JsonProperty("award_id")]
        public string AwardId { get; set; }

        [JsonProperty("flag_photo")]
        public int FlagPhoto { get; set; }

        [JsonProperty("absolute_picture_path")]
        public string AbsolutePicturePath { get; set; }
    }
}
