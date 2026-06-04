export interface ApiResponseModel<T>
{
  success: boolean,
  state: string,
  message: string | null,
  code: string | null,
  data: T | null
}
