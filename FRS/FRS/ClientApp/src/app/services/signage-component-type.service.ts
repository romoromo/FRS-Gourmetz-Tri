import { Injectable } from '@angular/core';
import { SignageComponentType } from '../models/signage-component-type.model';
import { FreeHTML } from '../components/signage/signage-component-types/free-html/free-html.component';
import { FreeHTMLConfiguration } from '../components/signage/signage-component-types/free-html/free-html-configuration.component';
import { Clock } from '../components/signage/signage-component-types/clock/clock.component';
import { ClockConfiguration } from '../components/signage/signage-component-types/clock/clock-configuration.component';
import { Video } from '../components/signage/signage-component-types/video/video.component';
import { VideoConfiguration } from '../components/signage/signage-component-types/video/video-configuration.component';
import { SignagePlaylist } from '../components/signage/signage-component-types/playlist/playlist.component';
import { SignagePlaylistConfiguration } from '../components/signage/signage-component-types/playlist/playlist-configuration.component';
import { EmergencyMessage } from '../components/signage/signage-component-types/emergency-message/emergency-message.component';
import { EmergencyMessageConfiguration } from '../components/signage/signage-component-types/emergency-message/emergency-message-configuration.component';
import { SingleQueue } from '../components/signage/signage-component-types/single-queue/single-queue.component';
import { SingleQueueConfiguration } from '../components/signage/signage-component-types/single-queue/single-queue-configuration.component';
import { MultiQueueConfiguration } from '../components/signage/signage-component-types/multi-queue/multi-queue-configuration.component';
import { MultiQueue } from '../components/signage/signage-component-types/multi-queue/multi-queue.component';
import { EmployeeName } from '../components/signage/signage-component-types/employee-name/employee-name.component';
import { EmployeeNameConfiguration } from '../components/signage/signage-component-types/employee-name/employee-name-configuration.component';
import { LocationName } from '../components/signage/signage-component-types/location-name/location-name.component';
import { LocationNameConfiguration } from '../components/signage/signage-component-types/location-name/location-name-configuration.component';
import { MissedQueue } from '../components/signage/signage-component-types/missed-queue/missed-queue.component';
import { MissedQueueConfiguration } from '../components/signage/signage-component-types/missed-queue/missed-queue-configuration.component';
import { SingleBooking } from '../components/signage/signage-component-types/single-booking/single-booking.component';
import { SingleBookingConfiguration } from '../components/signage/signage-component-types/single-booking/single-booking-configuration.component';
import { MultiBooking } from '../components/signage/signage-component-types/multi-booking/multi-booking.component';
import { MultiBookingConfiguration } from '../components/signage/signage-component-types/multi-booking/multi-booking-configuration.component';
import { FreeText } from '../components/signage/signage-component-types/free-text/free-text.component';
import { FreeTextConfiguration } from '../components/signage/signage-component-types/free-text/free-text-configuration.component';
import { WebFrame } from '../components/signage/signage-component-types/web-frame/web-frame.component';
import { WebFrameConfiguration } from '../components/signage/signage-component-types/web-frame/web-frame-configuration.component';

@Injectable()
export class SignageComponentTypeService {

  types: any = {
    "free_text": new SignageComponentType("free_text", FreeText, "Free Text", FreeTextConfiguration),
    "free_html": new SignageComponentType("free_html", FreeHTML, "Free HTML", FreeHTMLConfiguration),
    "clock": new SignageComponentType("clock", Clock, "Date Time", ClockConfiguration),
    "video": new SignageComponentType("video", Video, "Video", VideoConfiguration),
    "playlist": new SignageComponentType("playlist", SignagePlaylist, "Playlist", SignagePlaylistConfiguration),
    "emergency_message": new SignageComponentType("emergency_message", EmergencyMessage, "Emergency Message", EmergencyMessageConfiguration),
    "single_queue": new SignageComponentType("single_queue", SingleQueue, "Single Queue", SingleQueueConfiguration),
    "multi_queue": new SignageComponentType("multi_queue", MultiQueue, "Multi Queue", MultiQueueConfiguration),
    "missed_queue": new SignageComponentType("missed_queue", MissedQueue, "Missed Queue", MissedQueueConfiguration),
    "employee_name": new SignageComponentType("employee_name", EmployeeName, "Employee Name", EmployeeNameConfiguration),
    "location_name": new SignageComponentType("location_name", LocationName, "Location Name", LocationNameConfiguration),
    "single_booking": new SignageComponentType("single_booking", SingleBooking, "Single Booking", SingleBookingConfiguration),
    "multi_booking": new SignageComponentType("multi_booking", MultiBooking, "Multi Booking", MultiBookingConfiguration),
    "web_frame": new SignageComponentType("web_frame", WebFrame, "Web URL", WebFrameConfiguration),
    
  }

  getComponentTypes() : SignageComponentType[] {
    return (<any>Object).values(this.types);
  }

  getComponentTypeById(id: string): SignageComponentType {
    return this.types[id];
  }

  getComponentTypeMap(): any {
    return this.types;
  }
}
