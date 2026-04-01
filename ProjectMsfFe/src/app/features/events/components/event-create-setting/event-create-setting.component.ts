import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventDraftService } from '../../../../core/services/event-draft.service';

@Component({
  selector: 'app-event-create-setting',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './event-create-setting.component.html',
  styleUrl: './event-create-setting.component.scss',
})
export class EventCreateSettingComponent implements OnInit, OnDestroy {
  private draftService = inject(EventDraftService);

  isImmediateStart: boolean = true;
  saleStartDate: string = '';
  saleStartTime: string = '00:00';

  isAutoEnd: boolean = false;
  saleEndDate: string = '';
  saleEndTime: string = '23:59';

  eventDate: Date | undefined = undefined;

  ngOnInit(): void {
    const draft = this.draftService.load();
    this.isImmediateStart = draft.isImmediateStart;
    this.saleStartDate = draft.saleStartDate;
    this.saleStartTime = draft.saleStartTime;
    this.isAutoEnd = draft.isAutoEnd;
    this.saleEndDate = draft.saleEndDate;
    this.saleEndTime = draft.saleEndTime;
    this.eventDate = draft.eventDate ? new Date(draft.eventDate) : undefined;

    if (!this.saleEndDate && this.eventDate) {
      this.saleEndDate = this.eventDate.toISOString().split('T')[0];
    }
  }

  ngOnDestroy(): void {
    this.saveToDraft();
  }

  saveToDraft(): void {
    this.draftService.save({
      isImmediateStart: this.isImmediateStart,
      saleStartDate: this.saleStartDate,
      saleStartTime: this.saleStartTime,
      isAutoEnd: this.isAutoEnd,
      saleEndDate: this.saleEndDate,
      saleEndTime: this.saleEndTime,
    });
  }
}
