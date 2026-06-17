import { ContractStatus } from "../../../../shared/models/contract-status";

export interface BaseContractDto {
  contractId: number;
  contractStatus: ContractStatus;
  createdAt: string;
  deadline: string;
  description: string;
  price: number;
  title: string;
  updatedAt: string | null;
}
