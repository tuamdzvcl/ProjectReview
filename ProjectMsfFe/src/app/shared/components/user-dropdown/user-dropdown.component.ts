import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { AuthService } from '../../../features/auth/auth.service';

@Component({
  selector: 'app-user-dropdown',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './user-dropdown.component.html',
  styleUrl: './user-dropdown.component.scss',
})
export class UserDropdownComponent {
  isOpen = false;
  constructor(private authService: AuthService, private router: Router) { }

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }

  @HostListener('document:click', ['$event'])
  clickOutside(event: Event) {
    const target = event.target as HTMLElement
    if (!target.closest('.user')) {
      this.isOpen = false
    }
  }
  user: any;

  get fullName(): string {
    return this.user
      ? `${this.user.FirstName} ${this.user.LastName}`
      : '';
  }
  ngOnInit(): void {
    this.user = this.authService.getUser();
  }
  hasRole(roles: string[]): boolean {
    return this.user?.RoleName?.some((r: string) => roles.includes(r)) ?? false;
  }

  signOut() {
    this.authService.logout();
    this.router.navigate(['auth/login']);
  }
}

