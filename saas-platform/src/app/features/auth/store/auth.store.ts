import { inject, Injectable, signal } from "@angular/core";
import { catchError, switchMap, tap, throwError } from "rxjs";

import { AuthApi } from "../../../core/services/auth-api";
import { LoginRequest } from "../models/login-request";
import { UserApi } from "../../../core/services/user-api";
import { RequestState } from "../../../core/models/request-state";
import { RegisterPentesterRequest } from "../models/register-pentester-request";
import { RegisterCompanyRequest } from "../models/register-company-request";
import { CurrentUser } from "../models/current-user/current-user-base";

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

  readonly currentUser = signal<CurrentUser | null>(null);

  clearRequestState() {
    this.request.set({
      state: 'idle',
      message: ''
    })
  }

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

  registerPentester(request: RegisterPentesterRequest) {
    this.request.set({
      state: 'loading',
      message: 'Rejestracja...'
    })

    return this.authApiService.registerPentester(request)
      .pipe(
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
      .subscribe();
  }

  registerCompany(request: RegisterCompanyRequest) {
    this.request.set({
      state: 'loading',
      message: 'Rejestracja...'
    })

    return this.authApiService.registerCompany(request)
      .pipe(
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
      .subscribe()
  }

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
