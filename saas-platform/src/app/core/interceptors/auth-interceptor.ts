import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { AuthApi } from "../services/auth-api";
import { catchError, switchMap, throwError } from "rxjs";
import { ApiEndpoints } from "../constants/api-endpoints";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authApi = inject(AuthApi);
  const exludedEndpoints = [
    ApiEndpoints.auth.login,
    ApiEndpoints.auth.registerCompany,
    ApiEndpoints.auth.registerPentester,
    ApiEndpoints.auth.refreshToken
  ]
  if (exludedEndpoints.some(x => req.url.includes(x))) {
    return next(req)
  }

  return next(req).pipe(
    catchError(error => {
      if (error.status !== 401) {
        return throwError(() => error);
      }

      return authApi.refreshToken().pipe(
        switchMap(() => next(req))
        // TODO: add local logout when request sent 401
      );
    })
  );
};
