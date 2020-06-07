using Microsoft.EntityFrameworkCore;
using Ord.WebApi.Entities;
using Ord.WebApi.Entities.Shared;

namespace Ord.WebApi.Data.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Menu>()
            .HasIndex(m => new { m.RestaurantId, m.Name })
            .IsUnique();

            builder.Entity<MenuCategory>()
            .HasIndex(mc => new { mc.MenuId, mc.Name })
            .IsUnique();

            builder.Entity<CollectionType>()
            .HasIndex(ct => new { ct.Name })
            .IsUnique();

            builder.Entity<RestaurantCollectionType>()
            .HasIndex(rc => new { rc.RestaurantId, rc.CollectionTypeId })
            .IsUnique();

            builder.Entity<RestaurantPaymentMethod>()
            .HasIndex(pm => new { pm.RestaurantId, pm.PaymentMethodId })
            .IsUnique();

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrdUser> OrdUsers { get; set; }
        public DbSet<OrdUserHomeAddress> OrdUserHomeAddresses { get; set; }
        public DbSet<OrdUserWorkAddress> OrdUserWorkAddresses { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<CollectionType> CollectionTypes { get; set; }
        public DbSet<RestaurantCollectionType> RestaurantCollectionTypes { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<RestaurantPaymentMethod> RestaurantPaymentMethods { get; set; }
        public DbSet<ServiceArea> ServiceAreas { get; set; }
    }
}
