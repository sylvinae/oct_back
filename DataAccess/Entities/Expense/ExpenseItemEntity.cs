namespace DataAccess.Entities.Expense
{
    public class ExpenseItemEntity
    {
        public Guid Id { get; set; }

        public Guid ExpenseId { get; set; }
        public ExpenseEntity Expense { get; set; } = null!;

        public required string Details { get; set; }

        public decimal Amount { get; set; }
    }
}
