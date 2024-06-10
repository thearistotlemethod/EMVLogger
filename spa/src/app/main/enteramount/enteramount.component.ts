import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DataService } from 'src/app/services/data.service';
import { ConfirmationComponent } from '../confirmation/confirmation.component';
import { TransactionComponent } from '../transaction/transaction.component';

@Component({
  selector: 'app-enteramount',
  templateUrl: './enteramount.component.html',
  styleUrls: ['./enteramount.component.scss']
})
export class EnteramountComponent implements OnInit {

  idx = 0;

  constructor(public dialog: MatDialog, public dataService: DataService) { }

  ngOnInit(): void {
  }

  public onKeyPressed(num: any): void {
    if (this.dataService.amount.length > 12) {
      return;
    }

    this.dataService.amount += num;
  }

  public onBackPressed(): void {
    if (this.dataService.amount.length > 0) {
      this.dataService.amount = this.dataService.amount.slice(0, -1);
    }
  }

  public onEnterPressed(): void {
    if (this.dataService.amount.length <= 0) {

    } else {

      const dialogRef = this.dialog.open(ConfirmationComponent, {
        width: 'auto',
        minWidth: '50vh',
        disableClose: true
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result === 'conf.yes') {
          this.dataService.state = 'processing';
          const dialogRef = this.dialog.open(TransactionComponent, {
            width: 'auto',
            minWidth: '50vh',
            disableClose: true
          });

          dialogRef.afterClosed().subscribe((result) => {
            if(result === 'trn.cancel'){
              this.dataService.reset();
            } else {
              this.dataService.reset();
            }
          });
        } else if (result === 'conf.no') {
          this.dataService.reset();
        }
      });
    }
  }
}
