import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { RoleItem } from '../../features/roles/models/role.model';

@Injectable({
  providedIn: 'root'
})
export class RolePermissionService extends BaseApiService {
  
  getRolePermissions(): Observable<any[]> {
    return this.get<any[]>('role-permission');
  }

  createRolePermission(request: any): Observable<any> {
    return this.post<any>('role-permission', request);
  }

  updateRolePermission(id: number, request: any): Observable<any> {
    return this.put<any>(`role-permission/${id}`, request);
  }
}
