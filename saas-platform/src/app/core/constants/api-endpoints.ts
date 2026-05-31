import { environment } from "../../../environments/environment"

const domain = environment.apiUrl
const version = 'v1'

export const ApiEndpoints = {
  auth: {
    login: `${domain}/${version}/auth/login`,
    register: `${domain}/${version}/auth/register`,
  }
}
