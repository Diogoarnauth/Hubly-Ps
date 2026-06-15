import { GetMyCoWorkerInfoResponse } from "../api/CoWorkerService";

interface ICoWorkerService {
    getMyCoWorkerInfo(): Promise<GetMyCoWorkerInfoResponse | null>;
    // Se precisares de outros métodos no futuro, adiciona aqui
}

export default ICoWorkerService;