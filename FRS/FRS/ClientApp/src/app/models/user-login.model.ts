export class UserLogin {
    constructor(email?: string, password?: string, rememberMe?: boolean) {
        this.email = email;
        this.password = password;
        this.rememberMe = rememberMe;
    }

    email: string;
    password: string;
    institutionCode: string;
    rememberMe: boolean;
    isAD: boolean;
}

export class ResetPassword {
  constructor(code?: string, userId?: string, password?: string) {
    this.userId = userId;
    this.password = password;
    this.code = code;
  }

  userId: string;
  password: string;
  institutionCode: string;
  code: string;
}
export class UserLogin2FA {
  constructor(userId?: string, code?: string) {
    this.userId = userId;
    this.code = code;
  }

  userId: string;
  code: string;
}
