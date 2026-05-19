import { Component, Input, OnInit } from '@angular/core';
import { DurationPipe } from '../../../../shared/pipes/duration.pipe';
import { FormatDatePipe } from '../../../../shared/pipes/format-date.pipe';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { EventModel } from '../../../../core/model/response/event.model';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FaviriteService } from '../../../../core/services/favorite-event.service';
import { TokenService } from '../../../../core/services/token.service';

@Component({
  selector: 'app-event-card',
  standalone: true,
  imports: [
    CommonModule,
    DurationPipe,
    FormatDatePipe,
    ImageUrlPipe,
    VndCurrencyPipe,
    RouterLink,
  ],
  templateUrl: './event-card.component.html',
  styleUrl: './event-card.component.scss',
})
export class EventCardComponent implements OnInit {
  @Input() eventmodel!: EventModel;
  isfavorite: boolean = false;

  constructor(private favrrite: FaviriteService) {}

  ngOnInit(): void {

    this.favrrite.favoritesState$.subscribe(()=>{
      if(this.eventmodel){
        this.isfavorite = this.favrrite.isFavorite(this.eventmodel.Id.toString())
      }
    })

  }
  
  firstPrice(): number {
    return this.eventmodel?.ListTypeTick?.[0]?.Price || 0;
  }

  bookmark(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.favrrite.toggleFavorite(this.eventmodel.Id.toString())
  
  }
}
