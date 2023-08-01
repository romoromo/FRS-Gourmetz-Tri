
export class Institution {

  constructor(id?: string, name?: string, description?: string) {

    this.id = id;
    this.name = name;
    this.description = description;
  }

  public id: string;
  public name: string;
  public description: string;
  public startTime: number;
  public endTime: number;
  public isRestrictDuplicateBooking: boolean;
  public restriction: string;
}
