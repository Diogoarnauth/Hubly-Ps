export interface SocialProfileOutputModel {
    id: number;
    platformId: number; //mudar 
    platformUserName: string;
    link: string;
    description: string;
    followersCount: number;
    priceMin: number;
    priceMax: number;
    platformName: string;
}
class GetCreatorOutputModel {
    id: number;
    artisticName: string;
    isVerified: boolean;
    availabilityStatus: string;
    globalRating: number | null;
    ratingsCount: number;
    chatsStartedCount: number;
    chatsRespondedCount: number;
    socialProfiles: SocialProfileOutputModel[];

    constructor(data: any) {
        this.id = data.id;
        this.artisticName = data.artisticName;
        this.isVerified = data.isVerified;
        this.availabilityStatus = data.availabilityStatus;
        this.globalRating = data.globalRating;
        this.ratingsCount = data.ratingsCount;
        this.chatsStartedCount = data.chatsStartedCount;
        this.chatsRespondedCount = data.chatsRespondedCount;
        this.socialProfiles = data.socialProfiles || [];
    }
}

export default GetCreatorOutputModel;