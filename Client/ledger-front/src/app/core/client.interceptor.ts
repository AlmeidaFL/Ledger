import { HttpInterceptorFn } from "@angular/common/http";
import { catchError } from "rxjs";

export const clientInterceptor: HttpInterceptorFn = (req, next) => {
    const modified = req.clone({
        setHeaders: {
            'X-Client-Type': 'spa'
        }
    });

    return next(modified);
};