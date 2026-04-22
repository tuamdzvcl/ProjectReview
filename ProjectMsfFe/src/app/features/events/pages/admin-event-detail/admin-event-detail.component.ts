import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../../../core/services/event.service';
import { EventModel } from '../../../../core/model/response/event.model';
import { FormatDatePipe } from '../../../../shared/pipes/format-date.pipe';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Toast } from 'primeng/toast';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { AppShellComponent } from '../../../../layouts/app-shell/app-shell.component';

@Component({
  selector: 'app-admin-event-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormatDatePipe,
    ImageUrlPipe,
    VndCurrencyPipe,
    Toast,
    ConfirmDialog,
    AppShellComponent,
  ],
  templateUrl: './admin-event-detail.component.html',
  styleUrl: './admin-event-detail.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class AdminEventDetailComponent implements OnInit {
  event: EventModel | null = null;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (id) {
        this.loadEventDetails(id);
      }
    });
  }

  loadEventDetails(id: string) {
    this.loading = true;
    this.eventService.GetEventId(id).subscribe({
      next: (data) => {
        this.event = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching event details', err);
        this.loading = false;
      },
    });
  }

  editEvent() {
    if (this.event) {
      this.router.navigate(['/event-create-page', this.event.Id]);
    }
  }

  publishEvent() {
    this.confirmationService.confirm({
      message: `Sự kiện công khai sẽ không thu hồi hay chỉnh sửa lại được nữa.Bạn có chắc chắn muốn công khai sự kiện "${this.event?.Title}" không?`,
      header: 'Xác nhận công khai',
      icon: 'pi pi-check-circle',
      acceptLabel: 'Công khai',
      rejectLabel: 'Hủy',
      acceptButtonStyleClass: 'p-button-success',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        if (!this.event) return;


        this.eventService.UpdateEventStatus(this.event.Id.toString(), 4).subscribe({
          next: () => {
            if (this.event) this.event.Status = 'PUBLISHED';
            this.messageService.add({
              severity: 'success',
              summary: 'Thành công',
              detail: 'Đã công khai sự kiện!',
            });
          },
          error: (err) => {
            console.error('Error publishing event', err);
            this.messageService.add({
              severity: 'error',
              summary: 'Lỗi',
              detail: 'Có lỗi xảy ra khi công khai sự kiện',
            });
          }
        });
      },
    });
  }

  backToEventList() {
    this.router.navigate(['/admin/events']);
  }
}
