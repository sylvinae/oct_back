using API.Utils;
using Newtonsoft.Json;

namespace API.DTOs
{
    public class ItemDto
    {
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
        [JsonIgnoreIfEmpty]
        public bool IsReagent { get; set; }

        [JsonProperty("usesLeft")]
        [JsonIgnoreIfEmpty]
        public int? UsesLeft { get; set; }

        [JsonProperty("usesMax")]
        [JsonIgnoreIfEmpty]
        public int? UsesMax { get; set; }
    }

    public class CreateItemDto : ItemDto { }

    public class UpdateItemDto : ItemDto
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }

    public class ItemResponseDto : ItemDto
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}
