import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { EventModel } from '../model/event.model';

export interface BookingState {
  event: EventModel | null;
  selectedTickets: { [key: number]: number };
  totalPrice: number;
}

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private bookingState = new BehaviorSubject<BookingState>({
    event: null,
    selectedTickets: {},
    totalPrice: 0
  });

  bookingState$ = this.bookingState.asObservable();

  setBooking(state: BookingState): void {
    this.bookingState.next(state);
  }

  getBooking(): BookingState {
    return this.bookingState.value;
  }

  clearBooking(): void {
    this.bookingState.next({
      event: null,
      selectedTickets: {},
      totalPrice: 0
    });
  }
}
