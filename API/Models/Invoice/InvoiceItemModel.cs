using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace API.Models.Invoice
{
    public class InvoiceItemModel
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [Required]
        [JsonProperty("invoiceId")]
        public Guid InvoiceId { get; set; }

        public InvoiceModel Invoice { get; set; } = null!;

        // Item details at purchase
        [Required]
        [JsonProperty("itemName")]
        public required string ItemName { get; set; }

        [JsonProperty("itemQuantity")]
        public int? ItemQuantity { get; set; }

        [JsonProperty("usesConsumed")]
        public int? UsesConsumed { get; set; }

        [Required]
        [JsonProperty("itemPrice")]
        public decimal ItemPrice { get; set; }

        [JsonProperty("discountedPrice")]
        public decimal? DiscountedPrice { get; set; }
    }
}
