import { toast } from "sonner";

export const toastSuccess= (title:string, desc:string) => {
    toast.success(title, {
        description: desc,
        duration: 5000,
        action: {
            label: "Close",
            onClick: () => {
                toast.dismiss();
            },
        },
        style: {
            backgroundColor: "#4caf50",
            color: "#fff",
        },
    });
}

export const toastError= (title:string, desc:string) => {
    toast.error(title, {
        description: desc,
        duration: 5000,
        action: {
            label: "Close",
            onClick: () => {
                toast.dismiss();
            },
        },
        style: {
            backgroundColor: "#f44336",
            color: "#fff",
        },
    });
}