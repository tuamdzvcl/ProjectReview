import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { TokenService } from '../../core/services/token.service';
import { jwtDecode } from 'jwt-decode';

import { FooterComponent } from '../../shared/components/footer/footer.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet,
    HeaderComponent,
    FooterComponent],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent {
  constructor(private readonly tokenService: TokenService) { }

  ngOnInit(): void {
    const params = new URLSearchParams(window.location.search);
    const token = params.get('token');

    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        const now = Math.floor(Date.now() / 1000);
        if (decoded.exp && decoded.exp > now) {
          this.tokenService.setToken(token, '');
          window.history.replaceState({}, document.title, '/');
        }
      } catch (e) {
        // Token không hợp lệ, bỏ qua
      }
    }
  }
}
