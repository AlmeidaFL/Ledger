import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login';
import { RegisterComponent } from './auth/register/register';
import { MainLayout as MainLayoutComponent } from './layout/main-layout/main-layout';
import { Home as HomeComponent } from './home/home/home';
import { Deposit as DepositComponent } from './home/deposit/deposit';
import { Transfer as TransferComponent } from './home/transfer/transfer';

export const routes: Routes = [
    {
        path: 'auth',
        children: [
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
            // { path: '', redirectTo: 'login', pathMatch: 'full' }
        ]
    },
    // { path: '', redirectTo: 'auth/login', pathMatch: 'full'},


    {
        path: '',
        component: MainLayoutComponent,
        children: [
            { path: 'home', component: HomeComponent},
            { path: 'home/deposit', component: DepositComponent},
            { path: 'home/transfer', component: TransferComponent},
            { path: '', redirectTo: 'home', pathMatch: 'full' }
        ]
    }
];
