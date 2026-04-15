import ISectorService, { Sector } from "../interfaces/ISectorService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

export interface Sector {
    Id: number;
    Name: string;
}

class SectorService implements ISectorService {
    private apiClient = new ApiClient();

    async getAllSectors(): Promise<Sector[]> {
        const response = await this.apiClient.get<Sector[]>(API_ENDPOINTS.sectors.getAll);
        return response || [];
    }
}

export default new SectorService();