import { Component, OnInit } from '@angular/core';
import { FinancialService } from '../../services/financial.service';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [CurrencyPipe],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  balance: number = 0;

  constructor(private financialService: FinancialService){

  }

  async ngOnInit() {
    const result = await this.financialService.getBalance()
    
    if (!result){
      alert("Error getting balance");
    }

    this.balance = result!.amount;
  }
}
