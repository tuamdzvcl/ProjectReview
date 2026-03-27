import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventDraftService } from '../../../core/services/event-draft.service';

@Component({
  selector: 'app-event-create-setting',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './event-create-setting.component.html',
  styleUrl: './event-create-setting.component.scss',
})
export class EventCreateSettingComponent implements OnInit, OnDestroy {
  private draftService = inject(EventDraftService);

  // Bắt đầu bán vé
  isImmediateStart: boolean = true;
  saleStartDate: string = ''; // yyyy-mm-dd
  saleStartTime: string = '00:00';

  // Kết thúc bán vé
  isAutoEnd: boolean = false;
  saleEndDate: string = ''; // yyyy-mm-dd
  saleEndTime: string = '23:59';

  ngOnInit(): void {
    // Tải dữ liệu từ Draft (đã được CreateEventComponent điền nếu là mode Edit)
    const draft = this.draftService.load();
    this.isImmediateStart = draft.isImmediateStart;
    this.saleStartDate    = draft.saleStartDate;
    this.saleStartTime    = draft.saleStartTime;
    this.isAutoEnd        = draft.isAutoEnd;
    this.saleEndDate      = draft.saleEndDate;
    this.saleEndTime      = draft.saleEndTime;
  }

  ngOnDestroy(): void {
    // Lưu lại cài đặt trước khi rời trang
    this.saveToDraft();
  }

  saveToDraft(): void {
    this.draftService.save({
      isImmediateStart: this.isImmediateStart,
      saleStartDate:    this.saleStartDate,
      saleStartTime:    this.saleStartTime,
      isAutoEnd:        this.isAutoEnd,
      saleEndDate:      this.saleEndDate,
      saleEndTime:      this.saleEndTime,
    });
  }
}
