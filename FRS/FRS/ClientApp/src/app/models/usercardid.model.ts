
export class UserCardId {

  constructor(id?: string, cardId?: string, remarks?: string) {

    this.id = id;
    this.cardId = cardId;
    this.remarks = remarks;
  }

  public id: string;
  public cardId: string;
  public status: string;
  public remarks: string;
  public userId: string;
  public userName: string;
  public isApprove: boolean;
}


