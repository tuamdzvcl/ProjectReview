import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  private ACCESS_TOKEN = 'access_token';
  private REFRESH_TOKEN = 'refresh_token';

  setToken(access: string, refresh: string) {
    localStorage.setItem(this.ACCESS_TOKEN, access);
    localStorage.setItem(this.REFRESH_TOKEN, refresh);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN);
  }

  getUserId(): string | null {
    const token = this.getAccessToken();
    if (!token) return null;
    try {
      const decoded: any = jwtDecode(token);
      return decoded.id || null;
    } catch (e) {
      return null;
    }
  }

  getRole(): string | null {
    const token = this.getAccessToken();
    if (!token) return null;
    try {
      const decoded: any = jwtDecode(token);
      const role = decoded.role || decoded.Role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      return role ? role.toString().toUpperCase() : null;
    } catch (e) {
      return null;
    }
  }

  clear() {
    localStorage.removeItem(this.ACCESS_TOKEN);
    localStorage.removeItem(this.REFRESH_TOKEN);
    localStorage.removeItem('user');
  }
}
