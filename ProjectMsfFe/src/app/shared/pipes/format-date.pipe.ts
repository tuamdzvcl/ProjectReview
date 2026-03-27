import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatDate',
  standalone: true
})
export class FormatDatePipe implements PipeTransform {

  transform(dateInput: string | Date): string {
    if (!dateInput) return '';

    const date = new Date(dateInput);

    const day = date.getDate();
    const month = date.toLocaleString('vn-VN', { month: 'short' });
    const weekday = date.toLocaleString('vn-VN', { weekday: 'short' });

    let hours = date.getHours();
    const minutes = date.getMinutes().toString().padStart(2, '0');

    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12 || 12;

    return `${day} ${month} • ${weekday}, ${hours}:${minutes} ${ampm}`;
  }

}
