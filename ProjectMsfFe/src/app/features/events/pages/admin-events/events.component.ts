import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
} from '@angular/core';
import { AppShellComponent } from '../../../../layouts/app-shell/app-shell.component';
import { EventService } from '../../../../core/services/event.service';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';
import { FormatDatePipe } from '../../../../shared/pipes/format-date.pipe';

import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { Toast } from 'primeng/toast';

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [
    CommonModule,
    AppShellComponent,
    ImageUrlPipe,
    FormatDatePipe,
    VndCurrencyPipe,
    ConfirmDialog,
    Toast
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
    private messageService: MessageService
  ) { }

  events: any[] = [];
  totalRecords = 0;
  pageIndex = 1;
  pageSize = 10;
  key = '';
  showDropdown: string | null = null;

  ngOnInit() {
    this.loadEvents();
  }

  loadEvents() {
    this.eventService
      .GetEventswithTypeticket(this.pageIndex, this.pageSize, this.key)
      .subscribe({
        next: (res) => {
          console.log('DATA:', res);
          console.log(res.items);
          this.events = res.items;
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

  // Dropdown actions
  toggleDropdown(eventId: string) {
    this.showDropdown = this.showDropdown === eventId ? null : eventId;
  }

  editEvent(event: any) {
    this.router.navigate(['/event-create-page', event.Id]);
  }

  duplicateEvent(event: any) {
    console.log('Duplicate event:', event);
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
          }
        });
      }
    });
  }

  viewDetails(event: any) {
    this.router.navigate(['/admin/events', event.Id]);
  }
}
