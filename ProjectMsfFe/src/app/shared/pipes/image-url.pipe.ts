import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'imageUrl',
  standalone: true
})
export class ImageUrlPipe implements PipeTransform {

  private baseUrl = 'http://localhost:5083';

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
