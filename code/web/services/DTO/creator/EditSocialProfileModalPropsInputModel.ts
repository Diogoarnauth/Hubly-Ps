import {GetSocialProfileOutputModel} from '@/services/DTO/creator/GetSocialProfileOutputModel';

export interface EditSocialProfileModalPropsInputModel {
    initialData: GetSocialProfileOutputModel;
    onClose: () => void;
    onSuccess: () => void;
}

export default EditSocialProfileModalPropsInputModel;