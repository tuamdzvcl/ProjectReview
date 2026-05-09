import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private config: any;

  constructor(private http: HttpClient) {}

  loadConfig(): Promise<any> {
    return lastValueFrom(
      this.http.get(`${environment.apiBaseUrl}/system-config`)
    )
      .then((data) => {
        this.config = data;
        console.log('System Configuration Loaded Successfully');
        return data;
      })
      .catch((err) => {
        console.error('Could not load system configuration', err);
        // Trả về default config nếu API lỗi để app không bị crash
        this.config = {
          Validation: {},
          Pagination: { DefaultPageSize: 10 }
        };
      });
  }

  get validation() {
    return this.config?.Validation;
  }

  get pagination() {
    return this.config?.Pagination;
  }

  getConfigValue(path: string): any {
    return path.split('.').reduce((obj, key) => obj?.[key], this.config);
  }
}
