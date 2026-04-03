import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  BookingService,
  BookingState,
} from '../../../../core/services/booking.service';
import { UserService } from '../../../../core/services/user.service';
import { TokenService } from '../../../../core/services/token.service';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';
import { FormatDatePipe } from '../../../../shared/pipes/format-date.pipe';
import { UserResponse } from '../../../../core/model/user.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-checkout-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    VndCurrencyPipe,
    ImageUrlPipe,
    FormatDatePipe,
    RouterLink,
  ],
  templateUrl: './checkout-page.component.html',
  styleUrl: './checkout-page.component.scss',
})
export class CheckoutPageComponent implements OnInit {
  bookingData: BookingState | null = null;
  userProfile: UserResponse | null = null;

  customerName = '';
  customerEmail = '';
  phoneNumber = '';
  address = '';

  selectedPaymentMethod = 'vnpay';

  paymentMethods = [
    {
      id: 'vnpay',
      name: 'VNPay',
      icon: 'payments',
      description: 'Thanh toán qua cổng VNPay (Atm/Qr-Code)',
    },
    {
      id: 'momo',
      name: 'MoMo',
      icon: 'account_balance_wallet',
      description: 'Thanh toán qua ví điện tử MoMo',
    },
    {
      id: 'bank',
      name: 'Chuyển khoản',
      icon: 'account_balance',
      description: 'Chuyển khoản ngân hàng trực tiếp',
    },
  ];

  constructor(
    private bookingService: BookingService,
    private userService: UserService,
    private tokenService: TokenService,
    public router: Router
  ) {}

  ngOnInit(): void {
    this.bookingData = this.bookingService.getBooking();

    if (!this.bookingData || !this.bookingData.event) {
      this.router.navigate(['/']);
      return;
    }

    this.loadUserProfile();
  }

  loadUserProfile(): void {
    const userId = this.tokenService.getUserId();

    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }

    this.userService.getUserEvents(userId).subscribe({
      next: (res) => {
        console.log('user ', res);
        this.userProfile = res.User;

        if (this.userProfile) {
          this.customerName = `${this.userProfile.FirstName || ''} ${
            this.userProfile.LastName || ''
          }`.trim();
          this.customerEmail = this.userProfile.Email || '';
        }
      },
      error: (err) => {
        console.error('Error loading profile', err);
      },
    });
  }

  get selectedTicketsList() {
    if (!this.bookingData?.event?.ListTypeTick) return [];
    return this.bookingData.event.ListTypeTick.filter(
      (t) => this.bookingData!.selectedTickets[t.Id] > 0
    );
  }

  onConfirmBooking(): void {
    if (!this.phoneNumber || !this.address) {
      Swal.fire({
        icon: 'error',
        title: 'Thiếu thông tin',
        text: 'Vui lòng nhập số điện thoại và địa chỉ nhận vé.',
      });
      return;
    }

    Swal.fire({
      title: 'Xác nhận đặt vé?',
      text: 'Hệ thống sẽ tiến hành khởi tạo đơn hàng của bạn.',
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#72bf44',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Xác nhận',
      cancelButtonText: 'Hủy',
    }).then((result) => {
      if (result.isConfirmed) {
        Swal.fire({
          title: 'Thành công!',
          text: 'Vui lòng kiểm tra email để hoàn tất thanh toán.',
          icon: 'success',
          timer: 2000,
          showConfirmButton: false,
        }).then(() => {
          this.bookingService.clearBooking();
          this.router.navigate(['/']);
        });
      }
    });
  }
}
