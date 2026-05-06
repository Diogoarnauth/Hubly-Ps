import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";
import ConversationSummaryOutputModel from "../DTO/ConversationSummaryOutputModel";

export interface Message {
    messageId: number;
    senderId: number;
    content: string;
    sentAt: number; 
    isEdited: boolean;
    isDeleted: boolean;
    type?: string;
}

export interface PagedMessages {
    items: Message[];
    totalItems: number;
    page: number;
    pageSize: number;
}

export interface CreateConversationData {
    Sender: {
        ProfileId: number;
        Type: number;
    };
    Receiver: {
        ProfileId: number;
        Type: number;
    };
}

export interface ConversationResponse {
    id: number;
}


class ConversationService {
    private apiClient = new ApiClient();

    async getMessages(conversationId: number, page: number = 1, pageSize: number = 25): Promise<PagedMessages | null> {
        try {
            const url = API_ENDPOINTS.conversation.getMessages.replace("{conversationId}", conversationId.toString());
            const response = await this.apiClient.get<PagedMessages>(url, { page, pageSize });
            return response;
        } catch (error) {
            console.error("Messages not found:", error);
            return null;
        }
    }


    async sendMessage(conversationId: number, content: string) {
        try {
            const url = API_ENDPOINTS.conversation.sendMessage.replace("{conversationId}", conversationId.toString());
            const response = await this.apiClient.post(url, {content});
            return { success: true, data: response };
        } catch (error: any) {
            return { 
                success: false, 
                message: error.message || "Message not sent" 
            };
        }
    }

    async createConversation(data: CreateConversationData) {
        try {
            const response = await this.apiClient.post<ConversationResponse>(API_ENDPOINTS.conversation.create, data);
            console.log("Create conversation response:", response);
            return { success: true, data: response };
        } catch (error: any) {
            return { 
                success: false, 
                message: error.message || "Conversation not created" 
            };
        }
    }

    async editMessage(messageId: number, newContent: string): Promise<boolean> {
        try {
            const url = API_ENDPOINTS.conversation.editMessage.replace("{messageId}", messageId.toString());
            await this.apiClient.post(url, { newContent });
            return true;
        } catch (error) {
            console.error("Message not found:", error);
            return false;
        }
    }

    async deleteMessage(messageId: number): Promise<boolean> {
        try {
            const url = API_ENDPOINTS.conversation.deleteMessage.replace("{messageId}", messageId.toString());
            await this.apiClient.post(url, {}); // Usei POST aqui porque é o que tens no service, mas no backend confirma se é DELETE
            return true;
        } catch (error) {
            console.error("Message not found:", error);
            return false;
        }
    }

    async getConversationsByProfileId(profileId: number): Promise<ConversationSummaryOutputModel[]> {
        try {
            const url = API_ENDPOINTS.conversation.getConversationsByProfileId.replace("{socialProfileId}", profileId.toString());
            const response = await this.apiClient.get<ConversationSummaryOutputModel[]>(url);
            return response || [];
        } catch (error) {
            console.error("Conversations not found:", error);
            return [];
        }
    }

    async getConversationsByCompanyId(companyId: number): Promise<ConversationSummaryOutputModel[]> {
        try {
            const url = API_ENDPOINTS.conversation.getConversationsByCompanyId.replace("{companyId}", companyId.toString());
            const response = await this.apiClient.get<ConversationSummaryOutputModel[]>(url);
            return response || [];
        } catch (error) {
            console.error("Company conversations not found:", error);
            return [];
        }
    }

    async checkConversationExists(senderProfileId: number, senderType: number, receiverProfileId: number, receiverType: number): Promise<{ exists: boolean; conversationId?: number } | null> {
        try {
            const response = await this.apiClient.post<{ exists: boolean; conversationId?: number }>(API_ENDPOINTS.conversation.checkExists, {
                Sender: {
                    ProfileId: senderProfileId,
                    Type: senderType,
                },
                Receiver: {
                    ProfileId: receiverProfileId,
                    Type: receiverType,
                },
            });
            return response ? { exists: response.exists ?? false, conversationId: response.conversationId } : null;
        } catch (error) {
            console.error("Error checking conversation:", error);
            return null;
        }
    }
    
}


export default new ConversationService();