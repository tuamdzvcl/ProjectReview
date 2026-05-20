import { CommonModule } from '@angular/common';
import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import { EventService } from '../../../../core/services/event.service';
import { EventModel } from '../../../../core/model/response/event.model';
import { EventCardComponent } from '../event-card/event-card.component';
import { FaviriteService } from '../../../../core/services/favorite-event.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-events-grid',
  standalone: true,
  imports: [CommonModule, EventCardComponent],
  templateUrl: './events-grid.component.html',
  styleUrl: './events-grid.component.scss',
})
export class EventsGridComponent implements OnInit, OnChanges {
  @Input() categoryIds: string[] = [];
  @Input() key: string = '';
  @Input() showFavorites: boolean = false;
  @Input() relatedEventId: string | null = null;

  events: EventModel[] = [];
  pageIndex: number = 1;
  pageSize: number = 8;
  isLoading: boolean = false;
  hasMore: boolean = true;
  isNotCount: boolean = false;

  constructor(
    private eventService: EventService,
    private favoriteService: FaviriteService
  ) { }

  get displayedEvents(): EventModel[] {
    if (this.showFavorites) {
      return this.events.filter((event) =>
        this.favoriteService.isFavorite(event.Id.toString())
      );
    }
    // TODO: If showFavorites is true, return a filtered array containing only favorite events (using FavoriteService)
    // TODO: Otherwise, return all events
    return this.events;
  }

  ngOnInit(): void {
    this.loadEvents();
  }

  ngOnChanges(changes: SimpleChanges): void {
    const categoryChanged =
      changes['categoryIds'] && !changes['categoryIds'].firstChange;
    const keyChanged = changes['key'] && !changes['key'].firstChange;

    if (categoryChanged || keyChanged) {
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
    const request$ = this.relatedEventId
      ? this.eventService.GetEventCatetoryPageEvent(
        this.relatedEventId,
        this.pageIndex,
        this.pageSize,
        ''
      )
      : this.eventService.GetEventswithTypeticket(
        this.pageIndex,
        this.pageSize,
        this.key,
        this.categoryIds
      );

    request$.subscribe({
      next: (res) => {
        this.isLoading = false
        const filteredEvents = res.items.filter(
          (event: any) => event.Status === 'PUBLISHED'
        );

        this.events = [...this.events, ...filteredEvents];

        if (res.items.length < this.pageSize) {
          this.hasMore = false;
        }
        if (res.totalRecords < 0) {
          this.isNotCount = true;
        }
      },
      error: (err) => {
        this.isLoading = false
        console.error('ERROR:', err);
      },
    });
  }

  loadMore() {
    this.pageIndex++;
    this.loadEvents();
  }
}
