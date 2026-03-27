import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/pages/login/login.component';
import { SignupComponent } from './features/auth/pages/signup/signup.component';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { EventsComponent } from './features/events/pages/events/events.component';
import { HeaderComponent } from './shared/components/header/header.component';
import { EventsPageComponent } from './features/events/pages/events-page/events-page.component';
import { EventCardComponent } from './shared/components/event-card/event-card.component';
import { EventsGridComponent } from './features/events/components/events-grid/events-grid.component';
import { UserDropdownComponent } from './shared/components/user-dropdown/user-dropdown.component';
import { CreateEventComponent } from './shared/pages/create-event/create-event.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { LoginSuccessComponent } from './features/auth/pages/login-success/login-success.component';
import { UserComponent } from './features/user/user.component';
import { UserFormComponent } from './features/user-form/user-form.component';
import { authGuard } from './core/guards/auth.guard';
import { EventCreatePageComponent } from './shared/pages/event-create-page/event-create-page.component';
import { EventCreateTypeComponent } from './shared/pages/event-create-type/event-create-type.component';
import { EventCreateSettingComponent } from './shared/pages/event-create-setting/event-create-setting.component';

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
      { path: 'user', component: UserComponent },
      {
        path: 'UserForm',
        component: UserFormComponent,
        canActivate: [authGuard],
        data: { requiresAuth: true },
      },
    ],
  },
];
