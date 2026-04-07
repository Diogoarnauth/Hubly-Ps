interface IUserService {
    login(email: string, password: string): Promise<any>;
}

export default IUserService;
