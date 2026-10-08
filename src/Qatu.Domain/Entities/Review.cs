namespace Qatu.Domain;

public sealed class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid ProductId { get; set; }
    public Guid SellerId { get; set; }
    public ReviewKind Kind { get; set; }
    public int Stars { get; set; }
    public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
