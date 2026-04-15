import ICreatorService from "../interfaces/ICreatorService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";
import GetCreatorOutputModel from "../DTO/GetCreatorOutputModel";

class CreatorService implements ICreatorService {
    private apiClient = new ApiClient();

    async registerCreator(artisticName: string) {
        const payload = {
            artisticName: artisticName
        };
        const response = await this.apiClient.post(API_ENDPOINTS.creator.register, payload);
        return response;
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
}

export default new CreatorService();