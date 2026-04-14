import { Pipe, PipeTransform } from '@angular/core';
import { environment } from '../../../environments/environment';

@Pipe({
  name: 'imageUrl',
  standalone: true
})
export class ImageUrlPipe implements PipeTransform {

  private baseUrl = environment.api;

  transform(path: string | null | undefined): string {
    if (!path) {
      return 'cat.jpg';
    }

    if (path.startsWith('http')) {
      return path;
    }

    return `${this.baseUrl}${path}`;
  }

}
