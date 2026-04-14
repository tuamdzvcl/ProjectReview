import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../auth.service';
import { AuthLayoutComponent } from '../../ui/auth-layout/auth-layout.component';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AuthLayoutComponent, RouterModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ResetPasswordComponent {
  readonly isSubmitting = signal(false);
  readonly isSuccess = signal(false);
  readonly passwordVisible = signal(false);
  readonly confirmPasswordVisible = signal(false);

  readonly form;
  private email: string = '';
  private token: string = '';

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly authService: AuthService
  ) {
    this.form = this.fb.nonNullable.group({
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
    }, {
      validators: (group: any) => {
        const password = group.get('password')?.value;
        const confirmPassword = group.get('confirmPassword')?.value;
        return password === confirmPassword ? null : { notMatching: true };
      }
    });

    this.route.queryParams.subscribe(params => {
      this.email = params['email'];
      this.token = params['token'];

      if (!this.email || !this.token) {
        alert('Đường dẫn không hợp lệ hoặc đã hết hạn.');
        this.router.navigate(['/auth/login']);
      }
    });
  }

  togglePassword(): void {
    this.passwordVisible.update(v => !v);
  }

  toggleConfirmPassword(): void {
    this.confirmPasswordVisible.update(v => !v);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.isSubmitting.set(true);

    const { password } = this.form.getRawValue();

    this.authService.resetPassword({
      email: this.email,
      token: this.token,
      newPassword: password
    }).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.isSuccess.set(true);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        alert(err.message || 'Đặt lại mật khẩu thất bại. Vui lòng thử lại.');
      }
    });
  }
}
