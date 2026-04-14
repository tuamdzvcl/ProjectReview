import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { PageResult } from '../model/base/api-page-response.model';
import { AuditLog } from '../model/audit-log.model';

@Injectable({
  providedIn: 'root'
})
export class AuditLogService extends BaseApiService {
  getAuditLogs(pageIndex: number, pageSize: number, username?: string, path?: string): Observable<PageResult<AuditLog>> {
    const params: any = {
      pageIndex: pageIndex,
      pageSize: pageSize
    };

    if (username) params.username = username;
    if (path) params.path = path;

    return this.getpage<AuditLog>('AuditLog', params);
  }
}
