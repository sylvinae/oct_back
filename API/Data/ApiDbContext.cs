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
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=../api/app.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<InvoiceModel>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<InvoiceItemModel>().Property(ii => ii.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ExpenseModel>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ExpenseItemModel>().Property(ei => ei.Id).ValueGeneratedOnAdd();

            modelBuilder
                .Entity<InvoiceModel>()
                .HasOne(i => i.User)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.UserId);

            modelBuilder
                .Entity<InvoiceItemModel>()
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(ii => ii.InvoiceId);

            modelBuilder
                .Entity<ExpenseModel>()
                .HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId);

            modelBuilder
                .Entity<ExpenseItemModel>()
                .HasOne(ei => ei.Expense)
                .WithMany(e => e.ExpenseItems)
                .HasForeignKey(ei => ei.ExpenseId);
        }
    }
}
