import { SocialPlatform } from "../api/PlatformService";

interface IPlatformService {
    getAllPlatforms(): Promise<SocialPlatform[]>;
}

export default IPlatformService;