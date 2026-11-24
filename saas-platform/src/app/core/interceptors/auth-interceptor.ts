import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { AuthApi } from "../services/auth-api";
import { catchError, finalize, Observable, shareReplay, switchMap, throwError } from "rxjs";
import { ApiEndpoints } from "../constants/api-endpoints";
import { AuthStore } from "../../features/auth/store/auth.store";
import { Router } from "@angular/router";

const EXCLUDED_ENDPOINTS = [
  ApiEndpoints.auth.login,
  ApiEndpoints.auth.logout,
  ApiEndpoints.auth.registerCompany,
  ApiEndpoints.auth.registerPentester,
  ApiEndpoints.auth.refreshToken
]
let refreshInProgress$: Observable<unknown> | null = null;


export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authApi = inject(AuthApi);
  const authStore = inject(AuthStore);
  const router = inject(Router);

  if (EXCLUDED_ENDPOINTS.some(x => req.url.includes(x))) {
    return next(req)
  }

  return next(req).pipe(
    catchError(error => {
      // Odświeżamy TYLKO przy 401
      if (error.status !== 401) {
        return throwError(() => error);
      }

      if (!refreshInProgress$) {
        refreshInProgress$ = authApi.refreshToken().pipe(
          catchError(refreshError => {
            authStore.currentUser.set(null);
            router.navigate(['/auth/login']);
            return throwError(() => refreshError);
          }),
          shareReplay(1),
          finalize(() => (refreshInProgress$ = null))
        );
      }

      return refreshInProgress$.pipe(
        switchMap(() => next(req))
      );
    })
  );
};
