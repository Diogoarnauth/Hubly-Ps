//import GetInviteDto, { GetInvitesResponseDto } from "../DTO/GetInviteDto";
//import GetUserDto from "../DTO/GetUserDto";
import IUserService from "../interfaces/IUserService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

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

        // 3. Se a resposta existe mas não tem token (pode ser o objeto de erro JSON)
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

}
export default new UsersService();