import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UpgradeService } from '../../../../core/services/upgrade.service';
import { UpgradeResponse } from '../../../../core/model/response/upgrade.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-pricing',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pricing.component.html',
  styleUrl: './pricing.component.scss'
})
export class PricingComponent implements OnInit {
  private upgradeService = inject(UpgradeService);

  packages = signal<UpgradeResponse[]>([]);
  isLoading = signal(true);

  ngOnInit() {
    this.loadPackages();
  }

  loadPackages() {
    this.isLoading.set(true);
    const params = {
      PageIndex: 1,
      PageSize: 10
    };
    this.upgradeService.getAll(params).subscribe({
      next: (res) => {
        // Chỉ lấy những gói đang hoạt động
        const activePackages = res.Items.filter(p => p.status === 'active');
        this.packages.set(activePackages);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Lỗi khi tải bảng giá:', err);
        this.isLoading.set(false);
      }
    });
  }

  registerPackage(id: number) {
    Swal.fire({
      title: 'Đăng ký nâng cấp?',
      text: 'Bạn sẽ được chuyển đến trang thanh toán để hoàn tất đăng ký.',
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Đăng ký ngay',
      cancelButtonText: 'Hủy'
    }).then((result) => {
      if (result.isConfirmed) {
        this.upgradeService.register(id).subscribe({
          next: (res) => {
            console.log(res);
            console.log(id)
            if (res.PayUrl) {
              window.location.href = res.PayUrl;
            } else {
              Swal.fire(
                'Thông báo',
                'Hệ thống đang xử lý đăng ký của bạn.',
                'info'
              );
            }
          },
          error: (err) => {
            Swal.fire('Thất bại', err.message || 'Không thể khởi tạo thanh toán', 'error');
          }
        });
      }
    });
  }
}
