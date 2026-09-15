import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { Login } from './features/login/login';
import { Register } from './features/register/register';
import { Profile } from './features/profile/profile';
import { Welcome } from './features/welcome/welcome';

import { Logout } from './features/logout/logout';

export const routes: Routes = [
  { path: '', component: Welcome, canActivate: [authGuard] },
  { path: 'login', component: Login },
  { path: 'logout', component: Logout },
  { path: 'register', component: Register },
  { path: 'profile', component: Profile, canActivate: [authGuard] },
];