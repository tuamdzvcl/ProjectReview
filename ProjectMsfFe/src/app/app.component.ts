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
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    this.router.navigate(['/']);
  }
}
