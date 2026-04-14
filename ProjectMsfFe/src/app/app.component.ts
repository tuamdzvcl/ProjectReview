import { AuthService } from './features/auth/auth.service';
import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  constructor(
    private authService: AuthService, 
    private router: Router 
  ) {}

  ngOnInit() {
    const accessToken = localStorage.getItem('access_token');
    if (accessToken && this.authService.checkTokenExpired()) {
      this.logout();
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
