import { Injectable } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { Reservation } from '../models/reservation.model';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  public reservations: Reservation[];

  private hubConnection: signalR.HubConnection

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:56767/reservations')
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))
  }

  public refreshBookingGridListener = () => {
  //will be called from the server
    this.hubConnection.on('refreshBookingGrid', (data) => {
      this.reservations = data;
      console.log(data);
    });
  }
}
