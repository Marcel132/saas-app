import { environment } from "../../../environments/environment"

const domain = environment.apiUrl;
const version = 'v1';

const authBase = `${domain}/${version}/auth`;
const usersBase = `${domain}/${version}/users`;
const contractsBase = `${domain}/${version}/contracts`;
const applicationsBase = `${domain}/${version}/applications`

export const ApiEndpoints = {
  auth: {
    login: `${authBase}/login`,
    registerPentester: `${authBase}/register/pentester`,
    registerCompany: `${authBase}/register/company`,
    refreshToken: `${authBase}/refresh-token`,
    logout: `${authBase}/logout`
  },
  users: {
    byId: (userId: string) => `${usersBase}/${userId}`,
    currentUser: `${usersBase}/me`,
    summary: `${usersBase}/me/summary`,
    delete: `${usersBase}/me`,
    updateCurrentUserPentester: `${usersBase}/me/pentester`,
    updateCurrentUserCompany: `${usersBase}/me/company`,
    currentUserContracts: `${usersBase}/me/contracts`,
    currentUserApplications: `${usersBase}/me/applications`
  },
  contracts: {
    public: `${contractsBase}/public`,
    byId: (contractId: number) => `${contractsBase}/${contractId}`,
    open: `${contractsBase}`,
    company: `${contractsBase}/company`,

    create: `${contractsBase}`,
    update: (contractId: number) => `${contractsBase}/${contractId}`,
    close: (contractId: number) => `${contractsBase}/${contractId}/close`,

    applications: (contractId: number) => `${contractsBase}/${contractId}/applications`,
    apply: (contractId: number) => `${contractsBase}/${contractId}/apply`,
  },
  applications: {
    accept: (applicationId: number) => `${applicationsBase}/${applicationId}/accept`,
    reject: (applicationId: number) => `${applicationsBase}/${applicationId}/reject`,
  }
}
