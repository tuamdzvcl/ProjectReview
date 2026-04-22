import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { PermissionItem } from '../../features/roles/models/role.model';

@Injectable({
  providedIn: 'root'
})
export class PermissionService extends BaseApiService {
  
  getPermissions(): Observable<PermissionItem[]> {
    return this.get<PermissionItem[]>('permission');
  }
}
