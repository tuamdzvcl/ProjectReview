import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { UserDropdownComponent } from '../user-dropdown/user-dropdown.component';
import { CommonModule } from '@angular/common';
import { TokenService } from '../../../core/services/token.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterOutlet, UserDropdownComponent, RouterLink, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  userRole: string = '';
  isMenuOpen: boolean = false;

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu() {
    this.isMenuOpen = false;
  }

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
