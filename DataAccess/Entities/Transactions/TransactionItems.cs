using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.Transactions
{
    public class TransactionItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Transaction")]
        public int TransactionId { get; set; }

        public Transaction Transaction { get; set; } = null!;

        // Item details at purchase
        [Required]
        public required string ItemName { get; set; }

        public int? ItemQuantity { get; set; }
        public int? UsesConsumed { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal ItemPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountedPrice { get; set; }
    }
}
