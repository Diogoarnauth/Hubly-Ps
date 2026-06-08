//import GetInviteDto, { GetInvitesResponseDto } from "../DTO/GetInviteDto";
//import GetUserDto from "../DTO/GetUserDto";
import IUserService from "../interfaces/IUserService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";
import { FullUserProfileOutputModel } from "../DTO/creator/FullUserProfileOutputModel";
import { FullCompanyProfileOutputModel } from "../DTO/company/FullCompanyProfileOutputModel";
import GetCreatorOutputModel from "../DTO/creator/GetCreatorOutputModel";
import GetCompanyOutputModel from "../DTO/company/GetCompanyOutputModel";
import { ProfileHistoryOutputModel } from "../DTO/ProfileHistoryOutputModel";


export interface UserInfo {
    id: number;
    name: string;
    email: string;
    role: 'creator' | 'company' | null;
}

class UsersService implements IUserService {
    private apiClient = new ApiClient();

    async login(email: string, password: string): Promise<boolean> {
        const user = {
            email: email,
            password: password
        };

        try {
            const response = await this.apiClient.post(API_ENDPOINTS.user.login, user);

            if (!response) {
                return false;
            }

            if (response.token) {
                return true;
            }

            return false;

        } catch (error) {
            console.error("Erro inesperado no login:", error);
            return false;
        }
    }

    async register(email: string, password: string, name: string): Promise<string | undefined> {
        const user = {
            name: name,
            email: email,
            password: password
        }
        const response = await this.apiClient.post(API_ENDPOINTS.user.register, user);
        return response as any;
    }

    async validateConfirmationCode(email: string, confirmationCode: string): Promise<boolean> {
        const user = {
            email: email,
            code: confirmationCode
        }
        const response = await this.apiClient.post(API_ENDPOINTS.user.verifyEmail, user);
        return !!response;
    }

    async resendEmailConfirmation(email: string): Promise<boolean> {
        const request = {
            email: email
        }
        const response = await this.apiClient.post(API_ENDPOINTS.user.resendEmailConfirmation, request);
        return !!response;
    }

    async checkHasProfile(): Promise<boolean> {
        try {
            const response = await this.apiClient.get(API_ENDPOINTS.user.checkProfile);
            return response?.hasProfile === true;
        } catch (error) {
            console.error("Error checking profile:", error);
            return false; // Por segurança, se falhar, assumimos que não tem
        }
    }

    async getFullCreatorProfile(id: number): Promise<FullUserProfileOutputModel | null> {
        const data = await this.apiClient.get<any>(API_ENDPOINTS.user.getFullCreatorProfile(id));
        if (!data) return null;

        return {
            ...data,
            // Instanciamos a classe para ativar o construtor que criaste
            creator: data.creator ? new GetCreatorOutputModel(data.creator) : null
        };
    }

    async getFullCompanyProfile(id: number): Promise<FullCompanyProfileOutputModel | null> {
        const data = await this.apiClient.get<any>(API_ENDPOINTS.user.getFullCompanyProfile(id));

        if (!data) return null;

        return {
            ...data,
            company: data.company ? new GetCompanyOutputModel(data.company) : null
        };
    }

    async editUsername(newUsername: string): Promise<boolean> {
        try {
            await this.apiClient.post(API_ENDPOINTS.user.edit, { newUsername });
            return true;
        } catch (error) {
            console.error("Erro ao editar username:", error);
            return false;
        }
    }


    async getCurrentUser(): Promise<UserInfo | null> {
        const response = await this.apiClient.get<UserInfo>(API_ENDPOINTS.user.getMyInfo);
        console.log("responseeee", response)

        if (!response) return null;

        return {
            id: response.id,
            name: response.name,
            email: response.email,
            role: response.role
        };
    } catch(error) {
        console.error("Erro ao obter info do utilizador:", error);
        return null;
    }

    async logout(): Promise<boolean> {//melhorar errors disto
        try {
            await this.apiClient.post(API_ENDPOINTS.user.logout, {});
            return true;
        } catch (error) {
            console.error("Erro ao realizar logout no servidor:", error);
            // Retornamos true na mesma ou gerimos o erro?? 
            return false;
        }
    }

    async getHistory(): Promise<ProfileHistoryOutputModel[]> {
        try {
            const response = await this.apiClient.get<ProfileHistoryOutputModel[]>(API_ENDPOINTS.user.getHistory);
            return response || [];
        } catch (error) {
            console.error("Erro ao obter histórico:", error);
            return [];
        }
    }
}

export default new UsersService();