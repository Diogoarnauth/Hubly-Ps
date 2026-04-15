import { SocialPlatform } from "../services/api/PlatformService";

interface IPlatformService {
    getAllPlatforms(): Promise<SocialPlatform[]>;
}

export default IPlatformService;