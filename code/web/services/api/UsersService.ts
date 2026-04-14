//import GetInviteDto, { GetInvitesResponseDto } from "../DTO/GetInviteDto";
//import GetUserDto from "../DTO/GetUserDto";
import IUserService from "../interfaces/IUserService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";
import { FullUserProfileOutputModel } from "../DTO/FullUserProfileOutputModel";
import  GetCreatorOutputModel  from "../DTO/GetCreatorOutputModel";


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
}
export default new UsersService();