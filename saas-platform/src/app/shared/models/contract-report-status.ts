export const ContractReportStatus = {
  Draft: 'Draft',
  Submitted: 'Submitted',
  ChangesRequested: 'ChangesRequested',
  Approved: 'Approved',
  Rejected: 'Rejected'
} as const;

export type ContractReportStatus =
  typeof ContractReportStatus[keyof typeof ContractReportStatus];
