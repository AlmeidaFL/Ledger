import { inject, } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    return next(req).pipe(
        catchError((error: any) => {
            if (error.status === 401 || error.status === 403) {
                inject(Router).navigate(['auth/login']);
            }
            return throwError(() => error);
        })
    );
};