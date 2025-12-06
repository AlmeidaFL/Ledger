import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-transfer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './transfer.html',
})
export class TransferComponent {

  form: FormGroup;

  constructor(private formBuilder: FormBuilder) {
    this.form = this.formBuilder.group({
    fromEmail: [{ value: this.userEmail, disabled: true }],
    toEmail: ['', [Validators.required, Validators.email]],
    amount: ['', [Validators.required]],
    description: ['']
  });
  }

  userEmail = "user@example.com";

  submittedValue: any = null;

  submit() {
    if (this.form.invalid) return;

    const rawAmount = this.form.value.amount!;
    const normalized = rawAmount.replace(',', '.');
    const floatValue = parseFloat(normalized);

    if (isNaN(floatValue) || floatValue <= 0) {
      alert('Invalid amount');
      return;
    }

    const cents = Math.round(floatValue * 100);

    this.submittedValue = {
      from: this.userEmail,
      to: this.form.value.toEmail,
      amountInCents: cents,
      description: this.form.value.description || null,
    };

    console.log("Prepared transfer:", this.submittedValue);
  }
}
