import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HostService {

  serviceUrl = 'http://localhost:8089/api/';
  public getOptions(): any {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
    };
  }

  constructor(private httpClient: HttpClient) { }

  runCommand(cmd: string, req: any): Observable<any> {
    console.log(cmd);
    return this.httpClient
      .post(this.serviceUrl + cmd, req, this.getOptions());
  }
}
