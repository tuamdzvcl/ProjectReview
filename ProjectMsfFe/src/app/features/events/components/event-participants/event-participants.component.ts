import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../../core/services/user.service';
import { UserInEvent } from '../../../../core/model/response/participant.model';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService } from 'primeng/api';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';
import { DialogModule } from 'primeng/dialog';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';

@Component({
  selector: 'app-event-participants',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    TooltipModule,
    ImageUrlPipe,
    DialogModule,
    VndCurrencyPipe
  ],
  templateUrl: './event-participants.component.html',
  styleUrl: './event-participants.component.scss'
})
export class EventParticipantsComponent implements OnInit {
  @Input() eventId: string = '';

  participants: UserInEvent[] = [];
  participantsLoading = false;
  totalRecords = 0;
  rows = 10;
  first = 0;

  displayDetails = false;
  selectedParticipant: UserInEvent | null = null;

  constructor(
    private userService: UserService,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    // Không cần gọi loadParticipants() ở đây vì p-table [lazy]="true" 
    // sẽ tự động kích hoạt onLazyLoad khi khởi tạo.
  }

  // 1205/2026-thay đổi
  loadParticipants(event?: any) {
    if (!this.eventId) return;

    this.participantsLoading = true;
    const pageIndex = event ? (event.first / event.rows) + 1 : 1;
    const pageSize = event ? event.rows : this.rows;

    this.userService.GetParticipantsByEvent(this.eventId, pageIndex, pageSize).subscribe({
      next: (res) => {
        this.participants = res.items;
        this.totalRecords = res.totalRecords;
        this.participantsLoading = false;
      },
      error: (err) => {
        console.error('Error fetching participants', err);
        this.participantsLoading = false;
      }
    });
  }

  sendEmail(participant: UserInEvent) {
    this.messageService.add({
      severity: 'info',
      summary: 'Thông báo',
      detail: `Đang chuẩn bị gửi email cho ${participant.Email}`
    });
  }

  viewParticipantDetails(participant: UserInEvent) {
    this.selectedParticipant = participant;
    this.displayDetails = true;
  }
}
