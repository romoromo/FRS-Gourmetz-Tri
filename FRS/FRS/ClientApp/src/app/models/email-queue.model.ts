
export class EmailQueue {

  constructor() {
  }

  public id: string;
  public action: string;
  public fromName: string;
  public fromEmail: string;
  public toName: string;
  public toEmail: string;
  public subject: string;
  public failMessage: string;
  public body: string;
  public isSent: boolean;
  public isFailed: boolean;
  public sentDate: Date;
  public createdDate: Date;
}
