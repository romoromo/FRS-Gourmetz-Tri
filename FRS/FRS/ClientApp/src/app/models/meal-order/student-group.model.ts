
export class StudentGroup {

  constructor(id?: string) {

    this.id = id;
  }
  public id: string;
  public code: string;
  public name: string;
  public outletId: string;
  public type: string;
  public startDate?: Date;
  public endDate?: Date;
  public isPublished: boolean;
  public price: number;
  public mealSessionId: string;
  public deliveryStartDate?: Date;
  public deliveryEndDate?: Date;
  public term: string;
  public fileName: string;
  public filePath: string;
  public sgdetails: StudentGroupDetail[];
}

export class StudentGroupDetail {
  public id: string;
  public studentGroupId: string;
  public studentId: string;
}

export enum StudentGroupType {
  MEAL_PLAN = 'Meal Plan',
  OTHERS = 'Others'
}
