// api/CoWorkerService.ts
import ICoWorkerService from "../interfaces/ICoWorkerService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

export interface GetMyCoWorkerInfoResponse {
    id: number;
    userId: number;
    ownerId: number;
    joinedAt: string;
}

class CoWorkerService implements ICoWorkerService {
    private apiClient = new ApiClient();

    async getMyCoWorkerInfo(): Promise<GetMyCoWorkerInfoResponse | null> {
        try {
            // Chamada ao backend
            const response = await this.apiClient.get<GetMyCoWorkerInfoResponse>(
                API_ENDPOINTS.coWorker.getMyInfo
            );
            return response;
        } catch (error) {
            console.error("Erro ao obter informações do CoWorker:", error);
            return null;
        }
    }
}

export default new CoWorkerService();