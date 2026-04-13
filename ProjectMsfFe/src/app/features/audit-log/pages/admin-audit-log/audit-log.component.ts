import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppShellComponent } from '../../../../layouts/app-shell/app-shell.component';
import { AuditLogService } from '../../../../core/services/audit-log.service';
import { AuditLog } from '../../../../core/model/audit-log.model';

// PrimeNG Modules
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    AppShellComponent,
    TableModule,
    TagModule,
    InputTextModule,
    ButtonModule,
    TooltipModule,
    CardModule,
  ],
  templateUrl: './audit-log.component.html',
  styleUrl: './audit-log.component.scss',
})
export class AuditLogComponent implements OnInit {
  logs: AuditLog[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  rows: number = 10;

  // Filters
  username: string = '';
  path: string = '';

  constructor(private auditLogService: AuditLogService) {}

  ngOnInit() {
    this.loadLogs();
  }

  loadLogs(event: any = { first: 0, rows: 10 }) {
    this.loading = true;
    this.rows = event.rows;
    const pageIndex = event.first / event.rows + 1;
    const pageSize = event.rows;

    this.auditLogService
      .getAuditLogs(pageIndex, pageSize, this.username, this.path)
      .subscribe({
        next: (res) => {
          if (res) {
            console.log(res.Items);

            this.logs = res.Items;
            this.totalRecords = res.TotalRecords;
          }
          this.loading = false;
        },
        error: (err) => {
          console.error('Lỗi khi tải nhật ký:', err);
          this.loading = false;
        },
      });
  }

  onFilter() {
    this.loadLogs();
  }

  clearFilter() {
    this.username = '';
    this.path = '';
    this.loadLogs();
  }

  getSeverity(
    statusCode: number
  ):
    | 'success'
    | 'secondary'
    | 'info'
    | 'warn'
    | 'danger'
    | 'contrast'
    | undefined {
    if (statusCode >= 200 && statusCode < 300) return 'success';
    if (statusCode >= 400 && statusCode < 500) return 'warn';
    if (statusCode >= 500) return 'danger';
    return 'info';
  }

  getStatusLabel(statusCode: number) {
    if (statusCode >= 200 && statusCode < 300) return 'Thành công';
    if (statusCode === 401 || statusCode === 403) return 'Từ chối';
    if (statusCode >= 400) return 'Lỗi';
    return 'Thông tin';
  }
}
