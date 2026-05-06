export interface CompanyRecommendationOutputModel {
  id: number;
  companyName: string;
  description?: string;
  countryHeadquarters?: string;
  sectors?: string[];
}

export default CompanyRecommendationOutputModel;
