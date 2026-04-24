import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { UserDropdownComponent } from '../user-dropdown/user-dropdown.component';
import { CommonModule } from '@angular/common';
import { TokenService } from '../../../core/services/token.service';
import { PermissionStoreService } from '../../../core/services/permission-store.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterOutlet, UserDropdownComponent, RouterLink, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent implements OnInit {
  private readonly permissionStore = inject(PermissionStoreService);

  hasPermission(permission: string): boolean {
    return this.permissionStore.hasPermission(permission);
  }

  isMenuOpen: boolean = false;

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu() {
    this.isMenuOpen = false;
  }

  constructor(private tokenService: TokenService) { }

  ngOnInit() {
    if (this.tokenService.getAccessToken() && !this.permissionStore.loaded()) {
      this.permissionStore.loadPermissions();
    }
  }

  canCreateEvent(): boolean {
    const role = this.tokenService.getRole();
    return role === 'ADMIN' || role === 'ORGANIZER';
  }
}
