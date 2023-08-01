"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.LocationTreeFilter = exports.ImageReference = exports.LocationType = exports.Location = void 0;
var Location = /** @class */ (function () {
    function Location(name, description, locationTypeId, locationTypeName, institutionName, facilities, facilityIds, institutionIds, facilityNames, directoryName, directoryId, capacity, parentLocationId, institutionId, colorTheme, locationGroup) {
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
    return Location;
}());
exports.Location = Location;
var LocationType = /** @class */ (function () {
    function LocationType() {
    }
    return LocationType;
}());
exports.LocationType = LocationType;
var ImageReference = /** @class */ (function () {
    function ImageReference(locationId, filePath, remarks, fileId, fileName, imageReferenceTypeId, referenceDate) {
        this.locationId = locationId;
        this.filePath = filePath;
        this.remarks = remarks;
        this.fileId = fileId;
        this.fileName = fileName;
        this.referenceDate = referenceDate;
        this.imageReferenceTypeId = imageReferenceTypeId;
    }
    return ImageReference;
}());
exports.ImageReference = ImageReference;
var LocationTreeFilter = /** @class */ (function () {
    function LocationTreeFilter() {
    }
    return LocationTreeFilter;
}());
exports.LocationTreeFilter = LocationTreeFilter;
//# sourceMappingURL=location.model.js.map