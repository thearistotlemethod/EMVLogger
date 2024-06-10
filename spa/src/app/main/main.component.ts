import { Component, OnDestroy, OnInit } from '@angular/core';
import { ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { WebsocketService } from '../services/websocket.service';
import { TranslateService } from '@ngx-translate/core';
import { CdkStepper } from '@angular/cdk/stepper';
import { DataService } from '../services/data.service';
import { HostService } from '../services/host.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PasswordComponent } from './password/password.component';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss'],
})
export class MainComponent implements OnInit, OnDestroy {
  _unsubscribeAll = new Subject();
  @ViewChild('cdkStepper') cdkStepper!: CdkStepper;
  
  lang?: string;
  public version = "Version 1.0.0";

  constructor(
    public dialog: MatDialog,
    private wsservice: WebsocketService,
    private translateService: TranslateService,
    public dataService: DataService,
    public hostService: HostService
  ) {
    this.lang = "en";
    translateService.setDefaultLang('en');
    
    const langcode = localStorage.getItem('langcode');
    if (langcode) {
      this.lang = langcode;
      translateService.use(langcode);
    }
   }

  ngAfterViewInit() {
  }

  ngOnInit(): void {
    this.hostService.runCommand('version', {
    }).pipe(takeUntil(this._unsubscribeAll)).subscribe(
      (ret) => {
        this.version = "Version " + ret.code;
      }
    );
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next(null);
    this._unsubscribeAll.complete();
  }

  onLangChange(value: any) {
    this.translateService.use(value);
    localStorage.setItem('langcode', value);
    //window.location.reload();
  }

  onSavePressed(): void {
    this.hostService.runCommand('savelog', {
    }).pipe(takeUntil(this._unsubscribeAll)).subscribe(
      (ret) => {

      }
    );
  }

  onSettingPressed(): void {
    this.hostService.runCommand('editconfig', {
    }).pipe(takeUntil(this._unsubscribeAll)).subscribe(
      (ret) => {

      }
    );
  }

  onHelpPressed(): void {
    this.hostService.runCommand('help', {
    }).pipe(takeUntil(this._unsubscribeAll)).subscribe(
      (ret) => {

      }
    );
  }
}
