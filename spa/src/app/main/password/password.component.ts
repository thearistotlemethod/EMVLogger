import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DataService } from 'src/app/services/data.service';
import { HostService } from 'src/app/services/host.service';

@Component({
  selector: 'app-password',
  templateUrl: './password.component.html',
  styleUrls: ['./password.component.scss']
})
export class PasswordComponent implements OnInit, OnDestroy {
  _unsubscribeAll = new Subject();
  
  password = "";
  pwd = "";
  wrongpwd = false;

  constructor(public dialogRef: MatDialogRef<PasswordComponent>, 
    public dataService:DataService,
    private hostService: HostService) { }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next(null);
    this._unsubscribeAll.complete();
  }

  public onKeyPressed(num: any): void {
    if (this.password.length >= 12) {
      return;
    }

    this.password += "*";
    this.pwd += num;
  }

  public onBackPressed(): void {
    if (this.password.length > 0) {
      this.password = this.password.slice(0, -1);
      this.pwd = this.pwd.slice(0, -1);
    }
  }

  public onEnterPressed(): void {
    this.hostService.runCommand('pinentered', {
      pin: this.pwd
    }).pipe(takeUntil(this._unsubscribeAll)).subscribe(
      (ret) => {
        if (ret.code === '0') {
          this.dialogRef.close('ok');
        } else {
          this.password = "";
          this.pwd = "";
          this.wrongpwd = true;
        }
      },
      (err) => {},
      () => {}
    );
  }

  public onCancelPressed(): void {
    this.hostService.runCommand('pinentered', {
      pin: ''
    }).pipe(takeUntil(this._unsubscribeAll)).subscribe(
      (ret) => {
        this.dialogRef.close('nok');
      },
      (err) => {},
      () => {}
    );
  }
}
