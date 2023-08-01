
export class CancelOrderRequest {

  constructor(id?: string) {

    this.id = id;
  }

  public id: string;
  public orderId: string;
  public userId: string;
  public userName: string;
  public reason: string;
  public note: string;
  public imgUrl: string;
  public fileName: string;
  public filePath: string;
  public status: string;
  public response: string;
  public cancellationDate: Date;
  public submissionDate: Date;
  public isApproved: boolean;
}

export class CancelOrderRequestApproval {
  public id: string;
  public isApproved: boolean;
  public response: string;
}
