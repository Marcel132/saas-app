import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { LoginRequest } from '../../features/auth/models/login-request';
import { RegisterRequest } from '../../features/auth/models/register-request';
import { ApiEndpoints } from '../constants/api-endpoints';
import { ApiResponseModel } from '../models/api-response-model';

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

  register(request: RegisterRequest) {
    return this.http.post<ApiResponseModel<null>>(
      ApiEndpoints.auth.register,
      request,
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
