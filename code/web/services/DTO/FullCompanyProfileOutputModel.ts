import GetCompanyOutputModel from "./GetCompanyOutputModel";

export interface FullCompanyProfileOutputModel {
    id: number;
    name: string;
    email: string;
    isOwner: boolean;
    company: GetCompanyOutputModel | null;
}