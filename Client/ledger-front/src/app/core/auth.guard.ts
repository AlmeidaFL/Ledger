import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthService } from "../services/auth.service";

export const authGuard: CanActivateFn = async (route, state) => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (auth.currentUser()) {
        return true;
    }

    if (!auth.sessionLoaded()) {
        await auth.bootstrapSession(); 
    }

    if (auth.currentUser()) {
        return true;
    }

    router.navigate(['/auth/login']);
    return false;
}