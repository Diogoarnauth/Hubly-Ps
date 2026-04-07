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
}
export default new UsersService();