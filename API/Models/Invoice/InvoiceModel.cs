using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace API.Models.Invoice
{
    public class InvoiceModel
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [Required]
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("user")]
        public UserModel User { get; set; } = null!;

        [Required]
        [JsonProperty("invoiceDate")]
        public required string InvoiceDate { get; set; }

        [Required]
        [JsonProperty("amountTendered")]
        public decimal AmountTendered { get; set; }

        [Required]
        [JsonProperty("totalPrice")]
        public decimal TotalPrice { get; set; }

        [JsonProperty("totalDiscountedPrice")]
        public decimal? TotalDiscountedPrice { get; set; }

        [JsonProperty("isVoided")]
        public bool IsVoided { get; set; }

        [JsonProperty("voidReason")]
        public string? VoidReason { get; set; }

        public ICollection<InvoiceItemModel> InvoiceItems { get; set; } = [];
    }
}
