import { environment } from "../../../environments/environment"

const domain = environment.apiUrl
const version = 'v1'

export const ApiEndpoints = {
  auth: {
    login: `${domain}/${version}/auth/login`,
    register: `${domain}/${version}/auth/register`,
    logout: `${domain}/${version}/auth/logout`
  },
  users: {
    currentUser: `${domain}/${version}/users/me`
  }
}
