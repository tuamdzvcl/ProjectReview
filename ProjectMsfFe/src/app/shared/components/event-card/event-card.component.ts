import { Component, Input } from '@angular/core';
import { EventModel } from '../../../core/model/event.model';
import { DurationPipe } from '../../pipes/duration.pipe';
import { FormatDatePipe } from '../../pipes/format-date.pipe';
import { ImageUrlPipe } from '../../pipes/image-url.pipe';
import { VndCurrencyPipe } from '../../pipes/vnd-currency.pipe';

@Component({
  selector: 'app-event-card',
  standalone: true,
  imports: [DurationPipe, FormatDatePipe, ImageUrlPipe, VndCurrencyPipe],
  templateUrl: './event-card.component.html',
  styleUrl: './event-card.component.scss',
})
export class EventCardComponent {
  @Input() eventmodel!: EventModel;
  firstPrice(): number {
    return this.eventmodel.ListTypeTick[0]?.Price || 0;
  }
}
