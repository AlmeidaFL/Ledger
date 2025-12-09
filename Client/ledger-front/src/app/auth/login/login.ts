import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-login',
    imports: [ReactiveFormsModule, RouterLink],
    templateUrl: './login.html',
    styleUrl: './login.css',
})
export class LoginComponent {
    form: FormGroup;

    constructor(private formBuilder: FormBuilder, private authService: AuthService) {
        this.form = this.formBuilder.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required]],
        });
    }

    async submit() {
        if (this.form.invalid) {
            return;
        }

        await this.authService.login({
          email: this.form.value.email,
          password: this.form.value.password
        })
    }
}
