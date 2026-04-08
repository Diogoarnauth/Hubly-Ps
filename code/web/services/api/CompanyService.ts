import ICompanyService, { CompanyData } from "../interfaces/ICompanyService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

class CompanyService implements ICompanyService {
    private apiClient = new ApiClient();

    async registerCompany(data: CompanyData) {
        // O ApiClient trata do JSON.stringify e dos headers
        const response = await this.apiClient.post(API_ENDPOINTS.company.register, data);
        return response;
    }
}

export default new CompanyService();