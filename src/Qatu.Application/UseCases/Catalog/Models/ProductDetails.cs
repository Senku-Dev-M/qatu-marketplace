using Qatu.Domain;

namespace Qatu.Application;

public sealed record ProductDetails(Product Product, string ShopName, List<Review> Reviews);
