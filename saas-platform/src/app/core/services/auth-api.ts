import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { LoginRequest } from '../../features/auth/models/login-request';
import { ApiEndpoints } from '../constants/api-endpoints';
import { ApiResponseModel } from '../models/api-response-model';
import { RegisterPentesterRequest } from '../../features/auth/models/register-pentester-request';
import { RegisterCompanyRequest } from '../../features/auth/models/register-company-request';

@Injectable({
  providedIn: 'root',
})
export class AuthApi {
  private readonly http = inject(HttpClient)

  login(request: LoginRequest) {
    return this.http.post<ApiResponseModel<null>>(
      ApiEndpoints.auth.login,
      request,
    )
  }

  registerPentester(request: RegisterPentesterRequest) {
    return this.http.post<ApiResponseModel<null>>(
      ApiEndpoints.auth.registerPentester,
      request
    )
  }
  registerCompany(request: RegisterCompanyRequest) {
    return this.http.post<ApiResponseModel<null>>(
      ApiEndpoints.auth.registerCompany,
      request
    )
  }

  refreshToken() {
    return this.http.post<ApiResponseModel<null>>(
      ApiEndpoints.auth.refreshToken,
      {},
    )
  }

  logout() {
    return this.http.post<ApiResponseModel<null>>(
      ApiEndpoints.auth.logout,
      {},
    )
  }
}
