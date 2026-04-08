interface IUserService {
    login(email: string, password: string): Promise<any>;
     register(email: string, password: string, name: string): Promise<string | undefined>;
}

export default IUserService;
