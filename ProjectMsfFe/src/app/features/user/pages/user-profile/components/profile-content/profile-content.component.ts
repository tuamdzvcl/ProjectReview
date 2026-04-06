import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventModel } from '../../../../../../core/model/response/event.model';
import { FormatDatePipe } from '../../../../../../shared/pipes/format-date.pipe';
import { ImageUrlPipe } from '../../../../../../shared/pipes/image-url.pipe';
import { OrderService } from '../../../../../../core/services/order.service';
import { TokenService } from '../../../../../../core/services/token.service';
import { VndCurrencyPipe } from '../../../../../../shared/pipes/vnd-currency.pipe';

@Component({
  selector: 'app-profile-content',
  standalone: true,
  imports: [CommonModule, FormatDatePipe, ImageUrlPipe, VndCurrencyPipe],

  templateUrl: './profile-content.component.html',
  styleUrl: './profile-content.component.scss'
})
export class ProfileContentComponent implements OnInit {
  @Input() isOwner: boolean = false;
  @Input() events: EventModel[] = [];

  activeTab = 'home';
  activeSubTab = 'organised';
  userOrders: any[] = [];
  isLoadingOrders = false;

  constructor(
    private orderService: OrderService,
    private tokenService: TokenService
  ) { }

  ngOnInit(): void {
    if (this.isOwner) {
      this.loadUserOrders();
    }
  }

  loadUserOrders(): void {
    this.isLoadingOrders = true;
    this.orderService.getUserOrders().subscribe({
      next: (res) => {
        console.log(res.Items)
        this.userOrders = res.Items || [];
        this.isLoadingOrders = false;
      },
      error: (err) => {
        console.error('Lỗi khi nạp đơn hàng:', err);
        this.isLoadingOrders = false;
      }
    });
  }
}
