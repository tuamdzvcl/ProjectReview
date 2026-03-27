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
    // Danh sách các URL cần kiểm tra role và role được phép
    const protectedRoutes = [
      {
        urls: ['/event-create-page'],
        allowedRoles: ['admin'.toUpperCase(), 'organizer'.toUpperCase()],
      },
      {
        urls: ['/user-management'],
        allowedRoles: ['admin'.toUpperCase()],
      },
      {
        urls: ['/settings'],
        allowedRoles: ['admin'.toUpperCase(), 'organizer'.toUpperCase()],
      },
      // Thêm các URL khác tại đây
      {
        urls: ['/reports'],
        allowedRoles: ['admin'.toUpperCase()],
      },
    ];

    const currentRoute = protectedRoutes.find((route) =>
      route.urls.some((url) => state.url.includes(url))
    );

    if (currentRoute) {
      try {
        const tokenPayload = JSON.parse(atob(accessToken.split('.')[1]));
        const userRole = tokenPayload.role || tokenPayload.Role;

        if (currentRoute.allowedRoles.includes(userRole)) {
          return true;
        } else {
          alert('làm gì có quyền mà vào vậy?');
          return router.createUrlTree(['/']);
        }
      } catch (error) {
        alert('có gì đó sai sai');
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
