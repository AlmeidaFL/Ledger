import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-deposit',
  imports: [ReactiveFormsModule],
  templateUrl: './deposit.html',
  styleUrl: './deposit.css',
})
export class Deposit {
  form: FormGroup
  submittedValueInCents: number | null = null;

  constructor(private formBuilder: FormBuilder){
    this.form = this.formBuilder.group({
      amount: ['', [Validators.required]]
    })
  }

  submit() {
    if (this.form.invalid){
      alert('Form invalid. Please revise')
      return;
    }

    const rawAmount = this.form.value.amount!
    const normalizedAmount = rawAmount.replace(',', '.')

    const floatAmount = parseFloat(normalizedAmount)

    if (isNaN(floatAmount) || floatAmount <= 0){
      alert('Invalid amount')
      return;
    }

    const cents = Math.round(floatAmount * 100)
    this.submittedValueInCents = cents;

    console.log("Deposit in cents: ", cents)
  }
}
