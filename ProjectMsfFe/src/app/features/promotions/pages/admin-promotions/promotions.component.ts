import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PromotionService } from '../../../../core/services/promotion.service';
import {
  PromotionRequest,
  PromotionResponse,
} from '../../../../core/model/response/promotion.model';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { TextareaModule } from 'primeng/textarea';
import { CalendarModule } from 'primeng/calendar';
import { InputSwitchModule } from 'primeng/inputswitch';
import Swal from 'sweetalert2';
import {
  DataFilterComponent,
  FilterData,
} from '../../../../shared/components/data-filter/data-filter.component';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';

@Component({
  selector: 'app-promotions',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    DropdownModule,
    TextareaModule,
    CalendarModule,
    InputSwitchModule,
    DataFilterComponent,
    PaginationComponent,
    VndCurrencyPipe,
  ],
  templateUrl: './promotions.component.html',
  styleUrl: './promotions.component.scss',
})
export class PromotionsComponent implements OnInit {
  private promotionService = inject(PromotionService);

  promotions = signal<PromotionResponse[]>([]);
  displayDialog = false;
  isEditMode = false;
  today: Date = new Date();

  constructor() {
    this.today.setHours(0, 0, 0, 0);
  }

  discountTypeOptions = [
    { label: 'Phần trăm (%)', value: 'Percentage' },
    { label: 'Số tiền cố định (VNĐ)', value: 'FixedAmount' },
  ];

  // Pagination & Filter State
  totalElements = 0;
  pageNumber = 1;
  pageSize = 12;

  filterParams: FilterData = {
    keyword: '',
    startDate: null,
    endDate: null,
    status: null,
  };

  statusOptions = [
    { label: 'Đang hoạt động', value: true },
    { label: 'Ngừng hoạt động', value: false },
  ];

  selectedPromo: PromotionRequest = {
    Code: '',
    DiscountAmount: null,
    AmountLimit: null,
    DiscountValue: 0,
    DiscountType: 'Percentage',
    StartDate: '',
    EndDate: '',
    IsActive: true,
    UsageLimit: null,
  };

  // Helper properties for p-calendar
  startDateObj: Date | null = null;
  endDateObj: Date | null = null;

  currentId: number | null = null;

  ngOnInit() {
    this.loadPromotions();
  }

  loadPromotions() {
    const params = {
      PageIndex: this.pageNumber,
      PageSize: this.pageSize,
      key: this.filterParams.keyword || '',
      Status: this.filterParams.status !== null ? this.filterParams.status : '',
      StartDate: this.filterParams.startDate
        ? new Date(this.filterParams.startDate.setUTCHours(0, 0, 0, 0)).toISOString()
        : '',
      EndDate: this.filterParams.endDate
        ? new Date(this.filterParams.endDate.setUTCHours(0, 0, 0, 0)).toISOString()
        : '',
    };

    this.promotionService.getAll(params).subscribe({
      next: (res) => {
        this.promotions.set(res.Items);
        this.totalElements = res.TotalRecords;
      },
      error: (err) => {
        console.error('Lỗi khi lấy danh sách khuyến mãi:', err);
        Swal.fire('Lỗi', 'Không thể lấy danh sách khuyến mãi', 'error');
      },
    });
  }

  showAddDialog() {
    this.isEditMode = false;
    this.currentId = null;
    this.selectedPromo = {
      Code: '',
      DiscountAmount: null,
      AmountLimit: null,
      DiscountValue: 0,
      DiscountType: 'Percentage',
      StartDate: '',
      EndDate: '',
      IsActive: true,
      UsageLimit: null,
    };
    this.startDateObj = null;
    this.endDateObj = null;
    this.displayDialog = true;
  }

  showEditDialog(promo: PromotionResponse) {
    this.isEditMode = true;
    this.currentId = promo.Id;
    this.selectedPromo = {
      Code: promo.Code,
      DiscountAmount: promo.DiscountAmount,
      AmountLimit: promo.AmountLimit,
      DiscountValue: promo.DiscountValue,
      DiscountType: promo.DiscountType,
      StartDate: promo.StartDate,
      EndDate: promo.EndDate,
      IsActive: promo.IsActive,
      UsageLimit: promo.UsageLimit,
    };
    this.startDateObj = promo.StartDate ? new Date(promo.StartDate) : null;
    this.endDateObj = promo.EndDate ? new Date(promo.EndDate) : null;
    this.displayDialog = true;
  }

  savePromotion() {
    if (!this.selectedPromo.Code) {
      Swal.fire('Cảnh báo', 'Vui lòng nhập mã khuyến mãi', 'warning');
      return;
    }

    if (this.startDateObj) {
      this.startDateObj.setUTCHours(0, 0, 0, 0);
      this.selectedPromo.StartDate = this.startDateObj.toISOString();
    } else {
      this.selectedPromo.StartDate = '';
    }

    if (this.endDateObj) {
      this.endDateObj.setUTCHours(0, 0, 0, 0);
      this.selectedPromo.EndDate = this.endDateObj.toISOString()
    } else {
      this.selectedPromo.EndDate = '';
    }

    console.log('Dữ liệu gửi lên service:', this.selectedPromo);

    if (this.isEditMode && this.currentId) {
      this.promotionService
        .update(this.currentId, this.selectedPromo)
        .subscribe({
          next: () => {
            Swal.fire(
              'Thành công',
              'Cập nhật khuyến mãi thành công',
              'success'
            );
            this.displayDialog = false;
            this.loadPromotions();
          },
          error: (err) => {
            Swal.fire('Lỗi', 'Cập nhật thất bại: ' + err.message, 'error');
          },
        });
    } else {
      this.promotionService.create(this.selectedPromo).subscribe({
        next: () => {
          Swal.fire('Thành công', 'Thêm khuyến mãi mới thành công', 'success');
          this.displayDialog = false;
          this.loadPromotions();
        },
        error: (err) => {
          Swal.fire('Lỗi', 'Thêm mới thất bại: ' + err.message, 'error');
        },
      });
    }
  }

  deletePromotion(id: number) {
    Swal.fire({
      title: 'Xác nhận xóa?',
      text: 'Bạn có chắc muốn xóa mã khuyến mãi này không?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Xóa',
      cancelButtonText: 'Hủy',
    }).then((result) => {
      if (result.isConfirmed) {
        this.promotionService.deletePromotion(id).subscribe({
          next: () => {
            Swal.fire('Đã xóa', 'Khuyến mãi đã được xóa thành công', 'success');
            this.loadPromotions();
          },
          error: (err) => {
            Swal.fire('Lỗi', 'Xóa thất bại: ' + err.message, 'error');
          },
        });
      }
    });
  }

  // EXCEL OPERATIONS
  exportToExcel() {
    this.promotionService.exportExcel().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Promotions.xlsx';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        Swal.fire('Lỗi', 'Không thể xuất file Excel', 'error');
      },
    });
  }

  downloadTemplate() {
    this.promotionService.downloadTemplate().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'PromotionTemplate.xlsx';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        Swal.fire('Lỗi', 'Không thể tải file mẫu', 'error');
      },
    });
  }

  triggerImport() {
    document.getElementById('importFile')?.click();
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      Swal.fire({
        title: 'Đang xử lý...',
        text: 'Vui lòng chờ trong giây lát',
        allowOutsideClick: false,
        didOpen: () => {
          Swal.showLoading();
        },
      });

      this.promotionService.importExcel(file).subscribe({
        next: (res) => {
          Swal.fire(
            'Thành công',
            'Đã nhập dữ liệu từ Excel thành công',
            'success'
          );
          this.loadPromotions();
          event.target.value = ''; // Reset input
        },
        error: (err) => {
          Swal.fire('Lỗi', 'Nhập dữ liệu thất bại: ' + err.message, 'error');
          event.target.value = ''; // Reset input
        },
      });
    }
  }

  isUpcoming(startDate: string): boolean {
    return new Date(startDate) > new Date();
  }

  isExpired(endDate: string): boolean {
    return new Date(endDate) < new Date();
  }

  toggleStatus(promo: PromotionResponse) {
    const request: PromotionRequest = {
      Code: promo.Code,
      DiscountAmount: promo.DiscountAmount,
      AmountLimit: promo.AmountLimit,
      DiscountValue: promo.DiscountValue,
      DiscountType: promo.DiscountType,
      StartDate: promo.StartDate,
      EndDate: promo.EndDate,
      IsActive: !promo.IsActive,
      UsageLimit: promo.UsageLimit,
    };

    this.promotionService.update(promo.Id, request).subscribe({
      next: () => {
        this.loadPromotions();
      },
      error: (err) => {
        Swal.fire(
          'Lỗi',
          'Không thể thay đổi trạng thái: ' + err.message,
          'error'
        );
      },
    });
  }

  // Handlers for Filter & Pagination
  onFilter(filterData: FilterData) {
    this.filterParams = filterData;
    this.pageNumber = 1; // Reset to first page when filtering
    this.loadPromotions();
  }

  onReset() {
    this.filterParams = {
      keyword: '',
      startDate: null,
      endDate: null,
      status: null,
    };
    this.pageNumber = 1;
    this.loadPromotions();
  }

  onPageChange(page: number) {
    this.pageNumber = page;
    this.loadPromotions();
  }
}
