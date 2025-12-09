import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';

interface DepositReponse {
    transactionId: string;
    status: string;
}

export interface DepositRequest {
    userEmail: string;
    amount: number; 
    currency: string; 
    idempotencyKey: string;
}
    
@Injectable({
  providedIn: 'root'
})
export class FinancialService {
    private readonly baseUrl = "http://localhost:5000/api/financial";

    constructor(private http: HttpClient) {}

    async deposit(request: DepositRequest): Promise<DepositReponse | null> {
        try{
            const response = await firstValueFrom(
                this.http.post<DepositReponse>(`${this.baseUrl}/deposit`, request))

            return response
        } catch (exception) {
            console.error("Deposit failed");
            return null;
        }
    }
}