//import GetInviteDto, { GetInvitesResponseDto } from "../DTO/GetInviteDto";
//import GetUserDto from "../DTO/GetUserDto";
import IUserService from "../interfaces/IUserService";
import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";

class UsersService implements IUserService {
    private apiClient = new ApiClient();

    async login(email: string, password: string) {
        const user = {
            email: email,
            password: password
        }
        const response = await this.apiClient.post(API_ENDPOINTS.user.login, user);
        return response as boolean
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
}
export default new UsersService();