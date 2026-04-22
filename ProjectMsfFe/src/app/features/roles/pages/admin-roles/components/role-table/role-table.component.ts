import { Component, Input, Output, EventEmitter, Signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { RoleItem } from '../../../../models/role.model';

@Component({
  selector: 'app-role-table',
  standalone: true,
  imports: [CommonModule, TableModule, TooltipModule],
  templateUrl: './role-table.component.html',
  styleUrl: './role-table.component.scss'
})
export class RoleTableComponent {
  @Input() roles!: Signal<RoleItem[]>;
  @Output() editRole = new EventEmitter<RoleItem>();
  @Output() viewPermissions = new EventEmitter<RoleItem>();
  @Output() managePermissions = new EventEmitter<RoleItem>();
  @Output() deleteRole = new EventEmitter<RoleItem>();

  onEdit(role: RoleItem) {
    this.editRole.emit(role);
  }

  onDelete(role: RoleItem) {
    this.deleteRole.emit(role);
  }

  onManagePermissions(role: RoleItem) {
    this.managePermissions.emit(role);
  }

  onViewPermissions(role: RoleItem) {
    this.viewPermissions.emit(role);
  }
}
