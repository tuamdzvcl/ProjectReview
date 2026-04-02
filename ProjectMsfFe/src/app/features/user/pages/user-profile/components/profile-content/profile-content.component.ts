import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventModel } from '../../../../../../core/model/event.model';
import { FormatDatePipe } from '../../../../../../shared/pipes/format-date.pipe';
import { ImageUrlPipe } from '../../../../../../shared/pipes/image-url.pipe';

@Component({
  selector: 'app-profile-content',
  standalone: true,
  imports: [CommonModule, FormatDatePipe, ImageUrlPipe],
  templateUrl: './profile-content.component.html',
  styleUrl: './profile-content.component.scss'
})
export class ProfileContentComponent {
  @Input() isOwner: boolean = false;
  @Input() events: EventModel[] = [];

  activeTab = 'home';
  activeSubTab = 'saved';
}
