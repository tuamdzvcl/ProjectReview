import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/pages/login/login.component';
import { SignupComponent } from './features/auth/pages/signup/signup.component';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { EventsComponent } from './features/events/pages/admin-events/events.component';
import { HeaderComponent } from './shared/components/header/header.component';
import { EventsPageComponent } from './features/events/pages/events-page/events-page.component';
import { CreateEventComponent } from './features/events/components/create-event/create-event.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { AppShellComponent } from './layouts/app-shell/app-shell.component';
import { LoginSuccessComponent } from './features/auth/pages/login-success/login-success.component';
import { UserFormComponent } from './features/user/componnets/user-form/user-form.component';
import { authGuard } from './core/guards/auth.guard';
import { EventCreateTypeComponent } from './features/events/components/event-create-type/event-create-type.component';
import { EventCreateSettingComponent } from './features/events/components/event-create-setting/event-create-setting.component';
import { EventCreatePageComponent } from './features/events/pages/event-create-page/event-create-page.component';
import { UserComponent } from './features/user/pages/admin-user/user.component';
import { EventDetailPageComponent } from './features/events/pages/event-detail-page/event-detail-page.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { AdminEventDetailComponent } from './features/events/pages/admin-event-detail/admin-event-detail.component';
import { UserProfileComponent } from './features/user/pages/user-profile/user-profile.component';
import { CheckoutPageComponent } from './features/events/pages/checkout-page/checkout-page.component';
import { OrderDetailPageComponent } from './features/events/pages/order-detail-page/order-detail-page.component';
import { ApproveEventsComponent } from './features/events/pages/approve-events/approve-events.component';
import { ParticipantListComponent } from './features/user/pages/participant-list/participant-list.component';

import { LandingPageComponent } from './features/home/pages/landing-page/landing-page.component';
import { PricingComponent } from './features/home/pages/pricing/pricing.component';
import { BlogComponent } from './features/home/pages/blog/blog.component';
import { SupportComponent } from './features/home/pages/support/support.component';

import { PromotionsComponent } from './features/promotions/pages/admin-promotions/promotions.component';
import { PaymentsComponent } from './features/payments/pages/admin-payments/payments.component';
import { AuditLogComponent } from './features/audit-log/pages/admin-audit-log/audit-log.component';
import { MembershipComponent } from './features/membership/pages/admin-membership/membership.component';
import { PaymentResultComponent } from './features/payment/pages/payment-result/payment-result.component';
import { ForgotPasswordComponent } from './features/auth/pages/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './features/auth/pages/reset-password/reset-password.component';
import { VerifyEmailComponent } from './features/auth/pages/verify-email/verify-email.component';
import { RegisterSuccessComponent } from './features/auth/pages/register-success/register-success.component';
import { RoleComponent } from './features/roles/pages/admin-roles/role.component';

export const routes: Routes = [
  { path: 'header', component: HeaderComponent },

  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        component: LandingPageComponent,
      },
      {
        path: 'discover',
        component: EventsPageComponent,
      },
      {
        path: 'create-event',
        component: CreateEventComponent,
        canActivate: [authGuard],
        data: { requiresAuth: true },
      },
      {
        path: 'event-create-page',
        component: EventCreatePageComponent,
        canActivate: [authGuard],
        data: { requiresAuth: true },
        children: [
          {
            path: '',
            component: CreateEventComponent,
          },
          {
            path: 'create-type',
            component: EventCreateTypeComponent,
          },
          {
            path: 'create-setting',
            component: EventCreateSettingComponent,
          },
        ],
      },
      {
        path: 'event-create-page/:id',
        component: EventCreatePageComponent,
        canActivate: [authGuard],
        data: { requiresAuth: true },
        children: [
          {
            path: '',
            component: CreateEventComponent,
          },
          {
            path: 'create-type',
            component: EventCreateTypeComponent,
          },
          {
            path: 'create-setting',
            component: EventCreateSettingComponent,
          },
        ],
      },
      {
        path: 'event/:id',
        component: EventDetailPageComponent,
      },
      {
        path: 'profile',
        component: UserProfileComponent,
      },
      {
        path: 'profile/:id',
        component: UserProfileComponent,
      },
      {
        path: 'pricing',
        component: PricingComponent,
      },
      {
        path: 'blog',
        component: BlogComponent,
      },
      {
        path: 'support',
        component: SupportComponent,
      },
      {
        path: 'checkout',
        component: CheckoutPageComponent,
        canActivate: [authGuard],
        data: { requiresAuth: true }
      },
      {
        path: 'payment',
        component: PaymentResultComponent,
      },
      {
        path: 'order/:id',
        component: OrderDetailPageComponent,
        canActivate: [authGuard],
        data: { requiresAuth: true }
      },
    ],
  },
  {
    path: 'auth',
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'signup', component: SignupComponent },
      { path: 'forgot-password', component: ForgotPasswordComponent },
      { path: 'reset-password', component: ResetPasswordComponent },
      { path: 'verify-email', component: VerifyEmailComponent },
      { path: 'register-success', component: RegisterSuccessComponent },

    ],
  },
  {
    path: 'login-success',
    component: LoginSuccessComponent,
  },
  {
    path: 'admin',
    component: AppShellComponent,
    canActivate: [authGuard],
    data: { requiresAuth: true },
    children: [
      { path: 'dashboard', component: DashboardComponent, data: { requiresAuth: true } },
      { path: 'events', component: EventsComponent, data: { requiresAuth: true } },
      { path: 'approve-events', component: ApproveEventsComponent, data: { requiresAuth: true } },
      { path: 'events/:id', component: AdminEventDetailComponent, data: { requiresAuth: true } },
      { path: 'user', component: UserComponent, data: { requiresAuth: true } },
      { path: 'participants', component: ParticipantListComponent, data: { requiresAuth: true } },
      {
        path: 'UserForm',
        component: UserFormComponent,
        canActivate: [authGuard],
        data: { requiresAuth: true },
      },
      { path: 'promotions', component: PromotionsComponent, data: { requiresAuth: true } },
      { path: 'payments', component: PaymentsComponent, data: { requiresAuth: true } },
      { path: 'audit-log', component: AuditLogComponent, data: { requiresAuth: true } },
      { path: 'membership', component: MembershipComponent, data: { requiresAuth: true } },
      { path: 'roles', component: RoleComponent, data: { requiresAuth: true } },
    ],
  },
  {
    path: '**',
    component: NotFoundComponent,
  },
];
