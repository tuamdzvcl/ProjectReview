import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-hero-search',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './hero-search.component.html',
  styleUrl: './hero-search.component.scss',
})
export class HeroSearchComponent {
  keyword: string = '';

  constructor(private router: Router) {}

  searchTitleEvent() {
    if (this.keyword && this.keyword.trim() !== '') {
      // Chuyển hướng sang trang danh sách sự kiện kèm theo tham số keyword
      this.router.navigate(['/discover'], { 
        queryParams: { keyword: this.keyword.trim() } 
      });
    } else {
      // Nếu bỏ trống thì chỉ chuyển sang trang danh sách
      this.router.navigate(['/discover']);
    }
  }
}
