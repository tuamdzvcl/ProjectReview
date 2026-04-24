import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { EventService } from '../../../../core/services/event.service';
import { EventModel } from '../../../../core/model/response/event.model';
import { EventCardComponent } from '../event-card/event-card.component';

@Component({
  selector: 'app-events-grid',
  standalone: true,
  imports: [CommonModule, EventCardComponent],
  templateUrl: './events-grid.component.html',
  styleUrl: './events-grid.component.scss',
})
export class EventsGridComponent implements OnInit, OnChanges {
  @Input() categoryIds: string[] = [];

  events: EventModel[] = [];
  pageIndex: number = 1;
  pageSize: number = 10;
  key: string = '';
  isLoading: boolean = false;
  hasMore: boolean = true;

  constructor(private eventService: EventService) { }

  ngOnInit(): void {
    this.loadEvents();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['categoryIds'] && !changes['categoryIds'].firstChange) {
      this.resetAndLoad();
    }
  }

  resetAndLoad() {
    this.events = [];
    this.pageIndex = 1;
    this.hasMore = true;
    this.loadEvents();
  }

  loadEvents() {
    if (this.isLoading || !this.hasMore) return;
    this.isLoading = true;

    this.eventService
      .GetEventswithTypeticket(this.pageIndex, this.pageSize, this.key, this.categoryIds)
      .subscribe({
        next: (res) => {

          const filteredEvents = res.items.filter(
            (event: any) => event.Status === 'PUBLISHED'
          );

          this.events = [...this.events, ...filteredEvents];

          if (res.items.length < this.pageSize) {
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
    this.pageIndex++;
    this.loadEvents();
  }
}
