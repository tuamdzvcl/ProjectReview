import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { AuthService } from '../../features/auth/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);
  const authService = inject(AuthService);

  const requiresAuth = route.data?.['requiresAuth'] === true;
  if (!requiresAuth) return true;

  const accessToken = tokenService.getAccessToken();
  if (accessToken && !authService.checkTokenExpired()) {
    
    const protectedRoutes = [
      {
        urls: ['/admin/user', '/admin/UserForm'],
        allowedRoles: ['ADMIN'],
      },
      {
        urls: ['/admin/'],
        allowedRoles: ['ADMIN', 'ORGANIZER'],
      },
      {
        urls: ['/event-create-page'],
        allowedRoles: ['ADMIN', 'ORGANIZER'],
      },
    ];

    const currentRoute = protectedRoutes.find((route) =>
      route.urls.some((url) => state.url.includes(url))
    );

    if (currentRoute) {
      const userRole = tokenService.getRole();

      if (userRole && currentRoute.allowedRoles.includes(userRole)) {
        return true;
      } else {
        alert('Làm gì có quyền mà vào vậy?');
        return router.createUrlTree(['/']);
      }
    }

    return true; 
  }

  tokenService.clear();

  return router.createUrlTree(['/auth/login'], {
    queryParams: { returnUrl: state.url },
  });
};
