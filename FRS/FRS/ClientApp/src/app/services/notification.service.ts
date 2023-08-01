import { Injectable } from '@angular/core';
import { Observable, interval } from 'rxjs';
import { map, flatMap, startWith } from 'rxjs/operators';

import { AuthService } from './auth.service';
import { NotificationEndpoint } from './notification-endpoint.service';
import { Notification, NotificationSetting } from '../models/notification.model';
import { Filter, PagedResult } from '../models/sieve-filter.model';
import { ConfigurationService } from './configuration.service';
import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { NotificationEvent } from '../models/meal-order/notification-event.model';



@Injectable()
export class NotificationService {

  private readonly _notificationEventUrl: string = "/api/notification/notificationevents";
  get notificationEventUrl() { return this.configurations.baseUrl + this._notificationEventUrl; }

  private readonly _notificationUrl: string = "/api/notification/notifications";
  get notificationUrl() { return this.configurations.baseUrl + this._notificationUrl; }

  private readonly _notificationSettingUrl: string = "/api/notification/notificationsettings";
  get notificationSettingUrl() { return this.configurations.baseUrl + this._notificationSettingUrl; }

  private lastNotificationDate: Date;
  private _recentNotifications: Notification[];

  get recentNotifications() {
    return this._recentNotifications;
  }

  set recentNotifications(notifications: Notification[]) {
    this._recentNotifications = notifications;
  }



  constructor(private notificationEndpoint: NotificationEndpoint, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService  ) {

  }


  getNotification(notificationId?: number) {

    return this.notificationEndpoint.getNotificationEndpoint(notificationId).pipe(
      map(response => Notification.Create(response)));
  }


  getNotifications(page: number, pageSize: number) {

    return this.notificationEndpoint.getNotificationsEndpoint(page, pageSize).pipe(
      map(response => {
        return this.getNotificationsFromResponse(response);
      }));
  }


  getUnreadNotifications(userId?: string) {

    return this.notificationEndpoint.getUnreadNotificationsEndpoint(userId).pipe(
      map(response => this.getNotificationsFromResponse(response)));
  }


  getNewNotifications(filter: Filter) {
      return this.notificationEndpoint.getNewNotificationsEndpoint(filter).pipe(
      map(response => this.processNewNotificationsFromResponse(response)));
  }


    getNewNotificationsPeriodically(filter: Filter) {
    return interval(10000).pipe(
      startWith(0),
      flatMap(() => {
          return this.notificationEndpoint.getNewNotificationsEndpoint(filter).pipe(
          map(response => this.processNewNotificationsFromResponse(response)));
      }));
  }




  pinUnpinNotification(notificationOrNotificationId: number | Notification, isPinned?: boolean): Observable<any> {

    if (typeof notificationOrNotificationId === 'number' || notificationOrNotificationId instanceof Number) {
      return this.notificationEndpoint.getPinUnpinNotificationEndpoint(<number>notificationOrNotificationId, isPinned);
    }
    else {
      return this.pinUnpinNotification(notificationOrNotificationId.id);
    }
  }


  readUnreadNotification(notificationIds: number[], isRead: boolean): Observable<any> {

    return this.notificationEndpoint.getReadUnreadNotificationEndpoint(notificationIds, isRead);
  }




  deleteNotification(notificationOrNotificationId: number | Notification): Observable<Notification> {

    if (typeof notificationOrNotificationId === 'number' || notificationOrNotificationId instanceof Number) { //Todo: Test me if its check is valid
      return this.notificationEndpoint.getDeleteNotificationEndpoint(<number>notificationOrNotificationId).pipe(
        map(response => {
          this.recentNotifications = this.recentNotifications.filter(n => n.id != notificationOrNotificationId);
          return Notification.Create(response);
        }));
    }
    else {
      return this.deleteNotification(notificationOrNotificationId.id);
    }
  }




    private processNewNotificationsFromResponse(response) {
    let notifications = this.getNotificationsFromResponse(response.pagedData);

    for (let n of notifications) {
      if (n.date > this.lastNotificationDate)
        this.lastNotificationDate = n.date;
    }

    return notifications;
  }


  private getNotificationsFromResponse(response) {
    let notifications: Notification[] = [];

    for (let i in response) {
      notifications[i] = Notification.Create(response[i]);
    }

    notifications.sort((a, b) => b.date.valueOf() - a.date.valueOf());
    notifications.sort((a, b) => (a.isPinned === b.isPinned) ? 0 : a.isPinned ? -1 : 1);

    this.recentNotifications = notifications;

    return notifications;
  }



  get currentUser() {
    return this.authService.currentUser;
  }


  //notification event
  getNotificationEventById(notificationEventId: string) {

    return this.commonEndpoint.getById<any>(this.notificationEventUrl + '/get', notificationEventId);
  }

  getNotificationEventsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.notificationEventUrl + '/sieve/list', filter);
  }

  updateNotificationEvent(notificationEvent: NotificationEvent) {
    if (notificationEvent.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.notificationEventUrl, notificationEvent, notificationEvent.id);
    }
  }


  newNotificationEvent(notificationEvent: NotificationEvent) {
    return this.commonEndpoint.getNewEndpoint<NotificationEvent>(this.notificationEventUrl, notificationEvent);
  }


  deleteNotificationEvent(notificationEventOrNotificationEventId: string | NotificationEvent): Observable<NotificationEvent> {
    return this.commonEndpoint.getDeleteEndpoint<NotificationEvent>(this.notificationEventUrl, <string>notificationEventOrNotificationEventId);
  }

  sendStudentNotification(templateId: string, email: string, userId: string) {
    return this.commonEndpoint.get<any>(`${this.notificationUrl}/student/trigger?notificationEventId=${templateId}&email=${email}&userId=${userId}`);
  }

  getNotificationSettingById(id?: string) {
    return this.commonEndpoint.getById<any>(this.notificationSettingUrl + '/get', id);
  }

  updateNotificationSetting(notificationSetting: NotificationSetting) {
    if (notificationSetting.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.notificationSettingUrl, notificationSetting, notificationSetting.id);
    }
  }

  bulkUpdateNotificationSetting(notificationSettings: NotificationSetting[]) {
    return this.commonEndpoint.getNewEndpoint<any>(this.notificationSettingUrl + '/update/bulk', notificationSettings);
  }

  getNotificationSettingsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.notificationSettingUrl + '/sieve/list', filter);
  }
}
