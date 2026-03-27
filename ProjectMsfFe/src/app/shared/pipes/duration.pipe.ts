import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'duration',
standalone: true

})
export class DurationPipe implements PipeTransform {

transform(start: string | Date, end: string | Date): string {
    if (!start || !end) return '';

    const startDate = new Date(start);
    const endDate = new Date(end);

    const diffMs = endDate.getTime() - startDate.getTime();
    const diffMinutes = Math.floor(diffMs / (1000 * 60));

    const hours = Math.floor(diffMinutes / 60);
    const minutes = diffMinutes % 60;

    if (hours > 0 && minutes > 0) {
      return `${hours}h ${minutes}m`;
    }

    if (hours > 0) {
      return `${hours}h`;
    }

    return `${minutes}m`;
  }
}