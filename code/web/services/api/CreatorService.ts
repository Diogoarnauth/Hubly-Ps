import ICreatorService from "../interfaces/ICreatorService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";
import GetCreatorOutputModel from "../DTO/GetCreatorOutputModel";
import { GetSocialProfileOutputModel } from "../DTO/GetSocialProfileOutputModel";

export interface TrendingCreator {
    user_id: number;
    socialProfile_id: number;
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
    
    async changeStatus(status: string): Promise<boolean> {
        try {
            await this.apiClient.post(API_ENDPOINTS.creator.status, { AvailabilityStatus: status });
            return true;
        } catch (error) {
            console.error("Erro ao mudar status:", error);
            return false;
        }
    }

    async editCreator(artisticName: string): Promise<boolean> {
        try {
            await this.apiClient.post(API_ENDPOINTS.creator.edit, { ArtisticName: artisticName });
            return true;
        } catch (error) {
            console.error("Erro ao editar creator:", error);
            return false;
        }
    }


    async getSocialProfileById(profileId: number): Promise<GetSocialProfileOutputModel> {
        try {
            const endpointTemplate = API_ENDPOINTS.creator.getSocialProfileById;

            const url = endpointTemplate.replace("{profileId}", profileId.toString());

            const response = await this.apiClient.get<GetSocialProfileOutputModel>(url);

            return response;
        } catch (error) {
            console.error(`Erro ao obter perfil social ${profileId}:`, error);
            throw error;
        }
    }

     async editSocialProfile(socialProfileId: number, data: any) {
        try {
            let endpoint = API_ENDPOINTS.creator.editSocialProfile;

            endpoint = endpoint.replace("{socialProfileId}", socialProfileId.toString());

            const response = await this.apiClient.post(endpoint, data);

            return response;
        } catch (error) {
            console.error(`Erro ao obter perfil social ${socialProfileId}:`, error);
            throw error;
        }
    }

     async deleteSocialProfile(limit: number = 15): Promise<TrendingCreator[]> {
        return await this.apiClient.get<TrendingCreator[]>(
            `${API_ENDPOINTS.creator.trending}?limit=${limit}`
        );
    }
}

export default new CreatorService();