import { MealCollectionType, PointType, PointsType,ScheduleDay,WalletType } from "../models/enums";

export enum ModulePath {
  PibDisplay = "/pibdisplay",
  NurseCheckinCounter = "/nursecheckincounter",
  PatientCommunicatorDisplay = "/communicatorpatientdisplay",
  Epaper = "/epaper",
  SignageDisplay = "/signagedisplay",
  MeetingRoom = "/meetingroom",
}

enum DeviceRotation {
  LEFT = "Left",
  RIGHT = "RIGHT",
  NORMAL = "Normal",
  INVERTED = "Inverted",
}

export const MealCollectionTypeList = [{
  id: MealCollectionType.BY_CLASS_ROASTER, label: "By Class Roster"
}, {
  id: MealCollectionType.STUDENT_SELECTS, label: "Student selects a collection slot"
}, {
  id: MealCollectionType.BY_CLASS_LEVEL, label: "By Class Level"
}]

export const WalletTypeList = [{
  id: WalletType.BASIC, label: "Basic"
}, {
  id: WalletType.FAS, label: "Fas"
}]

export const PointTypeList = [{
  id: PointType.GCP, label: "Good Conduct Point"
}, {
  id: PointType.HCP, label: "Healty Choices Point"
  }]

export const PointsTypeOptions = [
  { id: PointsType.GCP, label: "Good Conduct Points" },
  { id: PointsType.HCP, label: "Healthy Choices Points" },
];

export const DAYS = [
  { key: 'monday', label: 'Monday', enum: ScheduleDay.Monday },
  { key: 'tuesday', label: 'Tuesday', enum: ScheduleDay.Tuesday },
  { key: 'wednesday', label: 'Wednesday', enum: ScheduleDay.Wednesday },
  { key: 'thursday', label: 'Thursday', enum: ScheduleDay.Thursday },
  { key: 'friday', label: 'Friday', enum: ScheduleDay.Friday },
  { key: 'saturday', label: 'Saturday', enum: ScheduleDay.Saturday },
  { key: 'sunday', label: 'Sunday', enum: ScheduleDay.Sunday },
];