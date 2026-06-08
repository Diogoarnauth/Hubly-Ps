export interface PagedResponse<T> {
    items: T[];
    totalItems: number;
    page: number;
    pageSize: number;
    totalPages: number;
}
