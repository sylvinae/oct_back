using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace API.Models.Expense
{
    public class ExpenseModel
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [Required]
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        public UserModel User { get; set; } = null!;

        [Required]
        [JsonProperty("totalCost")]
        public required decimal TotalCost { get; set; }

        [Required]
        [JsonProperty("expenseDate")]
        public required string ExpenseDate { get; set; }

        public ICollection<ExpenseItemModel> ExpenseItems { get; set; } = [];
    }
}
