import { CreatorSearchInputModel, TrendingCreator, CreatorSearchResponse } from "../services/api/CreatorService";

interface ICreatorService {
    registerCreator(artisticName: string): Promise<any>;
    getTrendingCreators(limit?: number): Promise<TrendingCreator[]>;
    searchCreators(filters: CreatorSearchInputModel): Promise<CreatorSearchResponse>;
    changeStatus(status: string): Promise<boolean>;

}


export default ICreatorService;