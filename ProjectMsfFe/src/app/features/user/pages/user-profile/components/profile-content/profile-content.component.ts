import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventModel } from '../../../../../../core/model/response/event.model';
import { FormatDatePipe } from '../../../../../../shared/pipes/format-date.pipe';
import { ImageUrlPipe } from '../../../../../../shared/pipes/image-url.pipe';
import { OrderService } from '../../../../../../core/services/order.service';
import { TokenService } from '../../../../../../core/services/token.service';
import { VndCurrencyPipe } from '../../../../../../shared/pipes/vnd-currency.pipe';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-profile-content',
  standalone: true,
  imports: [
    RouterLink,
    CommonModule,
    FormatDatePipe,
    ImageUrlPipe,
    VndCurrencyPipe,
  ],

  templateUrl: './profile-content.component.html',
  styleUrl: './profile-content.component.scss',
})
export class ProfileContentComponent implements OnInit {
  @Input() isOwner: boolean = false;
  @Input() events: EventModel[] = [];

  activeTab = 'home';
  activeSubTab = 'organised';
  currentUser: any = null;
  userOrders: any[] = [];
  isLoadingOrders = false;
  savedEvents: any[] = [];
  attendingEvents: any[] = [];

  get canSeeOrganisedEvents(): boolean {
    const role = this.tokenService.getRole();
    return role === 'ADMIN' || role === 'ORGANIZER';
  }

  constructor(
    private orderService: OrderService,
    private tokenService: TokenService
  ) { }

  ngOnInit(): void {
    const userJson = localStorage.getItem('user');
    if (userJson) {
      this.currentUser = JSON.parse(userJson);
    }

    if (this.isOwner) {
      this.loadUserOrders();
      this.loadMockSavedEvents();
    }

    if (!this.canSeeOrganisedEvents) {
      this.activeSubTab = 'saved';
    }
  }

  loadMockSavedEvents(): void {
    this.savedEvents = [
      {
        Title: 'Lễ hội Âm nhạc Mùa Hè 2024',
        StartDate: '2024-07-15T19:00:00',
        PosterUrl:
          'https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?q=80&w=1000&auto=format&fit=crop',
      },
      {
        Title: 'Hội thảo Công nghệ Tương lai',
        StartDate: '2024-08-20T08:30:00',
        PosterUrl:
          'https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?q=80&w=1000&auto=format&fit=crop',
      },
    ];
  }

  loadUserOrders(): void {
    this.isLoadingOrders = true;
    this.orderService.getUserOrders().subscribe({
      next: (res) => {
        console.log(res.Items);
        this.userOrders = res.Items || [];
        this.extractAttendingEvents();
        this.isLoadingOrders = false;
      },
      error: (err) => {
        console.error('Lỗi khi nạp đơn hàng:', err);
        this.isLoadingOrders = false;
      },
    });
  }

  extractAttendingEvents(): void {
    const successfulOrders = this.userOrders.filter(
      (o) => o.Status === 'PAID' || o.Status === 'SUCCESS'
    );

    const eventMap = new Map<string, any>();

    successfulOrders.forEach((order) => {
      if (order.Event && order.Event.EventID) {
        if (!eventMap.has(order.Event.EventID)) {
          eventMap.set(order.Event.EventID, {
            Title: order.Event.EventName,
            PosterUrl: order.Event.EventPosterUrl,
            StartDate: order.Event.EventStartDate,
          });
        }
      }
    });

    this.attendingEvents = Array.from(eventMap.values());
  }
}
