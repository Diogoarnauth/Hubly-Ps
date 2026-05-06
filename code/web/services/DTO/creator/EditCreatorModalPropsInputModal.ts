export interface EditCreatorModalPropsInputModel{
  currentUsername: string;
  currentArtisticName: string; 
  currentStatus: string;
  onClose: () => void;
  onSuccess: () => void;
}

export default EditCreatorModalPropsInputModel;