namespace DataAccess.Entities.Expense
{
    public class ExpenseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public required decimal TotalCost { get; set; }
        public required string ExpenseDate { get; set; }
        public ICollection<ExpenseItemEntity> ExpenseItems { get; set; } = [];
    }
}
