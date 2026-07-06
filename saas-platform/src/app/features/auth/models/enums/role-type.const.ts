export const RoleTypeValues = {
  Company: 'Company',
  Pentester: 'Pentester'
} as const;

export type RoleType =
  typeof RoleTypeValues[keyof typeof RoleTypeValues];
