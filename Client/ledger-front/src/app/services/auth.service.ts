import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';
import { UserService } from './user.service';

interface RegisterRequest {
    email: string;
    fullName: string
    password: string;
}

interface LoginRequest {
    email: string;
    password: string;
}

interface User {
    id: string;
    email: string;
    fullName: string;
    isActive: boolean;
    createdAt: Date;
    updatedAt: Date;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
    private readonly baseUrl = "http://localhost:5000";

    sessionLoaded = signal(false);
    currentUser = signal<User | null>(null);

    constructor(private http: HttpClient, private router: Router, private userService: UserService) {}

    async register(request: RegisterRequest): Promise<boolean> {
        try {
            await firstValueFrom(this.http.post(`${this.baseUrl}/api/auth/register`, request, {withCredentials: true}));

            this.router.navigate(['/login']);
            return true;

        } catch (err) {
            console.error('register failed:', err);
            return false;
        }
    }

    async login(request: LoginRequest): Promise<boolean> {
        try {
            await firstValueFrom(
                this.http.post(`${this.baseUrl}/api/auth/login`, request, {withCredentials: true})
            );

            const user = await this.userService.getMe();

            if (!user){
                console.error('User not returned after login');
                return false;
            }

            if (!user.isActive) {
                console.error('User is not active');
                return false;
            }

            this.sessionLoaded.set(true);
            this.currentUser.set(user);
            this.router.navigate(['/home']);
            return true;

        } catch (err) {
            console.error('login failed:', err);
            return false;
        }
    }

    async bootstrapSession() {
        const user = await this.userService.getMe();
        this.currentUser.set(user);
        this.sessionLoaded.set(true);
    }

    async logout(): Promise<void> {
        try {
            await firstValueFrom(this.http.post(`${this.baseUrl}/api/auth/logout`, {}, {withCredentials: true}));
            
        } catch (err) {
            console.error('logout failed:', err);
        }

        this.currentUser.set(null);
        this.router.navigate(['/login']);
    }

    async loadMe(): Promise<boolean> {
        try {
            const user = await firstValueFrom(this.http.get<User>(`${this.baseUrl}/api/users?email=`, {withCredentials: true}));

            this.currentUser.set(user);
            return true;

        } catch {
            return false;
        }
    }

    async refreshToken(): Promise<boolean> {
        try {
            await firstValueFrom(this.http.post(`${this.baseUrl}/api/auth/refresh`, {}, {withCredentials: true}));
        } catch (err) {
            console.error('refresh failed:', err);
            return false;
        }

        return true;
    }
}