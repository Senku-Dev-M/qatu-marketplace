using Microsoft.EntityFrameworkCore;
using Qatu.Domain;

namespace Qatu.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<SellerApplication> SellerApplications => Set<SellerApplication>();
    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("qatu");
        modelBuilder.Entity<Profile>().HasKey(x => x.Id);
        modelBuilder.Entity<Profile>().HasIndex(x => x.Email);
        modelBuilder.Entity<SellerApplication>().Property(x => x.Status).HasConversion<string>();
        modelBuilder.Entity<SellerApplication>().HasIndex(x => new { x.ApplicantId, x.Status });
        modelBuilder.Entity<Shop>().HasIndex(x => x.OwnerId).IsUnique();
        modelBuilder.Entity<Product>().Property(x => x.Status).HasConversion<string>();
        modelBuilder.Entity<Product>().Property(x => x.PriceBob).HasPrecision(12, 2);
        modelBuilder.Entity<Product>().HasIndex(x => new { x.Status, x.Category });
        modelBuilder.Entity<PurchaseOrder>().Property(x => x.Status).HasConversion<string>();
        modelBuilder.Entity<PurchaseOrder>().Property(x => x.UnitPriceBob).HasPrecision(12, 2);
        modelBuilder.Entity<PurchaseOrder>().HasIndex(x => x.BuyerId);
        modelBuilder.Entity<Conversation>().HasIndex(x => new { x.ProductId, x.BuyerId }).IsUnique();
        modelBuilder.Entity<ChatMessage>().HasIndex(x => new { x.ConversationId, x.SentAt });
        modelBuilder.Entity<Review>().Property(x => x.Kind).HasConversion<string>();
        modelBuilder.Entity<Review>().HasIndex(x => new { x.OrderId, x.Kind }).IsUnique();
    }
}
