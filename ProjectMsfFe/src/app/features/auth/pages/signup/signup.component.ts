import { AuthData } from '../../../../core/model/response/auth-data.model';
import { ApiResponse } from '../../../../core/model/base/api-response.model';
import { AuthService } from '../../auth.service';
import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthLayoutComponent } from '../../ui/auth-layout/auth-layout.component';
import { Route, Router } from '@angular/router';

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

  readonly form;

  constructor(
    private readonly fb: FormBuilder,
    private readonly authservice: AuthService,
    private readonly router: Router
  ) {
    this.form = this.fb.nonNullable.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      username: [''],
    });
  }

  togglePassword(): void {
    this.passwordVisible.update((v) => !v);
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
        this.isSubmitting.set(false);
        alert('đăng kí thành công chuyển sang Login');
        this.router.navigate(['auth/login']);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        if (err.status === 400) {
          const errors = err.error.errors;

          if (errors?.FirstName) {
            alert(errors.FirstName[0]);
          }
          if (errors?.LastName) {
            alert(errors.LastName[0]);
          }

          if (errors?.Email) {
            alert(errors.Email[0]);
          }
        }
      },
    });
  }
}
