import { inject, Injectable, signal } from "@angular/core";
import { catchError, switchMap, tap, throwError } from "rxjs";

import { AuthApi } from "../../../core/services/auth-api";
import { LoginRequest } from "../models/login-request";
import { RegisterRequest } from "../models/register-request";
import { CurrentUserDto } from "../models/user-dto";
import { UserApi } from "../../../core/services/user-api";

@Injectable({
  providedIn: 'root'
})
export class AuthStore {
  // TODO: Change isLoading, error and success signal into status and message
  private readonly authApiService = inject(AuthApi)
  private readonly userApiService = inject(UserApi)

  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);
  readonly isLoading = signal<boolean>(false);

  readonly currentUser = signal<CurrentUserDto | null>(null);


  login(request: LoginRequest) {
    this.clearSignals();
    this.isLoading.set(true);

    return this.authApiService.login(request).pipe(
      switchMap(response => this.loadCurrentUser().pipe(
        tap(() => this.setSuccess(response.message))
      )),
      catchError(err => {
        this.setError(err.error.message);
        return throwError(() => err);
      })
    )
  }

  register(request: RegisterRequest) {
    this.clearSignals();
    this.isLoading.set(true);

    return this.authApiService.register(request).pipe(
      switchMap(response => this.loadCurrentUser().pipe(
        tap(() => this.setSuccess(response.message))
      )),
      catchError(err => {
        this.setError(err.error.message);
        return throwError(() => err)
      })
    )
  };


  loadCurrentUser() {
    return this.userApiService.getCurrentUser().pipe(
      tap(response => {
        this.currentUser.set(response.data);
      })
    )
  }

  logout() {
    return this.authApiService.logout().pipe(
      tap(() => this.currentUser.set(null))
    )
  }

  private clearSignals() {
    this.isLoading.set(false);
    this.error.set(null);
    this.success.set(null)
  }

  private setSuccess(message: string | null){
    this.isLoading.set(false)
    this.success.set(message)
  }
  private setError(message: string | null){
    this.isLoading.set(false)
    this.error.set(message)
  }
}
