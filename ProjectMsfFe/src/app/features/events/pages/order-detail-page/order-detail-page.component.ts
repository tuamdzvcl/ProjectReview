import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { OrderService } from '../../../../core/services/order.service';
import { FormatDatePipe } from '../../../../shared/pipes/format-date.pipe';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';

@Component({
  selector: 'app-order-detail-page',
  standalone: true,
  imports: [CommonModule, RouterModule, FormatDatePipe, ImageUrlPipe, VndCurrencyPipe],
  templateUrl: './order-detail-page.component.html',
  styleUrl: './order-detail-page.component.scss'
})
export class OrderDetailPageComponent implements OnInit {
  OrderId: string = '';
  order: any = null;
  isLoading = true;

  constructor(
    private route: ActivatedRoute,
    private orderService: OrderService
  ) { }

  ngOnInit(): void {
    this.OrderId = this.route.snapshot.paramMap.get('id') || '';
    if (this.OrderId) {
      this.loadOrderDetail();
    }
  }

  loadOrderDetail(): void {
    this.isLoading = true;
    this.orderService.getOrderDetail(this.OrderId).subscribe({
      next: (res) => {
        console.log(res)
        this.order = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Lỗi khi nạp chi tiết đơn hàng:', err);
        this.isLoading = false;
      }
    });
  }

  getQrCodeUrl(data: string): string {
    return `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(data)}`;
  }
}
