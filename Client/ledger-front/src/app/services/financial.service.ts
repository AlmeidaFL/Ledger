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

export interface TransferRequest {
    toUserEmail: string;
    amount: number;
    currency: string;
    idempotencyKey: string
}

interface TransferResponse {
    transactionId: string;
    status: string;
    isIdempotentReplay: boolean
}

interface BalanceResponse {
    amount: number;
    userEmail: string;
    currency: string;
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

    async transfer(request: TransferRequest): Promise<boolean> {
        try{
            await firstValueFrom(
                this.http.post<TransferResponse>(`${this.baseUrl}/transfer`, request))

            return true;
        } catch (exception) {
            console.error("Transfer failed");
            return false;
        }
    }

    async getBalance(): Promise<BalanceResponse | null> {
        try{
            return await firstValueFrom(
                this.http.get<BalanceResponse>(`${this.baseUrl}/balance`, ))

        } catch (exception) {
            console.error("Get Balance failed");
            return null;
        }
    }
}