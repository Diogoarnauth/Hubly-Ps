import { ApiClient } from "./apiClient";
import { API_ENDPOINTS } from "./apiEndpoints";
import  ConversationTagDTO  from "../DTO/conversation/ConversationSummaryOutputModel";

export interface CreateTagData {
    conversationId: string | number;
    tagName: string;
    colorHex: string;
}

export interface UpdateTagData {
    tagName: string;
    colorHex: string;
}

class ConversationTagService {
    private apiClient = new ApiClient();

    async getUserTags(): Promise<ConversationTagDTO[]> {
        try {
            const response = await this.apiClient.get<ConversationTagDTO[]>(API_ENDPOINTS.conversationTags.getUserTags);
            return response || [];
        } catch (error) {
            console.error("Hubly: Tags not found:", error);
            return [];
        }
    }

    async getConversationTags(conversationId: number): Promise<ConversationTagDTO[]> {
        try {
            const url = API_ENDPOINTS.conversationTags.getConversationTags.replace("{conversationId}", conversationId.toString());
            const response = await this.apiClient.get<ConversationTagDTO[]>(url);
            return response || [];
        } catch (error) {
            console.error("Hubly: Conversation tags not found:", error);
            return [];
        }
    }

    async createTag(data: CreateTagData) {
        try {
            const { conversationId, tagName, colorHex } = data;
            const payload = { tagName, colorHex };
            const response = await this.apiClient.post(API_ENDPOINTS.conversationTags.createTag, payload);
            return { success: true, data: response };
        } catch (error: any) {
            return { 
                success: false, 
                message: error.message || "Tag not created" 
            };
        }
    }

    async updateTag(tagId: number, data: UpdateTagData) {
        try {
            const url = API_ENDPOINTS.conversationTags.updateTag.replace("{tagId}", tagId.toString());
            const response = await this.apiClient.put(url, data);
            return { success: true, data: response };
        } catch (error: any) {
            return { success: false, message: error.message || "Tag not updated" };
        }
    }

    async deleteTag(tagId: number): Promise<boolean> {
        try {
            const url = API_ENDPOINTS.conversationTags.deleteTag.replace("{tagId}", tagId.toString());
            await this.apiClient.delete(url);
            return true;
        } catch (error) {
            console.error("Hubly: Error deleting tag:", error);
            return false;
        }
    }

    async tagConversation(conversationId: number, tagId: number): Promise<boolean> {
        try {
            const url = API_ENDPOINTS.conversationTags.tagConversation.replace("{conversationId}", conversationId.toString());
            await this.apiClient.post(url, { tagId });
            return true;
        } catch (error) {
            console.error("Hubly: Error tagging conversation:", error);
            return false;
        }
    }

    async untagConversation(conversationId: number): Promise<boolean> {
        try {
            const url = API_ENDPOINTS.conversationTags.untagConversation.replace("{conversationId}", conversationId.toString());
            await this.apiClient.post(url, {});
            return true;
        } catch (error) {
            console.error("Hubly: Error untagging conversation:", error);
            return false;
        }
    }
}

export default new ConversationTagService();