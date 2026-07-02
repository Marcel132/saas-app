import { ApplicationStatus } from "../../../shared/models/application-status";

export interface UserApplicationDto{
  applicationId: number,
  contractId: number,
  title: string,
  pricePerRequest: number,
  maxBudget: number,
  maxRequests: number,
  applicationStatus: ApplicationStatus,
  appliedAt: Date
}
