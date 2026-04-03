import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { UserDropdownComponent } from '../../shared/components/user-dropdown/user-dropdown.component';
import { CreateEventComponent } from '../../features/events/components/create-event/create-event.component';
import { TokenService } from '../../core/services/token.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    UserDropdownComponent,
    CreateEventComponent,
  ],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShellComponent {
  private tokenService = inject(TokenService);
  
  sidebarOpen = signal(true);
  userRole = signal<string | null>(this.tokenService.getRole());

  toggleSidebar() {
    this.sidebarOpen.update((v) => !v);
  }
}
