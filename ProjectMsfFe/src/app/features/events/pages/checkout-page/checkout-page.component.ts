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
import { UserResponse } from '../../../../core/model/response/user.model';
import Swal from 'sweetalert2';
import { OrderService } from '../../../../core/services/order.service';
import { CreateOrderRequest } from '../../../../core/model/request/orderRequest.model';

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
    private orderService: OrderService,
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

    this.userService.getUserEvents().subscribe({
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

    // Lấy danh sách các vé đã chọn (OrderItemRequest)
    const orderItems = Object.keys(this.bookingData?.selectedTickets || {})
      .filter((id) => this.bookingData!.selectedTickets[Number(id)] > 0)
      .map((id) => ({
        TicketTypeId: Number(id),
        Quantity: this.bookingData!.selectedTickets[Number(id)],
      }));

    // Tạo đối tượng CreateOrderRequest khớp với Backend
    const orderData: CreateOrderRequest = {
      User: {
        fullName: this.customerName,
        Address: this.address,
        Phone: this.phoneNumber,
        Email: this.customerEmail,
      },
      Items: orderItems,
    };

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
          title: 'Đang xử lý...',
          text: 'Vui lòng chờ trong giây lát.',
          allowOutsideClick: false,
          didOpen: () => {
            Swal.showLoading();
          },
        });

        this.orderService.createOrder(orderData).subscribe({
          next: (response) => {
            console.log(response);
            window.location.href = response.PayUrl;
            // Swal.fire({
            //   title: 'Thành công!',
            //   text: 'Đơn hàng của bạn đã được khởi tạo thành công.',
            //   icon: 'success',
            //   timer: 2000,
            //   showConfirmButton: false,
            // }).then(() => {
            //   this.bookingService.clearBooking();
            //   this.router.navigate(['/']);
            // });
          },
          error: (err) => {
            console.error('[DEBUG] Lỗi khi gọi API CreateOrder:', err);

            Swal.fire({
              icon: 'error',
              title: 'Đặt vé thất bại',
              text:
                err.message ||
                'Có lỗi xảy ra trong quá trình đặt vé. Vui lòng thử lại sau.',
            });
          },
        });
      }
    });
  }

  onBackToEvent(): void {
    if (this.bookingData?.event?.Id) {
      this.router.navigate(['/event', this.bookingData.event.Id]);
    } else {
      this.router.navigate(['/events']);
    }
  }
}
