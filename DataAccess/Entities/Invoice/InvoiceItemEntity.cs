namespace DataAccess.Entities.Invoice
{
    public class InvoiceItemEntity
    {
        public Guid Id { get; set; }

        public Guid InvoiceId { get; set; }

        public InvoiceEntity Invoice { get; set; } = null!;
        public required string ItemName { get; set; }

        public int? ItemQuantity { get; set; }
        public int? UsesConsumed { get; set; }

        public decimal ItemPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
    }
}
