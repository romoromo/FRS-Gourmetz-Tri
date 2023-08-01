
export class EmailTemplate {

  constructor(id?: string, subject?: string) {

    this.id = id;
    this.subject = subject;
  }

  public id: string;
  public subject: string;
  public body: string;
  public fromName: string;
  public fromEmail: string;
  public institutionId: string;
  public outletId: string;
}
