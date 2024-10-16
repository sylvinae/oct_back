using API.Utils;
using Newtonsoft.Json;

namespace API.DTOs
{
    public class RestockDto
    {
        [JsonProperty("barcode")]
        //[JsonIgnoreIfEmpty]
        public string? Barcode { get; set; }

        [JsonProperty("brand")]
        //[JsonIgnoreIfEmpty]
        public string? Brand { get; set; }

        [JsonProperty("generic")]
        //[JsonIgnoreIfEmpty]
        public string? Generic { get; set; }

        [JsonProperty("classification")]
        //[JsonIgnoreIfEmpty]
        public string? Classification { get; set; }

        [JsonProperty("formulation")]
        //[JsonIgnoreIfEmpty]
        public string? Formulation { get; set; }

        [JsonProperty("location")]
        //[JsonIgnoreIfEmpty]
        public string? Location { get; set; }

        [JsonProperty("wholesale")]
        public decimal Wholesale { get; set; }

        [JsonProperty("retail")]
        public decimal Retail { get; set; }

        [JsonProperty("stock")]
        public int Stock { get; set; }

        [JsonProperty("lowThreshold")]
        public int LowThreshold { get; set; }

        [JsonProperty("company")]
        //[JsonIgnoreIfEmpty]
        public string? Company { get; set; }

        [JsonProperty("hasExpiry")]
        public bool HasExpiry { get; set; }

        [JsonProperty("expiry")]
        //[JsonIgnoreIfEmpty]
        public string? Expiry { get; set; }

        [JsonProperty("isReagent")]
        //[JsonIgnoreIfEmpty]
        public bool IsReagent { get; set; }

        [JsonProperty("usesLeft")]
        //[JsonIgnoreIfEmpty]
        public int? UsesLeft { get; set; }

        [JsonProperty("usesMax")]
        //[JsonIgnoreIfEmpty]
        public int? UsesMax { get; set; }
    }
}
