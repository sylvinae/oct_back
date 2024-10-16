using Newtonsoft.Json;

namespace API.Models
{
    public class ItemModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("barcode")]
        public string? Barcode { get; set; }

        [JsonProperty("brand")]
        public string? Brand { get; set; }

        [JsonProperty("generic")]
        public string? Generic { get; set; }

        [JsonProperty("classification")]
        public string? Classification { get; set; }

        [JsonProperty("formulation")]
        public string? Formulation { get; set; }

        [JsonProperty("location")]
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
        public string? Company { get; set; }

        [JsonProperty("hasExpiry")]
        public bool HasExpiry { get; set; }

        [JsonProperty("expiry")]
        public string? Expiry { get; set; }

        [JsonProperty("isReagent")]
        public bool IsReagent { get; set; }

        [JsonProperty("usesLeft")]
        public int? UsesLeft { get; set; }

        [JsonProperty("usesMax")]
        public int? UsesMax { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public string? Hash { get; set; }
        public bool IsLow { get; set; }
        public bool IsExpired { get; set; }
    }
}
