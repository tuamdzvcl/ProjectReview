import {
  HttpErrorResponse,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  BehaviorSubject,
  catchError,
  filter,
  Observable,
  switchMap,
  take,
  throwError,
} from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TokenService } from '../../services/token.service';
import { AuthService } from '../../../features/auth/auth.service';

let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

function addToken(req: HttpRequest<unknown>): HttpRequest<unknown> {
  const token = localStorage.getItem('access_token');
  if (!token) return req;
  return req.clone({
    setHeaders: { Authorization: `Bearer ${token}` },
  });
}

export const AuthInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
) => {
  if (!req.url.startsWith(environment.apiBaseUrl)) return next(req);

  if (
    req.url.includes('/auth/refresh-token') ||
    req.url.includes('/auth/login')
  ) {
    return next(req);
  }

  const tokenService = inject(TokenService);
  const authService = inject(AuthService);
  const router = inject(Router);

  const authReq = addToken(req);

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401) {
        return throwError(() => error);
      }

      const refreshToken = tokenService.getRefreshToken();

      if (!refreshToken) {
        handleLogout(authService, router);
        return throwError(() => error);
      }

      if (isRefreshing) {
        return waitForRefreshAndRetry(req, next);
      }

      isRefreshing = true;
      refreshTokenSubject.next(null);

      return authService.refreshToken(refreshToken).pipe(
        switchMap((authData) => {
          isRefreshing = false;
          tokenService.setToken(authData.AccessToken, authData.RefreshToken);
          refreshTokenSubject.next(authData.AccessToken);
          return next(addToken(req));
        }),
        catchError((refreshError) => {
          isRefreshing = false;
          refreshTokenSubject.next(null);
          handleLogout(authService, router);
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};

function waitForRefreshAndRetry(
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<any> {
  return refreshTokenSubject.pipe(
    filter((token) => token !== null),
    take(1),
    switchMap(() => next(addToken(req))),
  );
}

function handleLogout(authService: AuthService, router: Router): void {
  authService.logout();
  router.navigate(['/auth/login'], { queryParams: { sessionExpired: true } });
}
