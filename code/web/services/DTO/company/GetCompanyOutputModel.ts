export class GetCompanyOutputModel {
    id: number;
    companyName: string;
    isVerified: boolean;
    description: string;
    sectors: string[]; // Corrigido para sintaxe TypeScript
    companySize: string;
    websiteLink: string;
    countryHeadquarters: string;

    constructor(data: any) {
        this.id = data.id;
        this.companyName = data.companyName;
        this.isVerified = data.isVerified;
        this.description = data.description;
        this.sectors = data.sectors || []; // Garante que inicia como array vazio se vier nulo
        this.companySize = data.companySize;
        this.websiteLink = data.websiteLink;
        this.countryHeadquarters = data.countryHeadquarters;
    }
}

export default GetCompanyOutputModel;