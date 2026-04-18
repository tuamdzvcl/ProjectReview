import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppShellComponent } from '../../../../layouts/app-shell/app-shell.component';
import { UpgradeService } from '../../../../core/services/upgrade.service';
import {
  UpgradeResponse,
  UpgradeRequest,
} from '../../../../core/model/response/upgrade.model';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-membership',
  standalone: true,
  imports: [
    CommonModule,
    AppShellComponent,
    FormsModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    DropdownModule,
  ],
  templateUrl: './membership.component.html',
  styleUrl: './membership.component.scss',
})
export class MembershipComponent implements OnInit {
  private upgradeService = inject(UpgradeService);

  packages = signal<UpgradeResponse[]>([]);
  displayDialog = false;
  isEditMode = false;

  statusOptions = [
    { label: 'Đang hoạt động', value: 'active' },
    { label: 'Ngừng hoạt động', value: 'inactive' },
  ];

  selectedPkg: UpgradeRequest = {
    titleUpgrade: '',
    description: '',
    status: 'active',
    dailyLimit: 0,
    price: 0,
  };

  currentId: number | null = null;

  ngOnInit() {
    this.loadPackages();
  }

  loadPackages() {
    const params = {
      PageIndex: 1,
      PageSize: 100,
    };
    this.upgradeService.getAll(params).subscribe({
      next: (res) => {
        console.log(res);
        this.packages.set(res.Items);
      },
      error: (err) => {
        console.error('Lỗi khi lấy danh sách gói:', err);
        Swal.fire('Lỗi', 'Không thể lấy danh sách gói thành viên', 'error');
      },
    });
  }

  showAddDialog() {
    this.isEditMode = false;
    this.currentId = null;
    this.selectedPkg = {
      titleUpgrade: '',
      description: '',
      status: 'active',
      dailyLimit: 10,
      price: 0,
    };
    this.displayDialog = true;
  }

  showEditDialog(pkg: UpgradeResponse) {
    this.isEditMode = true;
    this.currentId = pkg.Id;
    this.selectedPkg = {
      titleUpgrade: pkg.TitleUpgrade,
      description: pkg.Description,
      status: pkg.status,
      dailyLimit: pkg.DailyLimit,
      price: pkg.Price,
    };
    this.displayDialog = true;
  }

  savePackage() {
    if (!this.selectedPkg.titleUpgrade) {
      Swal.fire('Cảnh báo', 'Vui lòng nhập tên gói', 'warning');
      return;
    }

    if (this.isEditMode && this.currentId) {
      this.upgradeService.update(this.currentId, this.selectedPkg).subscribe({
        next: () => {
          Swal.fire('Thành công', 'Cập nhật gói thành công', 'success');
          this.displayDialog = false;
          this.loadPackages();
        },
        error: (err) => {
          Swal.fire('Lỗi', 'Cập nhật thất bại: ' + err.message, 'error');
        },
      });
    } else {
      this.upgradeService.create(this.selectedPkg).subscribe({
        next: () => {
          Swal.fire('Thành công', 'Thêm gói mới thành công', 'success');
          this.displayDialog = false;
          this.loadPackages();
        },
        error: (err) => {
          Swal.fire('Lỗi', 'Thêm mới thất bại: ' + err.message, 'error');
        },
      });
    }
  }

  deletePackage(Id: number) {
    Swal.fire({
      title: 'Xác nhận xóa?',
      text: 'Bạn có chắc muốn xóa gói này không? Hành động này không thể hoàn tác.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Xóa',
      cancelButtonText: 'Hủy',
    }).then((result) => {
      if (result.isConfirmed) {
        this.upgradeService.deleteUpgrade(Id).subscribe({
          next: () => {
            Swal.fire('Đã xóa', 'Gói đã được xóa thành công', 'success');
            this.loadPackages();
          },
          error: (err) => {
            Swal.fire('Lỗi', 'Xóa thất bại: ' + err.message, 'error');
          },
        });
      }
    });
  }
}
