
export class UserVehicle {

  constructor(id?: string, cardType?: string, plateNumber?: string) {

    this.id = id;
    this.cardType = cardType;
    this.plateNumber = plateNumber;
  }

  public id: string;
  public cardType: string;
  public plateNumber: string;
  public vehicleStatus: string;
  public userId: string;
  public userName: string;
  public isApprove: boolean;
}


export class VMSVehicleLog {

  constructor() {
  }

  public bookingDescription: string;
  public seasonId: string;
  public personName: string;
  public cardType: string;
  public vehicle: string;
  public issueDate?: Date;
  public expiryDate?: Date;
  public entryDate?: Date;
  public exitDate?: Date;

}

