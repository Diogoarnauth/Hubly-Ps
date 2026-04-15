import { 
    CompanyData, 
    CompanySearchInputModel, 
    CompanySearchResponse, 
    TrendingCompany 
} from "../services/api/CompanyService";

interface ICompanyService {
    registerCompany(data: CompanyData): Promise<any>;
    getTrendingCompanies(limit?: number): Promise<TrendingCompany[]>;
    search(filters: CompanySearchInputModel): Promise<CompanySearchResponse>;
    getCountries(): Promise<string[]>;

}

export default ICompanyService;