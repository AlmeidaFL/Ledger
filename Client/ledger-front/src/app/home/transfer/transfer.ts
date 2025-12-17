import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FinancialService, TransferRequest } from '../../services/financial.service';
import { v4 as uuidv4 } from 'uuid';

@Component({
  selector: 'app-transfer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './transfer.html',
})
export class TransferComponent {

  form: FormGroup;

  constructor(private formBuilder: FormBuilder, private financialService: FinancialService) {
    this.form = this.formBuilder.group({
    toEmail: ['', [Validators.required, Validators.email]],
    amount: ['', [Validators.required]],
    description: ['']
  });
  }

  async submit() {
    if (this.form.invalid) return;

    const rawAmount = this.form.value.amount!;
    const normalized = rawAmount.replace(',', '.');
    const floatValue = parseFloat(normalized);

    if (isNaN(floatValue) || floatValue <= 0) {
      alert('Invalid amount');
      return;
    }

    const cents = Math.round(floatValue * 100);

    const submittedValue = {
      toUserEmail: this.form.value.toEmail,
      amount: cents,
      idempotencyKey: uuidv4(),
      currency: "BRL",
      metadata: this.form.value.description
    } as TransferRequest;

    const transferred = await this.financialService.transfer(submittedValue)
    if (transferred){
      alert(transferred.status)
      return;
    }

    alert("Money wasn't trasnferred")
  }
}
