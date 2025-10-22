export class TryModel {
  constructor(id?: string) {
    this.id = id;
  }

  public id: string;
  public plcId: string;
  public ipAddress: string;
  public framework: string;

  public motorOutputNumber: number;
  public ledOutputNumber: number;
}
