export interface CompanyData {
    companySize: number;
    companyName: string;
    description: string;
    sectors: string[];
    websiteLink: string;
    countryHeadquarters: string;
}

interface ICompanyService {
    registerCompany(data: CompanyData): Promise<any>;
}

export default ICompanyService;