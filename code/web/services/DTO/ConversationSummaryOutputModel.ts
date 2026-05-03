export interface ConversationSummaryOutputModel {
    id: number;
    lastMessage: string;
    lastMessageAt: number;
    otherPartyName: string;
    platformId: number;
}

export default ConversationSummaryOutputModel;