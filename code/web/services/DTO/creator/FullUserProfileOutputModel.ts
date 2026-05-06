import GetCreatorOutputModel from "./GetCreatorOutputModel";

export interface FullUserProfileOutputModel {
    id: number;
    name: string;
    email: string;
    isOwner: boolean;
    creator: GetCreatorOutputModel | null;
}