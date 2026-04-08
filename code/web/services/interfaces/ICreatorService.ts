interface ICreatorService {
    registerCreator(artisticName: string): Promise<any>;
}

export default ICreatorService;