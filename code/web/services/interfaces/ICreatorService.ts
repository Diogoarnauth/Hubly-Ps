import GetCreatorOutputModel from "../DTO/GetCreatorOutputModel";

interface ICreatorService {
    registerCreator(artisticName: string): Promise<any>;
    changeStatus(status: string): Promise<boolean>;
}


export default ICreatorService;