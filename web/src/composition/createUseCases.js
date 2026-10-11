import { DecideSellerApplicationUseCase } from "../usecases/admin/DecideSellerApplicationUseCase.js";
import { DownloadAdminReportUseCase } from "../usecases/admin/DownloadAdminReportUseCase.js";
import { LoadAdminDashboardUseCase } from "../usecases/admin/LoadAdminDashboardUseCase.js";
import { GetProductDetailsUseCase } from "../usecases/catalog/GetProductDetailsUseCase.js";
import { SearchCatalogUseCase } from "../usecases/catalog/SearchCatalogUseCase.js";
import { GetConversationMessagesUseCase } from "../usecases/conversations/GetConversationMessagesUseCase.js";
import { ListConversationsUseCase } from "../usecases/conversations/ListConversationsUseCase.js";
import { OpenConversationUseCase } from "../usecases/conversations/OpenConversationUseCase.js";
import { SendConversationMessageUseCase } from "../usecases/conversations/SendConversationMessageUseCase.js";
import { GetOrdersUseCase } from "../usecases/orders/GetOrdersUseCase.js";
import { PlaceOrderUseCase } from "../usecases/orders/PlaceOrderUseCase.js";
import { CreateReviewUseCase } from "../usecases/reviews/CreateReviewUseCase.js";
import { ApplyAsSellerUseCase } from "../usecases/seller/ApplyAsSellerUseCase.js";
import { ChangeProductStatusUseCase } from "../usecases/seller/ChangeProductStatusUseCase.js";
import { CreateProductUseCase } from "../usecases/seller/CreateProductUseCase.js";
import { LoadSellerWorkspaceUseCase } from "../usecases/seller/LoadSellerWorkspaceUseCase.js";
import { UpdateProductUseCase } from "../usecases/seller/UpdateProductUseCase.js";
import { UploadProductImageUseCase } from "../usecases/seller/UploadProductImageUseCase.js";

export function createUseCases(apiClient) {
  return {
    applyAsSeller: new ApplyAsSellerUseCase(apiClient),
    changeProductStatus: new ChangeProductStatusUseCase(apiClient),
    createProduct: new CreateProductUseCase(apiClient),
    createReview: new CreateReviewUseCase(apiClient),
    decideSellerApplication: new DecideSellerApplicationUseCase(apiClient),
    downloadAdminReport: new DownloadAdminReportUseCase(apiClient),
    getConversationMessages: new GetConversationMessagesUseCase(apiClient),
    getOrders: new GetOrdersUseCase(apiClient),
    getProductDetails: new GetProductDetailsUseCase(apiClient),
    listConversations: new ListConversationsUseCase(apiClient),
    loadAdminDashboard: new LoadAdminDashboardUseCase(apiClient),
    loadSellerWorkspace: new LoadSellerWorkspaceUseCase(apiClient),
    openConversation: new OpenConversationUseCase(apiClient),
    placeOrder: new PlaceOrderUseCase(apiClient),
    searchCatalog: new SearchCatalogUseCase(apiClient),
    sendConversationMessage: new SendConversationMessageUseCase(apiClient),
    updateProduct: new UpdateProductUseCase(apiClient),
    uploadProductImage: new UploadProductImageUseCase(apiClient),
  };
}
