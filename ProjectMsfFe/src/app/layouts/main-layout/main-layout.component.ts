import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { TokenService } from '../../core/services/token.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet,
  HeaderComponent],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent {
constructor(private readonly tokenService: TokenService) {}

ngOnInit(): void {
  const params = new URLSearchParams(window.location.search);
  const token = params.get('token');

  if (token) {
    
    localStorage.setItem('access_token', token);
    
    window.history.replaceState({}, document.title, '/');
  }
}
}
