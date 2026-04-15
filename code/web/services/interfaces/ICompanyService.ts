export interface CompanyData {
    companySize: string;
    companyName: string;
    description: string;
    sectors: string[];
    websiteLink: string;
    countryHeadquarters: string;
}

interface ICompanyService {
    registerCompany(data: CompanyData): Promise<any>;
    editCompany(data: CompanyData): Promise<boolean>;
}

export default ICompanyService;