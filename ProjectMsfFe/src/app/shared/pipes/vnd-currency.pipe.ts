import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'vndCurrency',
  standalone: true
})
export class VndCurrencyPipe implements PipeTransform {

  transform(value: number | string | null | undefined): string {
    if (value === null || value === undefined || value === '') {
      return '0 ₫';
    }

    const numValue = typeof value === 'string' ? parseFloat(value) : value;

    if (isNaN(numValue)) {
      return '0 ₫';
    }

    const finalValue = numValue;

    const formatted = new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(finalValue);

    return formatted;
  }
}
