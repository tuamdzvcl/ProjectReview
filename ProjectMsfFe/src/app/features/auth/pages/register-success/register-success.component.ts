import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../auth.service';

@Component({
  selector: 'app-register-success',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './register-success.component.html',
  styleUrl: './register-success.component.scss'
})
export class RegisterSuccessComponent implements OnInit, OnDestroy {
  userEmail: string | null = null;
  countdown: number = 300; // 5 minutes in seconds
  timer: any;
  isSending = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.userEmail = this.route.snapshot.queryParamMap.get('email');
    if (!this.userEmail) {
      this.router.navigate(['auth/login']);
    } else {
      this.startCountdown();
    }
  }

  ngOnDestroy(): void {
    if (this.timer) {
      clearInterval(this.timer);
    }
  }

  startCountdown(): void {
    this.countdown = 300;
    if (this.timer) {
      clearInterval(this.timer);
    }
    this.timer = setInterval(() => {
      if (this.countdown > 0) {
        this.countdown--;
      } else {
        clearInterval(this.timer);
      }
    }, 1000);
  }

  formatTime(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s < 10 ? '0' : ''}${s}`;
  }

  resendEmail(): void {
    if (!this.userEmail || this.isSending) return;

    this.isSending = true;
    this.authService.resendVerification(this.userEmail).subscribe({
      next: () => {
        this.isSending = false;
        alert('Đã gửi lại email xác thực thành công. Vui lòng kiểm tra hộp thư.');
        this.startCountdown();
      },
      error: (err) => {
        this.isSending = false;
        const msg = err.error?.message || 'Có lỗi xảy ra khi gửi lại email.';
        alert(msg);
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['auth/login']);
  }
}
