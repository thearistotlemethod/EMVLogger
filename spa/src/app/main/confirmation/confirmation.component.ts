import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { DataService } from 'src/app/services/data.service';

@Component({
  selector: 'app-confirmation',
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.scss']
})
export class ConfirmationComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<ConfirmationComponent>, public dataService:DataService) { }

  ngOnInit(): void {
  }

  onYesPressed(): void {
    this.dialogRef.close('conf.yes');
  }

  onNoPressed(): void {
    this.dialogRef.close('conf.no');
  }
}
