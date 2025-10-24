export class PlcModel {
  constructor(id?: string, ipAddress?: string) {
    this.id = id;
    this.ipAddress = ipAddress;
  }

  public id: string;
  public ipAddress: string;
  public framework: string;
  public totalNumber: number;
  public institutionId: string;
  public outletId: string;
}
