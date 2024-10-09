namespace DataAccess.Entities.Invoice
{
    public class InvoiceEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;

        public required string InvoiceDate { get; set; }

        public decimal AmountTendered { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? TotalDiscountedPrice { get; set; }

        public bool IsVoided { get; set; }
        public string? VoidReason { get; set; }

        public ICollection<InvoiceItemEntity> InvoiceItems { get; set; } = [];
    }
}
