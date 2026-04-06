import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TokenService } from '../../../../core/services/token.service';
import { EventsGridComponent } from '../../../../features/events/components/events-grid/events-grid.component';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, RouterLink, EventsGridComponent],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.scss'
})
export class LandingPageComponent {
  categories = [
    { name: 'Âm nhạc', icon: 'fa-solid fa-music', color: '#62a839', route: '/discover?category=music' },
    { name: 'Ẩm thực', icon: 'fa-solid fa-utensils', color: '#62a839', route: '/discover?category=food' },
    { name: 'Hội thảo', icon: 'fa-solid fa-chalkboard-user', color: '#62a839', route: '/discover?category=workshop' },
    { name: 'Nghệ thuật', icon: 'fa-solid fa-palette', color: '#62a839', route: '/discover?category=art' }
  ];

  howItWorks = [
    { title: 'Khám phá', desc: 'Tìm kiếm những sự kiện hấp dẫn nhất xung quanh bạn.', icon: 'fa-solid fa-compass' },
    { title: 'Đặt vé', desc: 'Sở hữu vé tham gia nhanh chóng và an toàn.', icon: 'fa-solid fa-ticket' },
    { title: 'Trải nghiệm', desc: 'Tận hưởng những giây phút khó quên.', icon: 'fa-solid fa-star' }
  ];

  userRole: string = '';

  constructor(private tokenService: TokenService) {
    this.getUserRole();
  }

  private getUserRole() {
    try {
      const accessToken = this.tokenService.getAccessToken();
      if (accessToken) {
        const tokenPayload = JSON.parse(atob(accessToken.split('.')[1]));
        this.userRole = tokenPayload.role || tokenPayload.Role || '';
      }
    } catch (error) {
      this.userRole = '';
    }
  }

  canCreateEvent(): boolean {
    return (
      this.userRole === 'admin'.toUpperCase() ||
      this.userRole === 'organizer'.toUpperCase()
    );
  }
}
