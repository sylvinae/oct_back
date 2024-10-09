using DataAccess.Entities.Expense;
using DataAccess.Entities.Invoice;

namespace DataAccess.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;

        public ICollection<InvoiceEntity> Invoices { get; set; } = [];
        public ICollection<ExpenseEntity> Expenses { get; set; } = [];
    }
}
