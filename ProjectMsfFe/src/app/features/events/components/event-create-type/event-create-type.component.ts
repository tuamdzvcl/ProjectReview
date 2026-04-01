import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { MenuModule } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { Calendar } from 'primeng/calendar';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { EventDraftService } from '../../../../core/services/event-draft.service';

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
    VndCurrencyPipe
  ],
  templateUrl: './event-create-type.component.html',
  styleUrl: './event-create-type.component.scss'
})
export class EventCreateTypeComponent implements OnInit, OnDestroy {
  private draftService = inject(EventDraftService);

  showDialog: boolean = false;
  isEditMode: boolean = false;
  editingIndex: number = -1;

  ticket = {
    name: '',
    price: 0,
    quantity: 0,
    active: true,
    limit: 1,
    discount: 0
  };

  tickets: any[] = [];

  // Menu items cho từng dòng vé
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
    console.log('tickets :', this.tickets);
    this.draftService.save({ tickets: this.tickets });
  }

  initMenu() {
    this.ticketMenuItems = [
      {
        label: 'Sửa',
        icon: 'pi pi-pencil',
        command: () => this.editTicket(this.selectedTicketIndex)
      },
      {
        label: 'Xóa',
        icon: 'pi pi-trash',
        command: () => this.deleteTicket(this.selectedTicketIndex)
      }
    ];
  }

  openNew() {
    this.isEditMode = false;
    this.editingIndex = -1;
    this.ticket = { name: '', price: 0, quantity: 0, active: true, limit: 1, discount: 0 };
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
    if (!this.ticket.name) return;

    if (this.isEditMode && this.editingIndex !== -1) {
      // Cập nhật vé cũ
      this.tickets[this.editingIndex] = {
        ...this.tickets[this.editingIndex],
        ...this.ticket
      };
    } else {
      // Thêm vé mới
      this.tickets.push({
        ...this.ticket,
        date: new Date().toLocaleDateString('vi-VN', { month: 'short', day: 'numeric', year: 'numeric' }),
        active: true
      });
    }

    this.showDialog = false;
    this.saveToDraft(); // Cập nhật draft ngay sau khi lưu vé
  }
}
