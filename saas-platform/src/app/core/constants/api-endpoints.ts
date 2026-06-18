import { environment } from "../../../environments/environment"

const domain = environment.apiUrl
const version = 'v1'

export const ApiEndpoints = {
  auth: {
    login: `${domain}/${version}/auth/login`,
    register: `${domain}/${version}/auth/register`,
    refreshToken: `${domain}/${version}/auth/refresh-token`,
    logout: `${domain}/${version}/auth/logout`
  },
  users: {
    currentUser: `${domain}/${version}/users/me`,
    summary: `${domain}/${version}/users/me/summary`
  },
  contracts: {
    base: `${domain}/${version}/contracts`,
    update: (contractId: number) => `${domain}/${version}/contracts/${contractId}`,
    close: (contractId: number) => `${domain}/${version}/contracts/${contractId}/close`,

    public: `${domain}/${version}/contracts/public`,
    company: `${domain}/${version}/contracts/company`,
    pentester: `${domain}/${version}/contracts`,

    byId: (contractId: number) => `${domain}/${version}/contracts/${contractId}`,

    applications: (contractId: number) => `${domain}/${version}/contracts/${contractId}/applications`,
    apply: (contractId: number) => `${domain}/${version}/contracts/${contractId}/apply`,
  },
  applications: {
    base: `${domain}/${version}/applications`,

    accept: (applicationId: number) => `${domain}/${version}/applications/${applicationId}/accept`,
    reject: (applicationId: number) => `${domain}/${version}/applications/${applicationId}/reject`,

    myApplications: `${domain}/${version}/applications/me`
  }
}
