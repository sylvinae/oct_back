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
            // One-to-Many: User has many Invoices
            modelBuilder
                .Entity<InvoiceEntity>()
                .HasOne(i => i.User)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.UserId);

            // One-to-Many: Invoice has many InvoiceItems
            modelBuilder
                .Entity<InvoiceItemEntity>()
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(ii => ii.InvoiceId);

            // One-to-Many: User has many Expenses
            modelBuilder
                .Entity<ExpenseEntity>()
                .HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId);

            // One-to-Many: Expense has many ExpenseItems
            modelBuilder
                .Entity<ExpenseItemEntity>()
                .HasOne(ei => ei.Expense)
                .WithMany(e => e.ExpenseItems)
                .HasForeignKey(ei => ei.ExpenseId);

            // Decimal properties configuration (removing Column(TypeName))
            modelBuilder
                .Entity<InvoiceEntity>()
                .Property(i => i.AmountTendered)
                .HasColumnType("decimal(18,2)");

            modelBuilder
                .Entity<InvoiceEntity>()
                .Property(i => i.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder
                .Entity<InvoiceEntity>()
                .Property(i => i.TotalDiscountedPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder
                .Entity<InvoiceItemEntity>()
                .Property(ii => ii.ItemPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder
                .Entity<InvoiceItemEntity>()
                .Property(ii => ii.DiscountedPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder
                .Entity<ExpenseEntity>()
                .Property(e => e.TotalCost)
                .HasColumnType("decimal(18,2)");

            modelBuilder
                .Entity<ExpenseItemEntity>()
                .Property(ei => ei.Amount)
                .HasColumnType("decimal(18,2)");

            //Guid configs
            modelBuilder.Entity<UserEntity>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<InvoiceEntity>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<InvoiceEntity>().Property(i => i.UserId).ValueGeneratedNever();
            modelBuilder.Entity<InvoiceItemEntity>().Property(ii => ii.Id).ValueGeneratedOnAdd();
            modelBuilder
                .Entity<InvoiceItemEntity>()
                .Property(ii => ii.InvoiceId)
                .ValueGeneratedNever();
            modelBuilder.Entity<ExpenseEntity>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ExpenseEntity>().Property(e => e.UserId).ValueGeneratedNever();
            modelBuilder.Entity<ExpenseItemEntity>().Property(ei => ei.Id).ValueGeneratedOnAdd();

            modelBuilder
                .Entity<ExpenseItemEntity>()
                .Property(ei => ei.ExpenseId)
                .ValueGeneratedNever();

            // Default values for specific properties
            modelBuilder.Entity<InvoiceEntity>().Property(i => i.IsVoided).HasDefaultValue(false);
            modelBuilder.Entity<ItemEntity>().Property(i => i.Wholesale).HasDefaultValue(0.0m);
            modelBuilder.Entity<ItemEntity>().Property(i => i.Retail).HasDefaultValue(0.0m);
            modelBuilder.Entity<ItemEntity>().Property(i => i.Stock).HasDefaultValue(0);
            modelBuilder.Entity<ItemEntity>().Property(i => i.LowThreshold).HasDefaultValue(0);
            modelBuilder.Entity<ItemEntity>().Property(i => i.HasExpiry).HasDefaultValue(false);
        }
    }
}
