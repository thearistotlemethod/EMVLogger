import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  public pumpNo: string = '';
  public type: string = '';
  public amount: string = '0';
  public state: string = 'processing';  
  public mode: string = 'amount';
  public cardNo: string = '';
  public asisCode: string = '';
  public tdata: any;

  constructor() { }

  reset(): void {
    this.pumpNo = '';
    this.type = '';
    this.amount = '0';
    this.mode = 'amount';
  }

  public getAmountDispStr(): String {
    if (this.amount.length <= 0) {
      this.amount = '0';
    }
    return (parseInt(this.amount) / 100).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') + ' TL';
  }
}
