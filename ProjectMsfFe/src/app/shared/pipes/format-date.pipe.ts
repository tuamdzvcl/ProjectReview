import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatDate',
  standalone: true
})
export class FormatDatePipe implements PipeTransform {

  private readonly VN_TIMEZONE = 'Asia/Ho_Chi_Minh';
  private readonly VN_LOCALE = 'vi-VN';

  transform(dateInput: string | Date): string {
    if (!dateInput) return '';

    const date = new Date(dateInput);

    const day = date.toLocaleString(this.VN_LOCALE, { day: '2-digit', timeZone: this.VN_TIMEZONE });
    const month = date.toLocaleString(this.VN_LOCALE, { month: 'short', timeZone: this.VN_TIMEZONE });
    const weekday = date.toLocaleString(this.VN_LOCALE, { weekday: 'short', timeZone: this.VN_TIMEZONE });
    const hours = date.toLocaleString(this.VN_LOCALE, { hour: '2-digit', minute: '2-digit', hour12: false, timeZone: this.VN_TIMEZONE });

    return `${day} ${month} • ${weekday}, ${hours}`;
  }

}
