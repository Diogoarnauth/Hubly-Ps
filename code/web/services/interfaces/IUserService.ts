import { FullUserProfileOutputModel } from "../DTO/FullUserProfileOutputModel";

interface IUserService {
    login(email: string, password: string): Promise<boolean>;
    register(email: string, password: string, name: string): Promise<string | undefined>;
    validateConfirmationCode(email: string, confirmationCode: string): Promise<boolean>;
    checkHasProfile(): Promise<boolean>;
    getFullCreatorProfile(id: number): Promise<FullUserProfileOutputModel | null>;
    editUsername(newUsername: string): Promise<boolean>;
}

export default IUserService;