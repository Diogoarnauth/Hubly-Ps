const API_BASE_URL = "http://localhost:80/api"; 

export const API_ENDPOINTS = {
   
    user: {
        login: `${API_BASE_URL}/users/token`,
        register: `${API_BASE_URL}/users`,
        verifyEmail: `${API_BASE_URL}/users/verifyEmail`,
        resendEmailConfirmation: `${API_BASE_URL}/users/resendEmailConfirmation`,
        checkProfile: `${API_BASE_URL}/users/profile/CheckCreatorOrCompany`,
        getMyInfo : `${API_BASE_URL}/users/profile/me`,
        logout : `${API_BASE_URL}/users/logout`,
        getFullCreatorProfile: (id: number) => `${API_BASE_URL}/users/${id}/fullCreatorProfile`, 
        getFullCompanyProfile: (id: number) => `${API_BASE_URL}/users/${id}/fullCompanyProfile`,
        edit: `${API_BASE_URL}/users/edit`
    },
    
    creator: {
        register: `${API_BASE_URL}/creator`, 
        trending: `${API_BASE_URL}/creator/trending`,
        getRecommendations: `${API_BASE_URL}/creators/getRecommendations`,
        search: `${API_BASE_URL}/creators`,
        addSocialProfile: `${API_BASE_URL}/creator/socialProfile`,
        status: `${API_BASE_URL}/creator/status`,
        getSocialProfileById: `${API_BASE_URL}/creator/socialProfile/{profileId}`,
        edit: `${API_BASE_URL}/creator/edit`,
        editSocialProfile: `${API_BASE_URL}/creator/socialProfile/edit/{socialProfileId}`,
        rateCreator: `${API_BASE_URL}/creator/rateCreator/{id}`,
        getRating:`${API_BASE_URL}/creators/{id}/my-rating`, 

    },

    company: {
        register: `${API_BASE_URL}/company`, 
        trending: `${API_BASE_URL}/company/trending`,
        getRecommendations: `${API_BASE_URL}/company/getRecommendations`,
        search: `${API_BASE_URL}/company/`,


        edit: `${API_BASE_URL}/company/edit`
    },
    sectors: {
        getAll: `${API_BASE_URL}/sectors`,
    },
    socialPlatforms: {
        getAll: `${API_BASE_URL}/socialPlatform`,
    },
    countries: {
        getCountries:`${API_BASE_URL}/getCountries`,
    },

    conversation: {
        getMessages: `${API_BASE_URL}/conversation/{conversationId}/messages`,
        sendMessage: `${API_BASE_URL}/conversation/{conversationId}/message`,
        create: `${API_BASE_URL}/conversation`,
        editMessage: `${API_BASE_URL}/conversation/message/edit/{messageId}`,
        deleteMessage: `${API_BASE_URL}/conversation/message/{messageId}`,
        getConversationsByProfileId: `${API_BASE_URL}/conversation/{socialProfileId}/my-conversations`,
        getConversationsByCompanyId: `${API_BASE_URL}/conversation/{companyId}/company-conversations`,
        checkExists: `${API_BASE_URL}/conversation/check-exists`,
        markMessagesAsRead: `${API_BASE_URL}/conversation/{conversationId}/mark-read/{lastMessageId}`,
        getUnreadMessageCount: `${API_BASE_URL}/conversation/{conversationId}/unread-count`
    },

    conversationTags: {
        createTag : `${API_BASE_URL}/conversation/tags`,
        getUserTags : `${API_BASE_URL}/conversation/tags/my-tags`,
        getConversationTags : `${API_BASE_URL}/conversation/{conversationId}/tags`,
        updateTag : `${API_BASE_URL}/conversation/tags/{tagId}`,
        deleteTag : `${API_BASE_URL}/conversation/tags/{tagId}`,
        tagConversation : `${API_BASE_URL}/conversation/{conversationId}/assign-tag`,
        untagConversation : `${API_BASE_URL}/conversation/{conversationId}/remove-tag`
    }
}