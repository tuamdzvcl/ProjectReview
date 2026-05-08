import { Injectable } from '@angular/core';
import { Observable, tap, catchError, throwError } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { BaseApiService } from '../../core/services/base-api.service';
import { TokenService } from '../../core/services/token.service';
import { AuthData } from '../../core/model/response/auth-data.model';
import { jwtDecode } from 'jwt-decode';
import { PermissionStoreService } from '../../core/services/permission-store.service';

@Injectable({ providedIn: 'root' })
export class AuthService extends BaseApiService {
  constructor(http: HttpClient, private tokenService: TokenService, private permissionStore: PermissionStoreService) {
    super(http);
  }

  login(data: { email: string; password: string }) {
    return this.post<AuthData>('auth/login', data).pipe(
      tap((res) => {
        console.log('[Login] Response:', res);
        console.log('[Login] AccessToken:', res.AccessToken ? 'EXISTS' : 'NULL');
        console.log('[Login] RefreshToken:', res.RefreshToken ? 'EXISTS' : 'NULL');
        this.tokenService.setToken(res.AccessToken, res.RefreshToken);
      })
    );
  }

  register(data: any) {
    return this.post<any>('auth/register', data);
  }

  getGoogleResult(key: string) {
    return this.post<any>('auth/google-result', {
      key: key,
    });
  }

  refreshToken(refreshToken: string): Observable<AuthData> {
    console.log('[AuthService.refreshToken] Sending refresh request with token:', refreshToken?.substring(0, 20) + '...');
    console.log('[AuthService.refreshToken] Request body:', JSON.stringify({ RefreshToken: refreshToken }));

    return this.post<AuthData>('auth/refresh-token', {
      RefreshToken: refreshToken,
    }).pipe(
      tap((res) => {
        console.log('[AuthService.refreshToken] SUCCESS response:', res);
        this.tokenService.setToken(res.AccessToken, res.RefreshToken);
      }),
      catchError((err) => {
        console.log('[AuthService.refreshToken] ERROR:', err);
        return throwError(() => err);
      })
    );
  }

  logout() {
    const refreshToken = this.tokenService.getRefreshToken();

    if (refreshToken) {
      this.post<any>('auth/logout', { RefreshToken: refreshToken }).subscribe({
        error: () => { },
      });
    }

    this.tokenService.clear();
    this.permissionStore.clear();
  }

  getUser() {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  checkTokenExpired() {
    const token = this.tokenService.getAccessToken();
    if (!token) return true;

    const decoded: any = jwtDecode(token);
    const now = Math.floor(Date.now() / 1000);

    return decoded.exp < now;
  }

  forgotPassword(email: string) {
    return this.post<any>('auth/forgot-password', { email });
  }

  resetPassword(data: any) {
    return this.post<any>('auth/reset-password', data);
  }

  verifyEmail(token: string) {
    return this.post<any>('auth/verify-email', { token });
  }

  resendVerification(email: string) {
    return this.post<any>('auth/resend-verification', { email });
  }
}
