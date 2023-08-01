"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.PIBDevice = exports.PIBTemplateLocation = exports.PIBTemplate = exports.Department = void 0;
var Department = /** @class */ (function () {
    function Department(id, name, description) {
        this.id = id;
        this.name = name;
        this.description = description;
    }
    return Department;
}());
exports.Department = Department;
var PIBTemplate = /** @class */ (function () {
    function PIBTemplate(template_body, name, imgUri, device_api_url, is_post_to_device, id, description) {
        this.locationIds = [];
        this.locations = [];
        this.templateBody = template_body;
        this.name = name;
        this.imgUrl = imgUri;
        this.deviceAPIUrl = device_api_url;
        this.isPostToDevice = is_post_to_device;
        this.id = id;
        this.description = description;
    }
    return PIBTemplate;
}());
exports.PIBTemplate = PIBTemplate;
var PIBTemplateLocation = /** @class */ (function () {
    function PIBTemplateLocation() {
    }
    return PIBTemplateLocation;
}());
exports.PIBTemplateLocation = PIBTemplateLocation;
var PIBDevice = /** @class */ (function () {
    function PIBDevice() {
    }
    return PIBDevice;
}());
exports.PIBDevice = PIBDevice;
//# sourceMappingURL=department.model.js.map