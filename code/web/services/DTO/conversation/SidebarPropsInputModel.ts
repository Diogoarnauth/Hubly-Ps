export interface SidebarPropsInputModel {
    profileId: number;
    onSelectConversation: (id: number) => void;
    activeConversationId?: number;
    isCompany?: boolean;
}

export default SidebarPropsInputModel;