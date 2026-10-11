using Qatu.Api.Contracts;
using Qatu.Application;

namespace Qatu.Api.Mappers;

public static class ProductInputMapper
{
    public static ProductData ToProductData(this ProductInput input) => new(
        input.Name,
        input.Description,
        input.Category,
        input.PriceBob,
        input.Stock,
        input.Brand,
        input.Presentation,
        input.Unit);
}
