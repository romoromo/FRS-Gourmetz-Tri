
export class Class {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public classLevelId: string;
  public classLevelName: string;
  public detail: ClassDetail[]
}

export class ClassDetail{
  constructor() {    
  }
  public id: string;
  public classLevelId: string;
  public sessionId: string;
  public sessionName: string;
  public periodId: string;
  public periodName: string;
}