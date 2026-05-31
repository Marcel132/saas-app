import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { LoginRequest } from '../../features/auth/models/login-request';
import { RegisterRequest } from '../../features/auth/models/register-request';

@Injectable({
  providedIn: 'root',
})
export class AuthApi {
  private readonly http = inject(HttpClient)

  login(request: LoginRequest)
  {
    console.log("login...")
    return this.http.post<ApiResponseModel<object>>(
      'http://localhost:5149/api/v1/auth/login',
      request,
      {withCredentials: true}
    )
  }

  register(request: RegisterRequest)
  {
    console.log("register...")
    return this.http.post<ApiResponseModel<object>>(
      'http://localhost:5149/api/v1/auth/register',
      request,
      {withCredentials: true}
    )
  }

}
