import { CreatorSearchInputModel, TrendingCreator, CreatorSearchResponse } from "../api/CreatorService";
import { GetSocialProfileOutputModel } from "../DTO/creator/GetSocialProfileOutputModel";

interface ICreatorService {
    registerCreator(artisticName: string): Promise<any>;
    getTrendingCreators(limit?: number): Promise<TrendingCreator[]>;
    getRecommendedCreators(): Promise<GetSocialProfileOutputModel[]>;
    searchCreators(filters: CreatorSearchInputModel): Promise<CreatorSearchResponse>;
    changeStatus(status: string): Promise<boolean>;
    getSocialProfileById(profileId: number): Promise<GetSocialProfileOutputModel>;
    editSocialProfile(socialProfileId: number, data: any);

}


export default ICreatorService;