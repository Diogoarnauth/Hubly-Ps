import { 
    CreateConversationData, 
    PagedMessages
} from "../api/ConversationService";
import { ConversationSummaryOutputModel } from "../DTO/ConversationSummaryOutputModel";

interface IConversationService {
    getMessages(conversationId: number, page: number = 1, pageSize: number = 25): Promise<PagedMessages | null>;
    sendMessage(conversationId: number, content: string): Promise<any>;
    createConversation(data: CreateConversationData): Promise<any>;
    editMessage(messageId: number, newContent: string): Promise<boolean>;
    deleteMessage(messageId: number): Promise<boolean>;
    getConversationsByProfileId(profileId: number): Promise<boolean>;
    getConversationsByCompanyId(companyId: number): Promise<ConversationSummaryOutputModel[]>;

}

export default IConversationService;