import { inject, Injectable, signal } from "@angular/core";
import { catchError, finalize, switchMap, tap, throwError } from "rxjs";

import { AuthApi } from "../../../core/services/auth-api";
import { LoginRequest } from "../models/login-request";
import { UserApi } from "../../../core/services/user-api";
import { RegisterPentesterRequest } from "../models/register-pentester-request";
import { RegisterCompanyRequest } from "../models/register-company-request";
import { CurrentUser } from "../models/current-user/current-user-base";
import { createRequestState } from "../../../core/utils/create-request-state";

@Injectable({
  providedIn: 'root'
})
export class AuthStore {
  private readonly authApiService = inject(AuthApi)
  private readonly userApiService = inject(UserApi)

  private readonly _requestState = createRequestState();

  private readonly _currentUser = signal<CurrentUser | null>(null);

  readonly requestState = this._requestState.state
  readonly clearRequestState = this._requestState.clear
  readonly currentUser = this._currentUser.asReadonly();

  login(request: LoginRequest) {
    this._requestState.set('loading', 'Logowanie...')

    return this.authApiService.login(request).pipe(
      switchMap(response => this.loadCurrentUser().pipe(
        tap(() => this._requestState.set('success', response.message ?? 'Zalogowano'))
      )
      ),
      catchError(err => {
        this._requestState.set('error', err?.error?.message)
        return throwError(() => err);
      })
    )
  }

  registerPentester(request: RegisterPentesterRequest) {
    this._requestState.set('loading', 'Rejestracja...')

    return this.authApiService.registerPentester(request)
      .pipe(
        switchMap(response => this.loadCurrentUser().pipe(
          tap(() => this._requestState.set('success', response.message ?? 'Zarejestrowano'))
        )),
        catchError(err => {
          this._requestState.set('error', err?.error?.message)
          return throwError(() => err)
        })
      )
  }

  registerCompany(request: RegisterCompanyRequest) {
    this._requestState.set('loading', 'Rejestracja...')

    return this.authApiService.registerCompany(request)
      .pipe(
        switchMap(response => this.loadCurrentUser().pipe(
          tap(() => this._requestState.set('success', response.message ?? 'Zarejestrowano'))
        )),
        catchError(err => {
          this._requestState.set('error', err?.error?.message)
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
