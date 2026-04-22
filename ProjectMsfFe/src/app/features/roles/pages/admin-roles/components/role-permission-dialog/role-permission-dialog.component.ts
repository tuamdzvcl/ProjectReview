import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { PermissionItem, RoleItem, PermissionSaveEvent } from '../../../../models/role.model';

@Component({
  selector: 'app-role-permission-dialog',
  standalone: true,
  imports: [CommonModule, DialogModule, ButtonModule],
  templateUrl: './role-permission-dialog.component.html',
  styleUrl: './role-permission-dialog.component.scss'
})
export class RolePermissionDialogComponent implements OnChanges {
  @Input() visible: boolean = false;
  @Input() readonly: boolean = false;
  @Input() role: RoleItem | null = null;
  @Input() allPermissions: PermissionItem[] = [];

  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<PermissionSaveEvent>();

  selectedPermissionIds: number[] = [];

  ngOnChanges(changes: SimpleChanges) {
    if (changes['visible']?.currentValue === true && this.role) {
      this.selectedPermissionIds = this.role.Permissions.map(p => p.Id);
    }
  }

  onHide() {
    this.visibleChange.emit(false);
  }

  savePermissions() {
    if (!this.role) return;
    this.save.emit({
      roleId: this.role.Id,
      permissionIds: this.selectedPermissionIds
    });
  }

  // Permission selection logic
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
