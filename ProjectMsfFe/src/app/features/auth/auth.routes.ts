import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { SignupComponent } from './pages/signup/signup.component';
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password.component';
import { RegisterSuccessComponent } from './pages/register-success/register-success.component';
import { VerifyEmailComponent } from './pages/verify-email/verify-email.component';

export const AUTH_ROUTES: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'signup',
    component: SignupComponent
  },
  {
    path: 'forgot-password',
    component: ForgotPasswordComponent
  },
  {
    path: 'register-success',
    component: RegisterSuccessComponent
  },
  {
    path: 'verify-email',
    component: VerifyEmailComponent
  },
  { path: '', pathMatch: 'full', redirectTo: 'auth/login' }
];
