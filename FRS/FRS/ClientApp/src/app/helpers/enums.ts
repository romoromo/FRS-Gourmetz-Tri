import { MealCollectionType, PointType, PointsType,WalletType } from "../models/enums";

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
