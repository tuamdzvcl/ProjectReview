import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../../../core/services/user.service';
import { TokenService } from '../../../../core/services/token.service';
import { UserResponse } from '../../../../core/model/response/user.model';
import { EventModel } from '../../../../core/model/response/event.model';
import { UserEventsResponse } from '../../../../core/model/response/user-events-response.model';
import { UserProfile } from '../../../../core/model/response/userprofile.model';

import { ProfileCoverComponent } from './components/profile-cover/profile-cover.component';
import { ProfileSidebarComponent } from './components/profile-sidebar/profile-sidebar.component';
import { ProfileContentComponent } from './components/profile-content/profile-content.component';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [
    CommonModule,
    ProfileCoverComponent,
    ProfileSidebarComponent,
    ProfileContentComponent
  ],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.scss',
})
export class UserProfileComponent implements OnInit {
  user?: UserResponse;
  events: EventModel[] = [];
  isOwner: boolean = false;

  coverImageUrl =
    'https://images.pexels.com/photos/4697568/pexels-photo-4697568.jpeg';
  profileImageUrl =
    'https://images.pexels.com/photos/4697568/pexels-photo-4697568.jpeg';

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private tokenService: TokenService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      this.determineOwnership(id);
      this.loadUserProfile(id || undefined);
    });
  }

  determineOwnership(urlId: string | null) {
    const currentUserId = this.tokenService.getUserId();
    if (!urlId) {
      this.isOwner = true;
    } else {
      this.isOwner = currentUserId === urlId;
    }
  }

  loadUserProfile(id?: string) {
    if (id && !this.isOwner) {
      // Viewing another user's profile
      this.userService.getUserProfile(id).subscribe({
        next: (res: UserProfile) => {
          this.user = {
            Id: id,
            Email: '',
            Username: '',
            LastName: res.User.LastName,
            FirstName: res.User.FirstName,
            RoleName: [],
            AvatarUrl: res.User.AvatarUrl,
          };
          this.events = res.Events;
          if (this.user.AvatarUrl) {
            this.profileImageUrl = this.user.AvatarUrl;
          }
        },
      });
    } else {
      // Viewing own profile
      this.userService.getUserEvents().subscribe({
        next: (res: UserEventsResponse) => {
          this.user = res.User;
          this.events = res.Events;
          if (this.user.AvatarUrl) {
            this.profileImageUrl = this.user.AvatarUrl;
          }
        },
      });
    }
  }

  onCoverChange(file: File) {
    this.coverImageUrl = URL.createObjectURL(file);
  }

  onAvatarChange(file: File) {
    this.profileImageUrl = URL.createObjectURL(file);
  }

  onFollow() {
    console.log('Follow clicked');
  }
}


