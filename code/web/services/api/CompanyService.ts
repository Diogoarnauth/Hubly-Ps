import ICompanyService from "../interfaces/ICompanyService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

// Interface para o registo (vinda do ficheiro de interface original)
export interface CompanyData {
    companySize: string; // Alterado para string para coincidir com o teu select ("0 a 100", etc)
    companyName: string;
    description: string;
    sectors: string[];
    websiteLink: string;
    countryHeadquarters: string;
}

export interface TrendingCompany {
    user_id: number;
    company_name: string;
    description: string;
    country_headquarters: string;
    sectors: string[];
}

export interface CompanySearchInputModel {
    name?: string;
    sectors?: string[];
    companySize?: string;
    countries?: string[];
    page: number;
    pageSize: number;
}

// Representa uma empresa individual nos resultados da pesquisa
export interface CompanyOutputModel {
    id: number;
    companyName: string; // Ajustado para bater com o teu componente
    sectors: string[];
    companySize: string;
    countryHeadquarters: string; // Ajustado para bater com o teu componente
    description?: string;
    logoUrl?: string;
}

// Interface essencial para a paginação funcionar sem erros de tipagem
export interface CompanySearchResponse {
    items: CompanyOutputModel[];
    totalItems: number;
}

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

    async getTrendingCompanies(limit: number = 15): Promise<TrendingCompany[]> {
        const response = await this.apiClient.get<TrendingCompany[]>(
            `${API_ENDPOINTS.company.trending}?limit=${limit}`
        );
        return response;
    }

    async getCountries(): Promise<string[]> {
        try {
            return await this.apiClient.get<string[]>(API_ENDPOINTS.countries.getCountries);
        } catch (error) {
            console.error("Erro ao buscar países:", error);
            return [];
        }
    }

    async search(filters: CompanySearchInputModel): Promise<CompanySearchResponse> {
        return await this.apiClient.get<CompanySearchResponse>(
            API_ENDPOINTS.company.search,
            filters
        );
    }
}

export default new CompanyService();