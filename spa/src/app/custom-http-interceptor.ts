import { Injectable, NgZone, Injector } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
} from '@angular/common/http';
import { Overlay } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { MatSpinner } from '@angular/material/progress-spinner';
import { Observable } from 'rxjs';
import { finalize, catchError, tap, map } from 'rxjs/operators';

@Injectable()
export class CustomHttpInterceptor implements HttpInterceptor {
  private spinnerTopRef: any = null;

  constructor(private overlay: Overlay) {}
  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (request.url.indexOf('assets') < 0) {
      //this.startSpinner();
    }

    return next.handle(request).pipe(
      map((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          if (request.url.indexOf('assets') < 0) {
            //this.stopSpinner();
          }
        }
        return event;
      }),
      catchError((error, caught) => {
        this.stopSpinner();
        throw error;
      }) as any
    );
  }

  private startSpinner(): void {
    if (!this.spinnerTopRef) {
      this.spinnerTopRef = this.overlay.create({
        hasBackdrop: true,
        positionStrategy: this.overlay
          .position()
          .global()
          .centerHorizontally()
          .centerVertically(),
      });
    }

    if (!this.spinnerTopRef.hasAttached()) {
      this.spinnerTopRef.attach(new ComponentPortal(MatSpinner));
    }
  }

  private stopSpinner(): void {
    if (!this.spinnerTopRef) {
      this.spinnerTopRef = this.overlay.create({
        hasBackdrop: true,
        positionStrategy: this.overlay
          .position()
          .global()
          .centerHorizontally()
          .centerVertically(),
      });
    }

    this.spinnerTopRef.detach();
  }
}
