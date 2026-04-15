interface IUserService {
    login(email: string, password: string): Promise<boolean>;
    register(email: string, password: string, name: string): Promise<string | undefined>;
    validateConfirmationCode(email: string, confirmationCode: string): Promise<boolean>;
    checkHasProfile(): Promise<boolean>;
}

export default IUserService;