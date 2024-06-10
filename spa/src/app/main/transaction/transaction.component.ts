import { Component, Inject, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DataService } from 'src/app/services/data.service';
import { HostService } from 'src/app/services/host.service';
import { WebsocketService } from 'src/app/services/websocket.service';
import { PasswordComponent } from '../password/password.component';

@Component({
  selector: 'app-transaction',
  templateUrl: './transaction.component.html',
  styleUrls: ['./transaction.component.scss']
})
export class TransactionComponent implements OnInit, OnDestroy, AfterViewInit {
  public line1: string = 'Please Wait...';
  public bname: string = '';

  _unsubscribeAll = new Subject();

  constructor(public dialogRef: MatDialogRef<TransactionComponent>,
    public dataService: DataService,
    private translate: TranslateService,
    private hostService: HostService,
    private websocket: WebsocketService,
    public dialog: MatDialog) { }

  ngOnInit(): void {
    this.websocket.messages.pipe(takeUntil(this._unsubscribeAll)).subscribe(data => {
      console.log(data.msg);
      if(data.msg === 'PIN'){
        const dialogRef = this.dialog.open(PasswordComponent, {
          width: 'auto',
          minWidth: '50vh',
          disableClose: true
        });
    
        dialogRef.afterClosed().subscribe((result) => {
          if (result === 'ok') {

          }
        });
      } else {
        this.line1 = data.msg;        
      }
    });
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next(null);
    this._unsubscribeAll.complete();
  }

  ngAfterViewInit(): void {
    switch (this.dataService.state) {
      case 'processing':
        this.hostService.runCommand('processing', {
          type: this.dataService.type,
          amount: this.dataService.amount
        }).pipe(takeUntil(this._unsubscribeAll)).subscribe((ret) => {
          if (ret.code === '0') {
            this.line1 = ret.data;
            this.bname = this.translate.instant('button.ok');
            setTimeout(() => this.onButtonPressed(), 30000);
          } else {
            this.setError(this.getErrorStr(ret));
          }
        }, (err) => {
          this.setError('Unknown Error!!!');
        });
        break;
    }
  }

  setError(err: string): void {
    this.line1 = err;
    this.bname = this.translate.instant('button.ok');

    setTimeout(() => this.onButtonPressed(), 30000);
  }

  onButtonPressed(): void {
    this.dialogRef.close('trn.ok');
  }

  getErrorStr(ret: any): string {
    return this.translate.instant("error." + ret.code);
  }
}
