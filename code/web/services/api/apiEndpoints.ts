const API_BASE_URL = "http://localhost:80/api"; 

export const API_ENDPOINTS = {
   
    user: {
        login: `${API_BASE_URL}/users/token`,
        register: `${API_BASE_URL}/users`,
        verifyEmail: `${API_BASE_URL}/users/verifyEmail`,
        resendEmailConfirmation: `${API_BASE_URL}/users/resendEmailConfirmation`,
        checkProfile: `${API_BASE_URL}/users/profile/CheckCreatorOrCompany`
    },
    
    creator: {
        register: `${API_BASE_URL}/creator`, 
        trending: `${API_BASE_URL}/creator/trending`,
        search: `${API_BASE_URL}/creators`,
        addSocialProfile: `${API_BASE_URL}/creator/socialProfile`,
    },

    company: {
        register: `${API_BASE_URL}/company`, 
        trending: `${API_BASE_URL}/company/trending`,
        search: `${API_BASE_URL}/company/`,


    },
    sectors: {
        getAll: `${API_BASE_URL}/sectors`,
    },
    socialPlatforms: {
        getAll: `${API_BASE_URL}/socialPlatform`,
    },
    countries: {
        getCountries:`${API_BASE_URL}/getCountries`,
    }
}