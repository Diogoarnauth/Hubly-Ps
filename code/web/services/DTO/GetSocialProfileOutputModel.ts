export interface GetSocialProfileOutputModel {
    id: number;
    creatorId: number;
    platformId: number;
    platformUserName: string;
    link: string;
    description: string | null;
    followersCount: number;
    priceMin: number | null;
    priceMax: number | null;
    sectors: string[];
    platformName: string;
    isOwner: boolean;
}

export default GetSocialProfileOutputModel;