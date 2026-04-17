import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../auth.service';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './verify-email.component.html',
  styleUrl: './verify-email.component.scss'
})
export class VerifyEmailComponent implements OnInit {
  status: 'loading' | 'success' | 'error' = 'loading';
  icon = 'hourglass_empty';
  title = 'Đang xác thực...';
  message = 'Vui lòng chờ trong giây lát.';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');
    
    if (!token) {
      this.status = 'error';
      this.icon = 'error_outline';
      this.title = 'Lỗi xác thực';
      this.message = 'Không tìm thấy mã xác thực. Vui lòng kiểm tra lại đường dẫn email.';
      return;
    }

    this.authService.verifyEmail(token).subscribe({
      next: (res) => {
        this.status = 'success';
        this.icon = 'check_circle_outline';
        this.title = 'Xác thực thành công';
        this.message = 'Email của bạn đã được xác thực thành công. Tự động chuyển hướng về Đăng nhập...';
        
        setTimeout(() => {
          this.router.navigate(['auth/login']);
        }, 3000);
      },
      error: (err) => {
        this.status = 'error';
        this.icon = 'error_outline';
        this.title = 'Xác thực thất bại';
        
        if (err.error?.message) {
          this.message = err.error.message;
        } else {
          this.message = 'Link xác thực đã hết hạn hoặc không hợp lệ. Vui lòng thử đăng nhập và gửi lại email xác thực.';
        }
      }
    });
  }
}
