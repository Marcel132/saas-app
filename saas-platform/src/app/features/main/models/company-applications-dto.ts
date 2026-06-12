import { ApplicationStatus } from "../../../shared/models/application-status";

export interface CompanyApplicationsDto {
  applicationId: number,
  candidatedId: string,
  contractId: number,
  firstName: string,
  lastName: string,
  nickname: string,
  status: ApplicationStatus
  appliedAt: Date,
}
