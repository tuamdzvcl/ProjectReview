import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
} from '@angular/core';
import { EventService } from '../../../../core/services/event.service';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';
import { FormatDatePipe } from '../../../../shared/pipes/format-date.pipe';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { EventStatusPipe } from '../../../../shared/pipes/event-status.pipe';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { Toast } from 'primeng/toast';
import { TokenService } from '../../../../core/services/token.service';
import { PermissionStoreService } from '../../../../core/services/permission-store.service';
import { FormsModule } from '@angular/forms';
import { Dialog } from 'primeng/dialog';

@Component({
  selector: 'app-approve-events',
  standalone: true,
  imports: [
    CommonModule,
    ImageUrlPipe,
    FormatDatePipe,
    VndCurrencyPipe,
    ConfirmDialog,
    Toast,
    FormsModule,
    Dialog,
    EventStatusPipe,
  ],
  templateUrl: './approve-events.component.html',
  styleUrls: ['./approve-events.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
  providers: [MessageService, ConfirmationService]
})
export class ApproveEventsComponent implements OnInit {
  events: any[] = [];
  totalRecords = 0;
  pageIndex = 1;
  pageSize = 10;
  key = '';
  userRole: boolean | null = null;

  // Dialog xem lý do yêu cầu chỉnh sửa
  showEditRequestDialog = false;
  editRequestEvent: any = null;

  // Dialog nhập lý do từ chối
  showRejectDialog = false;
  rejectReason = '';

  constructor(
    private eventService: EventService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private tokenService: TokenService,
    private permissionStore: PermissionStoreService
  ) { }

  ngOnInit() {
    this.userRole = this.permissionStore.hasPermission("EVENT_BROWSE");
    if (this.userRole !== true) {
      this.router.navigate(['/']);
      return;
    }
    this.loadEvents();
  }

  loadEvents() {
    this.eventService.GetAdminPendingEvents(
      this.pageIndex,
      this.pageSize,
      this.key
    ).subscribe({
      next: (res) => {
        console.log('Admin Pending Events:', res);
        this.events = res.items;
        this.totalRecords = res.totalRecords;
      },
      error: (err) => {
        console.error('Error fetching admin pending events:', err);
      },
    });
  }

  onSearch(event: any) {
    this.key = event.target.value;
    this.pageIndex = 1;
    this.loadEvents();
  }

  approveEvent(event: any) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn duyệt sự kiện "${event.Title}"? Sau khi duyệt, sự kiện sẽ được công khai trên hệ thống.`,
      header: 'Xác nhận duyệt',
      icon: 'pi pi-check-circle',
      acceptLabel: 'Duyệt',
      rejectLabel: 'Hủy',
      acceptButtonStyleClass: 'p-button-success',
      accept: () => {
        this.eventService.UpdateEventStatus(event.Id, 2).subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Thành công',
              detail: `Sự kiện "${event.Title}" đã được duyệt thành công!`,
            });
            this.loadEvents();
          },
          error: (err: any) => {
            console.error('Lỗi khi duyệt:', err);
            this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể duyệt sự kiện.' });
          },
        });
      },
    });
  }

  rejectEvent(event: any) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn từ chối sự kiện "${event.Title}"?`,
      header: 'Xác nhận từ chối',
      icon: 'pi pi-times-circle',
      acceptLabel: 'Từ chối',
      rejectLabel: 'Hủy',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.eventService.UpdateEventStatus(event.Id, 3).subscribe({
          next: () => {
            this.messageService.add({
              severity: 'info',
              summary: 'Đã thực hiện',
              detail: `Đã từ chối sự kiện "${event.Title}".`,
            });
            this.loadEvents();
          },
          error: (err: any) => {
            console.error('Lỗi khi từ chối:', err);
            this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể từ chối sự kiện.' });
          },
        });
      },
    });
  }

  // Mở dialog xem lý do yêu cầu chỉnh sửa
  openEditRequestDialog(event: any) {
    this.editRequestEvent = event;
    this.rejectReason = '';
    this.showEditRequestDialog = true;
  }

  // Admin duyệt yêu cầu chỉnh sửa -> chuyển sự kiện về DRAFT (1)
  approveEditRequest() {
    this.eventService.UpdateEventStatus(this.editRequestEvent.Id, 1).subscribe({
      next: () => {
        this.showEditRequestDialog = false;
        this.messageService.add({
          severity: 'success',
          summary: 'Thành công',
          detail: `Đã duyệt yêu cầu chỉnh sửa. Sự kiện "${this.editRequestEvent.Title}" đã chuyển về bản nháp.`,
        });
        this.loadEvents();
      },
      error: (err: any) => {
        console.error('Lỗi khi duyệt yêu cầu chỉnh sửa:', err);
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể duyệt yêu cầu chỉnh sửa.' });
      },
    });
  }

  // Chuyển sang dialog nhập lý do từ chối
  openRejectEditDialog() {
    this.showEditRequestDialog = false;
    this.rejectReason = '';
    this.showRejectDialog = true;
  }

  // Admin từ chối yêu cầu chỉnh sửa -> giữ nguyên PUBLISHED (2) + gửi lý do
  submitRejectEdit() {
    if (!this.rejectReason.trim()) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Cảnh báo',
        detail: 'Vui lòng nhập lý do từ chối!',
      });
      return;
    }
    this.eventService.UpdateEventStatus(this.editRequestEvent.Id, 2, this.rejectReason).subscribe({
      next: () => {
        this.showRejectDialog = false;
        this.messageService.add({
          severity: 'info',
          summary: 'Đã từ chối',
          detail: `Đã từ chối yêu cầu chỉnh sửa sự kiện "${this.editRequestEvent.Title}". Email thông báo đã được gửi.`,
        });
        this.loadEvents();
      },
      error: (err: any) => {
        console.error('Lỗi khi từ chối yêu cầu chỉnh sửa:', err);
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể từ chối yêu cầu chỉnh sửa.' });
      },
    });
  }

  deleteEvent(event: any) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn xóa sự kiện "${event.Title}" không?`,
      header: 'Xác nhận xóa',
      icon: 'pi pi-trash',
      acceptLabel: 'Xóa',
      rejectLabel: 'Hủy',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.eventService.DeleteEvent(event.Id).subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Thành công',
              detail: `Đã xóa sự kiện "${event.Title}" thành công!`,
            });
            this.loadEvents();
          },
          error: (err: any) => {
            console.error('Lỗi khi xóa:', err);
            this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể xóa sự kiện.' });
          },
        });
      },
    });
  }

  viewDetails(event: any) {
    this.router.navigate(['/admin/events', event.Id]);
  }
}
