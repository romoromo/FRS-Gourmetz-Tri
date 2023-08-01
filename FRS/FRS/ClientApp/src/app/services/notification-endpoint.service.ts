import { Injectable } from '@angular/core';
import { HttpResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import * as signalR from '@aspnet/signalr';
import { Reservation } from '../models/reservation.model';
import { AccountService } from './account.service';
import { Device } from '../models/device.model';
import { ConfigurationService } from './configuration.service';
import { Notification } from '../models/notification.model';
import { CommonEndpoint } from './common-endpoint.service';
import { Filter, PagedResult } from '../models/sieve-filter.model';



@Injectable()
export class NotificationEndpoint {
  private connection: signalR.HubConnection;
    constructor(private accountService: AccountService, private configurationService: ConfigurationService, private commonEndpoint: CommonEndpoint) {
    //this.connection = new signalR.HubConnectionBuilder()
    //    .withUrl(this.configurationService.baseUrl + "/hub/user?email=" + this.accountService.currentUser.email)
    //  .build();

    //this.connection.on("BroadcastReservationData", (reservation: Reservation) => {
    //  console.log(reservation);
    //  if (!this.notifications || this.notifications.length == 0) {
    //    this.notifications = [];
    //  }


    //  var email = this.accountService.currentUser.email;
    //  var userId = this.accountService.currentUser.id;
    //  if (this.accountService.currentUser.institutionId == reservation.institutionId) {
    //    var invitees = reservation.invitees.filter(function (p, index, array) {
    //      if ((p.userId == userId || p.email == email) && reservation.createdBy != userId) {
    //        return p;
    //      }
    //    });

    //    if (invitees != null && invitees.length > 0) {
    //      var notification = {
    //        header: reservation.shortDescription,
    //        body: reservation.shortDescription,
    //        date: new Date(),
    //        id: reservation.id
    //      };

    //      this.notifications.push(notification);
    //    }
    //  }

    //});

    //this.connection.on("BroadcastDeviceData", (device: Device) => {
    //  console.log(device);
    //});

    //this.connection.on("NotificationTriggered", (notification: Notification) => {
    //    this.notifications.push(notification);

    //});

    //this.connection.start().catch(err => {
    //  console.log(err);
    //});

  }



  private notifications = [
    //{
    //  "id": 1,
    //  "header": "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s by \"administrator\"",
    //  "body": "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s \"administrator\" on 5/28/2017 4:54:13 PM",
    //  "isRead": true,
    //  "isPinned": true,
    //  "date": "2017-05-28T16:29:13.5877958"
    //},
    //{
    //  "id": 2,
    //  "header": "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s",
    //  "body": "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s",
    //  "isRead": false,
    //  "isPinned": false,
    //  "date": "2017-05-28T19:54:42.4144502"
    //},
    //{
    //  "id": 3,
    //  "header": "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s",
    //  "body": "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s",
    //  "isRead": false,
    //  "isPinned": false,
    //  "date": "2017-05-30T11:13:42.4144502"
    //}
  ];



  getNotificationEndpoint<T>(notificationId: number): Observable<T> {

    let notification = this.notifications.find(val => val.id == notificationId);
    let response: HttpResponse<T>;

    if (notification) {
      response = this.createResponse<T>(notification, 200);
    }
    else {
      response = this.createResponse<T>(null, 404);
    }

    return of(response.body);
  }



  getNotificationsEndpoint<T>(page: number, pageSize: number): Observable<T> {

    let notifications = this.notifications;
    let response = this.createResponse<T>(this.notifications, 200);

    return of(response.body);
  }



  getUnreadNotificationsEndpoint<T>(userId?: string): Observable<T> {

    let unreadNotifications = this.notifications.filter(val => !val.isRead);
    let response = this.createResponse<T>(unreadNotifications, 200);

    return of(response.body);
  }

    private readonly _notificationUrl: string = "/api/notification/notifications";
    get notificationUrl() { return this.configurationService.baseUrl + this._notificationUrl; }

  getNewNotificationsEndpoint<T>(filter: Filter): Observable<T> {
      return this.commonEndpoint.getSieve<T>(this.notificationUrl + '/sieve/list', filter);
    //let unreadNotifications = this.notifications;
    //let response = this.createResponse<T>(unreadNotifications, 200);

    //return of(response.body);
  }

  getPinUnpinNotificationEndpoint<T>(notificationId: number, isPinned?: boolean, ): Observable<T> {

    let notification = this.notifications.find(val => val.id == notificationId);
    let response: HttpResponse<T>;

    if (notification) {
      response = this.createResponse<T>(null, 204);

      if (isPinned == null)
        isPinned = !notification.isPinned;

      notification.isPinned = isPinned;
      notification.isRead = true;
    }
    else {
      response = this.createResponse<T>(null, 404);
    }


    return of(response.body);
  }



  getReadUnreadNotificationEndpoint<T>(notificationIds: number[], isRead: boolean, ): Observable<T> {
      this.commonEndpoint.getNewEndpoint<any>(this.notificationUrl + '/read', notificationIds)
          .subscribe(res => { }, error => { console.log(error); });

    for (let notificationId of notificationIds) {

      let notification = this.notifications.find(val => val.id == notificationId);

      if (notification) {
        notification.isRead = isRead;
      }
    }

    let response = this.createResponse<T>(null, 204);
    return of(response.body);
  }



  getDeleteNotificationEndpoint<T>(notificationId: number): Observable<T> {

    let notification = this.notifications.find(val => val.id == notificationId);
    let response: HttpResponse<T>;

    if (notification) {
      this.notifications = this.notifications.filter(val => val.id != notificationId)
      response = this.createResponse<T>(notification, 200);
    }
    else {
      response = this.createResponse<T>(null, 404);
    }

    return of(response.body);
  }



  private createResponse<T>(body, status: number) {
    return new HttpResponse<T>({ body: body, status: status });
  }
}
