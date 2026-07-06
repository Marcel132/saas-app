export const SpecializationType = {
  WebSecurity: 'WebSecurity',
  ApiSecurity: 'ApiSecurity',
  MobileSecurity: 'MobileSecurity',
  CloudSecurity: 'CloudSecurity',
  InfrastructureSecurity: 'InfrastructureSecurity',
  RedTeam: 'RedTeam'
} as const;

export type SpecializationType =
  typeof SpecializationType[keyof typeof SpecializationType]
