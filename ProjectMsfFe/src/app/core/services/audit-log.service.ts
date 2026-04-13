import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuditLogResponse } from '../model/audit-log.model';

@Injectable({
  providedIn: 'root',
})
export class AuditLogService {
  private apiUrl = `${environment.apiBaseUrl}/AuditLog`;

  constructor(private http: HttpClient) {}

  getAuditLogs(
    pageIndex: number,
    pageSize: number,
    username?: string,
    path?: string
  ): Observable<AuditLogResponse> {
    let params = new HttpParams()
      .set('pageIndex', pageIndex.toString())
      .set('pageSize', pageSize.toString());

    if (username) params = params.set('username', username);
    if (path) params = params.set('path', path);

    return this.http
      .get<any>(this.apiUrl, { params })
      .pipe(map((res) => res.Data));
  }
}
