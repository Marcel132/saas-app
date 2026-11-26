import { inject, Injectable, signal } from "@angular/core";
import { catchError, finalize, switchMap, tap, throwError } from "rxjs";

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

  private readonly _request = signal<RequestState>({
    state: 'idle',
    message: ''
  })
  private readonly _currentUser = signal<CurrentUser | null>(null);

  readonly request = this._request.asReadonly();
  readonly currentUser = this._currentUser.asReadonly();

  clearRequestState() {
    this._request.set({
      state: 'idle',
      message: ''
    })
  }

  login(request: LoginRequest) {
    this._request.set({
      state: 'loading',
      message: 'Logowanie...'
    })
    return this.authApiService.login(request).pipe(
      switchMap(response => this.loadCurrentUser().pipe(
        tap(() => this._request.set({
          state: "success",
          message: response.message ?? "Zalogowano"
        }))
      )),
      catchError(err => {
        this._request.set({
          state: "error",
          message: err?.error?.message ?? 'Wystąpił nieoczekiwany błąd'
        });
        return throwError(() => err);
      })
    )
  }

  registerPentester(request: RegisterPentesterRequest) {
    this._request.set({
      state: 'loading',
      message: 'Rejestracja...'
    })

    return this.authApiService.registerPentester(request)
      .pipe(
        switchMap(response => this.loadCurrentUser().pipe(
          tap(() => this._request.set({
            state: 'success',
            message: response.message ?? "Zarejestrowano"
          }))
        )),
        catchError(err => {
          this._request.set({
            state: 'error',
            message: err?.error?.message ?? 'Wystąpił nieoczekiwany błąd'
          })
          return throwError(() => err)
        })
      )
  }

  registerCompany(request: RegisterCompanyRequest) {
    this._request.set({
      state: 'loading',
      message: 'Rejestracja...'
    })


    return this.authApiService.registerCompany(request)
      .pipe(
        switchMap(response => this.loadCurrentUser().pipe(
          tap(() => this._request.set({
            state: 'success',
            message: response.message ?? "Zarejestrowano"
          }))
        )),
        catchError(err => {
          this._request.set({
            state: 'error',
            message: err?.error?.message ?? 'Wystąpił nieoczekiwany błąd'
          })
          return throwError(() => err)
        })
      )
  }

  loadCurrentUser() {
    return this.userApiService.getCurrentUser().pipe(
      tap(response => {
        this._currentUser.set(response.data);
      })
    )
  }

  clearCurrentUser() {
    return this._currentUser.set(null);
  }

  logout() {
    return this.authApiService.logout().pipe(
      finalize(() => this.clearCurrentUser())
    )
  }

}
