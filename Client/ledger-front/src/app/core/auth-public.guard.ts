import { CanActivateFn, Router } from "@angular/router";
import { AuthService } from "../auth/auth.service";
import { inject } from "@angular/core";

export const authPublicGuard: CanActivateFn = () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (authService.currentUser()) {
        router.navigate(['/home']);
        return false;
    }
    
    return true;
};