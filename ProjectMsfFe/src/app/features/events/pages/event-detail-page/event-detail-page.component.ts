import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EventService } from '../../../../core/services/event.service';
import { BookingService } from '../../../../core/services/booking.service';
import { EventModel } from '../../../../core/model/response/event.model';
import { CommonModule } from '@angular/common';
import { Subscription, interval } from 'rxjs';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { EventsGridComponent } from '../../components/events-grid/events-grid.component';
import { FormatDatePipe } from '../../../../shared/pipes/format-date.pipe';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-event-detail-page',
  standalone: true,
  imports: [
    CommonModule,
    ImageUrlPipe,
    VndCurrencyPipe,
    EventsGridComponent,
    RouterLink,
    FormatDatePipe,
    FormsModule
  ],
  templateUrl: './event-detail-page.component.html',
  styleUrls: ['./event-detail-page.component.scss']
})
export class EventDetailPageComponent implements OnInit, OnDestroy {
  event: EventModel | null = null;
  loading = true;
  error: string | null = null;


  days = 0;
  hours = 0;
  minutes = 0;
  seconds = 0;
  private timerSubscription: Subscription | null = null;


  selectedTickets: { [key: number]: number } = {};
  ticketErrors: { [key: number]: string | null } = {};
  totalPrice = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private bookingService: BookingService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const eventId = params.get('id');
      if (eventId) {
        this.loadEvent(eventId);
      } else {
        this.error = 'Event ID not found';
        this.loading = false;
      }
    });
  }

  ngOnDestroy(): void {
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
    }
  }

  loadEvent(id: string): void {
    this.loading = true;
    this.error = null;
    this.event = null;
    this.selectedTickets = {};
    this.totalPrice = 0;

    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
      this.timerSubscription = null;
    }

    this.eventService.GetEventId(id).subscribe({
      next: (data: EventModel) => {
        this.event = data;
        this.loading = false;
        this.initializeTickets();
        this.startCountdown();
        window.scrollTo(0, 0);
      },
      error: (err: any) => {
        console.error('Error loading event', err);
        this.error = 'Failed to load event details';
        this.loading = false;
      }
    });
  }

  initializeTickets(): void {
    if (this.event?.ListTypeTick) {
      this.event.ListTypeTick.forEach((ticket: any) => {
        this.selectedTickets[ticket.Id] = 0;
      });
    }
  }

  startCountdown(): void {
    if (!this.event?.StartDate) return;

    const startDate = new Date(this.event.StartDate).getTime();

    this.timerSubscription = interval(1000).subscribe(() => {
      const now = new Date().getTime();
      const distance = startDate - now;

      if (distance < 0) {
        this.days = 0;
        this.hours = 0;
        this.minutes = 0;
        this.seconds = 0;
        if (this.timerSubscription) {
          this.timerSubscription.unsubscribe();
        }
        return;
      }

      this.days = Math.floor(distance / (1000 * 60 * 60 * 24));
      this.hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
      this.minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
      this.seconds = Math.floor((distance % (1000 * 60)) / 1000);
    });
  }

  updateTicketQuantity(ticketId: number, delta: number): void {
    const currentQty = this.selectedTickets[ticketId] || 0;
    const newQty = Math.max(0, currentQty + delta);

    const ticket = this.event?.ListTypeTick.find((t: any) => t.Id === ticketId);
    if (ticket) {
      const maxAvailable = ticket.TotalQuantity - ticket.SoldQuantity;
      if (newQty <= maxAvailable) {
        this.selectedTickets[ticketId] = newQty;
        this.ticketErrors[ticketId] = null;
        this.calculateTotal();
      }
    }
  }

  onQuantityChange(ticketId: number, event: any): void {
    let inputVal = event.target.value;
    let newQty = parseInt(inputVal, 10);

    if (inputVal === '') {
      newQty = 0;
    }

    if (isNaN(newQty) || newQty < 0) {
      newQty = 0;
    }

    const ticket = this.event?.ListTypeTick.find((t: any) => t.Id === ticketId);
    if (ticket) {
      const maxAvailable = ticket.TotalQuantity - ticket.SoldQuantity;
      if (newQty > maxAvailable) {

        newQty = maxAvailable;
        event.target.value = newQty;
        this.ticketErrors[ticketId] = 'error';


        Swal.fire({
          toast: true,
          position: 'top-end',
          icon: 'warning',
          title: `Quá số lượng cho phép!`,
          text: `Chỉ còn lại ${maxAvailable} vé cho loại này.`,
          showConfirmButton: false,
          timer: 3000,
          timerProgressBar: true,
          background: '#fff',
          color: '#111827',
          iconColor: '#f59e0b'
        });
      } else {
        this.ticketErrors[ticketId] = null;
      }
    }

    this.selectedTickets[ticketId] = newQty;
    this.calculateTotal();
  }

  calculateTotal(): void {
    if (!this.event?.ListTypeTick) return;

    this.totalPrice = this.event.ListTypeTick.reduce((acc: number, ticket: any) => {
      const qty = this.selectedTickets[ticket.Id] || 0;
      const maxAvailable = ticket.TotalQuantity - ticket.SoldQuantity;
      const validQty = Math.min(qty, maxAvailable);
      return acc + (ticket.Price * validQty);
    }, 0);
  }


  get totalTicketCount(): number {
    if (!this.event?.ListTypeTick) return 0;
    return this.event.ListTypeTick.reduce((acc: number, ticket: any) => {
      const qty = this.selectedTickets[ticket.Id] || 0;
      const maxAvailable = Math.max(0, ticket.TotalQuantity - ticket.SoldQuantity);
      return acc + Math.min(qty, maxAvailable);
    }, 0);
  }

  get hasErrors(): boolean {
    return Object.values(this.ticketErrors).some(error => error !== null);
  }

  get activeTickets(): any[] {
    return this.event?.ListTypeTick.filter((t: any) => t.Status?.toLowerCase() === 'active') || [];
  }
  onBookNow(): void {
    if (this.totalTicketCount === 0 || this.hasErrors) return;

    this.bookingService.setBooking({
      event: this.event,
      selectedTickets: this.selectedTickets,
      totalPrice: this.totalPrice
    });

    this.router.navigate(['/checkout']);
  }
}
