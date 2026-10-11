using Qatu.Application;

namespace Qatu.Api.Configuration;

internal static class ApplicationRegistration
{
    internal static IServiceCollection AddQatuApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<UserAccessPolicy>();
        services.AddScoped<ProductAccessPolicy>();
        services.AddScoped<ConversationAccessPolicy>();
        services.AddScoped<EnsureUserProfileUseCase>();
        services.AddScoped<GetCurrentProfileUseCase>();
        services.AddScoped<GetCatalogUseCase>();
        services.AddScoped<GetProductDetailsUseCase>();
        services.AddScoped<GetProductImageUseCase>();
        services.AddScoped<GetSellerProductsUseCase>();
        services.AddScoped<CreateProductUseCase>();
        services.AddScoped<UpdateProductUseCase>();
        services.AddScoped<ChangeProductStatusUseCase>();
        services.AddScoped<UploadProductImageUseCase>();
        services.AddScoped<ApplyAsSellerUseCase>();
        services.AddScoped<ListSellerApplicationsUseCase>();
        services.AddScoped<DecideSellerApplicationUseCase>();
        services.AddScoped<GetMyShopUseCase>();
        services.AddScoped<PlaceOrderUseCase>();
        services.AddScoped<GetUserOrdersUseCase>();
        services.AddScoped<CreateReviewUseCase>();
        services.AddScoped<GetSellerReviewsUseCase>();
        services.AddScoped<OpenConversationUseCase>();
        services.AddScoped<ListConversationsUseCase>();
        services.AddScoped<AuthorizeConversationUseCase>();
        services.AddScoped<GetConversationMessagesUseCase>();
        services.AddScoped<SendConversationMessageUseCase>();
        services.AddScoped<GetAdminReportUseCase>();

        var adminUserId = Guid.TryParse(configuration["Bootstrap:AdminUserId"], out var adminId)
            ? adminId
            : (Guid?)null;
        services.AddSingleton(new MarketplaceOptions(adminUserId));
        return services;
    }
}
