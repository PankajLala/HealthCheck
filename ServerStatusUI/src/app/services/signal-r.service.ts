import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';

import { ServerStatus } from '../interface/ServerStatus';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  public data: ServerStatus[];

  constructor() {
    this.responseReceivedEventSubject = new BehaviorSubject<ServerStatus[]>(null);
    this._responseReceivedEvents$ = this.responseReceivedEventSubject.asObservable();
  }

  private hubConnection: signalR.HubConnection
  public _responseReceivedEvents$: Observable<ServerStatus[]>;
  private responseReceivedEventSubject: BehaviorSubject<ServerStatus[]>;

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder().withUrl(`${environment.apiBaseUrl}serverstatushub`).build();

    this.hubConnection
    .start()
    .then(() => console.log('Connected'))
    .catch(error => console.log('Failed to connect: ' + error));
  }

  public serverStatusDataListner = () => {
    this.hubConnection.on('serverstatusdata', (data) => {
      this.data = data;
      this.handleResponseReceivedEvent(data);
    })
  }

  private handleResponseReceivedEvent(data: ServerStatus[]): void {
    this.responseReceivedEventSubject.next(data);

  }

}
