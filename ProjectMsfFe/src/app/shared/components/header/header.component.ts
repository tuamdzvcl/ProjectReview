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
  isMenuOpen: boolean = false;

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu() {
    this.isMenuOpen = false;
  }

  constructor(private tokenService: TokenService) {}

  canCreateEvent(): boolean {
    const role = this.tokenService.getRole();
    return role === 'ADMIN' || role === 'ORGANIZER';
  }
}
