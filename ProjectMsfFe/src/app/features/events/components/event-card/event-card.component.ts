import { Component, Input } from '@angular/core';
import { DurationPipe } from '../../../../shared/pipes/duration.pipe';
import { FormatDatePipe } from '../../../../shared/pipes/format-date.pipe';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { EventModel } from '../../../../core/model/event.model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-event-card',
  standalone: true,
  imports: [DurationPipe, FormatDatePipe, ImageUrlPipe, VndCurrencyPipe, RouterLink],
  templateUrl: './event-card.component.html',
  styleUrl: './event-card.component.scss',
})
export class EventCardComponent {
  @Input() eventmodel!: EventModel;
  firstPrice(): number {
    return this.eventmodel.ListTypeTick[0]?.Price || 0;
  }
}
