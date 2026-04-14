import { BaseApiService } from '../../../../core/services/base-api.service';
import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiResponse } from '../../../../core/model/base/api-response.model';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './auth-layout.component.html',
  styleUrl: './auth-layout.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AuthLayoutComponent {
  @Input({ required: true }) topText!: string;
  @Input({ required: true }) topLinkText!: string;
  @Input({ required: true }) topLinkTo!: string | any[];

  @Input({ required: true }) title!: string;
  @Input() showSocial: boolean = true;

  constructor(
    private readonly baseApiService: BaseApiService
  ) { }

  logingoogle() {
    this.baseApiService.get<string>('auth/google-login-url')
      .subscribe({
        next: (res) => {
          window.location.href = res;
        },
        error: (err) => {
          alert(err.message)
          console.error('Login Google error', err);
        }
      });
  }


}

