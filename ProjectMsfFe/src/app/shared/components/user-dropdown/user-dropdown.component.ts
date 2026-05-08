import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  HostListener,
  OnInit,
} from '@angular/core';
import { AuthService } from '../../../features/auth/auth.service';
import { UserService } from '../../../core/services/user.service';
import { TokenService } from '../../../core/services/token.service';
import { ImageUrlPipe } from '../../pipes/image-url.pipe';

@Component({
  selector: 'app-user-dropdown',
  standalone: true,
  imports: [CommonModule, RouterLink, ImageUrlPipe],
  templateUrl: './user-dropdown.component.html',
  styleUrl: './user-dropdown.component.scss',
})
export class UserDropdownComponent implements OnInit {
  isOpen = false;
  constructor(
    private userService: UserService,
    private router: Router,
    private cd: ChangeDetectorRef,
    private tokenService: TokenService,
    private authService: AuthService
  ) {}

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }

  @HostListener('document:click', ['$event'])
  clickOutside(event: Event) {
    const target = event.target as HTMLElement;
    if (!target.closest('.user')) {
      this.isOpen = false;
    }
  }
  user: any;

  get fullName(): string {
    return this.user ? `${this.user.FirstName} ${this.user.LastName}` : '';
  }
  ngOnInit(): void {
    if (this.tokenService.getAccessToken()) {
      this.userService.GetUserbyid().subscribe({
        next: (user) => {
          this.user = user;
        },
        error: (err) => {
          console.error('lỗi', err);
        },
      });
    }
  }
  hasRole(roles: string[]): boolean {
    return this.user?.RoleName?.some((r: string) => roles.includes(r)) ?? false;
  }

  signOut() {
    this.authService.logout();
    this.router.navigate(['auth/login']);
  }
}
