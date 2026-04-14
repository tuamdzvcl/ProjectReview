import { Component, inject } from '@angular/core';
import { Router, RouterOutlet, ActivatedRoute } from '@angular/router';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { EventService } from '../../../../core/services/event.service';
import { CatetoryService } from '../../../../core/services/catetory.service';
import { EventDraftService } from '../../../../core/services/event-draft.service';
import { environment } from '../../../../../environments/environment';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-event-create-page',
  standalone: true,
  imports: [CommonModule, RouterOutlet, BreadcrumbModule, Toast],
  templateUrl: './event-create-page.component.html',
  styleUrl: './event-create-page.component.scss',
  providers: [MessageService],
})
export class EventCreatePageComponent {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private eventService = inject(EventService);
  private categoryService = inject(CatetoryService);
  private draftService = inject(EventDraftService);
  private messageService = inject(MessageService);

  item: any;
  steps = [{ label: 'Chi Tiết' }, { label: 'Vé' }, { label: 'Cài Đặt' }];
  activeIndex = 0;
  isDataLoaded: boolean = false;
  eventId: string | null = null;

  ngOnInit() {
    this.setActiveIndexFromRoute();

    this.eventId = this.route.snapshot.paramMap.get('id');

    this.item = [
      { label: 'Home' },
      { label: this.eventId ? 'Edit' : 'Create' },
      { label: this.eventId ? 'Update Event' : 'Create Event' },
    ];

    if (this.eventId) {
      forkJoin({
        event: this.eventService.GetEventId(this.eventId),
        categories: this.categoryService.GetCatetory(),
      }).subscribe({
        next: ({ event, categories }: any) => {
          this.fillDraftFromEvent(event, categories?.Data || []);
          this.isDataLoaded = true;
        },
        error: () => {
          this.isDataLoaded = true;
        },
      });
    } else {
      this.draftService.clear();
      this.isDataLoaded = true;
    }
  }

  private fillDraftFromEvent(eventData: any, categories: any[]) {
    const sStart = eventData.SaleStartDate
      ? new Date(eventData.SaleStartDate)
      : null;
    const sEnd = eventData.SaleEndDate ? new Date(eventData.SaleEndDate) : null;
    const matched = categories.find((c) => c.Name === eventData.CatetoryName);

    const mappedTickets = (eventData.ListTypeTick || []).map((t: any) => ({
      id: t.Id,
      name: t.Name,
      price: t.Price,
      quantity: t.TotalQuantity,
      limit: 1,
      discount: 0,
      active: t.Status?.toLowerCase() === 'active',
      date: new Date(eventData.StartDate).toLocaleDateString('vi-VN', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
      }),
    }));

    let finalPreviewUrl = '';
    if (eventData.PosterUrl) {
      const serverOrigin = environment.apiBaseUrl.replace('/api', '');
      finalPreviewUrl = eventData.PosterUrl.startsWith('http')
        ? eventData.PosterUrl
        : `${serverOrigin}${eventData.PosterUrl}`;
    }

    this.draftService.save({
      title: eventData.Title || '',
      description: eventData.Description || '',
      location: eventData.Location || '',
      selectedCategory: matched ? matched.CatetoryId : null,
      eventDate: eventData.StartDate ? new Date(eventData.StartDate) : undefined,
      eventTime: eventData.StartDate ? new Date(eventData.StartDate) : undefined,
      duration: (eventData.StartDate && eventData.EndDate) ? this.calcDurationHours(eventData.StartDate, eventData.EndDate) : 1,
      previewUrl: finalPreviewUrl,
      tickets: mappedTickets,
      isImmediateStart: false,
      saleStartDate: sStart ? sStart.toISOString().split('T')[0] : '',
      saleStartTime: sStart ? sStart.toTimeString().substring(0, 5) : '00:00',
      isAutoEnd: false,
      saleEndDate: sEnd ? sEnd.toISOString().split('T')[0] : '',
      saleEndTime: sEnd ? sEnd.toTimeString().substring(0, 5) : '23:59',
    });
  }

  private calcDurationHours(startDate: string, endDate: string): number {
    const diffMs = new Date(endDate).getTime() - new Date(startDate).getTime();
    return Math.floor(diffMs / (1000 * 60 * 60));
  }

  private setActiveIndexFromRoute() {
    const url = this.router.url;

    if (url.includes('create-type')) {
      this.activeIndex = 1;
    } else if (url.includes('create-setting')) {
      this.activeIndex = 2;
    } else {
      this.activeIndex = 0;
    }
  }

  onNextStep() {
    if (this.activeIndex < this.steps.length - 1) {
      this.activeIndex++;

      if (this.activeIndex === 1) {
        this.router.navigate(['create-type'], { relativeTo: this.route });
      } else if (this.activeIndex === 2) {
        this.router.navigate(['create-setting'], {
          relativeTo: this.route,
        });
      }
    } else {
      this.saveEvent();
    }
  }

  onBackStep() {
    if (this.activeIndex > 0) {
      this.activeIndex--;

      if (this.activeIndex === 0) {
        this.router.navigate(['./'], { relativeTo: this.route });
      } else if (this.activeIndex === 1) {
        this.router.navigate(['create-type'], { relativeTo: this.route });
      }
    }
  }

  private saveEvent() {
    const draft = this.draftService.load();
    const file = this.draftService.selectedFile;

    if (!this.isFormDataValid(draft)) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Cảnh báo',
        detail: 'Vui lòng điền đầy đủ thông tin bắt buộc (Tiêu đề, Ngày sự kiện)',
      });
      return;
    }

    const { startDate, endDate } = this.calculateEventDates(draft);
    const { saleStartDate, saleEndDate } = this.calculateSaleDates(draft, startDate);

    this.categoryService.GetCatetory().subscribe({
      next: (res: any) => {
        const categories = res.Data || [];
        const matchedCategory = categories.find(
          (c: any) => c.CatetoryId === draft.selectedCategory
        );
        const categoryName = matchedCategory ? matchedCategory.Name : '';

        const formData = this.buildFormData({
          draft,
          file,
          startDate,
          endDate,
          saleStartDate,
          saleEndDate,
          categoryName,
        });

        this.submitEvent(formData);
        console.log('formdate ', formData);
      },
      error: (err: any) => {
        console.error('Không thể lấy danh mục:', err);
      },
    });
  }

  private isFormDataValid(draft: any): boolean {
    return !!(draft.title && draft.eventDate);
  }


  private calculateEventDates(draft: any) {
    const startDate = new Date(draft.eventDate);
    if (draft.eventTime) {
      const time = new Date(draft.eventTime);
      startDate.setHours(time.getHours(), time.getMinutes(), 0, 0);
    }

    const endDate = new Date(startDate);
    endDate.setHours(endDate.getHours() + (draft.duration || 0));

    return { startDate, endDate };
  }

  private calculateSaleDates(draft: any, startDate: Date) {
    let saleStartDate: Date;
    if (draft.isImmediateStart) {
      saleStartDate = new Date();
    } else {
      saleStartDate = new Date(`${draft.saleStartDate}T${draft.saleStartTime}`);
    }


    let saleEndDate: Date;
    if (draft.isAutoEnd) {
      saleEndDate = new Date(startDate);
    } else {
      saleEndDate = new Date(`${draft.saleEndDate}T${draft.saleEndTime}`);
    }

    return { saleStartDate, saleEndDate };
  }

  private buildFormData(data: any): FormData {
    const {
      draft,
      file,
      startDate,
      endDate,
      saleStartDate,
      saleEndDate,
      categoryName,
    } = data;
    const formData = new FormData();
    formData.append('Title', draft.title);
    formData.append('Description', draft.description);
    formData.append('Location', draft.location);
    formData.append('CatetoryName', categoryName);

    formData.append('StartDate', this.formatToLocalISO(startDate));
    formData.append('EndDate', this.formatToLocalISO(endDate));
    formData.append('SaleStartDate', this.formatToLocalISO(saleStartDate));
    formData.append('SaleEndDate', this.formatToLocalISO(saleEndDate));

    if (file) {
      formData.append('PosterUrl', file);
    }
    this.appendTicketsToFormData(formData, draft.tickets || []);
    formData.forEach((value, key) => {
      console.log(key, value);
    });
    return formData;
  }

  private formatToLocalISO(date: Date): string {
    const pad = (num: number) => num.toString().padStart(2, '0');
    const YYYY = date.getFullYear();
    const MM = pad(date.getMonth() + 1);
    const DD = pad(date.getDate());
    const hh = pad(date.getHours());
    const mm = pad(date.getMinutes());
    const ss = pad(date.getSeconds());

    return `${YYYY}-${MM}-${DD}T${hh}:${mm}:${ss}`;
  }
  private appendTicketsToFormData(formData: FormData, tickets: any[]) {
    tickets.forEach((ticket, index) => {
      formData.append(`TicketTypes[${index}].Name`, ticket.name);
      formData.append(`TicketTypes[${index}].Price`, ticket.price.toString());
      formData.append(
        `TicketTypes[${index}].TotalQuantity`,
        ticket.quantity.toString()
      );
      formData.append(`TicketTypes[${index}].Status`, ticket.active ? 'active' : 'stop');
      if (ticket.id) {
        formData.append(`TicketTypes[${index}].Id`, ticket.id);
      }
    });
  }
  private submitEvent(formData: FormData) {
    console.log('formData', formData);
    const request$ = this.eventId
      ? this.eventService.UpdateEvent(this.eventId, formData)
      : this.eventService.CreateEvent(formData);
    request$.subscribe({
      next: (result) => {
        this.messageService.add({
          severity: 'success',
          summary: 'Thành công',
          detail: `Sự kiện đã được ${this.eventId ? 'cập nhật' : 'tạo'
            } thành công!`,
        });
        this.draftService.clear();
        setTimeout(() => {
          this.router.navigate(['/admin/events']);
        }, 1500);
      },
      error: (err) => {
        let errorMessage = 'Có lỗi xảy ra khi lưu sự kiện. Vui lòng thử lại!';

        if (err.error && err.error.Message) {
          errorMessage = err.error.Message;
        } else if (err.message) {
          errorMessage = err.message;
        }

        this.messageService.add({
          severity: 'error',
          summary: 'Lỗi',
          detail: errorMessage,
        });
        console.error('Lỗi API:', err);
      },
    });
  }

  onStepClick(index: number) {
    this.activeIndex = index;

    if (index === 0) {
      this.router.navigate(['./'], { relativeTo: this.route });
    } else if (index === 1) {
      this.router.navigate(['create-type'], { relativeTo: this.route });
    } else if (index === 2) {
      this.router.navigate(['create-setting'], { relativeTo: this.route });
    }
  }
}
