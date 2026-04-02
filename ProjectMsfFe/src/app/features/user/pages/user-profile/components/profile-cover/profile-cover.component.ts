import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile-cover',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile-cover.component.html',
  styleUrl: './profile-cover.component.scss'
})
export class ProfileCoverComponent {
  @Input() coverImageUrl: string = '';
  @Input() isOwner: boolean = false;
  @Output() coverChange = new EventEmitter<File>();

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) {
      this.coverChange.emit(file);
    }
  }
}
