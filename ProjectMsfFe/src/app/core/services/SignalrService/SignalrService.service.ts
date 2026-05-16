import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  private hubConnection!: signalR.HubConnection;

  // Subjects for components to subscribe to
  public voucherUpdate$ = new Subject<any>();
  public ticketUpdate$ = new Subject<any>();

  constructor() {}

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5083/orderHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR Connected');
        this.registerHandlers();
      })
      .catch((err) => console.error('SignalR Connection Error: ', err));
  }

  joinEvent(eventId: string | number) {
    if (
      this.hubConnection &&
      this.hubConnection.state === signalR.HubConnectionState.Connected
    ) {
      this.hubConnection
        .invoke('JoinEventGroup', eventId.toString())
        .then(() => console.log(`Joined group: event_${eventId}`))
        .catch((err) => console.error('Error joining group:', err));
    } else {
      console.warn('SignalR not connected. Cannot join group.');
    }
  }

  leaveEvent(eventId: string | number) {
    if (
      this.hubConnection &&
      this.hubConnection.state === signalR.HubConnectionState.Connected
    ) {
      this.hubConnection
        .invoke('LeaveEventGroup', eventId.toString())
        .then(() => console.log(`Left group: event_${eventId}`))
        .catch((err) => console.error('Error leaving group:', err));
    }
  }

  private registerHandlers() {
    this.hubConnection.on('ReceiveVoucherUpdate', (data) => {
      this.voucherUpdate$.next(data);
      console.log('Voucher update:', data);
    });

    this.hubConnection.on('Tickquantity', (data) => {
      this.ticketUpdate$.next(data);
      console.log('Ticket quantity update:', data);
    });
  }
}
