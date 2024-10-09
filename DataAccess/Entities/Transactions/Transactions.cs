using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.Transactions
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        public required string TransactionDate { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal AmountTendered { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalDiscountedPrice { get; set; }

        [DefaultValue(false)]
        public bool IsVoided { get; set; }

        public string? VoidReason { get; set; }
    }
}
