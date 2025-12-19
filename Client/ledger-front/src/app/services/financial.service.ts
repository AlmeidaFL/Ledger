import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';

interface DepositReponse {
    transactionId: string;
    status: string;
}

export interface DepositRequest {
    amount: number; 
    currency: string; 
    idempotencyKey: string;
}

export interface TransferRequest {
    toUserEmail: string;
    amount: number;
    currency: string;
    idempotencyKey: string,
    metadata: string
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
    private readonly baseUrl = "api/financial";

    constructor(private http: HttpClient) {}

    async deposit(request: DepositRequest): Promise<DepositReponse | null> {
        try{
            const response = await firstValueFrom(
                this.http.post<DepositReponse>(`${this.baseUrl}/deposit`, request, {withCredentials: true}))

            return response
        } catch (exception) {
            return null;
        }
    }

    async transfer(request: TransferRequest): Promise<TransferResponse | null> {
        try{
            const response = await firstValueFrom(
                this.http.post<TransferResponse>(`${this.baseUrl}/transfer`, request, {withCredentials: true}))

            return response;
        } catch (exception) {
            return null;
        }
    }

    async getBalance(): Promise<BalanceResponse | null> {
        try{
            return await firstValueFrom(
                this.http.get<BalanceResponse>(`${this.baseUrl}/balance`, {withCredentials: true}))

        } catch (exception) {
            return null;
        }
    }
}