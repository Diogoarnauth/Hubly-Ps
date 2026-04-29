import { 
    CompanyData, 
    CompanySearchInputModel, 
    CompanySearchResponse, 
    TrendingCompany 
} from "../api/CompanyService";

interface ICompanyService {
    registerCompany(data: CompanyData): Promise<any>;
    getTrendingCompanies(limit?: number): Promise<TrendingCompany[]>;
    search(filters: CompanySearchInputModel): Promise<CompanySearchResponse>;
    getCountries(): Promise<string[]>;
    editCompany(data: CompanyData): Promise<boolean>;

}

export default ICompanyService;