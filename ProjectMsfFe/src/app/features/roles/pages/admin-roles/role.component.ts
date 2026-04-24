import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import Swal from 'sweetalert2';

import { RoleStatsComponent } from './components/role-stats/role-stats.component';
import { RoleTableComponent } from './components/role-table/role-table.component';
import { RoleFormDialogComponent } from './components/role-form-dialog/role-form-dialog.component';
import { RolePermissionDialogComponent } from './components/role-permission-dialog/role-permission-dialog.component';
import { PermissionItem, RoleItem, RoleSaveEvent, PermissionSaveEvent } from '../../models/role.model';
import { PermissionService } from '../../../../core/services/permission.service';
import { RolePermissionService } from '../../../../core/services/role-permission.service';
import { forkJoin } from 'rxjs';
import { ApiError } from '../../../../core/model/base/ApiError.model';
import { ApiErrorHandler } from '../../../../core/utils/api-error-handler.util';

@Component({
  selector: 'app-role',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RoleStatsComponent,
    RoleTableComponent,
    RoleFormDialogComponent,
    RolePermissionDialogComponent
  ],
  templateUrl: './role.component.html',
  styleUrl: './role.component.scss',
})
export class RoleComponent implements OnInit {
  private permissionService = inject(PermissionService);
  private rolePermissionService = inject(RolePermissionService);

  roles = signal<RoleItem[]>([]);
  allPermissions: PermissionItem[] = [];

  // Dialog states
  displayRoleDialog = false;
  displayPermDialog = false;
  permDialogReadonly = false;
  isEditMode = false;
  selectedRole: RoleItem | null = null;

  get systemRoleCount(): number {
    return this.roles().filter((r) => r.IsSystem).length;
  }

  get customRoleCount(): number {
    return this.roles().filter((r) => !r.IsSystem).length;
  }

  ngOnInit() {
    this.loadData();
  }

  private loadData() {
    forkJoin({
      allPerms: this.permissionService.getPermissions(),
      rolePerms: this.rolePermissionService.getRolePermissions()
    }).subscribe({
      next: (res) => {
        this.allPermissions = res.allPerms;

        // Map backend data to RoleItem model
        const mappedRoles: RoleItem[] = res.rolePerms.map((r: any, index: number) => ({
          Id: r.Id,
          RoleName: r.RoleName,
          Permissions: r.Permissions,
          CreatedAt: r.CreateDate ? new Date(r.CreateDate).toLocaleDateString() : 'N/A',
          IsSystem: r.IsSystem

        }));
        console.log("Map role", mappedRoles)
        this.roles.set(mappedRoles);
      },
      error: (err) => {
        ApiErrorHandler.handleError(err, "Thông báo hệ thống")
      }
    });
  }

  // ======= ACTIONS =======
  showAddRoleDialog() {
    this.isEditMode = false;
    this.selectedRole = null;
    this.displayRoleDialog = true;
  }

  showEditRoleDialog(role: RoleItem) {
    this.isEditMode = true;
    this.selectedRole = role;
    this.permDialogReadonly = false;
    this.displayRoleDialog = true;
  }

  openPermissionModal(role: RoleItem) {
    this.selectedRole = role;
    this.permDialogReadonly = true;
    this.displayPermDialog = true;
  }

  handleViewPermissions(role: RoleItem) {
    this.selectedRole = role;
    this.permDialogReadonly = true;
    this.displayPermDialog = true;
  }

  handleManagePermissions(role: RoleItem) {
    this.selectedRole = role;
    this.permDialogReadonly = false;
    this.displayPermDialog = true;
  }

  handleSaveRole(event: RoleSaveEvent) {
    const payload = {
      RoleName: event.roleName,
      permissionResquests: event.permissionIds.map((id) => ({
        PermissionId: id,
      })),
    };

    Swal.fire({
      title: 'Đang xử lý...',
      allowOutsideClick: false,
      didOpen: () => Swal.showLoading(),
    });

    const action$ =
      event.isEdit && event.roleId
        ? this.rolePermissionService.updateRolePermission(event.roleId, payload)
        : this.rolePermissionService.createRolePermission(payload);

    action$.subscribe({
      next: (res) => {
        Swal.fire('Thành công', res.message || 'Thao tác thành công', 'success');
        this.loadData();
        this.displayRoleDialog = false;

      },
      error: (err) => {
        ApiErrorHandler.handleError(err, "Thông báo hệ thống")
      },
    });
  }

  handleSavePermissions(event: PermissionSaveEvent) {
    if (!this.selectedRole) return;

    const payload = {
      RoleName: this.selectedRole.RoleName,
      permissionResquests: event.permissionIds.map((id) => ({
        PermissionId: id,
      })),
    };

    Swal.fire({
      title: 'Đang cập nhật quyền...',
      allowOutsideClick: false,
      didOpen: () => Swal.showLoading(),
    });

    this.rolePermissionService
      .updateRolePermission(event.roleId, payload)
      .subscribe({
        next: (res) => {
          console.log(res)

          Swal.fire('Thành công', 'Đã cập nhật quyền thành công', 'success');
          this.loadData();
          this.displayPermDialog = false;

        },
        error: (err) => {
          ApiErrorHandler.handleError(err, "Thông báo hệ thống")
        },
      });
  }

  deleteRole(role: RoleItem) {
    if (role.IsSystem) {
      Swal.fire('Không thể xóa', 'Đây là Role hệ thống, không được phép xóa.', 'error');
      return;
    }

    Swal.fire({
      title: 'Xác nhận xóa?',
      text: `Bạn có chắc muốn xóa Role "${role.RoleName}"?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Xóa',
      cancelButtonText: 'Hủy',
    }).then((result) => {
      if (result.isConfirmed) {
        this.roles.set(this.roles().filter((r) => r.Id !== role.Id));
        Swal.fire('Đã xóa', 'Role đã được xóa thành công', 'success');
      }
    });
  }
}
