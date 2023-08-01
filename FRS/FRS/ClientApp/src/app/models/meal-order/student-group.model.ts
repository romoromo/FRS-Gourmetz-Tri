
export class StudentGroup {

  constructor(id?: string) {

    this.id = id;
  }
  public id: string;
  public code: string;
  public name: string;
  public outletId: string;
  public sgdetails: StudentGroupDetail[];
}

export class StudentGroupDetail {
  public id: string;
  public studentGroupId: string;
  public studentId: string;
}
