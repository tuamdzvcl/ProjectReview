import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/pages/login/login.component';
import { SignupComponent } from './features/auth/pages/signup/signup.component';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { EventsComponent } from './features/events/pages/admin-events/events.component';
import { HeaderComponent } from './shared/components/header/header.component';
import { EventsPageComponent } from './features/events/pages/events-page/events-page.component';
import { CreateEventComponent } from './features/events/components/create-event/create-event.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
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

export const routes: Routes = [
  { path: 'header', component: HeaderComponent },

  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
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
    ],
  },
  {
    path: 'auth',
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'signup', component: SignupComponent },
    ],
  },
  {
    path: 'login-success',
    component: LoginSuccessComponent,
  },
  {
    path: 'admin',
    canActivate: [authGuard],
    data: { requiresAuth: true },
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'events', component: EventsComponent },
      { path: 'events/:id', component: AdminEventDetailComponent },
      { path: 'user', component: UserComponent },
      {
        path: 'UserForm',
        component: UserFormComponent,
        canActivate: [authGuard],
        data: { requiresAuth: true },
      },
    ],
  },
  {
    path: '**',
    component: NotFoundComponent,
  },
];
