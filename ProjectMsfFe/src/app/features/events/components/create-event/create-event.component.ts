import {
  Component,
  inject,
  ViewChild,
  ElementRef,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

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

import { CatetoryService } from '../../../../core/services/catetory.service';

import { EventDraftService } from '../../../../core/services/event-draft.service';
import { ConfigService } from '../../../../core/services/config.service';

import { environment } from '../../../../../environments/environment';

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
  ],
  templateUrl: './create-event.component.html',
  styleUrl: './create-event.component.scss',
})
export class CreateEventComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private categoryService = inject(CatetoryService);
  private draftService = inject(EventDraftService);
  public configService = inject(ConfigService);

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

  descriptionLength: number = 0;

  @ViewChild('editor') editor: any;

  onDescriptionChange(event: any): void {
    const max = this.configService.validation?.Event?.Description?.Max || 2000;
    const quill = this.editor.getQuill();
    const text = quill.getText().trim();

    if (text.length > max) {
      // Chặn và cắt bỏ phần thừa
      quill.deleteText(max, text.length - max);
      this.descriptionLength = max;
    } else {
      this.descriptionLength = text.length;
    }
  }

  durations = [
    { label: '1 giờ', value: 1 },
    { label: '2 giờ', value: 2 },
  ];
  private eventId: string | null = null;
  isEdit: boolean = false;

  ngOnInit(): void {
    this.loadCategories();
    this.restoreFromDraft();
    this.eventId = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.eventId;
  }

  ngOnDestroy(): void {
    this.saveFormToDraft();
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
      this.selectedFile
    );
  }

  private isValidDate(d: any): boolean {
    if (!d || d === 'null' || d === 'undefined') return false;
    const date = new Date(d);
    return !isNaN(date.getTime());
  }

  private restoreFromDraft(): void {
    if (!this.draftService.hasDraft()) return;

    const draft = this.draftService.load();

    this.title = draft.title;
    this.description = draft.description;
    // Update length when restoring
    const doc = new DOMParser().parseFromString(
      this.description || '',
      'text/html'
    );
    this.descriptionLength = (doc.body.textContent || '').trim().length;
    this.location = draft.location;
    this.selectedCategory = draft.selectedCategory;
    this.eventDate = this.isValidDate(draft.eventDate)
      ? new Date(draft.eventDate as any)
      : undefined;
    this.eventTime = this.isValidDate(draft.eventTime)
      ? new Date(draft.eventTime as any)
      : undefined;
    this.duration = draft.duration;
    this.previewUrl = draft.previewUrl;
    this.selectedFile = this.draftService.selectedFile;
  }

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

  
}
