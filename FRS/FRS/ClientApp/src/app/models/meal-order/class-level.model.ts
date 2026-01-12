
export class ClassLevel {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public year: string;
  public institutionId: string;
  public outletId: string;
  public outletName: string;
  public mealSessionId: string;
  public mealSessionDetailId: string;
  public detail: ClassLevelDetail[];

  public fasRechargeable: boolean = false;
  public day: string | null = null;
  public time: Date | null = null;
  public amount: number | null = null;
}

export class ClassLevelDetail{
  constructor() {    
  }
  public id: string;
  public classLevelId: string;
  public sessionId: string;
  public sessionName: string;
  public periodId: string;
  public periodName: string;
}