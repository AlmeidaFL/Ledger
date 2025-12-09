import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { v4 as uuidv4 } from 'uuid';
import { FinancialService } from '../../services/financial.service';

@Component({
  selector: 'app-deposit',
  imports: [ReactiveFormsModule],
  templateUrl: './deposit.html',
  styleUrl: './deposit.css',
})
export class Deposit {
  form: FormGroup
  submittedValueInCents: number | null = null;

  constructor(private formBuilder: FormBuilder, private authService: AuthService, private financialService: FinancialService){
    this.form = this.formBuilder.group({
      amount: ['', [Validators.required]]
    })
  }

  async submit() {
    if (this.form.invalid) {
      alert('Form invalid. Please revise');
      return;
    }

    const rawAmount = this.form.value.amount!;
    const normalizedAmount = rawAmount.replace(',', '.');
    const floatAmount = parseFloat(normalizedAmount);

    if (isNaN(floatAmount) || floatAmount <= 0) {
      alert('Invalid amount');
      return;
    }

    const cents = Math.round(floatAmount * 100);
    this.submittedValueInCents = cents;

    const user = this.authService.currentUser();
    if (!user) {
      alert("User not authenticated.");
      return;
    }

    const request = {
      userEmail: user.email,
      amount: cents,
      currency: "BRL",
      idempotencyKey: uuidv4()
    };

    const response = await this.financialService.deposit(request);

    if (response) {
      alert("Deposit successful!");
    } else {
      alert("Deposit failed!");
    }
  }
}
