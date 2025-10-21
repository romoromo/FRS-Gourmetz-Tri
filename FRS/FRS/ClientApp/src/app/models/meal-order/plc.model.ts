export class PlcModel {
  constructor(id?: string, ipAddress?: string, framework?: string, totalNumber?: number) {
    this.id = id;
    this.ipAddress = ipAddress;
    this.framework = framework;
    this.totalNumber = totalNumber;
  }

  public id: string;
  public ipAddress: string;
  public framework: string;
  public totalNumber: number;
}
