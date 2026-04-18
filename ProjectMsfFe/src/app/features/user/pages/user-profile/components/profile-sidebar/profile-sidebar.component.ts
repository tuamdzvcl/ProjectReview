import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserResponse } from '../../../../../../core/model/response/user.model';
import { EventModel } from '../../../../../../core/model/response/event.model';
import { FormatDatePipe } from '../../../../../../shared/pipes/format-date.pipe';
import { ImageUrlPipe } from '../../../../../../shared/pipes/image-url.pipe';
import { UserService } from '../../../../../../core/services/user.service';

@Component({
  selector: 'app-profile-sidebar',
  standalone: true,
  imports: [CommonModule, ImageUrlPipe],
  templateUrl: './profile-sidebar.component.html',
  styleUrl: './profile-sidebar.component.scss',
})
export class ProfileSidebarComponent {
  private userService = inject(UserService);

  @Input() user?: UserResponse;
  @Input() profileImageUrl: string = '';
  @Input() isOwner: boolean = false;

  @Output() avatarChange = new EventEmitter<string>(); 
  @Output() followClick = new EventEmitter<void>();

  get isCustomer(): boolean {
    return this.user?.RoleName?.includes('customer') ?? false;
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) {
      this.userService.updateAvatar(file).subscribe({
        next: (res) => {
          if (res.Success) {
            this.profileImageUrl = res.Data;
            const userJson = localStorage.getItem('user');
            if (userJson) {
              const user = JSON.parse(userJson);
              user.AvatarUrl = res.Data;
              localStorage.setItem('user', JSON.stringify(user));
            }

            this.avatarChange.emit(res.Data);
            console.log('Cập nhật avatar thành công');
          }
        },
        error: (err) => {
          console.error('Lỗi khi cập nhật avatar:', err);
        }
      });
    }
  }

  onFollow() {
    this.followClick.emit();
  }
}
