
export class AuthLog {
  public userName: string;
  public message: string;
  public institutionId: string;
}

export class ExternalLoginLog {
  public userName: string;
  public email: string;
  public message: string;
  public eventDateTime: Date;
}
