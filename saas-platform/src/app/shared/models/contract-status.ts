export const ContractStatus = {
  Open: 'Open',
  InProgress: 'InProgress',
  Completed: 'Completed',
  Cancelled: 'Cancelled'
} as const;

export type ContractStatus =
  typeof ContractStatus[keyof typeof ContractStatus]
