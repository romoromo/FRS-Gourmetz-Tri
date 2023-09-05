import { UserCardId } from "../usercardid.model";

export class Student {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public gender: string;
  public classId: string;
  public classLevelId: string;
  public classBatchId: string;
  public outletId: string;
  public outletName: string;
  public isFAS: boolean;
  public weight: string;
  public height: string;
  public className: string;
  public classLevel: string;
  public userId: string;
  public username: string;
  public email: string;
  public fullName: string;
  public newPassword: string;
  public currentPassword: string;
  public confirmPassword: string;
  public checked: boolean;
  public cards: UserCardId[];
  public studentCards: StudentCard[];
  public restrictions: StudentRestriction[];
  public interestGroups: StudentInterestGroup[];
  public vouchers: StudentVoucher[];
  
}

export class StudentCard {

  constructor(id?: string, cardId?: string, remarks?: string) {

    this.id = id;
    this.cardId = cardId;
    this.remarks = remarks;
  }

  public id: string;
  public cardId: string;
  public status: string;
  public remarks: string;
  public studentId: string;
  public userName: string;
  public isApprove: boolean;
}

export class StudentRestriction {

  constructor() {
  }

  public studentId: string;
  public restrictionId: string;
  public checked: boolean;
}

export class StudentInterestGroup {

  constructor() {
  }

  public studentId: string;
  public interestGroupId: string;
  public checked: boolean;
}

export class StudentVoucher {

  constructor() {
  }

  public studentId: string;
  public voucherId: string;
  public status: string;
  public voucherCode: string;
}
