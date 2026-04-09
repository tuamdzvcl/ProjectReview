import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  selector: 'app-payment-result',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './payment-result.component.html',
  styleUrl: './payment-result.component.scss',
})
export class PaymentResultComponent implements OnInit {
  resultCode: number | null = null;
  orderId: string | null = null;
  isSuccess = false;
  isLoading = true;

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.resultCode = params['resultCode'] != null ? +params['resultCode'] : null;
      this.orderId = params['orderId'] || null;
      this.isSuccess = this.resultCode === 0;
      setTimeout(() => {
        this.isLoading = false;
      }, 1500);
    });
  }

  get statusTitle(): string {
    return this.isSuccess ? 'Thanh toán thành công!' : 'Thanh toán thất bại!';
  }

  get statusMessage(): string {
    return this.isSuccess
      ? 'Đơn hàng của bạn đã được thanh toán thành công. Vé sẽ được gửi đến email của bạn trong ít phút.'
      : 'Đã xảy ra lỗi trong quá trình thanh toán. Vui lòng thử lại hoặc liên hệ hỗ trợ.';
  }
}
