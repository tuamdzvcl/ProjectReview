import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.scss',
})
export class UserProfileComponent {
  coverImageUrl =
    'https://images.pexels.com/photos/4697568/pexels-photo-4697568.jpeg';
  profileImageUrl =
    'https://images.pexels.com/photos/4697568/pexels-photo-4697568.jpeg';

  activeTab = 'home';
  activeSubTab = 'saved';

  events = [
    {
      id: 1,
      title: 'Step Up Open Mic Show',
      date: 'Thu, Jun 30, 2022 4:30 AM',
      imageUrl:
        'https://images.pexels.com/photos/4697568/pexels-photo-4697568.jpeg',
    },
  ];

  onCoverImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) {
      this.coverImageUrl = URL.createObjectURL(file);
    }
  }

  onProfileImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) {
      this.profileImageUrl = URL.createObjectURL(file);
    }
  }
}
