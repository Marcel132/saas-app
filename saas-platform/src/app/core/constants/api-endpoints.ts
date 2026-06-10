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
    contracts: `${domain}/${version}/contracts`,
  },
  me: {
    applications: `${domain}/${version}/me/applications`,
    contracts: `${domain}/${version}/me/contracts`
  }
}
