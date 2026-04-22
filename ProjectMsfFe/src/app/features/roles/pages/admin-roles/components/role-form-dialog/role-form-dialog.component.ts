import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PermissionItem, RoleItem, RoleSaveEvent } from '../../../../models/role.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-role-form-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, DialogModule, ButtonModule, InputTextModule],
  templateUrl: './role-form-dialog.component.html',
  styleUrl: './role-form-dialog.component.scss'
})
export class RoleFormDialogComponent implements OnChanges {
  @Input() visible: boolean = false;
  @Input() isEditMode: boolean = false;
  @Input() role: RoleItem | null = null;
  @Input() allPermissions: PermissionItem[] = [];
  
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<RoleSaveEvent>();

  roleName: string = '';
  selectedPermissionIds: number[] = [];

  ngOnChanges(changes: SimpleChanges) {
    if (changes['visible']?.currentValue === true) {
      if (this.isEditMode && this.role) {
        this.roleName = this.role.RoleName;
        this.selectedPermissionIds = this.role.Permissions.map(p => p.Id);
      } else {
        this.roleName = '';
        this.selectedPermissionIds = [];
      }
    }
  }

  onHide() {
    this.visibleChange.emit(false);
  }

  saveRole() {
    if (!this.roleName.trim()) {
      Swal.fire('Cảnh báo', 'Vui lòng nhập tên Role', 'warning');
      return;
    }

    this.save.emit({
      roleName: this.roleName,
      permissionIds: this.selectedPermissionIds,
      roleId: this.isEditMode && this.role ? this.role.Id : null,
      isEdit: this.isEditMode
    });
  }

  // Permission logic
  isPermissionSelected(permId: number): boolean {
    return this.selectedPermissionIds.includes(permId);
  }

  togglePermission(permId: number) {
    if (this.selectedPermissionIds.includes(permId)) {
      this.selectedPermissionIds = this.selectedPermissionIds.filter(id => id !== permId);
    } else {
      this.selectedPermissionIds = [...this.selectedPermissionIds, permId];
    }
  }

  selectAllPermissions() {
    this.selectedPermissionIds = this.allPermissions.map(p => p.Id);
  }

  deselectAllPermissions() {
    this.selectedPermissionIds = [];
  }
}
