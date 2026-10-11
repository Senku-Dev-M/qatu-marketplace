export class DownloadAdminReportUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute() {
    return this.apiClient.download("/admin/report.csv");
  }
}
