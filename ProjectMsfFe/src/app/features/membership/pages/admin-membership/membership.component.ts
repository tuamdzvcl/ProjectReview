import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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
import { TextareaModule } from 'primeng/textarea';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-membership',
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
    TitleUpgrade: '',
    Description: '',
    status: 'active',
    DailyLimit: 0,
    Price: 0,
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
      TitleUpgrade: '',
      Description: '',
      status: 'active',
      DailyLimit: 10,
      Price: 0,
    };
    this.displayDialog = true;
  }

  showEditDialog(pkg: UpgradeResponse) {
    this.isEditMode = true;
    this.currentId = pkg.Id;
    this.selectedPkg = {
      TitleUpgrade: pkg.TitleUpgrade,
      Description: pkg.Description,
      status: pkg.status,
      DailyLimit: pkg.DailyLimit,
      Price: pkg.Price,
    };
    this.displayDialog = true;
  }

  savePackage() {
    if (!this.selectedPkg.TitleUpgrade) {
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

  // EXCEL OPERATIONS
  exportToExcel() {
    this.upgradeService.exportExcel().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'MembershipPackages.xlsx';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        Swal.fire('Lỗi', 'Không thể xuất file Excel', 'error');
      },
    });
  }

  downloadTemplate() {
    this.upgradeService.downloadTemplate().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'MembershipTemplate.xlsx';
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

      this.upgradeService.importExcel(file).subscribe({
        next: (res) => {
          Swal.fire('Thành công', 'Đã nhập dữ liệu từ Excel thành công', 'success');
          this.loadPackages();
          event.target.value = ''; // Reset input
        },
        error: (err) => {
          Swal.fire('Lỗi', 'Nhập dữ liệu thất bại: ' + err.message, 'error');
          event.target.value = ''; // Reset input
        },
      });
    }
  }
}
