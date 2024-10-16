using API.Models.Expense;
using API.Models.Invoice;
using API.Utils;
using Newtonsoft.Json;

namespace API.Models
{
    public class UserModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("firstName")]
        public required string FirstName { get; set; }

        [JsonProperty("middleName")]
        public string? MiddleName { get; set; }

        [JsonProperty("lastName")]
        public required string LastName { get; set; }

        [JsonProperty("email")]
        public required string Email { get; set; }

        [JsonProperty("password")]
        [JsonIgnoreIfEmpty]
        public required string Password { get; set; }

        [JsonProperty("role")]
        public required string Role { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnoreIfEmpty]
        public ICollection<InvoiceModel> Invoices { get; set; } = [];

        [JsonIgnoreIfEmpty]
        public ICollection<ExpenseModel> Expenses { get; set; } = [];
    }
}
