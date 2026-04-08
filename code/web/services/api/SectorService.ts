import ISectorService, { Sector } from "../interfaces/ISectorService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

class SectorService implements ISectorService {
    private apiClient = new ApiClient();

    async getAllSectors(): Promise<Sector[]> {
        // Assume que o endpoint devolve um array de objetos [{id, name}, ...]
        const response = await this.apiClient.get<Sector[]>(API_ENDPOINTS.sectors.getAll);
        return response || [];
    }
}

export default new SectorService();