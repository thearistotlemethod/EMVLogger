import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTabsModule } from '@angular/material/tabs';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatSelectModule } from '@angular/material/select';
import {MatStepperModule} from '@angular/material/stepper';
import {
  MatProgressSpinnerModule,
  MatSpinner,
} from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import {
  ReactiveFormsModule,
  FormsModule,
} from '@angular/forms';
import { FlexLayoutModule, FlexModule } from '@angular/flex-layout';
import { CustomHttpInterceptor } from './custom-http-interceptor';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { MainComponent } from './main/main.component';
import { MatChipsModule } from '@angular/material/chips';
import { CommonModule } from '@angular/common';
import {
  MatDialogModule,
} from '@angular/material/dialog';
import { LayoutModule } from '@angular/cdk/layout';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { IgnoreWheelDirective } from "./ignore-wheel.directive";
import { CdkStepperModule } from '@angular/cdk/stepper';
import { EnteramountComponent } from './main/enteramount/enteramount.component';
import { ConfirmationComponent } from './main/confirmation/confirmation.component';
import { TransactionComponent } from './main/transaction/transaction.component';
import { PasswordComponent } from './main/password/password.component';

// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient): any {
  return new TranslateHttpLoader(http,
    "./assets/i18n/", ".json",
  );
}

@NgModule({
    declarations: [
        AppComponent,
        MainComponent,
        IgnoreWheelDirective,
        EnteramountComponent,
        ConfirmationComponent,
        TransactionComponent,
        PasswordComponent
    ],
    imports: [
        BrowserModule,
        FormsModule,
        CommonModule,
        CdkStepperModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        HttpClientModule,
        MatButtonModule,
        MatCardModule,
        MatIconModule,
        MatInputModule,
        MatProgressSpinnerModule,
        MatTabsModule,
        MatGridListModule,
        MatSelectModule,
        MatMenuModule,
        MatToolbarModule,
        MatExpansionModule,
        MatChipsModule,
        MatTableModule,
        ReactiveFormsModule,
        FlexModule,
        MatDialogModule,
        NgxDatatableModule,
        MatButtonToggleModule,
        MatTooltipModule,
        MatDividerModule,
        MatCheckboxModule,
        MatStepperModule,
        LayoutModule,
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: HttpLoaderFactory,
                deps: [HttpClient]
            }
        })
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: CustomHttpInterceptor,
            multi: true,
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
