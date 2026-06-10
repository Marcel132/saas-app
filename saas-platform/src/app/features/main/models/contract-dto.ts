export interface ContractDto {
  authorId: string;
  contractId: number;
  contractStatus: string;
  createdAt: string;
  deadline: string;
  description: string;
  price: number;
  title: string;
  updatedAt: string | null;
  hasApplied: boolean
}
