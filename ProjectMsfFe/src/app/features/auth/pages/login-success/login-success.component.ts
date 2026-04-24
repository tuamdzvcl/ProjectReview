import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { TokenService } from '../../../../core/services/token.service';
import { AuthService } from '../../auth.service';
import { PermissionStoreService } from '../../../../core/services/permission-store.service';

@Component({
  selector: 'app-login-success',
  template: `<p>Đang xử lý đăng nhập...</p>`
})
export class LoginSuccessComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router,
    private token: TokenService,
    private permissionStore: PermissionStoreService
  ) {}

  ngOnInit(): void {
    const key = this.route.snapshot.queryParamMap.get('key');

    if (!key) {
      console.error('Không có key');
      return;
    }
    this.authService.getGoogleResult(key).subscribe({
      next: async (res) => {
        this.token.setToken(res.AccessToken, res.RefreshToken);
        localStorage.setItem('user', JSON.stringify(res.User));

        // Load permissions sau Google login
        await this.permissionStore.loadPermissions();

        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error('Lỗi login Google:', err);
      }
    });
  }
}