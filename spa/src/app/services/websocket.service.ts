import { Injectable } from "@angular/core";
import { WebSocketSubject, webSocket } from 'rxjs/webSocket';
import { Subject } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class WebsocketService {
    private socket$!: WebSocketSubject<any>;
    public messages: Subject<any> = new Subject();

    constructor() {        
        this._connect();
    }

    public _connect() {
        if(this.socket$ != null){
            this.socket$.unsubscribe();
        }

        console.log('connecting....');

        this.socket$ = webSocket({
            url: 'ws://localhost:8089/channel',
            openObserver: {
                next: () => {
                    console.log('connected');
                }
            },
            closeObserver: {
                next: () => {                          
                }
            }
        });

        this.socket$.subscribe(
            message => this.handleMessage(message),
            error => this.handleError(error)
        );
    }

    private handleMessage(data: any) {
        console.log(data);
        this.messages.next(data);
    }

    private handleError(msg: any) {
        console.log(msg);   
        setTimeout(() => {
            this._connect();
        }, 10000);  
    }

    public send(msg: any) {
        this.socket$.next(msg);
    }
}

