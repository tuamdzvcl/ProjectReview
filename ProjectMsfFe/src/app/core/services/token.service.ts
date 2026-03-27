import { Injectable } from '@angular/core';

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

  clear() {
    localStorage.clear();
  }
}