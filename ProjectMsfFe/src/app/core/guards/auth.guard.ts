import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { AuthService } from '../../features/auth/auth.service';
import { PermissionStoreService } from '../services/permission-store.service';

export const authGuard: CanActivateFn = async (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);
  const authService = inject(AuthService);
  const permissionStore = inject(PermissionStoreService);

  const requiresAuth = route.data?.['requiresAuth'] === true;
  if (!requiresAuth) return true;

  const accessToken = tokenService.getAccessToken();
  if (!accessToken || authService.checkTokenExpired()) {
    tokenService.clear();
    return router.createUrlTree(['/auth/login'], {
      queryParams: { returnUrl: state.url },
    });
  }

  // Load permissions nếu chưa load
  if (!permissionStore.loaded()) {
    await permissionStore.loadPermissions();
  }

  // Check permissions nếu route có khai báo
  const requiredPermissions: string[] = route.data?.['permissions'];
  if (requiredPermissions && requiredPermissions.length > 0) {
    if (!permissionStore.hasAnyPermission(requiredPermissions)) {
      return router.createUrlTree(['notFound']);
    }
  }

  return true;
};
