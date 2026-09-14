using CustomerService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.DAO
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(
            DbContextOptions<CustomerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(x => x.LastName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(x => x.Email)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(x => x.PhoneNumber)
                      .HasMaxLength(20);

                entity.Property(x => x.CreatedDate)
                      .IsRequired();

                entity.Property(x => x.IsActive)
                      .IsRequired();

                entity.HasIndex(x => x.Email)
                      .IsUnique();
            });
        }
    }
}