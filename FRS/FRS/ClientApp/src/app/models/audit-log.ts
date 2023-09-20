
export class AuditLog {
  public id: string;
  public userName: string;
  public eventDateTime: string;
  public logType: string;
  public tableName: string;
  public recordId: string;
}

export class UserActivityLog {
  
  public groupId: string;
  public userName: string;
  public actionName: string;
  public auditLogId: string;
  public eventDateTime: string;
  public recordId: string;
  public logType: string;
  public remarks: string;
  public propertyName: string;
  public oldVal: string;
  public newVal: string;
  public detailRemarks: string;
}
