import { AuthData } from '../../../../core/model/response/auth-data.model';
import Swal from 'sweetalert2';
import { ApiResponse } from '../../../../core/model/base/api-response.model';
import { AuthService } from '../../auth.service';
import { CommonModule } from '@angular/common';
import { ApiError } from '../../../../core/model/base/ApiError.model';
import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { AuthLayoutComponent } from '../../ui/auth-layout/auth-layout.component';
import { Route, Router } from '@angular/router';

export function noWhitespaceValidator(control: AbstractControl): ValidationErrors | null {
  const isWhitespace = (control.value || '').trim().length === 0;
  return isWhitespace ? { whitespace: true } : null;
}

export function passwordMatchValidator(group: AbstractControl): ValidationErrors | null {
  const password = group.get('password')?.value;
  const confirmPassword = group.get('confirmPassword')?.value;
  if (!password || !confirmPassword) return null;
  return password === confirmPassword ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AuthLayoutComponent],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SignupComponent {
  readonly isSubmitting = signal(false);
  readonly passwordVisible = signal(false);
  readonly confirmPasswordVisible = signal(false);

  readonly form;

  constructor(
    private readonly fb: FormBuilder,
    private readonly authservice: AuthService,
    private readonly router: Router
  ) {
    this.form = this.fb.nonNullable.group({
      firstName: ['', [Validators.required, Validators.min(1), Validators.max(20)]],
      lastName: ['', [Validators.required, Validators.min(1), Validators.max(20)]],
      email: ['', [Validators.required, Validators.email, noWhitespaceValidator]],
      password: ['', [Validators.required, Validators.minLength(6), noWhitespaceValidator]],
      confirmPassword: ['', [Validators.required]],
      username: [''],
    }, { validators: passwordMatchValidator });
  }

  togglePassword(): void {
    this.passwordVisible.update((v) => !v);
  }

  toggleConfirmPassword(): void {
    this.confirmPasswordVisible.update((v) => !v);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const data = this.form.getRawValue();
    const payload = {
      Username: data.username.trim(),
      FirstName: data.firstName.trim(),
      LastName: data.lastName.trim(),
      Email: data.email.trim(),
      password: data.password.trim(),
    };

    this.authservice.register(payload).subscribe({
      next: (res) => {
        console.log('Đăng ký thành công, dữ liệu nhận được:', res);
        this.isSubmitting.set(false);
        this.router.navigate(['auth/register-success'], { queryParams: { email: payload.Email } });
      },
      error: (err) => {
        console.error('Call API Error:', err);
        this.isSubmitting.set(false);

        if (err instanceof ApiError || err.name === 'ApiError') {
          Swal.fire({
            icon: 'error',
            title: 'Đăng ký thất bại',
            text: err.message || 'Lỗi từ Backend.',
            confirmButtonColor: '#1976d2'
          });
        }
        else if (err.status === 400 && err.error?.errors) {
          const errors = err.error.errors;
          let errorMessage = '<ul style="text-align: left; list-style-type: disc; margin-left: 20px;">';

          if (errors?.FirstName) {
            errorMessage += `<li>${errors.FirstName[0]}</li>`;
          }
          if (errors?.LastName) {
            errorMessage += `<li>${errors.LastName[0]}</li>`;
          }
          if (errors?.Email) {
            errorMessage += `<li>${errors.Email[0]}</li>`;
          }

          errorMessage += '</ul>';

          Swal.fire({
            icon: 'warning',
            title: 'Lỗi thông tin',
            html: errorMessage,
            confirmButtonColor: '#1976d2'
          });
        } else if (err.status === 500) {
          Swal.fire({
            icon: 'error',
            title: 'Lỗi hệ thống',
            text: 'Lỗi hệ thống Server. Vui lòng thử lại sau.',
            confirmButtonColor: '#1976d2'
          });
        } else {
          // HTTP error generic fallback
          Swal.fire({
            icon: 'error',
            title: 'Lỗi mạng',
            text: err.error?.message || err.message || 'Đã có lỗi xảy ra. Hãy kiểm tra lại kết nối mạng.',
            confirmButtonColor: '#1976d2'
          });
        }
      },
    });
  }
}
