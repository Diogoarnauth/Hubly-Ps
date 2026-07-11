//api/CoWorkerService.ts

import ICoWorkerService from "../interfaces/ICoWorkerService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

export interface SendInviteInputModel {
    email: string;
}

export interface CoWorkerInviteOutputModel {
    id: number;
    ownerId: number;
    coWorkerEmail: string;
    status: string; 
    createdAt: string;
    expiresAt?: string;
}

export interface GetMyCoWorkerInfoResponse {
    id: number;
    userId: number;
    ownerId: number;
    joinedAt: string;
}


export interface GetMyCoWorkerWithEmailInfoResponse {
    id: number;
    userId: number;
    ownerId: number;
    joinedAt: string;
    coWorkerEmail: string;
}

class CoWorkerService implements ICoWorkerService {
    private apiClient = new ApiClient();

    async getMyCoWorkerInfo(): Promise<GetMyCoWorkerInfoResponse | null> {
        try {
            // Chamada ao backend
            const response = await this.apiClient.get<GetMyCoWorkerInfoResponse>(
                API_ENDPOINTS.coWorker.getMyInfo,
                undefined,
                {suppressError : true}
            );
            return response;
        } catch (error) {
            console.error("Erro ao obter informações do CoWorker:", error);
            return null;
        }
    }

    async sendInvite(data: SendInviteInputModel) {
        try {
            const response = await this.apiClient.post(API_ENDPOINTS.coWorker.sendInvite, data);

            if (!response) {
                return { success: false, message: "Connection failed" };
            }

            return { success: true, data: response };
        } catch (error: any) {
            const errorMessage = error.message || "An unexpected error occurred while sending the invite";
            return {
                success: false,
                message: errorMessage
            };
        }
    }

    async acceptInvite(inviteId: number): Promise<boolean> {
        try {
            await this.apiClient.post(API_ENDPOINTS.coWorker.acceptInvite(inviteId), {});
            return true;
        } catch (error) {
            console.error(`Erro ao aceitar convite ${inviteId}:`, error);
            return false;
        }
    }

    async rejectInvite(inviteId: number): Promise<boolean> {
        try {
            await this.apiClient.post(API_ENDPOINTS.coWorker.rejectInvite(inviteId), {});
            return true;
        } catch (error) {
            console.error(`Erro ao rejeitar convite ${inviteId}:`, error);
            return false;
        }
    }

    async getReceivedInvites(): Promise<CoWorkerInviteOutputModel[]> {
        try {
            const response = await this.apiClient.get<CoWorkerInviteOutputModel[]>(
                API_ENDPOINTS.coWorker.getReceivedInvites
            );
            return Array.isArray(response) ? response : [];
        } catch (error) {
            console.error("Erro ao buscar convites recebidos:", error);
            return [];
        }
    }

    async getSentInvites(): Promise<CoWorkerInviteOutputModel[]> {
        try {
            const response = await this.apiClient.get<CoWorkerInviteOutputModel[]>(
                API_ENDPOINTS.coWorker.getSentInvites
            );
            return Array.isArray(response) ? response : [];
        } catch (error) {
            console.error("Erro ao buscar convites enviados:", error);
            return [];
        }
    }

    async cancelCoworking(): Promise<boolean> {
        try {
            await this.apiClient.delete(API_ENDPOINTS.coWorker.CancelCoworking);
            return true;
        } catch (error) {
            console.error("Erro ao cancelar coworking (auto-desassociação):", error);
            return false;
        }
    }

    /**
     * Owner remove um CoWorker específico da sua equipa
     * @param coWorkerUserId O ID do utilizador que está a ser removido
     */
    async ownerCancelCoworking(coWorkerUserId: number): Promise<boolean> {
        try {
            await this.apiClient.delete(API_ENDPOINTS.coWorker.OwnerCancelCoworking(coWorkerUserId));
            return true;
        } catch (error) {
            console.error(`Erro ao remover CoWorker ${coWorkerUserId}:`, error);
            return false;
        }
    }

    async getMyTeam(): Promise<GetMyCoWorkerWithEmailInfoResponse[]> {
        try {
            const response = await this.apiClient.get<GetMyCoWorkerWithEmailInfoResponse[]>(
                API_ENDPOINTS.coWorker.GetMyTeam
            );
            return Array.isArray(response) ? response : [];
        } catch (error) {
            console.error("Erro ao buscar equipa (team):", error);
            return [];
        }
    }
}

export default new CoWorkerService();