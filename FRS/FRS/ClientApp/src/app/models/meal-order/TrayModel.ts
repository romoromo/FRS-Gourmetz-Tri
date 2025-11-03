export class TrayModel {
  constructor(id?: string) {
    this.id = id;
  }

  public id: string;
  public trayId: string;
  public plcId: string;
  public dispenserId: string;
  public ipAddress: string;
  public framework: string;

  public motorOutputNumber: number;
  public ledOutputNumber: number;
}
