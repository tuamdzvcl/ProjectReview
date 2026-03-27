import { Component, inject, ViewChild, ElementRef, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

// PrimeNG UI components
import { StepsModule } from 'primeng/steps';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { EditorModule } from 'primeng/editor';
import { DatePickerModule } from 'primeng/datepicker';
import { SelectModule } from 'primeng/select';
import { MessageService } from 'primeng/api';

// Services của app
import { CatetoryService } from '../../../core/services/catetory.service';
import { EventService } from '../../../core/services/event.service';
import { EventDraftService } from '../../../core/services/event-draft.service';

// Biến môi trường (chứa apiBaseUrl)
import { environment } from '../../../../environments/environment';
import { ImageUrlPipe } from '../../pipes/image-url.pipe';

@Component({
  selector: 'app-create-venue-event',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    StepsModule,
    InputTextModule,
    DropdownModule,
    CalendarModule,
    InputNumberModule,
    ButtonModule,
    ToastModule,
    EditorModule,
    DatePickerModule,
    SelectModule,
    ImageUrlPipe
  ],
  templateUrl: './create-event.component.html',
  styleUrl: './create-event.component.scss',
})
export class CreateEventComponent implements OnInit, OnDestroy {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private messageService = inject(MessageService);
  private categoryService = inject(CatetoryService);
  private eventService = inject(EventService);
  private draftService = inject(EventDraftService);

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  title: string = '';
  description: string = '';
  location: string = '';
  eventDate: Date | undefined;
  eventTime: Date | undefined;
  duration: number = 1;

  categories: any[] = [];
  selectedCategory: any = null;

  selectedFile: File | null = null;
  previewUrl: string | null = null;

  durations = [
    { label: '1 giờ', value: 1 },
    { label: '2 giờ', value: 2 },
  ];
  private eventId: string | null = null;
  private eventData: any = null;
  isEdit: boolean = false;

  ngOnInit(): void {
    this.loadCategories();
    this.restoreFromDraft();
    this.eventId = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.eventId;
    if (this.isEdit) {
      this.loadEventData();
    }
  }
  ngOnDestroy(): void {
    if (!this.isEdit) {
      this.saveFormToDraft();
    }
  }

  private loadCategories(): void {
    this.categoryService.GetCatetory().subscribe({
      next: (res: any) => {
        this.categories = res.Data || [];
      },
      error: () => {
        this.categories = [];
      },
    });
  }

  private loadEventData() {
    if (!this.eventId) return;

    this.eventService.GetEventId(this.eventId).subscribe({
      next: (eventData: any) => {
        this.eventData = eventData;
        this.fillFormFromEventData();
      },
      error: (err: any) => {
        console.error('Failed to load event data:', err);
        this.showToast('error', 'Lỗi', 'Không thể tải dữ liệu sự kiện');
      },
    });
  }
  // lưu dữ liệu tạo thời vào formDratf
  private saveFormToDraft(): void {
    this.draftService.save(
      {
        title: this.title,
        description: this.description,
        location: this.location,
        selectedCategory: this.selectedCategory,
        eventDate: this.eventDate,
        eventTime: this.eventTime,
        duration: this.duration,
        previewUrl: this.previewUrl,
        tickets: this.draftService.load().tickets || [],
      },
      this.selectedFile,
    );
  }

  // khôi phục dữ liệu form từ DraftService (nếu có)
  private restoreFromDraft(): void {
    if (!this.draftService.hasDraft()) return;

    const draft = this.draftService.load();
    this.title = draft.title;
    this.description = draft.description;
    this.location = draft.location;
    this.selectedCategory = draft.selectedCategory;
    this.eventDate = draft.eventDate ? new Date(draft.eventDate) : undefined;
    this.eventTime = draft.eventTime ? new Date(draft.eventTime) : undefined;
    this.duration = draft.duration;
    this.previewUrl = draft.previewUrl;
    this.selectedFile = this.draftService.selectedFile;
  }

  // ĐIỀN DỮ LIỆU VÀO FORM (mode chỉnh sửa)
  private fillFormFromEventData(): void {
    if (!this.eventData) return;

    this.title = this.eventData.Title || '';
    this.description = this.eventData.Description || '';
    this.location = this.eventData.Location || '';
    this.eventDate = new Date(this.eventData.StartDate);
    this.eventTime = new Date(this.eventData.StartDate);
    this.duration = this.calcDurationHours(this.eventData.StartDate, this.eventData.EndDate);

    const matched = this.categories.find(c => c.Name === this.eventData.CatetoryName);
    this.selectedCategory = matched ? matched.CatetoryId : null;

    if (this.eventData.PosterUrl) {
      const serverOrigin = environment.apiBaseUrl.replace('/api', '');
      const posterPath = this.eventData.PosterUrl;
      this.previewUrl = posterPath.startsWith('http')
        ? posterPath
        : `${serverOrigin}${posterPath}`;
    }
  }

  private calcDurationHours(startDate: string, endDate: string): number {
    const diffMs = new Date(endDate).getTime() - new Date(startDate).getTime();
    return Math.floor(diffMs / (1000 * 60 * 60));
  }

  // XỬ LÝ ẢNH
  triggerFileInput(): void {
    this.fileInput.nativeElement.click();
  }

  onFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    this.selectedFile = input.files[0];
    const reader = new FileReader();
    reader.onload = (e) => {
      this.previewUrl = e.target?.result as string;
    };
    reader.readAsDataURL(this.selectedFile);
  }

  // LƯU SỰ KIỆN
  saveEvent(): void {
    if (!this.title || !this.selectedCategory || !this.eventDate || !this.location) {
      this.showToast('warn', 'Cảnh báo', 'Vui lòng điền đầy đủ thông tin bắt buộc (*)');
      return;
    }

    const startDate = new Date(this.eventDate!);
    if (this.eventTime) {
      startDate.setHours(this.eventTime.getHours(), this.eventTime.getMinutes());
    }

    const endDate = new Date(startDate);
    endDate.setHours(endDate.getHours() + this.duration);

    const matchedCategory = this.categories.find(c => c.CatetoryId === this.selectedCategory);
    const categoryName = matchedCategory ? matchedCategory.Name : '';

    const formData = new FormData();
    formData.append('Title', this.title);
    formData.append('Description', this.description);
    formData.append('Location', this.location);
    formData.append('CatetoryName', categoryName);
    formData.append('StartDate', startDate.toISOString());
    formData.append('EndDate', endDate.toISOString());

    if (this.selectedFile) {
      formData.append('PosterUrl', this.selectedFile);
    }

    const obs = this.isEdit && this.eventId
      ? this.eventService.UpdateEvent(this.eventId, formData)
      : this.eventService.CreateEvent(formData);

    obs.subscribe({
      next: () => {
        this.draftService.clear();
        this.showToast('success', 'Thành công', `Sự kiện đã được ${this.isEdit ? 'cập nhật' : 'tạo'} thành công!`);
        setTimeout(() => this.router.navigate(['/events']), 1000);
      },
      error: (err: any) => {
        console.error('Save event error:', err);
        this.showToast('error', 'Lỗi', 'Không thể lưu sự kiện, vui lòng thử lại');
      },
    });
  }

  cancel(): void {
    this.draftService.clear();
    this.showToast('info', 'Đã hủy', 'Đã hủy bỏ các thay đổi');
    this.router.navigate(['/events']);
  }

  private showToast(severity: string, summary: string, detail: string): void {
    this.messageService.add({ severity, summary, detail });
  }
}
