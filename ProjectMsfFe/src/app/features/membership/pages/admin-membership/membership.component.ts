import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppShellComponent } from '../../../../layouts/app-shell/app-shell.component';

interface MembershipFeature {
  text: string;
}

export interface MembershipPackage {
  id: string;
  name: string;
  price: number;
  highlight?: boolean;
  features: MembershipFeature[];
  activeUsers: number;
  status: 'active' | 'inactive';
}

@Component({
  selector: 'app-membership',
  standalone: true,
  imports: [CommonModule, AppShellComponent],
  templateUrl: './membership.component.html',
  styleUrl: './membership.component.scss'
})
export class MembershipComponent {
  packages = signal<MembershipPackage[]>([
    {
      id: '1',
      name: 'Cơ bản',
      price: 300000,
      activeUsers: 1245,
      status: 'active',
      features: [
        { text: 'Tạo tối đa 5 sự kiện/tháng' },
        { text: 'Quản lý người tham dự cơ bản' },
        { text: 'Hỗ trợ qua email' }
      ]
    },
    {
      id: '2',
      name: 'Chuyên nghiệp',
      price: 500000,
      highlight: true,
      activeUsers: 850,
      status: 'active',
      features: [
        { text: 'Tạo tối đa 15 sự kiện/tháng' },
        { text: 'Quản lý người tham dự nâng cao' },
        { text: 'Hỗ trợ qua email & chat' },
        { text: 'Phân tích báo cáo cơ bản' }
      ]
    },
    {
      id: '3',
      name: 'Cao cấp',
      price: 1000000,
      activeUsers: 120,
      status: 'active',
      features: [
        { text: 'Không giới hạn số lượng sự kiện' },
        { text: 'Đầy đủ tính năng quản lý' },
        { text: 'Hỗ trợ 24/7 (Email, Chat, Phone)' },
        { text: 'Báo cáo phân tích chuyên sâu' }
      ]
    }
  ]);

  addPackage() {
    const name = prompt('Nhập tên gói thành viên mới (VD: Siêu cấp):');
    if (!name) return;
    const priceStr = prompt('Nhập giá VNĐ/tháng cho gói này (VD: 2000000):', '300000');
    const price = priceStr ? parseInt(priceStr, 10) : 0;
    
    const newPkg: MembershipPackage = {
      id: Date.now().toString(),
      name,
      price,
      activeUsers: 0,
      status: 'active',
      features: [{ text: 'Tính năng mặc định 1' }]
    };
    
    this.packages.update(pkgs => [...pkgs, newPkg]);
  }

  editPackage(pkg: MembershipPackage) {
    const newName = prompt('Chỉnh sửa tên gói:', pkg.name);
    if (!newName) return;
    
    const newPriceStr = prompt('Chỉnh sửa giá VNĐ/tháng:', pkg.price.toString());
    const newPrice = newPriceStr ? parseInt(newPriceStr, 10) : pkg.price;
    
    this.packages.update(pkgs => pkgs.map(p => {
      if (p.id === pkg.id) {
        return { ...p, name: newName, price: newPrice };
      }
      return p;
    }));
  }

  deletePackage(id: string) {
    if (confirm('Bạn có chắc chắn muốn xóa gói dịch vụ này? Hành động này không thể hoàn tác.')) {
      this.packages.update(pkgs => pkgs.filter(p => p.id !== id));
    }
  }
}
