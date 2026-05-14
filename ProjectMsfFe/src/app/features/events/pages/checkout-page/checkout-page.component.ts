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
import { PromotionService } from '../../../../core/services/promotion.service';
import { PromotionResponse } from '../../../../core/model/response/promotion.model';
import { ApiError } from '../../../../core/model/base/ApiError.model';
import { ApiErrorHandler } from '../../../../core/utils/api-error-handler.util';

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

  // Voucher state
  showVoucherPopup = false;
  vouchers: PromotionResponse[] = [];
  searchTerm = '';
  page = 1;
  pageSize = 5;
  totalRecords = 0;
  loadingVouchers = false;
  hasMoreVouchers = true;
  selectedVoucher: PromotionResponse | null = null;
  discountAmount = 0;
  finalTotal = 0;

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
    private promotionService: PromotionService,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.bookingData = this.bookingService.getBooking();

    if (!this.bookingData || !this.bookingData.event) {
      this.router.navigate(['/']);
      return;
    }

    this.loadUserProfile();
    this.calculateFinalTotal();
  }

  loadUserProfile(): void {
    const userId = this.tokenService.getUserId();

    if (!userId) {
      this.router.navigate(['/auth/login']);
      return;
    }

    this.userService.getUserEvents().subscribe({
      next: (res) => {
        this.userProfile = res.User;

        if (this.userProfile) {
          this.customerName = `${this.userProfile.FirstName || ''} ${this.userProfile.LastName || ''
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

  // Voucher Methods
  openVoucherPopup(): void {
    this.showVoucherPopup = true;
    if (this.vouchers.length === 0) {
      this.loadVouchers();
    }
  }

  closeVoucherPopup(): void {
    this.showVoucherPopup = false;
  }

  loadVouchers(isLoadMore: boolean = false): void {
    if (this.loadingVouchers || (!isLoadMore && this.vouchers.length > 0))
      return;
    if (isLoadMore && !this.hasMoreVouchers) return;

    this.loadingVouchers = true;
    if (!isLoadMore) {
      this.page = 1;
      this.vouchers = [];
      this.hasMoreVouchers = true;
      this.totalRecords = 0;
    } else {
      if (this.vouchers.length >= this.totalRecords && this.totalRecords > 0) {
        this.hasMoreVouchers = false;
        return;
      }
    }

    const params = {
      page: this.page,
      pageSize: this.pageSize,
      search: this.searchTerm,
    };

    this.promotionService.getAll(params).subscribe({
      next: (res) => {
        if (res.Items && res.Items.length > 0) {
          const now = new Date();
          const filteredItems = res.Items.filter(v => {
            const startDate = new Date(v.StartDate);
            const endDate = new Date(v.EndDate);
            return startDate <= now && endDate >= now;
          });
          
          this.vouchers = [...this.vouchers, ...filteredItems];
          this.totalRecords = res.TotalRecords;
          this.page++;
          
          if (this.vouchers.length >= res.TotalRecords || res.Items.length < this.pageSize) {
            this.hasMoreVouchers = false;
          }
        } else {
          this.hasMoreVouchers = false;
        }
        this.loadingVouchers = false;
      },
      error: (err) => {
        ApiErrorHandler.handleError(err)
        console.error('Error loading vouchers', err);
        this.loadingVouchers = false;
      },
    });
  }

  onSearchVoucher(): void {
    this.loadVouchers(false);
  }

  onScrollVouchers(event: any): void {
    const element = event.target;
    if (element.scrollHeight - element.scrollTop <= element.clientHeight + 1) {
      if (this.hasMoreVouchers && !this.loadingVouchers) {
        this.loadVouchers(true);
      }
    }
  }

  isVoucherDisabled(voucher: PromotionResponse): boolean {
    if (!this.bookingData || voucher.AmountLimit === null) return false;
    return voucher.AmountLimit > this.bookingData.totalPrice;
  }

  selectVoucher(voucher: PromotionResponse): void {
    if (this.isVoucherDisabled(voucher)) return;
    
    if (this.selectedVoucher?.Id === voucher.Id) {
      this.selectedVoucher = null;
    } else {
      this.selectedVoucher = voucher;
    }
    this.calculateDiscount();
    this.closeVoucherPopup();
  }

  calculateDiscount(): void {
    if (!this.selectedVoucher || !this.bookingData) {
      this.discountAmount = 0;
      this.calculateFinalTotal();
      return;
    }

    const basePrice = this.bookingData.totalPrice;
    if (this.selectedVoucher.DiscountType === 'Percentage') {
      this.discountAmount =
        (basePrice * this.selectedVoucher.DiscountValue) / 100;
    } else {
      this.discountAmount = this.selectedVoucher.DiscountValue;
    }

    // Ensure discount doesn't exceed total price
    if (this.discountAmount > basePrice) {
      this.discountAmount = basePrice;
    }

    this.calculateFinalTotal();
  }

  calculateFinalTotal(): void {
    const basePrice = this.bookingData?.totalPrice || 0;
    this.finalTotal = basePrice - this.discountAmount;
  }

  onConfirmBooking(): void {
    const orderItems = Object.keys(this.bookingData?.selectedTickets || {})
      .filter((id) => this.bookingData!.selectedTickets[Number(id)] > 0)
      .map((id) => ({
        TicketTypeId: Number(id),
        Quantity: this.bookingData!.selectedTickets[Number(id)],
      }));

    const orderData: CreateOrderRequest = {
      User: {
        fullName: this.customerName,
        Email: this.customerEmail,
      },
      Items: orderItems,
      PromotionId: this.selectedVoucher?.Id,
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
            this.bookingService.clearBooking();
            window.location.href = response.PayUrl;
          },
          error: (err) => {
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
      this.router.navigate(['/discover']);
    }
  }
}
