namespace Qatu.Domain;

public sealed class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ShopId { get; set; }
    public Guid SellerId { get; set; }
    public string Name { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Presentation { get; set; } = "";
    public string Unit { get; set; } = "unidad";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal PriceBob { get; set; }
    public int Stock { get; set; }
    public string? ImagePath { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
