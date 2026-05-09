import { ConversationTagInputModel } from "./ConversationTagInputModel";

export interface ConversationSummaryOutputModel {
    id: number;
    lastMessage: string;
    lastMessageAt: number;
    otherPartyName: string;
    platformId: number;
    unreadCount: number;
    tag?: ConversationTagInputModel; 
}

export default ConversationSummaryOutputModel;