
export class RestrictionType {

  constructor(id?: string, code?: string) {

    this.id = id;
    this.code = code;
    this.isHandledEMR = false;
    this.isMealFiltering = false;
    this.isDishTypeCustomisation = false;
    this.isSpecialCondition = false;
    this.isCarriedForward = false;
    this.isMealFiltering = false;
    this.sequence = 0;
    this.dietCondition = 'Optional';
  }

  public id: string;
  public code: string;
  public label: string;
  public isHandledEMR: boolean;
  public isDishTypeCustomisation: boolean;
  public isSpecialCondition: boolean;
  public isCarriedForward: boolean;
  public isMealFiltering: boolean;
  public sequence: number;
  public dietCondition: string;
  public institutionId: string;
}
