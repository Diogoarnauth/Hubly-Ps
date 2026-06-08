export interface ProfileHistoryOutputModel {
    id: number;
    viewedAt: string;
    targetId: number;
    targetType: 'Company' | 'CreatorSocialProfile';
    targetName: string;
}
