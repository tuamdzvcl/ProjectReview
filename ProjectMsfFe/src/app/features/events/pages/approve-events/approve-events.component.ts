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
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { Toast } from 'primeng/toast';
import { TokenService } from '../../../../core/services/token.service';

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
  userRole: string | null = null;

  constructor(
    private eventService: EventService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private tokenService: TokenService
  ) { }

  ngOnInit() {
    this.userRole = this.tokenService.getRole();
    if (this.userRole !== 'ADMIN') {
      this.router.navigate(['/']);
      return;
    }
    this.loadEvents();
  }

  loadEvents() {
    this.eventService.GetEventswithTypeticket(
      this.pageIndex,
      this.pageSize,
      this.key
    ).subscribe({
      next: (res) => {
        console.log('Admin Events Data:', res);
        this.events = res.items;
        this.totalRecords = res.totalRecords;
      },
      error: (err) => {
        console.error('Error fetching admin events:', err);
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
        this.eventService.UpdateEventStatus(event.Id, 2).subscribe({ // Assuming 2 is PUBLISHED
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
        this.eventService.UpdateEventStatus(event.Id, 3).subscribe({ // Assuming 3 is REJECTED
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
