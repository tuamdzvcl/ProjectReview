import { Component, inject } from '@angular/core';
import { Router, RouterOutlet, ActivatedRoute } from '@angular/router';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { EventService } from '../../../../core/services/event.service';
import { CatetoryService } from '../../../../core/services/catetory.service';
import { EventDraftService } from '../../../../core/services/event-draft.service';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-event-create-page',
  standalone: true,
  imports: [CommonModule, RouterOutlet, BreadcrumbModule],
  templateUrl: './event-create-page.component.html',
  styleUrl: './event-create-page.component.scss',
})
export class EventCreatePageComponent {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private eventService = inject(EventService);
  private categoryService = inject(CatetoryService);
  private draftService = inject(EventDraftService);

  item: any;
  steps = [{ label: 'Chi Tiết' }, { label: 'Vé' }, { label: 'Cài Đặt' }];
  activeIndex = 0;
  isDataLoaded: boolean = false;
  eventId: string | null = null; // Lưu ID để biết là tạo mới hay chỉnh sửa

  ngOnInit() {
    this.item = [
      { label: 'Home' },
      { label: 'Create' },
      { label: 'Create Event' },
    ];

    this.setActiveIndexFromRoute();

    // --- MỚI: TẢI DỮ LIỆU ĐỂ PHÂN PHỐI CHO CÁC CON ---
    this.eventId = this.route.snapshot.paramMap.get('id');

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
          this.isDataLoaded = true; // Vẫn cho hiện dù lỗi để hiện form trống hoặc thông báo
        },
      });
    } else {
      this.isDataLoaded = true;
    }
  }

  private fillDraftFromEvent(eventData: any, categories: any[]) {
    // 1. Ánh xạ cơ bản
    const sStart = eventData.SaleStartDate ? new Date(eventData.SaleStartDate) : null;
    const sEnd = eventData.SaleEndDate ? new Date(eventData.SaleEndDate) : null;
    const now = new Date();

    // 2. Tìm danh mục
    const matched = categories.find(c => c.Name === eventData.CatetoryName);

    // 3. Map vé
    const mappedTickets = (eventData.ListTypeTick || []).map((t: any) => ({
      id: t.Id,
      name: t.Name,
      price: t.Price,
      quantity: t.TotalQuantity,
      limit: 1,
      discount: 0,
      active: t.Status?.toLowerCase() === 'active',
      date: new Date(eventData.StartDate).toLocaleDateString('vi-VN', { month: 'short', day: 'numeric', year: 'numeric' })
    }));

    // 4. Xử lý ảnh PosterUrl
    let finalPreviewUrl = '';
    if (eventData.PosterUrl) {
      const serverOrigin = environment.apiBaseUrl.replace('/api', '');
      finalPreviewUrl = eventData.PosterUrl.startsWith('http')
        ? eventData.PosterUrl
        : `${serverOrigin}${eventData.PosterUrl}`;
    }

    // 5. Lưu tất cả vào DraftService một lần duy nhất
    this.draftService.save({
      title: eventData.Title || '',
      description: eventData.Description || '',
      location: eventData.Location || '',
      selectedCategory: matched ? matched.CatetoryId : null,
      eventDate: new Date(eventData.StartDate),
      eventTime: new Date(eventData.StartDate),
      duration: this.calcDurationHours(eventData.StartDate, eventData.EndDate),
      previewUrl: finalPreviewUrl,
      tickets: mappedTickets,
      isImmediateStart: sStart ? sStart.getTime() <= now.getTime() : true,
      saleStartDate: sStart ? sStart.toISOString().split('T')[0] : '',
      saleStartTime: sStart ? sStart.toTimeString().substring(0, 5) : '00:00',
      isAutoEnd: !!sEnd,
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
      // Nếu ở bước cuối cùng (index 2), thực hiện Lưu/Cập nhật
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
    console.log('Đang thực hiện lưu dữ liệu từ DraftService...', this.draftService.load());
    // TODO: Gọi API Create hoặc Update ở đây
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
