import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'roleName',
  standalone: true
})
export class RolePipe implements PipeTransform {
  transform(value: string): string {
    if (!value) return 'Không xác định';

    const roleMap: { [key: string]: string } = {
      'ADMIN': 'Quản trị viên',
      'ORGANIZER': 'Nhà tổ chức',
      'CUSTOMER': 'Người dùng',
      'STAFF': 'Nhân viên',
      'SUPERADMIN': 'Quản trị viên cấp cao'
    };

    // Chuyển về uppercase để so khớp chính xác
    const key = value.toUpperCase();
    return roleMap[key] || value;
  }
}
