using Qatu.Application;

namespace Qatu.Api.Parsing;

public static class ContractParser
{
    public static TEnum ParseEnum<TEnum>(string value, string errorMessage)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, true, out var result) && Enum.IsDefined(result))
            return result;
        throw new MarketException(MarketError.Invalid, errorMessage);
    }
}
