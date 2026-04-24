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
    ToastModule
  ],
  templateUrl: './event-create-type.component.html',
  styleUrl: './event-create-type.component.scss'
})
export class EventCreateTypeComponent implements OnInit, OnDestroy {
  private draftService = inject(EventDraftService);
  private messageService = inject(MessageService);

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
    if (!this.ticket.name || this.ticket.name.trim().length <= 5) {
      this.messageService.add({severity:'warn', summary:'Lỗi thông tin', detail:'Tên vé phải lớn hơn 5 ký tự.'});
      return;
    }

    const regex = /[!@#$%^&*()_+={}\[\]|\\:;"'<>\/?]+/;
    if (regex.test(this.ticket.name)) {
      this.messageService.add({severity:'warn', summary:'Lỗi thông tin', detail:'Tên vé không được chứa ký tự đặc biệt (!@#$...).'});
      return;
    }

    if (this.isEditMode && this.editingIndex !== -1) {
      
      this.tickets[this.editingIndex] = {
        ...this.tickets[this.editingIndex],
        ...this.ticket
      };
    } else {
      
      this.tickets.push({
        ...this.ticket,
        date: new Date().toLocaleDateString('vi-VN', { month: 'short', day: 'numeric', year: 'numeric' }),
        active: true
      });
    }

    this.showDialog = false;
    this.saveToDraft(); 
  }
}
