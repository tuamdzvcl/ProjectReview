import {
  HttpInterceptorFn,
} from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith(environment.apiBaseUrl)) return next(req);

  const token = localStorage.getItem('access_token');

  if (!token) return next(req);

  const cloned = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
  });

  return next(cloned);
};
