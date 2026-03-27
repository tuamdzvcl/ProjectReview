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

    // Convert to number if it's a string
    const numValue = typeof value === 'string' ? parseFloat(value) : value;

    // Check if it's a valid number
    if (isNaN(numValue)) {
      return '0 ₫';
    }

    // For small numbers like 30, assume it's in thousands (30,000 VND)
    // For larger numbers, use as-is
    const finalValue = numValue < 1000 ? numValue * 1000 : numValue;

    // Format with Vietnamese locale
    const formatted = new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(finalValue);

    return formatted;
  }
}
