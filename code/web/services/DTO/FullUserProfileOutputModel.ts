import GetCreatorOutputModel from "./GetCreatorOutputModel";

export interface FullUserProfileOutputModel {
    id: number;
    name: string;
    email: string;
    creator: GetCreatorOutputModel | null;
}