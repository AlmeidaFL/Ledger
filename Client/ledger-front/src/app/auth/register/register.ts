import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class RegisterComponent {
  form: FormGroup;

  constructor(private formBuilder: FormBuilder, private authService: AuthService) {
    this.form = this.formBuilder.group({
      fullName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(3)]],
      confirmPassword: ['', [Validators.required]]
    })
  }

  async submit() {
    if (this.form.invalid){
      alert("Form is invalid")
      return;
    }

    const password = this.form.value.password;
    const confirmPassword= this.form.value.confirmPassword;

    if (password !== confirmPassword){
      alert("Password is different")
    }

    await this.authService.register({
      email: this.form.value.email,
      fullName: this.form.value.fullName,
      password: password
    })
  }
}
