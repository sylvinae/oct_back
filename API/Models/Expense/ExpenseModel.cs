using Newtonsoft.Json;

namespace API.Models.Expense
{
    public class ExpenseModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        public UserModel User { get; set; } = null!;

        [JsonProperty("totalCost")]
        public required decimal TotalCost { get; set; }

        [JsonProperty("expenseDate")]
        public required string ExpenseDate { get; set; }

        public ICollection<ExpenseItemModel> ExpenseItems { get; set; } = [];
    }
}
