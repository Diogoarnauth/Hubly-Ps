import { FullUserProfileOutputModel } from "../DTO/FullUserProfileOutputModel";

interface IUserService {
    login(email: string, password: string): Promise<any>;
    register(email: string, password: string, name: string): Promise<string | undefined>;
    validateConfirmationCode(email: string, confirmationCode: string): Promise<boolean>;
    getFullCreatorProfile(id: number): Promise<FullUserProfileOutputModel | null>;
}

export default IUserService;
