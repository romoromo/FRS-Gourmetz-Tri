import { Permission } from './permission.model';


export class FacilityType {

  constructor(name?: string, description?: string) {

    this.name = name;
    this.description = description;
  }

  public id: string;
  public name: string;
  public description: string;
  public facilitiesCount: string;
  public institutionId: string;
}
