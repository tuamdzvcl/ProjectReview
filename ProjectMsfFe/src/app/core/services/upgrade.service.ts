import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseApiService } from './base-api.service';
import { UpgradeRequest, UpgradeResponse } from '../model/response/upgrade.model';
import { Observable } from 'rxjs';
import { PageResult } from '../model/base/api-page-response.model';

@Injectable({
  providedIn: 'root'
})
export class UpgradeService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }

  getAll(params: any): Observable<PageResult<UpgradeResponse>> {
    return this.getpage<UpgradeResponse>('upgrade/all', params);
  }

  register(upgradeId: number): Observable<any> {
    // End point: POST /api/upgrade
    // Body: { id: upgradeId } based on UpdateUserToOrganizer model in backend
    return this.post<any>('upgrade', { id: upgradeId });
  }

  create(data: UpgradeRequest): Observable<UpgradeResponse> {
    return this.post<UpgradeResponse>('upgrade/admin', data);
  }

  update(id: number, data: UpgradeRequest): Observable<UpgradeResponse> {
    return this.putById<UpgradeResponse>('upgrade/admin', id, data);
  }

  getPackageById(id: number): Observable<UpgradeResponse> {
    return this.get<UpgradeResponse>(`upgrade/admin/${id}`);
  }

  deleteUpgrade(id: number): Observable<any> {
    return this.deleteById('upgrade/admin', id);
  }
}
