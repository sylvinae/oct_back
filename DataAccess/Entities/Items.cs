using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? Barcode { get; set; }
        public string? Brand { get; set; }
        public string? Generic { get; set; }
        public string? Classification { get; set; }
        public string? Formulation { get; set; }
        public string? Location { get; set; }

        [DefaultValue(0.0)]
        public decimal Wholesale { get; set; }

        [DefaultValue(0.0)]
        public decimal Retail { get; set; }

        [DefaultValue(0)]
        public int Stock { get; set; }

        [DefaultValue(0)]
        public int LowThreshold { get; set; }
        public string? Company { get; set; }

        [DefaultValue(false)]
        public bool HasExpiry { get; set; }
        public string? Expiry { get; set; }

        [Required]
        [DefaultValue(false)]
        public required int IsReagent { get; set; }
        public int? UsesLeft { get; set; }
        public int? UsesMax { get; set; }

        [DefaultValue(false)]
        public required bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Missing hash.")]
        public required string Hash { get; set; }
    }
}
