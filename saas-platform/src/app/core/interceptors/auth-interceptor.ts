import {HttpHandler,HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { AuthApi } from "../services/auth-api";
import { catchError, switchMap, throwError } from "rxjs";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authApi = inject(AuthApi);

  return next(req).pipe(
    catchError(error => {
      if (error.status !== 401) {
        return throwError(() => error);
      }

      if (req.url.includes('refresh-token')) {
        return throwError(() => error);
      }

      return authApi.refreshToken().pipe(
        switchMap(() => next(req))
      );
    })
  );
};
