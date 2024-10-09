using Newtonsoft.Json;

namespace API.Models.Expense
{
    public class ExpenseItemModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("expenseId")]
        public Guid ExpenseId { get; set; }

        public ExpenseModel Expense { get; set; } = null!;

        [JsonProperty("details")]
        public required string Details { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
