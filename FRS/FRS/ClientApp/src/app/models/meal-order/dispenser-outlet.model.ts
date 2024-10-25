
export class DispenserOutlet {

  constructor(id?: string, dispenserCode?: string) {

    this.id = id;
    this.dispenserCode = dispenserCode;
  }

  public id: string;
  public dispenserCode: string;
  public countername: string;
  public password: string;
  public locationCode: string;
  public color: string;
  public plcipAddress: string;
  public plcPort: string;
  public plcToken: string;
  public plcApiVer: string;


  public institutionId: string;
  public outletId: string;
  public outletName: string;
}
