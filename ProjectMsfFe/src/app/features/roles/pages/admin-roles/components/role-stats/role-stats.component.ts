import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-role-stats',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './role-stats.component.html',
  styleUrl: './role-stats.component.scss'
})
export class RoleStatsComponent {
  @Input() totalRoles: number = 0;
  @Input() systemRoleCount: number = 0;
  @Input() customRoleCount: number = 0;
  @Input() totalPermissions: number = 0;
}
