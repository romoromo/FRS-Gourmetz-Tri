import { Permission } from './permission.model';
import { Facility } from './facility.model';
import { eachDay } from 'date-fns';
import { FacilityType } from './facility-type.model';
import { Asset, LocationAsset } from './asset.model';

export class Location {

  constructor(name?: string, description?: string, locationTypeId?: string, locationTypeName?: string, institutionName?: string,
    facilities?: Facility[], facilityIds?: number[], institutionIds?: number[], facilityNames?: string, directoryName?: string, directoryId?: string,
    capacity?: number,parentLocationId?: number, institutionId?: string, colorTheme?: string, locationGroup?: string) {

    this.name = name;
    this.description = description;
    this.locationTypeId = locationTypeId;
    this.locationTypeName = locationTypeName;
    this.institutionName = institutionName;
    this.facilityNames = facilityNames;
    this.directoryName = directoryName;
    this.directoryId = directoryId;
    this.facilities = facilities;
    this.facilityIds = facilityIds;
    this.institutionIds = institutionIds;
    this.capacity = capacity;
    this.parentLocationId = parentLocationId;
    this.institutionId = institutionId;
    this.colorTheme = colorTheme;
    this.locationGroup = locationGroup;
  }

  public id: string;
  public name: string;
  public description: string;
  public locationTypeId: string;
  public locationTypeName: string;
  public facilityNames: string;
  public institutionName: string;
  public directoryName: string;
  public directoryId: string;
  public facilities: Facility[];
  public facilityIds?: number[];
  public facilityTypes: Facility[];
  public facilityTypeIds?: number[];
  public institutionIds?: number[];
  public capacity: number;
  public parentLocationId: number;
  public parentLocation: Location;
  public filePath: string;
  public fileName: string;
  public availableFrom: string;
  public availableTo: string;
  public availableTimeDisplay: string;
  public institutionId: string;
  public colorTheme: string;
  public locationGroup: string;
  public isBooking: boolean;
  public facePlateNumber: string;
  public imageReferences: ImageReference[];
  public locationAssets: LocationAsset[];

  public FloorId: number;
  public ExchangeId: string;
  public Extension: string;
  public IP: string;
  public deviceName: string;
  public category: string;
  public remarks: string;
  public floorX: number;
  public floorY: number;
  public seatingCapacity: number;
}

export class LocationType {
  public id: string;
  public name: string;
  public description: string;
}

export class ImageReference {
  constructor(locationId?: string, filePath?: string, remarks?: string, fileId?: number, fileName?: string, imageReferenceTypeId?: string, referenceDate?: Date) {
    this.locationId = locationId;
    this.filePath = filePath;
    this.remarks = remarks;
    this.fileId = fileId;
    this.fileName = fileName;
    this.referenceDate = referenceDate;
    this.imageReferenceTypeId = imageReferenceTypeId;
  }

  public fileId: number;
  public id: string;
  public locationId: string;
  public filePath: string;
  public fileName: string;
  public remarks: string;
  public referenceDate: Date;
  public imageReferenceTypeId: string;
  public imageReferenceColorId: string;
  public imageReferenceColorCode: string;
  public hidden: boolean;
}


export class LocationTreeFilter {
  constructor(
  ) {

  }
  public currentUserInstitutionId?: string;
  public institutionId?: string;
  public imageReferenceColorId?: string;
  public keyword?: string;
  public isBookingOnly: boolean;
}
