"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var signage_component_type_model_1 = require("../models/signage-component-type.model");
var free_html_component_1 = require("../components/signage/signage-component-types/free-html/free-html.component");
var free_html_configuration_component_1 = require("../components/signage/signage-component-types/free-html/free-html-configuration.component");
var clock_component_1 = require("../components/signage/signage-component-types/clock/clock.component");
var clock_configuration_component_1 = require("../components/signage/signage-component-types/clock/clock-configuration.component");
var video_component_1 = require("../components/signage/signage-component-types/video/video.component");
var video_configuration_component_1 = require("../components/signage/signage-component-types/video/video-configuration.component");
var playlist_component_1 = require("../components/signage/signage-component-types/playlist/playlist.component");
var playlist_configuration_component_1 = require("../components/signage/signage-component-types/playlist/playlist-configuration.component");
var emergency_message_component_1 = require("../components/signage/signage-component-types/emergency-message/emergency-message.component");
var emergency_message_configuration_component_1 = require("../components/signage/signage-component-types/emergency-message/emergency-message-configuration.component");
var single_queue_component_1 = require("../components/signage/signage-component-types/single-queue/single-queue.component");
var single_queue_configuration_component_1 = require("../components/signage/signage-component-types/single-queue/single-queue-configuration.component");
var multi_queue_configuration_component_1 = require("../components/signage/signage-component-types/multi-queue/multi-queue-configuration.component");
var multi_queue_component_1 = require("../components/signage/signage-component-types/multi-queue/multi-queue.component");
var employee_name_component_1 = require("../components/signage/signage-component-types/employee-name/employee-name.component");
var employee_name_configuration_component_1 = require("../components/signage/signage-component-types/employee-name/employee-name-configuration.component");
var location_name_component_1 = require("../components/signage/signage-component-types/location-name/location-name.component");
var location_name_configuration_component_1 = require("../components/signage/signage-component-types/location-name/location-name-configuration.component");
var missed_queue_component_1 = require("../components/signage/signage-component-types/missed-queue/missed-queue.component");
var missed_queue_configuration_component_1 = require("../components/signage/signage-component-types/missed-queue/missed-queue-configuration.component");
var single_booking_component_1 = require("../components/signage/signage-component-types/single-booking/single-booking.component");
var single_booking_configuration_component_1 = require("../components/signage/signage-component-types/single-booking/single-booking-configuration.component");
var multi_booking_component_1 = require("../components/signage/signage-component-types/multi-booking/multi-booking.component");
var multi_booking_configuration_component_1 = require("../components/signage/signage-component-types/multi-booking/multi-booking-configuration.component");
var free_text_component_1 = require("../components/signage/signage-component-types/free-text/free-text.component");
var free_text_configuration_component_1 = require("../components/signage/signage-component-types/free-text/free-text-configuration.component");
var web_frame_component_1 = require("../components/signage/signage-component-types/web-frame/web-frame.component");
var web_frame_configuration_component_1 = require("../components/signage/signage-component-types/web-frame/web-frame-configuration.component");
var SignageComponentTypeService = /** @class */ (function () {
    function SignageComponentTypeService() {
        this.types = {
            "free_text": new signage_component_type_model_1.SignageComponentType("free_text", free_text_component_1.FreeText, "Free Text", free_text_configuration_component_1.FreeTextConfiguration),
            "free_html": new signage_component_type_model_1.SignageComponentType("free_html", free_html_component_1.FreeHTML, "Free HTML", free_html_configuration_component_1.FreeHTMLConfiguration),
            "clock": new signage_component_type_model_1.SignageComponentType("clock", clock_component_1.Clock, "Date Time", clock_configuration_component_1.ClockConfiguration),
            "video": new signage_component_type_model_1.SignageComponentType("video", video_component_1.Video, "Video", video_configuration_component_1.VideoConfiguration),
            "playlist": new signage_component_type_model_1.SignageComponentType("playlist", playlist_component_1.SignagePlaylist, "Playlist", playlist_configuration_component_1.SignagePlaylistConfiguration),
            "emergency_message": new signage_component_type_model_1.SignageComponentType("emergency_message", emergency_message_component_1.EmergencyMessage, "Emergency Message", emergency_message_configuration_component_1.EmergencyMessageConfiguration),
            "single_queue": new signage_component_type_model_1.SignageComponentType("single_queue", single_queue_component_1.SingleQueue, "Single Queue", single_queue_configuration_component_1.SingleQueueConfiguration),
            "multi_queue": new signage_component_type_model_1.SignageComponentType("multi_queue", multi_queue_component_1.MultiQueue, "Multi Queue", multi_queue_configuration_component_1.MultiQueueConfiguration),
            "missed_queue": new signage_component_type_model_1.SignageComponentType("missed_queue", missed_queue_component_1.MissedQueue, "Missed Queue", missed_queue_configuration_component_1.MissedQueueConfiguration),
            "employee_name": new signage_component_type_model_1.SignageComponentType("employee_name", employee_name_component_1.EmployeeName, "Employee Name", employee_name_configuration_component_1.EmployeeNameConfiguration),
            "location_name": new signage_component_type_model_1.SignageComponentType("location_name", location_name_component_1.LocationName, "Location Name", location_name_configuration_component_1.LocationNameConfiguration),
            "single_booking": new signage_component_type_model_1.SignageComponentType("single_booking", single_booking_component_1.SingleBooking, "Single Booking", single_booking_configuration_component_1.SingleBookingConfiguration),
            "multi_booking": new signage_component_type_model_1.SignageComponentType("multi_booking", multi_booking_component_1.MultiBooking, "Multi Booking", multi_booking_configuration_component_1.MultiBookingConfiguration),
            "web_frame": new signage_component_type_model_1.SignageComponentType("web_frame", web_frame_component_1.WebFrame, "Web URL", web_frame_configuration_component_1.WebFrameConfiguration),
        };
    }
    SignageComponentTypeService.prototype.getComponentTypes = function () {
        return Object.values(this.types);
    };
    SignageComponentTypeService.prototype.getComponentTypeById = function (id) {
        return this.types[id];
    };
    SignageComponentTypeService.prototype.getComponentTypeMap = function () {
        return this.types;
    };
    SignageComponentTypeService = __decorate([
        core_1.Injectable()
    ], SignageComponentTypeService);
    return SignageComponentTypeService;
}());
exports.SignageComponentTypeService = SignageComponentTypeService;
//# sourceMappingURL=signage-component-type.service.js.map