import { inject, Injectable, signal } from "@angular/core";
import { catchError, switchMap, tap, throwError } from "rxjs";

import { AuthApi } from "../../../core/services/auth-api";
import { LoginRequest } from "../models/login-request";
import { RegisterRequest } from "../models/register-request";
import { CurrentUserDto } from "../models/current-user-dto";
import { UserApi } from "../../../core/services/user-api";
import { RequestState } from "../../../core/models/request-state";

@Injectable({
  providedIn: 'root'
})
export class AuthStore {
  private readonly authApiService = inject(AuthApi)
  private readonly userApiService = inject(UserApi)

  readonly request = signal<RequestState>({
    state: 'idle',
    message: ''
  })

  readonly currentUser = signal<CurrentUserDto | null>(null);


  login(request: LoginRequest) {
    this.request.set({
      state: 'loading',
      message: 'Logowanie...'
    })
    return this.authApiService.login(request).pipe(
      switchMap(response => this.loadCurrentUser().pipe(
        tap(() => this.request.set({
          state: "success",
          message: response.message ?? "Zalogowano"
        }))
      )),
      catchError(err => {
        this.request.set({
          state: "error",
          message: err.error.message
        });
        return throwError(() => err);
      })
    )
  }

  register(request: RegisterRequest) {
    this.request.set({
      state: 'loading',
      message: 'Rejestracja...'
    })
    return this.authApiService.register(request).pipe(
      switchMap(response => this.loadCurrentUser().pipe(
        tap(() => this.request.set({
          state: 'success',
          message: response.message ?? "Zarejestrowano"
        }))
      )),
      catchError(err => {
        this.request.set({
          state: 'error',
          message: err.error.message
        })
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
}
