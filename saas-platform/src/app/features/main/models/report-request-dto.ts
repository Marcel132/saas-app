import { ContractRequestStatus } from "../../../shared/models/contract-requet-status";

export interface ReportRequestDto{
  requestId: number,
  title: string,
  status: ContractRequestStatus
}
