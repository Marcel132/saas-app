import { ApplicationStatus } from "../../../shared/models/application-status";

export interface CompanyApplicationsDto {
  applicationId: number,
  candidatedId: string,
  contractId: number,
  firstName: string,
  lastName: string,
  nickName: string,
  status: ApplicationStatus
  appliedAt: Date,
}
