import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login';
import { RegisterComponent } from './auth/register/register';
import { MainLayout as MainLayoutComponent } from './layout/main-layout/main-layout';
import { Home as HomeComponent } from './home/home/home';
import { Deposit as DepositComponent } from './home/deposit/deposit';
import { TransferComponent} from './home/transfer/transfer';
import { authGuard } from './core/auth.guard';
import { authPublicGuard } from './core/auth-public.guard';

export const routes: Routes = [
    {
        path: 'auth',
        canMatch: [authPublicGuard],
        children: [
            { path: 'login', component: LoginComponent},
            { path: 'register', component: RegisterComponent},
            { path: '', redirectTo: 'login', pathMatch: 'full'}
        ]
    },

    { path: '**', redirectTo: '' },

    {
        path: '',
        component: MainLayoutComponent,
        children: [
            { path: 'home', component: HomeComponent, canActivate: [authGuard]},
            { path: 'home/deposit', component: DepositComponent, canActivate: [authGuard]},
            { path: 'home/transfer', component: TransferComponent, canActivate: [authGuard]},
            { path: '', redirectTo: 'home', pathMatch: 'full' }
        ]
    }
];
