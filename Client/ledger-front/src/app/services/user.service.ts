import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';

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
export class UserService {
    private readonly baseUrl = "http://localhost:5000/api/users";

    constructor(private http: HttpClient) {}

    async getMe(): Promise<User | null> {
        try {
            const user = await firstValueFrom(this.http.get<User>(`${this.baseUrl}/me`));

            return user;

        } catch (exception) {
            console.log(exception);
            return null;
        }
    }

}