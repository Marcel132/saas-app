export interface PagedResponseModel<T> {
  page: number,
  pageSize: number,
  totalItems: number,
  totalPages: number,
  items: T[],
}
