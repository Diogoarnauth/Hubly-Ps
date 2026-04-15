import IPlatformService from "../interfaces/IPlatformService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

export interface SocialPlatform {
    id: number;
    namePlatform: string;
}

class PlatformService implements IPlatformService {
    private apiClient = new ApiClient();

    async getAllPlatforms(): Promise<SocialPlatform[]> {
        return await this.apiClient.get<SocialPlatform[]>(
            API_ENDPOINTS.socialPlatforms.getAll
        );
    }
}

export default new PlatformService();