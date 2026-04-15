import GetCompanyOutputModel from "./GetCompanyOutputModel";

export interface FullCompanyProfileOutputModel {
    id: number;
    name: string;
    email: string;
    company: GetCompanyOutputModel | null;
}