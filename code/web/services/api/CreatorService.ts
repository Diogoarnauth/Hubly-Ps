import ICreatorService from "../interfaces/ICreatorService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

export interface TrendingCreator {
    user_id: number;
    PlatformUserName: string;
    PlatformName: string;
    Description: string;
}

// Modelo de Input atualizado para os filtros que estamos a usar
export interface CreatorSearchInputModel {
    platformId?: number;
    platformUserName?: string;
    followersCountMin?: number;
    followersCountMax?: number;
    priceMin?: number;
    priceMax?: number;
    sectors?: string[];
    page: number;
    pageSize: number;
}

export interface SocialProfileInputModel {
    platform_user_name: string;
    link: string;
    description: string;
    followers_count: number;
    priceMin?: number;
    priceMax?: number;
    platformId: number;
    sectors: string[];
}

export interface CreatorSearchResponse {
    items: any[]; 
    totalItems: number;
}

class CreatorService implements ICreatorService {
    private apiClient = new ApiClient();

    async registerCreator(artisticName: string): Promise<any> {
        const payload = { artisticName };
        return await this.apiClient.post(API_ENDPOINTS.creator.register, payload);
    }

    async getTrendingCreators(limit: number = 15): Promise<TrendingCreator[]> {
        return await this.apiClient.get<TrendingCreator[]>(
            `${API_ENDPOINTS.creator.trending}?limit=${limit}`
        );
    }

    async searchCreators(filters: CreatorSearchInputModel): Promise<CreatorSearchResponse> {
        // Agora o TypeScript sabe que isto devolve items e totalItems
        return await this.apiClient.get<CreatorSearchResponse>(
            API_ENDPOINTS.creator.search, 
            filters
        );
    }

    async addSocialProfile(data: SocialProfileInputModel): Promise<{ success: boolean; message?: string; data?: any }> {
    try {
        const response = await this.apiClient.post(
            API_ENDPOINTS.creator.addSocialProfile, 
            data
        );
        
        if (!response || (response as any).error) {
            throw new Error((response as any).message || "Invalid API response");
        }
        
        return { success: true, data: response };
    } catch (error: any) {
        console.error("Error in addSocialProfile catch:", error);

        const apiMessage = error.response?.data?.detail 
                        || error.response?.data?.title 
                        || "Failed to add social profile";

        return { 
            success: false, 
            message: apiMessage 
        };
    }
}
    
}

export default new CreatorService();