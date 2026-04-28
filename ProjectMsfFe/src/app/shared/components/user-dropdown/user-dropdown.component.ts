import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  HostListener,
  OnInit,
} from '@angular/core';
import { AuthService } from '../../../features/auth/auth.service';
import { ImageUrlPipe } from '../../pipes/image-url.pipe';
import { UserService } from '../../../core/services/user.service';

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
    private authService: AuthService,
    private userService: UserService,
    private router: Router,
    private cd: ChangeDetectorRef
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
    console.log('test');
    this.userService.GetUserbyid().subscribe({
      next: (user) => {
        this.user = user;
      },
      error: (err) => {
        console.error('lỗi', err);
      },
    });
    // this.user = this.authService.getUser();
  }
  hasRole(roles: string[]): boolean {
    return this.user?.RoleName?.some((r: string) => roles.includes(r)) ?? false;
  }

  signOut() {
    this.authService.logout();
    this.router.navigate(['auth/login']);
  }
}
