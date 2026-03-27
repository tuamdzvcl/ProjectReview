import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { EventService } from '../../../../core/services/event.service';
import { EventModel } from '../../../../core/model/event.model';
import { EventCardComponent } from '../../../../shared/components/event-card/event-card.component';

@Component({
  selector: 'app-events-grid',
  standalone: true,
  imports: [CommonModule, EventCardComponent],
  templateUrl: './events-grid.component.html',
  styleUrl: './events-grid.component.scss',
})
export class EventsGridComponent {
  events: EventModel[] = [];
  pageIndex: number = 1;
  pageSize: number = 10;
  key: string = '';
  isLoading: boolean = false;
  hasMore: boolean = true;
  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.loadEvents();
  }
  loadEvents() {
    if (this.isLoading || !this.hasMore) return;
    this.isLoading = true;

    this.eventService
      .GetEventswithTypeticket(this.pageIndex, this.pageSize, this.key)
      .subscribe({
        next: (res) => {
          console.log('DATA-update:', res);

          // Filter events with Status = 2
          const filteredEvents = res.items.filter(
            (event: any) => event.Status === 'PUBLISHED'
          );
          console.log(filteredEvents);
          this.events = [...this.events, ...filteredEvents];

          if (filteredEvents.length < this.pageSize) {
            this.hasMore = false;
          }
        },
        error: (err) => {
          console.error('ERROR:', err);
        },
        complete: () => {
          this.isLoading = false;
        },
      });
  }

  loadMore() {
    console.log('vào rồi nhé');
    this.pageIndex++;
    this.loadEvents();
  }
}
