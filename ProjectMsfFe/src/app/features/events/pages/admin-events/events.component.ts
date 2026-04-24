import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  HostListener,
} from '@angular/core';
import { EventService } from '../../../../core/services/event.service';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';
import { FormatDatePipe } from '../../../../shared/pipes/format-date.pipe';
import { EventStatusPipe } from '../../../../shared/pipes/event-status.pipe';

import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { Toast } from 'primeng/toast';
import { TokenService } from '../../../../core/services/token.service';
import { FormsModule } from '@angular/forms';
import { Dialog } from 'primeng/dialog';

@Component({
  selector: 'app-events',
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
  templateUrl: './events.component.html',
  styleUrls: ['./events.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class EventsComponent implements OnInit {
  constructor(
    private eventService: EventService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private tokenService: TokenService
  ) { }

  events: any[] = [];
  totalRecords = 0;
  pageIndex = 1;
  pageSize = 10;
  key = '';
  showDropdown: string | null = null;
  userRole: string | null = null;

  // Request Edit Dialog
  showRequestEditDialog = false;
  requestEditReason = '';
  requestEditEvent: any = null;

  ngOnInit() {
    this.userRole = this.tokenService.getRole();
    this.loadEvents();
  }

  loadEvents() {
    const serviceMethod =
      this.eventService.GetEventswithTypeticketbyid(
        this.pageIndex,
        this.pageSize,
        this.key
      );

    serviceMethod.subscribe({
      next: (res) => {
        console.log('DATA:', res);
        this.events = res.items;
        this.totalRecords = res.totalRecords;
      },
      error: (err) => {
        console.error('ERROR:', err);
      },
    });
  }

  onSearch(event: any) {
    const value = event.target.value;
    this.key = value;
    this.pageIndex = 1;
    this.loadEvents();
  }

  onTabChange(filter: string) {
    this.key = filter;
    this.pageIndex = 1;
    this.loadEvents();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    if (!target.closest('.action-dropdown')) {
      this.showDropdown = null;
    }
  }

  toggleDropdown(eventId: string, event?: Event) {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    this.showDropdown = this.showDropdown === eventId ? null : eventId;
  }

  editEvent(event: any) {
    this.router.navigate(['/event-create-page', event.Id]);
  }

  requestEdit(event: any) {
    this.showDropdown = null;
    this.requestEditEvent = event;
    this.requestEditReason = '';
    this.showRequestEditDialog = true;
  }

  submitRequestEdit() {
    if (!this.requestEditReason.trim()) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Cảnh báo',
        detail: 'Vui lòng nhập lý do yêu cầu chỉnh sửa!',
      });
      return;
    }
    this.eventService.UpdateEventStatus(this.requestEditEvent.Id, 6, this.requestEditReason).subscribe({
      next: () => {
        this.showRequestEditDialog = false;
        this.messageService.add({
          severity: 'success',
          summary: 'Thành công',
          detail: 'Yêu cầu chỉnh sửa sự kiện đã được gửi thành công!',
        });
        this.loadEvents();
      },
      error: (err) => {
        console.error('Lỗi khi yêu cầu chỉnh sửa:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'Lỗi',
          detail: 'Có lỗi xảy ra khi yêu cầu chỉnh sửa sự kiện!',
        });
      },
    });
  }

  get isEventEnded(): (event: any) => boolean {
    return (event: any) => {
      return event.Status === 'ENDED' || new Date(event.EndDate) < new Date();
    };
  }

  

  getTotalTickets(event: any): number {
    if (!event.ListTypeTick || !event.ListTypeTick.length) return 0;
    return event.ListTypeTick.reduce(
      (sum: number, t: any) => sum + (t.TotalQuantity || 0),
      0
    );
  }

  getTotalSold(event: any): number {
    if (!event.ListTypeTick || !event.ListTypeTick.length) return 0;
    return event.ListTypeTick.reduce(
      (sum: number, t: any) => sum + (t.SoldQuantity || 0),
      0
    );
  }

  duplicateEvent(event: any) {
    this.showDropdown = null;
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn nhân bản sự kiện "${event.Title}" không?`,
      header: 'Xác nhận nhân bản',
      icon: 'pi pi-copy',
      acceptLabel: 'Nhân bản',
      rejectLabel: 'Hủy',
      acceptButtonStyleClass: 'p-button-success',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.eventService.DuplicateEvent(event.Id).subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Thành công',
              detail: `Nhân bản sự kiện thành công!`,
            });
            this.loadEvents();
          },
          error: (err) => {
            console.error('Lỗi khi nhân bản:', err);
            this.messageService.add({
              severity: 'error',
              summary: 'Lỗi',
              detail: 'Có lỗi xảy ra khi nhân bản sự kiện!',
            });
          },
        });
      },
    });
  }

  publicEvent(event: any) {
    this.showDropdown = null;
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn công khai sự kiện "${event.Title}"? Sau khi công khai, Admin sẽ có thể duyệt sự kiện của bạn.`,
      header: 'Xác nhận công khai',
      icon: 'pi pi-megaphone',
      acceptLabel: 'Công khai',
      rejectLabel: 'Hủy',
      acceptButtonStyleClass: 'p-button-info',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.eventService.UpdateEventStatus(event.Id, 4).subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Thành công',
              detail: `Sự kiện đã được chuyển sang trạng thái chờ duyệt!`,
            });
            this.loadEvents();
          },
          error: (err) => {
            console.error('Lỗi khi công khai:', err);
            this.messageService.add({
              severity: 'error',
              summary: 'Lỗi',
              detail: 'Có lỗi xảy ra khi thay đổi trạng thái sự kiện!',
            });
          },
        });
      },
    });
  }

  approveEvent(event: any) {
    this.showDropdown = null;
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn duyệt và công khai sự kiện "${event.Title}" không?`,
      header: 'Xác nhận duyệt sự kiện',
      icon: 'pi pi-check-circle',
      acceptLabel: 'Duyệt',
      rejectLabel: 'Hủy',
      acceptButtonStyleClass: 'p-button-success',
      rejectButtonStyleClass: 'p-button-text',
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
          error: (err) => {
            console.error('Lỗi khi duyệt sự kiện:', err);
            this.messageService.add({
              severity: 'error',
              summary: 'Lỗi',
              detail: 'Có lỗi xảy ra khi duyệt sự kiện. Vui lòng thử lại!',
            });
          },
        });
      },
    });
  }

  deleteEvent(event: any) {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn xóa sự kiện "${event.Title}" không?`,
      header: 'Xác nhận xóa',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Xóa',
      rejectLabel: 'Hủy',
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-text',
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
          error: (err) => {
            console.error('Lỗi khi xóa sự kiện:', err);
            this.messageService.add({
              severity: 'error',
              summary: 'Lỗi',
              detail: 'Có lỗi xảy ra khi xóa sự kiện. Vui lòng thử lại!',
            });
          },
        });
      },
    });
  }

  viewDetails(event: any) {
    this.router.navigate(['/admin/events', event.Id]);
  }
}
