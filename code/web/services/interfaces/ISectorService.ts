export interface Sector {
    id: string; // ou number, dependendo do teu backend
    name: string;
}

interface ISectorService {
    getAllSectors(): Promise<Sector[]>;
}

export default ISectorService;