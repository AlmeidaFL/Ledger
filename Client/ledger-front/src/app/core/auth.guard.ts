import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthService } from "../auth/auth.service";

export const authGuard: CanActivateFn = async (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (authService.currentUser()){
        return true;
    }

    const success = await authService.loadMe();  
    if (success) {
        return true;
    }

    // if (await authService.refreshToken()){
    //     return true;
    // }

    router.navigate(['auth/login']);
    return false;
}