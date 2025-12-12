import { Routes } from "@angular/router";
import { LoginComponent } from "./auth/login/login";
import { RegisterComponent } from "./auth/register/register";
import { authPublicGuard } from "./core/auth-public.guard";
import { authGuard } from "./core/auth.guard";
import { Deposit } from "./home/deposit/deposit";
import { Home } from "./home/home/home";
import { TransferComponent } from "./home/transfer/transfer";
import { MainLayout } from "./layout/main-layout/main-layout";

export const routes: Routes = [
  {
    path: 'auth',
    canMatch: [authPublicGuard],
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  },

  {
    path: '',
    component: MainLayout,
    canActivateChild: [authGuard],
    children: [
      {
        path: 'home',
        component: Home,
      },
      {
        path: 'home',
        children: [
          { path: 'deposit', component: Deposit },
          { path: 'transfer', component: TransferComponent },
        ]
      },
      { path: '', redirectTo: 'home', pathMatch: 'full' }
    ]
  },

  { path: '**', redirectTo: '' }
];
