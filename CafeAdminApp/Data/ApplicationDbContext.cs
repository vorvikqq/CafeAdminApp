using CafeAdminApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Check> Checks { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoicePriceItem> InvoicePrice { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderPriceItem> OrderPrice { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StockItem> Stock { get; set; }


        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-Many: Invoice <-> Price через InvoicePriceItem
            modelBuilder.Entity<InvoicePriceItem>()
                .HasKey(ip => new { ip.PriceId, ip.InvoiceId });

            modelBuilder.Entity<InvoicePriceItem>()
                .HasOne(ip => ip.Invoice)
                .WithMany(i => i.InvoicePrices) 
                .HasForeignKey(ip => ip.InvoiceId);

            modelBuilder.Entity<InvoicePriceItem>()
                .HasOne(ip => ip.Price)
                .WithMany(p => p.InvoicePrices) 
                .HasForeignKey(ip => ip.PriceId);

            // Many-to-Many: Order <-> Price через OrderPriceItem
            modelBuilder.Entity<OrderPriceItem>()
                .HasKey(op => new { op.OrderId, op.PriceId });

            modelBuilder.Entity<OrderPriceItem>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderPrices)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderPriceItem>()
                .HasOne(op => op.Price)
                .WithMany(p => p.OrderPrices) 
                .HasForeignKey(op => op.PriceId);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.CreateDate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.UtcDateTime,  
                    v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc), TimeSpan.Zero)
                );

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderDate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.UtcDateTime,
                    v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc), TimeSpan.Zero) 
                );

            modelBuilder.Entity<Price>()
                .Property(p => p.Date)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.UtcDateTime, 
                    v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc), TimeSpan.Zero) 
                );

            modelBuilder.Entity<Product>()
                .Property(p => p.ManufactureDate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.UtcDateTime,  
                    v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc), TimeSpan.Zero) 
                );

            modelBuilder.Entity<Product>()
                .Property(p => p.ConsumptionDate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.UtcDateTime,  
                    v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc), TimeSpan.Zero) 
                );
            modelBuilder.Entity<Check>()
               .Property(p => p.SaleDate)
               .HasColumnType("timestamp with time zone")
               .HasConversion(
                   v => v.UtcDateTime, 
                   v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc), TimeSpan.Zero) 
               );
        }
    }
}
