export const ApplicationStatus = {
 Accepted: 'Accepted',
 Pending: 'Pending',
 Rejected: 'Rejected'
} as const

export type ApplicationStatus =
  typeof ApplicationStatus[keyof typeof ApplicationStatus]
