import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserResponse } from '../../../../../../core/model/user.model';
import { EventModel } from '../../../../../../core/model/event.model';
import { FormatDatePipe } from '../../../../../../shared/pipes/format-date.pipe';
import { ImageUrlPipe } from '../../../../../../shared/pipes/image-url.pipe';

@Component({
  selector: 'app-profile-sidebar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile-sidebar.component.html',
  styleUrl: './profile-sidebar.component.scss'
})
export class ProfileSidebarComponent {
  @Input() user?: UserResponse;
  @Input() profileImageUrl: string = '';
  @Input() isOwner: boolean = false;

  @Output() avatarChange = new EventEmitter<File>();
  @Output() followClick = new EventEmitter<void>();

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) {
      this.avatarChange.emit(file);
    }
  }

  onFollow() {
    this.followClick.emit();
  }
}
