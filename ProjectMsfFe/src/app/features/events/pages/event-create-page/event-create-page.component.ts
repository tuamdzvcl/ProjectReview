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
  providers: [MessageService]
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

  /** 
   * Hàm chính để lưu sự kiện. 
   * Được chia nhỏ thành các bước để người mới dễ dàng đọc hiểu.
   */
  private saveEvent() {
    const draft = this.draftService.load();
    const file = this.draftService.selectedFile;

    // Bước 1: Kiểm tra các thông tin bắt buộc
    if (!this.isFormDataValid(draft)) {
      console.warn('Vui lòng điền đầy đủ thông tin bắt buộc');
      return;
    }

    // Bước 2: Tính toán tất cả các mốc thời gian cần thiết
    const { startDate, endDate } = this.calculateEventDates(draft);
    const { saleStartDate, saleEndDate } = this.calculateSaleDates(draft);

    // Bước 3: Lấy danh mục, xây dựng FormData và gọi API
    this.categoryService.GetCatetory().subscribe({
      next: (res: any) => {
        const categories = res.Data || [];
        const matchedCategory = categories.find((c: any) => c.CatetoryId === draft.selectedCategory);
        const categoryName = matchedCategory ? matchedCategory.Name : '';

        // Tạo FormData để gửi lên server (bao gồm cả file ảnh)
        const formData = this.buildFormData({
          draft,
          file,
          startDate,
          endDate,
          saleStartDate,
          saleEndDate,
          categoryName
        });

        // Gửi dữ liệu tới Server
        this.submitEvent(formData);
      },
      error: (err) => {
        console.error('Không thể lấy danh mục:', err);
      }
    });
  }

  /**
   * Bước 1: Kiểm tra tính hợp lệ của dữ liệu
   */
  private isFormDataValid(draft: any): boolean {
    return !!(draft.title && draft.eventDate);
  }

  /**
   * Bước 2a: Tính toán ngày bắt đầu và kết thúc sự kiện
   */
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

  /**
   * Bước 2b: Tính toán ngày bắt đầu và kết thúc bán vé
   */
  private calculateSaleDates(draft: any) {
    let saleStartDate: Date;
    if (draft.isImmediateStart) {
      saleStartDate = new Date(); // Bán ngay bây giờ
    } else {
      saleStartDate = new Date(`${draft.saleStartDate}T${draft.saleStartTime}`);
    }

    let saleEndDate: Date;
    if (draft.isAutoEnd) {
      // Tự động đóng sau 24h kể từ khi bắt đầu bán
      saleEndDate = new Date(saleStartDate.getTime() + 24 * 60 * 60 * 1000);
    } else {
      saleEndDate = new Date(`${draft.saleEndDate}T${draft.saleEndTime}`);
    }

    return { saleStartDate, saleEndDate };
  }


  private buildFormData(data: any): FormData {
    const { draft, file, startDate, endDate, saleStartDate, saleEndDate, categoryName } = data;
    const formData = new FormData();

    // Thông tin cơ bản
    formData.append('Title', draft.title);
    formData.append('Description', draft.description);
    formData.append('Location', draft.location);
    formData.append('CatetoryName', categoryName);

    // Các mốc thời gian (chuyển về định dạng chuẩn ISO)
    formData.append('StartDate', startDate.toISOString());
    formData.append('EndDate', endDate.toISOString());
    formData.append('SaleStartDate', saleStartDate.toISOString());
    formData.append('SaleEndDate', saleEndDate.toISOString());

    // File ảnh poster (nếu có)
    if (file) {
      formData.append('PosterUrl', file);
    }

    // Danh sách các loại vé
    this.appendTicketsToFormData(formData, draft.tickets || []);

    return formData;
  }

  private appendTicketsToFormData(formData: FormData, tickets: any[]) {
    tickets.forEach((ticket, index) => {
      formData.append(`TicketTypes[${index}].Name`, ticket.name);
      formData.append(`TicketTypes[${index}].Price`, ticket.price.toString());
      formData.append(`TicketTypes[${index}].TotalQuantity`, ticket.quantity.toString());
    });
  }

  /**
   * Bước cuối: Thực hiện lệnh gửi API
   */
  private submitEvent(formData: FormData) {
    console.log('Đang gửi dữ liệu...');

    const request$ = this.eventId
      ? this.eventService.UpdateEvent(this.eventId, formData)
      : this.eventService.CreateEvent(formData);

    request$.subscribe({
      next: (result) => {
        this.messageService.add({ 
          severity: 'success', 
          summary: 'Thành công', 
          detail: `Sự kiện đã được ${this.eventId ? 'cập nhật' : 'tạo'} thành công!` 
        });
        
        console.log('Lưu thành công:', result);
        this.draftService.clear(); // Xóa bản nháp sau khi lưu xong
        
        // Chờ 1 chút để người dùng thấy thông báo rồi mới chuyển trang
        setTimeout(() => {
          this.router.navigate(['/admin/events']); 
        }, 1500);
      },
      error: (err) => {
        this.messageService.add({ 
          severity: 'error', 
          summary: 'Lỗi', 
          detail: 'Có lỗi xảy ra khi lưu sự kiện. Vui lòng thử lại!' 
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
