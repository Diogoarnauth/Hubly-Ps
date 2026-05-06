export interface MessageOutputModel {
    content: string;
    conversationId: number;
    id: number;
    isEdited: boolean;
    senderId: number;
    sentAt: number;
}

export default MessageOutputModel;