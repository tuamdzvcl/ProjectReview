import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { MenuModule } from 'primeng/menu';
import { MenuItem, MessageService } from 'primeng/api';
import { Calendar } from 'primeng/calendar';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { EventDraftService } from '../../../../core/services/event-draft.service';
import { ToastModule } from 'primeng/toast';
import { ConfigService } from '../../../../core/services/config.service';

@Component({
  selector: 'app-event-create-type',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    MenuModule,
    Calendar,
    VndCurrencyPipe,
    ToastModule,
  ],
  templateUrl: './event-create-type.component.html',
  styleUrl: './event-create-type.component.scss',
})
export class EventCreateTypeComponent implements OnInit, OnDestroy {
  private draftService = inject(EventDraftService);
  private messageService = inject(MessageService);
  public configService = inject(ConfigService);

  showDialog: boolean = false;
  isEditMode: boolean = false;
  editingIndex: number = -1;

  ticket = {
    name: '',
    price: 0,
    quantity: 0,
    active: true,
    limit: 1,
    discount: 0,
  };

  tickets: any[] = [];

  ticketMenuItems: MenuItem[] = [];
  selectedTicketIndex: number = -1;

  constructor() {
    this.initMenu();
  }

  ngOnInit(): void {
    const draft = this.draftService.load();
    if (draft.tickets && draft.tickets.length > 0) {
      this.tickets = [...draft.tickets];
    }
  }

  ngOnDestroy(): void {
    this.saveToDraft();
  }

  saveToDraft(): void {
    this.draftService.save({ tickets: this.tickets });
  }

  initMenu() {
    this.ticketMenuItems = [
      {
        label: 'Sửa',
        icon: 'pi pi-pencil',
        command: () => this.editTicket(this.selectedTicketIndex),
      },
      {
        label: 'Xóa',
        icon: 'pi pi-trash',
        command: () => this.deleteTicket(this.selectedTicketIndex),
      },
    ];
  }

  openNew() {
    this.isEditMode = false;
    this.editingIndex = -1;
    this.ticket = {
      name: '',
      price: 0,
      quantity: 0,
      active: true,
      limit: 1,
      discount: 0,
    };
    this.showDialog = true;
  }

  editTicket(index: number) {
    this.isEditMode = true;
    this.editingIndex = index;
    const t = this.tickets[index];
    this.ticket = { ...t };
    this.showDialog = true;
  }

  deleteTicket(index: number) {
    this.tickets.splice(index, 1);
  }

  saveTicket() {
    if (!this.ticket.name || this.ticket.name.trim().length <= 5) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Lỗi thông tin',
        detail: 'Tên vé phải lớn hơn 5 ký tự.',
      });
      return;
    }

    const regex = /[!@#$%^&*()_+={}\[\]|\\:;"'<>\/?]+/;
    if (regex.test(this.ticket.name)) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Lỗi thông tin',
        detail: 'Tên vé không được chứa ký tự đặc biệt (!@#$...).',
      });
      return;
    }

    const minPrice = 1000;
    const maxPrice =
      this.configService.validation?.Default?.MaxNumber || 9999999999;
    if (this.ticket.price === null || this.ticket.price === undefined) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Lỗi thông tin',
        detail: 'Vui lòng nhập giá vé.',
      });
      return;
    }
    if (this.ticket.price < minPrice) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Lỗi thông tin',
        detail: `Giá vé tối thiểu là ${minPrice.toLocaleString('vi-VN')} đ.`,
      });
      return;
    }
    if (this.ticket.price >= maxPrice) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Lỗi thông tin',
        detail: `Giá vé không được vượt quá ${maxPrice.toLocaleString(
          'vi-VN'
        )} đ.`,
      });
      return;
    }

    const minQuantity = 1;
    const maxQuantity =
      this.configService.validation?.Default?.MaxQuantity || 99999999;
    if (this.ticket.quantity === null || this.ticket.quantity === undefined) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Lỗi thông tin',
        detail: 'Vui lòng nhập số lượng.',
      });
      return;
    }
    if (this.ticket.quantity < minQuantity) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Lỗi thông tin',
        detail: `Số lượng tối thiểu là ${minQuantity.toLocaleString('vi-VN')}.`,
      });
      return;
    }
    if (this.ticket.quantity >= maxQuantity) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Lỗi thông tin',
        detail: `Số lượng không được vượt quá ${maxQuantity.toLocaleString(
          'vi-VN'
        )}.`,
      });
      return;
    }

    if (this.isEditMode && this.editingIndex !== -1) {
      this.tickets[this.editingIndex] = {
        ...this.tickets[this.editingIndex],
        ...this.ticket,
      };
    } else {
      this.tickets.push({
        ...this.ticket,
        date: new Date().toLocaleDateString('vi-VN', {
          month: 'short',
          day: 'numeric',
          year: 'numeric',
        }),
        active: true,
      });
    }

    this.showDialog = false;
    this.saveToDraft();
  }
}
