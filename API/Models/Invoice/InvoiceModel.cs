using Newtonsoft.Json;

namespace API.Models.Invoice
{
    public class InvoiceModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("user")]
        public UserModel User { get; set; } = null!;

        [JsonProperty("invoiceDate")]
        public required string InvoiceDate { get; set; }

        [JsonProperty("amountTendered")]
        public required decimal AmountTendered { get; set; }

        [JsonProperty("totalPrice")]
        public required decimal TotalPrice { get; set; }

        [JsonProperty("totalDiscountedPrice")]
        public decimal? TotalDiscountedPrice { get; set; }

        [JsonProperty("isVoided")]
        public bool IsVoided { get; set; }

        [JsonProperty("voidReason")]
        public string? VoidReason { get; set; }

        public ICollection<InvoiceItemModel> InvoiceItems { get; set; } = [];
    }
}
