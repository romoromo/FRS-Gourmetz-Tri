export enum Gender {
    None,
    Female,
    Male
}

export enum LocType {
  Hospital = "Hospital",
  Building = "Building",
  Level = "Level",
  Room = "Room"
}

export enum UserActivityLogType {
  Order = "Order",
  Roster = "Roster",
  Payment = "Payment"
}

export enum SalesOrderReportType {
  SalesOrderReport = 1,
  CancellationReport = 2,
  CollectionReport = 3
}

export enum PaymentTypes {
  Adhoc = "Adhoc",
  Fas = "FAS",
  MealPlan = "Meal Plan"
}

export enum VoucherUsageStatus {
  Expired = "Expired",
  NotUsed = "Not Used",
  Used = "Used"
  
}

export enum MealCollectionType {
  BY_CLASS_ROASTER = 1,
  STUDENT_SELECTS = 2,
  BY_CLASS_LEVEL = 3
}

export enum PointsType{
  GCP = "GCP",
  HCP = "HCP"
}
export enum WalletType {
  BASIC = "BASIC",
  FAS = "FAS"
}

export enum PointType {
  GCP = "GCP",
  HCP = "HCP"
}

export enum ScheduleDay {
  Monday = 1,
  Tuesday,
  Wednesday,
  Thursday,
  Friday,
  Saturday,
  Sunday
}
