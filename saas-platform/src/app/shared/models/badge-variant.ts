export const BadgeVariant = {
  accepted: 'accepted',
  cancelled: 'cancelled',
  completed: 'completed',
  draft: 'draft',
  error: 'error',
  idle: 'idle',
  info: 'info',
  inProgress: 'in-progress',
  open: 'open',
  pending: 'pending',
  primary: 'primary',
  rejected: 'rejected',
  success: 'success',
  warning: 'warning'
} as const

export type BadgeVariant =
  typeof BadgeVariant[keyof typeof BadgeVariant]
