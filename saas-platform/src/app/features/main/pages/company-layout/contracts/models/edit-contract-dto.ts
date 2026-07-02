export interface EditContractDto {
  title: string,
  description: string,
  pricePerRequest: number,
  maxRequests: number,
  newDeadline: Date
}
