
export class FaqSubject {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public description: string;
  public institutionId: string;
}


export class FaqDetail {

  constructor(id?: string, label?: string) {

    this.id = id;
    this.label = label;
  }

  public id: string;
  public label: string;
  public institutionId: string;
  public faqSubjectId: string;
  public faqSubjectName: string;
  public faqSubjectDescription: string;
  public description: string;
}
