import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { EventModel } from '../model/response/event.model';

export interface BookingState {
  event: EventModel | null;
  selectedTickets: { [key: number]: number };
  totalPrice: number;
}

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private readonly STORAGE_KEY = 'msf_booking_data';
  private bookingState: BehaviorSubject<BookingState>;

  bookingState$;

  constructor() {
    const savedData = localStorage.getItem(this.STORAGE_KEY);
    const initialState: BookingState = savedData
      ? JSON.parse(savedData)
      : { event: null, selectedTickets: {}, totalPrice: 0 };

    this.bookingState = new BehaviorSubject<BookingState>(initialState);
    this.bookingState$ = this.bookingState.asObservable();
  }

  setBooking(state: BookingState): void {
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(state));
    this.bookingState.next(state);
  }

  getBooking(): BookingState {
    return this.bookingState.value;
  }

  clearBooking(): void {
    localStorage.removeItem(this.STORAGE_KEY);
    this.bookingState.next({
      event: null,
      selectedTickets: {},
      totalPrice: 0
    });
  }
}
