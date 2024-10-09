using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace API.Models.Expense
{
    public class ExpenseItemModel
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [Required]
        [JsonProperty("expenseId")]
        public Guid ExpenseId { get; set; }

        public ExpenseModel Expense { get; set; } = null!;

        [Required]
        [JsonProperty("details")]
        public required string Details { get; set; }

        [Required]
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
