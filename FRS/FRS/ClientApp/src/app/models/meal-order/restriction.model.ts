
export class Restriction {

  constructor(id?: string, label?: string) {

    this.id = id;
    this.label = label;
    this.isHighPriority = false;
    this.isIncludeHighPriorityFiltering = false;
    this.isAvailableDateSpecific = false;
    this.isMealFiltering = false;
    this.sequence = 0;
  }

  public id: string;
  public code: string;
  public label: string;
  public isHighPriority: boolean;
  public isIncludeHighPriorityFiltering: boolean;
  public isAvailableDateSpecific: boolean;
  public isMealFiltering: boolean;
  public sequence: number;
  public restrictionTypeId: string;
}
