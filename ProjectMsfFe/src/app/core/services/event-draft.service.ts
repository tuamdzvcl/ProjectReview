import { Injectable } from '@angular/core';

export interface EventDraft {
  title: string;
  description: string;
  location: string;
  selectedCategory: any;
  eventDate: Date | undefined;
  eventTime: Date | undefined;
  duration: number;
  previewUrl: string | null;
  selectedFileName: string | null;
  tickets: any[];
  isImmediateStart: boolean;
  saleStartDate: string;
  saleStartTime: string;
  isAutoEnd: boolean;
  saleEndDate: string;
  saleEndTime: string;
}

const DEFAULT_DRAFT: EventDraft = {
  title: '',
  description: '',
  location: '',
  selectedCategory: null,
  eventDate: undefined,
  eventTime: undefined,
  duration: 1,
  previewUrl: null,
  selectedFileName: null,
  tickets: [],
  isImmediateStart: true,
  saleStartDate: '',
  saleStartTime: '00:00',
  isAutoEnd: false,
  saleEndDate: '',
  saleEndTime: '23:59',
};

@Injectable({
  providedIn: 'root',
})
export class EventDraftService {
  private draft: EventDraft = { ...DEFAULT_DRAFT };
  // File object cannot be stored in JSON, keep it separately in memory
  private _selectedFile: File | null = null;

  get selectedFile(): File | null {
    return this._selectedFile;
  }

  save(draft: Partial<EventDraft>, file?: File | null): void {
    this.draft = { ...this.draft, ...draft };
    if (file !== undefined) {
      this._selectedFile = file;
      this.draft.selectedFileName = file ? file.name : null;
    }
  }

  load(): EventDraft {
    return { ...this.draft };
  }

  clear(): void {
    this.draft = { ...DEFAULT_DRAFT };
    this._selectedFile = null;
  }

  hasDraft(): boolean {
    return !!(
      this.draft.title ||
      this.draft.location ||
      this.draft.selectedCategory ||
      this.draft.eventDate
    );
  }
}
