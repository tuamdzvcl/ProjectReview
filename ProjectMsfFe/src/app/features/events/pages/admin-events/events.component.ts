import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  Inject,
  OnInit,
} from '@angular/core';
import { AppShellComponent } from '../../../../layouts/app-shell/app-shell.component';
import { EventService } from '../../../../core/services/event.service';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';
import { FormatDatePipe } from '../../../../shared/pipes/format-date.pipe';
import { ChangeDetectorRef } from '@angular/core';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { Router } from '@angular/router';

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [
    CommonModule,
    AppShellComponent,
    ImageUrlPipe,
    FormatDatePipe,
    VndCurrencyPipe,
  ],
  templateUrl: './events.component.html',
  styleUrls: ['./events.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default, // Thay đổi từ OnPush thành Default
})
export class EventsComponent implements OnInit {
  constructor(
    private eventService: EventService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) { }

  events: any[] = [];
  totalRecords = 0;
  pageIndex = 1;
  pageSize = 10;
  key = '';
  showDropdown: string | null = null;

  ngOnInit() {
    this.loadEvents();
  }

  loadEvents() {
    this.eventService
      .GetEventswithTypeticket(this.pageIndex, this.pageSize, this.key)
      .subscribe({
        next: (res) => {
          console.log('DATA:', res);
          console.log(res.items);
          this.events = res.items;
        },
        error: (err) => {
          console.error('ERROR:', err);
        },
      });
  }

  onSearch(event: any) {
    const value = event.target.value;
    this.key = value;
    this.pageIndex = 1;
    this.loadEvents();
  }

  onTabChange(filter: string) {
    this.key = filter;
    this.pageIndex = 1;
    this.loadEvents();
  }

  // Dropdown actions
  toggleDropdown(eventId: string) {
    this.showDropdown = this.showDropdown === eventId ? null : eventId;
  }

  editEvent(event: any) {
    console.log('Edit event:', event);
    this.router.navigate(['/event-create-page', event.Id]);
  }

  duplicateEvent(event: any) {
    console.log('Duplicate event:', event);
  }

  deleteEvent(event: any) {
    console.log('Delete event:', event);

  }

  viewDetails(event: any) {
    console.log('View details:', event);

  }
}
