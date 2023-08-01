import { Utilities } from "../services/utilities";


export class Notification {

    public static Create(data: {}) {
        let n = new Notification();
        Object.assign(n, data);

        if (n.date)
            n.date = Utilities.parseDate(n.date);

        return n;
    }


    public id: number;
    public userId: number;
    public eventId: number;
    public header: string;
    public body: string;
    public isRead: boolean;
    public isPinned: boolean;
    public date: Date;
}

export class NotificationSetting {
  public id: string;
  public type: string;
  public template: any;
  public allowedEmails: string;
  public dayEnabled: string;
  public isEmailEnabled: boolean;
  public isAlertEnabled: boolean;
  public numDaysBeforeCutoff: number;
  public numHoursLeftCutoff: number;
  public subject: string;
}
