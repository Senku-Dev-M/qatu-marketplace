using Qatu.Domain;

namespace Qatu.Application;

internal static class ProductRules
{
    internal static void Validate(ProductData data)
    {
        if (string.IsNullOrWhiteSpace(data.Name)
            || data.Name.Trim().Length > 120
            || string.IsNullOrWhiteSpace(data.Category)
            || data.PriceBob <= 0
            || data.Stock < 0)
            throw MarketErrors.Invalid("Revisa nombre, categoría, precio y existencias.");

        if (data.Brand?.Length > 80 || data.Presentation?.Length > 40 || data.Unit?.Length > 30)
            throw MarketErrors.Invalid("Marca, presentación o unidad demasiado largas.");
    }

    internal static void Apply(Product product, ProductData data)
    {
        product.Name = data.Name.Trim();
        product.Description = data.Description.Trim();
        product.Category = data.Category.Trim();
        product.Brand = data.Brand?.Trim() ?? "";
        product.Presentation = data.Presentation?.Trim() ?? "";
        product.Unit = string.IsNullOrWhiteSpace(data.Unit) ? "unidad" : data.Unit.Trim();
        product.PriceBob = decimal.Round(data.PriceBob, 2);
        product.Stock = data.Stock;
    }
}
