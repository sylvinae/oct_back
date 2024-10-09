using DataAccess.Entities;
using DataAccess.Entities.Expense;
using DataAccess.Entities.Invoice;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public class DataDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<InvoiceEntity> Invoices { get; set; }
        public DbSet<InvoiceItemEntity> InvoiceItems { get; set; }
        public DbSet<ExpenseEntity> Expenses { get; set; }
        public DbSet<ExpenseItemEntity> ExpenseItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=../api/app.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User configurations
            modelBuilder.Entity<UserEntity>(user =>
            {
                user.HasMany(u => u.Invoices).WithOne(i => i.User).HasForeignKey(i => i.UserId);

                user.HasMany(u => u.Expenses).WithOne(e => e.User).HasForeignKey(e => e.UserId);

                // GUID configurations
                user.Property(u => u.Id).ValueGeneratedOnAdd();
            });

            // Invoice configurations
            modelBuilder.Entity<InvoiceEntity>(invoice =>
            {
                invoice
                    .HasMany(i => i.InvoiceItems)
                    .WithOne(ii => ii.Invoice)
                    .HasForeignKey(ii => ii.InvoiceId);

                invoice.Property(i => i.InvoiceDate).IsRequired();
                invoice.Property(i => i.AmountTendered).HasColumnType("decimal(18,2)");
                invoice.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");
                invoice.Property(i => i.TotalDiscountedPrice).HasColumnType("decimal(18,2)");
                invoice.Property(i => i.IsVoided).HasDefaultValue(false);

                // GUID configurations
                invoice.Property(i => i.Id).ValueGeneratedOnAdd();
                invoice.Property(i => i.UserId).ValueGeneratedNever();
            });

            // InvoiceItem configurations
            modelBuilder.Entity<InvoiceItemEntity>(invoiceItem =>
            {
                invoiceItem.Property(ii => ii.ItemPrice).HasColumnType("decimal(18,2)");
                invoiceItem.Property(ii => ii.DiscountedPrice).HasColumnType("decimal(18,2)");

                // GUID configurations
                invoiceItem.Property(ii => ii.Id).ValueGeneratedOnAdd();
                invoiceItem.Property(ii => ii.InvoiceId).ValueGeneratedNever();
            });

            // Expense configurations
            modelBuilder.Entity<ExpenseEntity>(expense =>
            {
                expense
                    .HasMany(e => e.ExpenseItems)
                    .WithOne(ei => ei.Expense)
                    .HasForeignKey(ei => ei.ExpenseId);

                expense.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");
                expense.Property(e => e.ExpenseDate).IsRequired();

                // GUID configurations
                expense.Property(e => e.Id).ValueGeneratedOnAdd();
                expense.Property(e => e.UserId).ValueGeneratedNever();
            });

            // ExpenseItem configurations
            modelBuilder.Entity<ExpenseItemEntity>(expenseItem =>
            {
                expenseItem.Property(ei => ei.Amount).HasColumnType("decimal(18,2)");

                // GUID configurations
                expenseItem.Property(ei => ei.Id).ValueGeneratedOnAdd();
                expenseItem.Property(ei => ei.ExpenseId).ValueGeneratedNever();
            });

            // Item configurations
            modelBuilder.Entity<ItemEntity>(item =>
            {
                item.Property(i => i.Wholesale).HasDefaultValue(0.0m);
                item.Property(i => i.Retail).HasDefaultValue(0.0m);
                item.Property(i => i.Stock).HasDefaultValue(0);
                item.Property(i => i.LowThreshold).HasDefaultValue(0);
                item.Property(i => i.HasExpiry).HasDefaultValue(false);

                // GUID configurations
                item.Property(i => i.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
