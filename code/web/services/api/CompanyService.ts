import ICompanyService, { CompanyData } from "../interfaces/ICompanyService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

class CompanyService implements ICompanyService {
    private apiClient = new ApiClient();

    async registerCompany(data: CompanyData) {
       try {
            const response = await this.apiClient.post(API_ENDPOINTS.company.register, data);

            if (!response) {
                return { success: false, message: "Connection failed" };
            }

            return { success: true, data: response };
        } catch (error: any) {
            const errorMessage = error.message || "An unexpected error occurred";

            return { 
                success: false, 
                message: errorMessage 
            };
        }
    }
    async editCompany(data: CompanyData): Promise<boolean> {
        try {
            await this.apiClient.post(API_ENDPOINTS.company.edit, data);
            return true;
        } catch (error) {
            console.error("Erro ao editar empresa:", error);
            return false;
        }
    }
}

export default new CompanyService();