import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'eventStatus',
  standalone: true
})
export class EventStatusPipe implements PipeTransform {
  transform(value: string | number): string {
    if (!value) return 'Không xác định';

    const statusMap: { [key: string]: string } = {
      'DRAFT': 'Bản nháp',
      'PUBLISHED': 'Đã công khai',
      'CANNEL': 'Đã hủy',
      'PUBLIC': 'Chờ duyệt',
      'ENDED': 'Đã kết thúc',
      'REQUEST_EDIT': 'Xin chỉnh sửa',
      // Có thể nhận vào dạng số từ backend nếu không parse string
      '1': 'Bản nháp',
      '2': 'Đã công khai',
      '3': 'Đã hủy',
      '4': 'Chờ duyệt',
      '5': 'Đã kết thúc',
      '6': 'Xin chỉnh sửa'
    };

    return statusMap[value.toString()] || value.toString();
  }
}
