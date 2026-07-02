import { ContractStatus } from "../../../../shared/models/contract-status";

export interface BaseContractDto {
  contractId: number;
  title: string;
  description: string;
  pricePerRequest: number;
  maxBudget: number;
  maxRequests: number;
  contractStatus: ContractStatus;
  createdAt: string;
  updatedAt: string | null;
  deadline: string;
}
