using API.Models;
using API.Models.Expense;
using API.Models.Invoice;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<InvoiceModel> Invoices { get; set; }
        public DbSet<InvoiceItemModel> InvoiceItems { get; set; }
        public DbSet<ExpenseModel> Expenses { get; set; }
        public DbSet<ExpenseItemModel> ExpenseItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=../api/app.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
                entity.Property(u => u.FirstName).IsRequired();
                entity.Property(u => u.LastName).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.Role).IsRequired();
            });

            modelBuilder.Entity<InvoiceModel>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.UserId).IsRequired();
                entity.Property(i => i.InvoiceDate).IsRequired();
                entity.Property(i => i.AmountTendered).IsRequired();
                entity.Property(i => i.TotalPrice).IsRequired();

                entity.Property(i => i.TotalDiscountedPrice);
                entity.Property(i => i.IsVoided);
                entity.Property(i => i.VoidReason);

                entity.HasOne(i => i.User).WithMany(u => u.Invoices).HasForeignKey(i => i.UserId);

                entity
                    .HasMany(i => i.InvoiceItems)
                    .WithOne(ii => ii.Invoice)
                    .HasForeignKey(ii => ii.InvoiceId);
            });

            modelBuilder.Entity<InvoiceItemModel>(entity =>
            {
                entity.HasKey(ii => ii.Id);

                entity.Property(ii => ii.InvoiceId).IsRequired();
                entity.Property(ii => ii.ItemName).IsRequired();
                entity.Property(ii => ii.ItemPrice).IsRequired();

                entity.Property(ii => ii.ItemQuantity);
                entity.Property(ii => ii.UsesConsumed);
                entity.Property(ii => ii.DiscountedPrice);

                entity
                    .HasOne(ii => ii.Invoice)
                    .WithMany(i => i.InvoiceItems)
                    .HasForeignKey(ii => ii.InvoiceId);
            });

            modelBuilder.Entity<ExpenseModel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.TotalCost).IsRequired();
                entity.Property(e => e.ExpenseDate).IsRequired();

                entity.HasOne(e => e.User).WithMany(u => u.Expenses).HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.ExpenseItems).WithOne().HasForeignKey(ei => ei.ExpenseId);
            });

            modelBuilder.Entity<ExpenseItemModel>(entity =>
            {
                entity.HasKey(ei => ei.Id);

                entity.Property(ei => ei.ExpenseId).IsRequired();
                entity.Property(ei => ei.Details).IsRequired();
                entity.Property(ei => ei.Amount).IsRequired();

                entity
                    .HasOne(ei => ei.Expense)
                    .WithMany(e => e.ExpenseItems)
                    .HasForeignKey(ei => ei.ExpenseId);
            });
        }
    }
}
