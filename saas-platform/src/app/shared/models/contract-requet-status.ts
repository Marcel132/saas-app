export const ContractRequestStatus = {
  Created: 'created',
  Testing: 'testing',
  ReportSubmitted: 'report_submitted',
  Completed: 'completed'
} as const;

export type ContractRequestStatus =
  typeof ContractRequestStatus[keyof typeof ContractRequestStatus]
