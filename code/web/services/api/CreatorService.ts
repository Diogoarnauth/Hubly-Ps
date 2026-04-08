import ICreatorService from "../interfaces/ICreatorService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

class CreatorService implements ICreatorService {
    private apiClient = new ApiClient();

    async registerCreator(artisticName: string) {
        const payload = {
            artisticName: artisticName
        };
        const response = await this.apiClient.post(API_ENDPOINTS.creator.register, payload);
        return response;
    }
}

export default new CreatorService();