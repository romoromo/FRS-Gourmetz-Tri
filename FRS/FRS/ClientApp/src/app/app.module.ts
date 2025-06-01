import 'core-js/es6/reflect';
import 'core-js/es7/reflect';
import 'zone.js/dist/zone';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { NgModule, ErrorHandler, LOCALE_ID, APP_INITIALIZER } from "@angular/core";
import { RouterModule } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FlatpickrModule } from 'angularx-flatpickr';
import { CalendarModule, DateAdapter, CalendarUtils } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { MaterialModule } from './material-module';
import { MatDialogModule, MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

 

import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { ToastaModule } from 'ngx-toasta';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TooltipModule } from "ngx-bootstrap/tooltip";
import { PopoverModule } from "ngx-bootstrap/popover";
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { ChartsModule } from 'ng2-charts';
import { TreeModule } from 'ng2-tree';
import { NgxSummernoteModule } from 'ngx-summernote';

import { AppRoutingModule } from './app-routing.module';
import { AppErrorHandler } from './app-error.handler';
import { AppTitleService } from './services/app-title.service';
import { AppTranslationService, TranslateLanguageLoader } from './services/app-translation.service';
import { ConfigurationService } from './services/configuration.service';
import { AlertService } from './services/alert.service';
import { LocalStoreManager } from './services/local-store-manager.service';
import { EndpointFactory } from './services/endpoint-factory.service';
import { NotificationService } from './services/notification.service';
import { NotificationEndpoint } from './services/notification-endpoint.service';
import { AccountService } from './services/account.service';
import { AccountEndpoint } from './services/account-endpoint.service';
import { FacilityService } from './services/facility.service';
import { FacilityTypeService } from './services/facility-type.service';
import { CommonEndpoint } from './services/common-endpoint.service';
import { ReservationService } from './services/reservation.service';
import { DeviceService } from './services/device.service';
import { InstitutionService } from './services/institution.service';
import { LocationService } from './services/location.service';

import { EqualValidator } from './directives/equal-validator.directive';
import { LastElementDirective } from './directives/last-element.directive';
import { AutofocusDirective } from './directives/autofocus.directive';
import { BootstrapTabDirective } from './directives/bootstrap-tab.directive';
import { BootstrapToggleDirective } from './directives/bootstrap-toggle.directive';
import { BootstrapSelectDirective } from './directives/bootstrap-select.directive';
import { BootstrapDatepickerDirective } from './directives/bootstrap-datepicker.directive';
import { GroupByPipe } from './pipes/group-by.pipe';
import { DateOnlyPipe } from './pipes/datetime.pipe';
import { DateTimeOnlyPipe } from './pipes/datetime.pipe';

import { AppComponent } from "./components/app.component";
import { LoginComponent } from "./components/login/login.component";
import { HomeComponent } from "./components/home/home.component";
import { SettingsComponent } from "./components/settings/settings.component";
import { NotFoundComponent } from "./components/not-found/not-found.component";
import { FacilitiesComponent } from "./components/facilities/facilities.component";
import { ReservationsComponent } from "./components/reservations/reservations.component";
import { ReservationEditorComponent } from "./components/reservations/reservation-editor.component";
import { ReservationPictureComponent } from "./components/reservations/reservation-picture.component";
import { PictureEditorComponent } from './components/reservations/picture-editor/picture-editor.component';
import { CalendarComponent } from "./components/calendar/calendar.component";
import { BookingComponent } from "./components/calendar/booking/booking.component";
import { BookingEditorComponent } from "./components/calendar/booking/booking-editor.component";
import { BookingGridComponent } from "./components/calendar/booking-grid/booking-grid.component";
import { BookingGridEditorComponent } from "./components/calendar/booking-grid/booking-grid-editor.component";
import { CalendarHeaderComponent } from './components/calendar/calendar-header.component';
import { DayViewSchedulerComponent, DayViewSchedulerCalendarUtils } from './components/calendar/calendar-booking-grid/day-view-scheduler.component';

import { BannerDemoComponent } from "./components/controls/banner-demo.component";
import { TodoDemoComponent } from "./components/controls/todo-demo.component";
import { StatisticsDemoComponent } from "./components/controls/statistics-demo.component";
import { NotificationsViewerComponent } from "./components/controls/notifications-viewer.component";
import { UpcomingEventsComponent } from "./components//dashboard/upcoming-events/upcoming-events.component";
import { SearchBoxComponent } from "./components/controls/search-box.component";
import { UserInfoComponent } from "./components/controls/user-info.component";
import { UserPreferencesComponent } from "./components/controls/user-preferences.component";
import { UsersManagementComponent } from "./components/controls/users-management.component";
import { RolesManagementComponent } from "./components/controls/roles-management.component";
import { RoleEditorComponent } from "./components/controls/role-editor.component";
import { FacilitiesManagementComponent } from "./components/controls/facilities/facilities-management.component";
import { FacilityEditorComponent } from "./components/controls/facilities/facility-editor.component";
import { FacilityTypesManagementComponent } from "./components/controls/facility-type/facility-types-management.component";
import { FacilityTypeEditorComponent } from "./components/controls/facility-type/facility-type-editor.component";
import { DeviceManagerComponent } from "./components/device-manager/device-manager.component";

import { DevicesManagementComponent } from "./components/device-manager/unknown-device/devices-management.component";
import { DeviceEditorComponent } from "./components/device-manager/unknown-device/device-editor.component";

import { LocationsManagementComponent } from "./components/controls/locations/locations-management.component";
import { LocationEditorComponent } from "./components/controls/locations/location-editor.component";
import { MatInputModule, MatFormFieldModule, MAT_LABEL_GLOBAL_OPTIONS, MAT_DATE_LOCALE } from '@angular/material';
import { ReservationListComponent } from './components/calendar/reservations/reservations.component';
import { ReservationListEditorComponent } from './components/calendar/reservations/reservation-editor.component';

import { InstitutionsManagementComponent } from "./components/controls/institution/institutions-management.component";
import { InstitutionEditorComponent } from "./components/controls/institution/institution-editor.component";
import { LocationsTreeManagementComponent } from './components/controls/locations-tree/locations-tree-management.component';
import { LocationTreeEditorComponent } from './components/controls/locations-tree/location-tree-editor.component';
import { FileUploadComponent } from './components/common/fileuploader/fileuploader.component';
import { ImageUploadComponent } from './components/common/ImageUploader/imageuploader.component';
import { FileService } from './services/file.service';
import { ImageFileService } from './services/image-file.service';
import { PlaylistService } from './services/playlist.service';
import { CalendarBookingGridComponent } from './components/calendar/calendar-booking-grid/calendar-booking-grid.component';
import { CalendarBookingGridEditorComponent } from './components/calendar/calendar-booking-grid/calendar-booking-grid-editor.component';
import { BookingService } from './services/booking.service';
import { DashboardService } from './services/dashboard.service';
import { DeviceForApprovalComponent } from './components/dashboard/device-for-approval/device-for-approval.component';
import { DashboardAvailableRoomComponent } from './components/dashboard/available-room/available-room.component';
import { DepartmentEditorComponent } from './components/controls/department/department-editor.component';
import { DepartmentsManagementComponent } from './components/controls/department/departments-management.component';
import { DepartmentService } from './services/department.service';
import { RegisterComponent } from './components/register/register.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { ExternalLoginComponent } from './components/login/external-login.component';
import { AttendanceSignInComponent } from './components/attendance-signin/attendance-signin.component';
import { ContactGroupsManagementComponent } from './components/controls/contactgroup/contactgroups-management.component';
import { ContactGroupEditorComponent } from './components/controls/contactgroup/contactgroup-editor.component';
import { ContactGroupService } from './services/contactgroup.service';
import { CGNewContactEmailEditorComponent } from './components/controls/contactgroup/emailcontact/emailcontact-editor.component';
import { ContactGroupSelectorComponent } from './components/common/contact-group-selector/contact-group-selector.component';
import { UserSelectorComponent } from './components/common/user-selector/user-selector.component';
import { UserPhonebookEditorComponent } from './components/controls/userphonebook/userphonebook-editor.component';
import { UserPhonebooksManagementComponent } from './components/controls/userphonebook/userphonebooks-management.component';
import { UserPhonebookSelectorComponent } from './components/common/userphonebook-selector/userphonebook-selector.component';
import { ReportsReservationListComponent } from './components/reports/reservations/reservations.component';
import { ShapeService } from './services/shape.service';
import { TextNodeService } from './services/text-node.service';
import { TemplateEditorComponent } from './components/template-editor/template-editor.component';
import { PIBService } from './services/pib.service';
import { KioskSettingsService } from './services/kiosk-settings.service';
import { TemplateEditorPreviewDialog } from './components/template-editor/preview/preview.component';
import { TemplateSaveDialogPreviewDialog } from './components/template-editor/save-dialog/save-dialog.component';
import { PIBDevicesManagementComponent } from './components/pib-device-manager/pib-unknown-device/pib-devices-management.component';
import { PIBDeviceEditorComponent } from './components/pib-device-manager/pib-unknown-device/pib-device-editor.component';
import { PIBDeviceManagerComponent } from './components/pib-device-manager/pib-device-manager.component';
import { NoNavigationAppComponent } from './components/no_navigation_app.component';
import { RootAppComponent } from './components/root-app.component';
import { ImageUploadsComponent } from "./components/imageUpload/image-upload.component";
import { ImageListsComponent } from "./components/imageUpload/image-list.component";
import { PlaylistCreatorComponent } from "./components/playlistCreator/playlist-creator.component";
import { FeedbackComponent } from './components/feedback/feedback.component';
import { FRSTemplateSaveDialogPreviewDialog } from './components/frs-template-editor/frs-save-dialog/frs-save-dialog.component';
import { FRSTemplateEditorPreviewDialog } from './components/frs-template-editor/frs-preview/frs-preview.component';
import { FRSTemplateEditorComponent } from './components/frs-template-editor/frs-template-editor.component';
import { EpaperService } from './services/epaper.service';
import { EpaperDevicesManagementComponent } from './components/frs-device-manager/frs-unknown-device/frs-devices-management.component';
import { EpaperDeviceEditorComponent } from './components/frs-device-manager/frs-unknown-device/frs-device-editor.component';
import { EpaperDeviceManagerComponent } from './components/frs-device-manager/frs-device-manager.component';
import { PIBDisplayComponent } from './components/template-editor/pibdisplay/pibdisplay.component';
import { PIBDeviceStatusListComponent } from './components/dashboard/pib-device-list/pib-device-list.component';
import { UserVehicleEditorComponent } from './components/controls/uservehicle/uservehicle-editor.component';
import { UserCardIdEditorComponent } from './components/controls/usercardid/usercardid-editor.component';
import { UserVehiclesManagementComponent } from './components/controls/uservehicle/uservehicle-management.component';
import { SignageComponentService } from './services/signage-component.service';
import { SignageComponentsManagementComponent } from './components/signage/signage-component/signage-components-management.component';
import { MeetingRoomDisplayComponent } from './components/meeting-room/meeting-room.component';
import { DeviceDisplayWarningComponent } from './components/device-warning/device-warning.component';
import { ReportContainerComponent } from './components/reports/report-container/report-container.component';
import { SignageComponentEditorComponent } from './components/signage/signage-component/signage-component-editor.component';
import { SignageComponentTypeService } from './services/signage-component-type.service';
import { SignageComponentTypeWrapperComponent } from './components/signage/signage-component-types/signage-component-type-wrapper.component';
import { FreeHTMLConfiguration } from './components/signage/signage-component-types/free-html/free-html-configuration.component';
import { SignageComponentTypeDirective } from './directives/signage-component-type.directive';
import { FreeHTML } from './components/signage/signage-component-types/free-html/free-html.component';
import { ClockConfiguration } from './components/signage/signage-component-types/clock/clock-configuration.component';
import { Clock } from './components/signage/signage-component-types/clock/clock.component';
import { SignageCompilationsManagementComponent } from './components/signage/signage-compilation/signage-compilations-management.component';
import { SignageCompilationEditorComponent } from './components/signage/signage-compilation/signage-compilation-editor.component';
import { SignageCompilationService } from './services/signage-compilation.service';
import { SignagePublicationsManagementComponent } from './components/signage/signage-publication/signage-publications-management.component';
import { SignagePublicationEditorComponent } from './components/signage/signage-publication/signage-publication-editor.component';
import { SignagePublicationService } from './services/signage-publication.service';
import { SignageDisplayComponent } from './components/signage/signage-display/signage-display.component';
import { VideoConfiguration } from './components/signage/signage-component-types/video/video-configuration.component';
import { Video } from './components/signage/signage-component-types/video/video.component';
import { VjsPlayerComponent } from './components/signage/signage-component-types/vjs-player.component';
import { ApplicationSettingEditorComponent } from './components/application-setting/application-setting-editor.component';
import { ApplicationSettingsManagementComponent } from './components/application-setting/application-settings-management.component';
import { MediasManagementComponent } from './components/controls/media/medias-management.component';
import { MediaEditorComponent } from './components/controls/media/media-editor.component';
import { ApplicationSettingService } from './services/application-setting.service';
import { MediaService } from './services/media.service';
import { SignagePlaylistConfiguration } from './components/signage/signage-component-types/playlist/playlist-configuration.component';
import { SignagePlaylist } from './components/signage/signage-component-types/playlist/playlist.component';
import { PlaylistsManagementComponent } from './components/playlistManagement/playlists-management.component';
import { DirectoryListingCategoryEditorComponent } from './components/directory-listing-category/directory-listing-category-editor.component';
import { DirectoryListingCategorysManagementComponent } from './components/directory-listing-category/directory-listing-categorys-management.component';
import { DirectoryListingCategoryService } from './services/directory-listing-category.service';
import { DirectoryListingEditorComponent } from './components/directory-listing/directory-listing-editor.component';
import { DirectoryListingsManagementComponent } from './components/directory-listing/directory-listings-management.component';
import { DirectoryListingService } from './services/directory-listing.service';
import { BuildingsManagementComponent } from './components/building/buildings-management.component';
import { BuildingEditorComponent } from './components/building/building-editor.component';
import { ReportsVehicleLogListComponent } from './components/reports/vehicle-logs/vehicle-logs.component';
import { FRSHubConnections } from './models/user.model';
import { BuildingService } from './services/building.service';
import { FloorsManagementComponent } from './components/floor/floors-management.component';
import { FloorEditorComponent } from './components/floor/floor-editor.component';
import { FloorService } from './services/floor.service';
import { MapService } from './services/map.service';
import { MapEditorComponent } from './components/map/map-editor.component';
import { MapsManagementComponent } from './components/map/maps-management.component';
import { EmergencyMessageConfiguration } from './components/signage/signage-component-types/emergency-message/emergency-message-configuration.component';
import { EmergencyMessage } from './components/signage/signage-component-types/emergency-message/emergency-message.component';
import { OccupancyLogEditorComponent } from './components/occupancy-log/occupancy-log-editor.component';
import { OccupancyLogsManagementComponent } from './components/occupancy-log/occupancy-logs-management.component';
import { OccupancyLogService } from './services/occupancy-log.service';
import { QueueTableMapEditorComponent } from './components/queue-table-map/queue-table-map-editor.component';
import { QueueTableMapSimulator, QueueTableMapsManagementComponent } from './components/queue-table-map/queue-table-maps-management.component';
import { QueueTableMapService } from './services/queue-table-map.service';
import { SingleQueueConfiguration } from './components/signage/signage-component-types/single-queue/single-queue-configuration.component';
import { SingleQueue } from './components/signage/signage-component-types/single-queue/single-queue.component';
import { TextConfiguration } from './components/signage/signage-component-types/text-configuration.component';
import { ModuleEditorComponent } from './components/controls/module/module-editor.component';
import { ModuleService } from './services/module.service';
import { ModulesManagementComponent } from './components/controls/module/modules-management.component';
import { MultiQueue, NgInit } from './components/signage/signage-component-types/multi-queue/multi-queue.component';
import { MultiQueueConfiguration } from './components/signage/signage-component-types/multi-queue/multi-queue-configuration.component';
import { EmployeeDesignationEditorComponent } from './components/employee-designation/employee-designation-editor.component';
import { EmployeeDesignationsManagementComponent } from './components/employee-designation/employee-designations-management.component';
import { EmployeeDesignationService } from './services/employee-designation.service';
import { EmployeeDataEditorComponent } from './components/employee-data/employee-data-editor.component';
import { EmployeeDatasManagementComponent } from './components/employee-data/employee-datas-management.component';
import { EmployeeDataService } from './services/employee-data.service';
import { EmployeeScheduleEditorComponent } from './components/employee-schedule/employee-schedule-editor.component';
import { EmployeeSchedulesManagementComponent } from './components/employee-schedule/employee-schedules-management.component';
import { EmployeeScheduleService } from './services/employee-schedule.service';
import { EmployeeScheduleAssignComponent } from './components/employee-schedule/employee-schedule-assign.component';
import { EmployeeName } from './components/signage/signage-component-types/employee-name/employee-name.component';
import { EmployeeNameConfiguration } from './components/signage/signage-component-types/employee-name/employee-name-configuration.component';
import { LocationNameConfiguration } from './components/signage/signage-component-types/location-name/location-name-configuration.component';
import { LocationName } from './components/signage/signage-component-types/location-name/location-name.component';
import { WayfinderComponent } from './components/wayfinder-view/wayfinder-view.component'
import { WayfinderLandscapeComponent } from './components/wayfinder-landscape-view/wayfinder-landscape-view.component'
import { KioskSettingsListComponent } from './components/kioskSettings/kiosk-settings-list.component';
import { KioskSettingsEditComponent } from './components/kioskSettings/kiosk-settings-edit.component';

import { CarouselModule as OwlCarousel } from 'ngx-owl-carousel-o';
import { MissedQueueConfiguration } from './components/signage/signage-component-types/missed-queue/missed-queue-configuration.component';
import { MissedQueue } from './components/signage/signage-component-types/missed-queue/missed-queue.component';
import { ColorPickerModule } from 'ngx-color-picker';
import { EmsesManagementComponent } from './components/controls/ems/emses-management.component';
import { EmsEditorComponent } from './components/controls/ems/ems-editor.component';
import { EmsProfileService } from './services/ems-profile.service';
import { SignageDashboardContainerComponent } from './components/dashboard/signage/signage-dashboard-container/signage-dashboard-container.component';
import { UserGroupsManagementComponent } from './components/controls/user-group/user-groups-management.component';
import { UserGroupEditorComponent } from './components/controls/user-group/user-group-editor.component';
import { UserGroupService } from './services/userGroup.service';
import { EmsScheduleService } from './services/ems-schedule.service';
import { EmsService } from './services/ems.service';
import { EmsProfilesComponent } from './components/ems-profile/ems-profiles.component';
import { EmsProfileEditorComponent } from './components/ems-profile/ems-profile-editor.component';
import { EmsSchedulesComponent } from './components/ems-schedule/ems-schedules.component';
import { EmsScheduleEditorComponent } from './components/ems-schedule/ems-schedule-editor.component';
import { LocationPanelComponent } from './components/dashboard/signage/location-panel/location-panel.component';
import { CalendarLandingPageComponent } from './components/calendar/landing-page/landing-page.component';
import { DevicePushMessageDialog } from './components/dashboard/signage/push-message-dialog/push-message-dialog.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MenuPanelComponent } from './components/menu/menu-panel/menu-panel.component';
import { SubMenuPanelComponent } from './components/menu/sub-menu-panel/sub-menu-panel.component';
import { TreeModule as CirclonTreeModule } from 'angular-tree-component';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { ImageViewerComponent } from './components/common/image-viewer/image-viewer.component';
import { ImageZoomModule } from 'angular2-image-zoom';
import { MediaExtensionService } from './services/media-extension.service';
import { MediaExtensionEditorComponent } from './components/media-extension/media-extension-editor.component';
import { MediaExtensionsManagementComponent } from './components/media-extension/media-extensions-management.component';
//import { PdfViewerModule } from 'ng2-pdf-viewer';
import { AuthLogsManagementComponent } from './components/audit/auth-log/auth-logs-management.component';
import { DataLogsManagementComponent } from './components/audit/data-log/data-logs-management.component';
import { AuditService } from './services/audit.service';
import { DataLogViewerComponent } from './components/audit/data-log/data-log-viewer.component';
import { ImageReferenceTypeEditorComponent } from './components/controls/image-reference-type/image-reference-type-editor.component';
import { ImageReferenceTypesManagementComponent } from './components/controls/image-reference-type/image-reference-types-management.component';
import { ImageReferenceTypeService } from './services/image-reference-type.service';
import { ImageReferenceColorsManagementComponent } from './components/controls/image-reference-color/image-reference-colors-management.component';
import { ImageReferenceColorEditorComponent } from './components/controls/image-reference-color/image-reference-color-editor.component';
import { ImageReferenceColorService } from './services/image-reference-color.service';
import { NgxDocViewerModule } from 'ngx-doc-viewer';
import { SmartRoomSchedulerLogsManagementComponent } from './components/smartroom-scheduler-log/smartroom-scheduler-logs-management.component';
import { SmartRoomSchedulerService } from './services/smartroom.service';
import { SmartRoomSchedulerDetailsViewerComponent } from './components/smartroom-scheduler-log/details-log-viewer.component';
import { SingleBookingConfiguration } from './components/signage/signage-component-types/single-booking/single-booking-configuration.component';
import { SingleBooking } from './components/signage/signage-component-types/single-booking/single-booking.component';
import { TextDisplay } from './components/signage/signage-component-types/text-display.component';
import { MultiBookingConfiguration } from './components/signage/signage-component-types/multi-booking/multi-booking-configuration.component';
import { MultiBooking } from './components/signage/signage-component-types/multi-booking/multi-booking.component';
import { AssetTypesManagementComponent } from './components/controls/asset-type/asset-types-management.component';
import { AssetTypeEditorComponent } from './components/controls/asset-type/asset-type-editor.component';
import { AssetModelsManagementComponent } from './components/controls/asset-model/asset-models-management.component';
import { AssetModelEditorComponent } from './components/controls/asset-model/asset-model-editor.component';
import { AssetService } from './services/asset.service';
import { ServiceContractService } from './services/service-contract.service';
import { ServiceContractsManagementComponent } from './components/service-contract/service-contracts-management.component';
import { ServiceContractEditorComponent } from './components/service-contract/service-contract-editor.component';
import { AssetEditorComponent } from './components/controls/asset/asset-editor.component';
import { AssetQrCodeComponent, AssetsManagementComponent } from './components/controls/asset/assets-management.component';
import { AssetSelectorComponent } from './components/common/asset-selector/asset-selector.component';
import { FreeTextConfiguration } from './components/signage/signage-component-types/free-text/free-text-configuration.component';
import { FreeText } from './components/signage/signage-component-types/free-text/free-text.component';
import { EmailQueuesManagementComponent } from './components/controls/email-queue/email-queues-management.component';
import { EmailQueueService } from './services/emailqueue.service';
import { UpDownTimeLogsManagementComponent } from './components/audit/updowntime-log/updowntime-logs-management.component';
import { QRCodeModule } from 'angular2-qrcode';
import { DeviceTypeEditorComponent } from './components/controls/device-type/device-type-editor.component';
import { DeviceTypesManagementComponent } from './components/controls/device-type/device-types-management.component';
import { WebFrame } from './components/signage/signage-component-types/web-frame/web-frame.component';
import { WebFrameConfiguration } from './components/signage/signage-component-types/web-frame/web-frame-configuration.component';
import { StudentService } from './services/meal-order/student.service';
import { ClassService } from './services/meal-order/class.service';
import { CreateAccountMultiple, StudentsManagementComponent } from './components/meal-order-system/student/students-management.component';
import { StudentGroupsManagementComponent } from './components/meal-order-system/student-group/student-groups-management.component';
import { ClassesManagementComponent } from './components/meal-order-system/class/classes-management.component';
import { ClassLevelsManagementComponent } from './components/meal-order-system/class-level/class-levels-management.component';
import { DispenserOutletManagementComponent } from './components/meal-order-system/dispenser-outlet/dispenser-outlet-management.component';
import { StudentEditorComponent } from './components/meal-order-system/student/student-editor.component';
import { StudentGroupEditorComponent } from './components/meal-order-system/student-group/student-group-editor.component';
import { StudentSelectorComponent } from './components/meal-order-system/student-group/student-selector/student-selector.component';
import { ClassLevelEditorComponent } from './components/meal-order-system/class-level/class-level-editor.component';
import { DispenserOutletEditorComponent } from './components/meal-order-system/dispenser-outlet/dispenser-outlet-editor.component';
import { ClassEditorComponent } from './components/meal-order-system/class/class-editor.component';
import { UserService } from './services/meal-order/user.service';
import { StaffService } from './services/meal-order/staff.service';
import { StaffEditorComponent } from './components/meal-order-system/staff/staff-editor.component';
import { StaffsManagementComponent } from './components/meal-order-system/staff/staffs-management.component';
import { StaffTypeEditorComponent } from './components/meal-order-system/staff-type/staff-type-editor.component';
import { StaffTypesManagementComponent } from './components/meal-order-system/staff-type/staff-types-management.component';
import { RestrictionTypesManagementComponent } from './components/meal-order-system/restriction-type/restriction-types-management.component';
import { RestrictionTypeEditorComponent } from './components/meal-order-system/restriction-type/restriction-type-editor.component';
import { RestrictionEditorComponent } from './components/meal-order-system/restriction/restriction-editor.component';
import { RestrictionsManagementComponent } from './components/meal-order-system/restriction/restrictions-management.component';
import { RestrictionService } from './services/meal-order/restriction.service';
import { MealTypeEditorComponent } from './components/meal-order-system/meal-type/meal-type-editor.component';
import { DishTypeEditorComponent } from './components/meal-order-system/dish-type/dish-type-editor.component';
import { MealPeriodEditorComponent } from './components/meal-order-system/meal-period/meal-period-editor.component';
import { DishService } from './services/meal-order/dish.service';
import { MealService } from './services/meal-order/meal.service';
import { DishTypesManagementComponent } from './components/meal-order-system/dish-type/dish-types-management.component';
import { MealTypesManagementComponent } from './components/meal-order-system/meal-type/meal-types-management.component';
import { MealPeriodsManagementComponent } from './components/meal-order-system/meal-period/meal-periods-management.component';
import { DishEditorComponent } from './components/meal-order-system/dishes/dish-editor.component';
import { DishesManagementComponent } from './components/meal-order-system/dishes/dishes-management.component';
import { MenuEditorComponent } from './components/meal-order-system/menus/menu-editor.component';
import { TokenOrderEditorComponent } from './components/meal-order-system/token-order/token-order-editor.component';
import { MenuService } from './services/meal-order/menu.service';
import { MenusManagementComponent } from './components/meal-order-system/menus/menus-management.component';
import { TokenOrdersManagementComponent } from './components/meal-order-system/token-order/token-orders-management.component';
import { TokenOrdersBulkManagementComponent } from './components/meal-order-system/token-order-bulk/token-orders-bulk-management.component';
import { TokenOrderBulkEditorComponent } from './components/meal-order-system/token-order-bulk/token-order-bulk-editor.component';
import { MealAllocationssManagementComponent } from './components/meal-order-system/meal-allocation/meal-allocations-management.component';
import { PrintAllocationComponent } from './components/meal-order-system/meal-allocation/print-allocation.component';
import { PackingAllocationssManagementComponent } from './components/meal-order-system/packing-allocation/packing-allocations-management.component';
import { PackingAllocationEditorComponent } from './components/meal-order-system/packing-allocation/packing-allocation-editor.component';
import { PackingTagsManagementComponent } from './components/meal-order-system/packing-tag/packing-tags-management.component';
import { PackingTagEditorComponent } from './components/meal-order-system/packing-tag/packing-tag-editor.component';
import { ScanTagComponent } from './components/meal-order-system/packing-tag/scan-tag/scan-tag.component';
import { MenuCycleEditorComponent } from './components/meal-order-system/menu-cycle/menu-cycle-editor.component';
import { MenuCyclesManagementComponent } from './components/meal-order-system/menu-cycle/menu-cycles-management.component';
import { CatererInfosManagementComponent } from './components/meal-order-system/caterer-info/caterer-infos-management.component';
import { CatererInfoEditorComponent } from './components/meal-order-system/caterer-info/caterer-info-editor.component';
import { BentoBoxTypeEditorComponent } from './components/meal-order-system/bento-box-type/bento-box-type-editor.component';
import { BentoBoxTypesManagementComponent } from './components/meal-order-system/bento-box-type/bento-box-types-management.component';
import { DeliveryService } from './services/meal-order/delivery.service';
import { BentoAssetEditorComponent } from './components/meal-order-system/bento-asset/bento-asset-editor.component';
import { BentoAssetsManagementComponent, NewMultipleBentoAsset } from './components/meal-order-system/bento-asset/bento-assets-management.component';
import { BentoUsageManagementComponent } from './components/meal-order-system/bento-usage/bento-usage-management.component';
import { CartonTypeEditorComponent } from './components/meal-order-system/carton-type/carton-type-editor.component';
import { CartonTypesManagementComponent } from './components/meal-order-system/carton-type/carton-types-management.component';
import { CartonAssetEditorComponent } from './components/meal-order-system/carton-asset/carton-asset-editor.component';
import { CartonAssetsManagementComponent, CartonAssetQrCodeComponent } from './components/meal-order-system/carton-asset/carton-assets-management.component';
import { TrackingStatusEditorComponent } from './components/meal-order-system/tracking-status/tracking-status-editor.component';
import { TrackingStatussManagementComponent } from './components/meal-order-system/tracking-status/tracking-statuss-management.component';
import { DeliveryOrderEditorComponent } from './components/meal-order-system/delivery-order/delivery-order-editor.component';
import { DeliveryOrdersManagementComponent } from './components/meal-order-system/delivery-order/delivery-orders-management.component';
import { DeliveryOrderNewEditorComponent } from './components/meal-order-system/delivery-order-new/delivery-order-new-editor.component';
import { DeliveryOrdersNewManagementComponent } from './components/meal-order-system/delivery-order-new/delivery-orders-new-management.component';
import { DishingProcesssManagementComponent } from './components/meal-order-system/dishing-process/dishing-processs-management.component';
import { ClassBatchesManagementComponent } from './components/meal-order-system/class-batch/class-batches-management.component';
import { ClassBatchEditorComponent } from './components/meal-order-system/class-batch/class-batch-editor.component';
import { DishSelectorComponent } from './components/meal-order-system/dishes/dish-selector/dish-selector.component';
import { DishPreviewComponent } from './components/meal-order-system/dishes/dish-preview/dish-preview.component';
import { OutletsManagementComponent } from './components/meal-order-system/outlet/outlets-management.component';
import { OutletEditorComponent } from './components/meal-order-system/outlet/outlet-editor.component';
import { CatererSelectorComponent } from './components/meal-order-system/outlet/caterer-selector/caterer-selector.component';
import { StoreSelectorComponent } from './components/meal-order-system/outlet/store-selector/store-selector.component';
import { OrderReportComponent } from './components/meal-order-system/outlet/order-report/order-report.component';
import { OrderDeliveryReportComponent } from './components/meal-order-system/outlet/order-delivery-report/order-delivery-report.component';
import { PrintLabelComponent } from './components/meal-order-system/outlet/print-label/print-label.component';
import { StoreSelectorDetailsComponent } from './components/meal-order-system/outlet/store-selector-details/store-selector-details.component';
import { DOSelectorComponent } from './components/meal-order-system/store-info/do-selector/do-selector.component';
import { ReceiveDOComponent } from './components/meal-order-system/store-info/receive-do/receive-do.component';
import { InventoryDetailComponent } from './components/meal-order-system/store-info/inventory-detail/inventory-detail.component';
import { InventorySelectorComponent } from './components/meal-order-system/store-info/inventory-selector/inventory-selector.component';
import { OutletProfilesManagementComponent } from './components/meal-order-system/outlet-profile/outlet-profiles-management.component';
import { OutletProfileEditorComponent } from './components/meal-order-system/outlet-profile/outlet-profile-editor.component';
import { CuisineEditorComponent } from './components/meal-order-system/cuisine/cuisine-editor.component';
import { CuisinesManagementComponent } from './components/meal-order-system/cuisine/cuisines-management.component';
import { StoreInfoEditorComponent } from './components/meal-order-system/store-info/store-info-editor.component';
import { StoreInfosManagementComponent } from './components/meal-order-system/store-info/store-infos-management.component';
import { DoProcesssManagementComponent } from './components/meal-order-system/do-process/do-processs-management.component';
import { PackingProcesssManagementComponent } from './components/meal-order-system/packing-process/packing-processs-management.component';
import { InterestGroupEditorComponent } from './components/meal-order-system/interest-group/interest-group-editor.component';
import { InterestGroupsManagementComponent } from './components/meal-order-system/interest-group/interest-groups-management.component';
import { DriverEditorComponent } from './components/meal-order-system/driver/driver-editor.component';
import { DriversManagementComponent } from './components/meal-order-system/driver/drivers-management.component';
import { RouteEditorComponent } from './components/meal-order-system/route/route-editor.component';
import { RoutesManagementComponent } from './components/meal-order-system/route/routes-management.component';
import { MealSessionEditorComponent } from './components/meal-order-system/outlet/meal-session/meal-session-editor.component';
import { MenuCycleCalendarComponent } from './components/meal-order-system/caterer-info/caterer-calendar/caterer-calendar.component';
import { CatererApprovalComponent } from './components/meal-order-system/caterer-info/caterer-selector/caterer-selector.component';
import { ViewMenuCycleMenuComponent } from './components/meal-order-system/menu-cycle/view-menu/view-menu.component';
import { OutletMenuCycleCalendarComponent } from './components/meal-order-system/outlet/outlet-calendar/outlet-calendar.component';
import { OutletViewMenuCycleMenuComponent } from './components/meal-order-system/outlet/view-menu/view-menu.component';
import { OutletClassRosterEditorComponent } from './components/meal-order-system/outlet/class-roster/class-roster-editor.component';
import { MenuDishSelectorEditorComponent } from './components/meal-order-system/menus/dish-selector/dish-selector-editor.component';
import { StudentMenuCycleCalendarComponent } from './components/meal-order-system/student/student-calendar/student-calendar.component';
import { DishCycleEditorComponent } from './components/meal-order-system/dish-cycle/dish-cycle-editor.component';
import { DishCyclesManagementComponent } from './components/meal-order-system/dish-cycle/dish-cycles-management.component';
import { DishCycleCalendarComponent } from './components/meal-order-system/caterer-info/caterer-dish-calendar/caterer-dish-calendar.component';
import { ViewDishCycleMenuComponent } from './components/meal-order-system/dish-cycle/view-menu/view-menu.component';
import { OutletDishCycleCalendarComponent } from './components/meal-order-system/outlet/dish-calendar/dish-calendar.component';
import { OutletViewDishCycleMenuComponent } from './components/meal-order-system/outlet/view-dish/view-dish.component';
import { ChannelInfoEditorComponent } from './components/controls/channel-info/channel-info-editor.component';
import { ChannelInfosManagementComponent } from './components/controls/channel-info/channel-infos-management.component';
import { ChannelInfoService } from './services/channel-info.service';
import { TokenOrderPaymentThankyouComponent } from './components/meal-order-system/token-order-payment/token-order-payment-thankyou/token-order-payment-thankyou.component';
import { TokenOrderPaymentComponent } from './components/meal-order-system/token-order-payment/token-order-payment.component';
import { OutletClassRostersManagementComponent } from './components/meal-order-system/outlet/class-roster/class-rosters-management.component';
import { PaymentTypesManagementComponent } from './components/meal-order-system/payment-type/payment-types-management.component';
import { PaymentTypeEditorComponent } from './components/meal-order-system/payment-type/payment-type-editor.component';
import { TransactionFeesManagementComponent } from './components/meal-order-system/transaction-fee/transaction-fees-management.component';
import { TransactionFeeEditorComponent } from './components/meal-order-system/transaction-fee/transaction-fee-editor.component';
import { PaymentService } from './services/meal-order/payment.service';
import { VoucherTypeEditorComponent } from './components/meal-order-system/voucher-type/voucher-type-editor.component';
import { VoucherEditorComponent } from './components/meal-order-system/voucher/voucher-editor.component';
import { VoucherTypesManagementComponent } from './components/meal-order-system/voucher-type/voucher-types-management.component';
import { VouchersManagementComponent } from './components/meal-order-system/voucher/vouchers-management.component';
import { WaiverEditorComponent } from './components/meal-order-system/waiver/waiver-editor.component';
import { WaiversManagementComponent } from './components/meal-order-system/waiver/waivers-management.component';
import { ContactUsSubject } from './models/meal-order/contact-us-subject.model';
import { ContactUsDetailsManagementComponent } from './components/meal-order-system/contact-us-detail/contact-us-details-management.component';
import { ContactUsDetailEditorComponent } from './components/meal-order-system/contact-us-detail/contact-us-detail-editor.component';
import { ContactUsSubjectEditorComponent } from './components/meal-order-system/contact-us-subject/contact-us-subject-editor.component';
import { EmailService } from './services/meal-order/email.service';
import { ContactUsSubjectsManagementComponent } from './components/meal-order-system/contact-us-subject/contact-us-subjects-management.component';
import { EmailTemplatesManagementComponent } from './components/meal-order-system/email-template/email-templates-management.component';
import { EmailTemplateEditorComponent } from './components/meal-order-system/email-template/email-template-editor.component';
import { InterestGroupEmailComponent } from './components/meal-order-system/interest-group/interest-group-email.component';
import { NotificationEventEditorComponent } from './components/meal-order-system/notification-event/notification-event-editor.component';
import { NotificationEventsManagementComponent } from './components/meal-order-system/notification-event/notification-events-management.component';
import { StudentNotificationDialogComponent } from './components/meal-order-system/student/notification-dialog.component';
import { CancelOrderRequestEditorComponent } from './components/meal-order-system/cancel-order-request/cancel-order-request-editor.component';
import { CancelOrderRequestsManagementComponent } from './components/meal-order-system/cancel-order-request/cancel-order-requests-management.component';
import { OrderLogsManagementComponent } from './components/audit/order-log/order-logs-management.component';
import { ExternalLoginLogsManagementComponent } from './components/audit/external-login-log/external-login-logs-management.component';
import { OrderMealSummaryComponent } from './components/meal-order-system/outlet/order-meal-summary/order-meal-summary.component';
import { UserChangePasswordComponent } from './components/user-change-password/user-change-password.component';
import { UserAccountsReportManagementComponent } from './components/reports/user-account/user-accounts-management.component';
import { FasMealOrderSummaryComponent } from './components/meal-order-system/fas-meal-order/fas-meal-orders-summary.component';
import { MenuGroupEditorComponent } from './components/meal-order-system/menu-group/menu-group-editor.component';
import { MenuGroupsManagementComponent } from './components/meal-order-system/menu-group/menu-groups-management.component';
import { OrderService } from './services/meal-order/order.service';
import { OrderCancellationsManagementComponent } from './components/meal-order-system/order-cancellation/order-cancellations-management.component';
import { UserRolesReportManagementComponent } from './components/reports/user-role/user-roles-management.component';
import { ReportService } from './services/report.service';
import { OrderCancellationReportManagementComponent } from './components/reports/order-cancellation/order-cancellations-management.component';
import { NotificationSettingComponent } from './components/notification-setting/notification-settings.component';
import { MealPlanSummaryComponent } from './components/meal-order-system/student-group/meal-plan/meal-plans-summary.component';
import { MultiFactorLoginComponent } from './components/multi-factor-login/multi-factor-login.component';
import { OrderPortalService } from './services/order-portal.service';
import { OrderPortalContentEditorComponent } from './components/meal-order-system/order-portal-content/order-portal-content.component';
import { OrderPortalContentsManagementComponent } from './components/meal-order-system/order-portal-content/order-portal-contents-management.component';
import { StudentGroupOrderSummaryComponent } from './components/meal-order-system/student-group/individual-order/individual-orders-summary.component';
import { UserActivityLogsManagementComponent } from './components/audit/user-activity-log/user-activity-logs-management.component';
import { UserActivityLogViewerComponent } from './components/audit/user-activity-log/user-activity-log-viewer.component';
import { OutletTermsManagementComponent } from './components/meal-order-system/outlet/outlet-term/outlet-terms-management.component';
import { OutletTermEditorComponent } from './components/meal-order-system/outlet/outlet-term/outlet-term-editor.component';
import { StudentOrderManagementComponent, StatusDropdownComponent, MatStatusDropdownComponent } from './components/meal-order-system/student/student-order/student-orders-management.component';
import { StudentOrderDetailComponent } from './components/meal-order-system/student/student-order/student-order-details.component';
import { DefaultPipe } from './pipes/default-string-val.pipe';
import { StudentOrderEditorComponent } from './components/meal-order-system/student/student-order/student-order-editor.component';
import { OrderCollectionLogsManagementComponent } from './components/meal-order-system/student-group/order-log/order-logs-management.component';
import { OrderByPipe } from './pipes/order-by.pipe';
import { PaymentsReportManagementComponent } from './components/reports/payment/payments-management.component';
import { FaqSubjectsManagementComponent } from './components/meal-order-system/faq-subject/faq-subjects-management.component';
import { FaqSubjectEditorComponent } from './components/meal-order-system/faq-subject/faq-subject-editor.component';
import { FaqDetailsManagementComponent } from './components/meal-order-system/faq-detail/faq-details-management.component';
import { FaqDetailEditorComponent } from './components/meal-order-system/faq-detail/faq-detail-editor.component';
import { FaqService } from './services/meal-order/faq.service';
import { VoucherUtilisationsReportManagementComponent } from './components/reports/voucher-utilisation/voucher-utilisations-management.component';
import { VouchersUnassignManagementComponent } from './components/meal-order-system/voucher-unassign/vouchers-unassign-management.component';
import { ClassTransferComponent } from './components/meal-order-system/class/class-transfer/class-transfer.component';
 
 


export const configFactory = (configService: ConfigurationService) => {
  return () => {
    configService.loadConfig();
    configService.loadMenus();
  }
};

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule, MatDialogModule,
    MatInputModule,
    AppRoutingModule,
    //BaseModule,
    ImageZoomModule,
    NgxImageZoomModule.forRoot(),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useClass: TranslateLanguageLoader
      }
    }),
    NgxDocViewerModule,
    NgxDatatableModule,
    ToastaModule.forRoot(),
    TooltipModule.forRoot(),
    PopoverModule.forRoot(),
    BsDropdownModule.forRoot(),
    CarouselModule.forRoot(),
    ModalModule.forRoot(),
    OwlCarousel,
    ChartsModule,
    CommonModule,
    NgbModalModule,
    MaterialModule,
    ColorPickerModule,
    FlatpickrModule.forRoot(),
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory
    }),
    TreeModule,
    NgxMatSelectSearchModule,
    CirclonTreeModule,
    //PdfViewerModule,
    QRCodeModule,
    NgxSummernoteModule
    //QuillModule.forRoot()
    //QuillModule.forRoot({
    //  suppressGlobalRegisterWarning: true
    //})
    //NgSelectModule, FormsModule
    //MOSClassModule, MOSStudentModule
  ],
  declarations: [
    AppComponent, NoNavigationAppComponent, RootAppComponent,
    TemplateEditorComponent, TemplateEditorPreviewDialog, TemplateSaveDialogPreviewDialog,
    PIBDisplayComponent,
    FRSTemplateEditorComponent, FRSTemplateEditorPreviewDialog, FRSTemplateSaveDialogPreviewDialog,
    LoginComponent, ExternalLoginComponent,
    RegisterComponent,
    AttendanceSignInComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    HomeComponent,
    FacilitiesComponent,
    ReservationsComponent, ReservationEditorComponent, ReservationPictureComponent,
    UpcomingEventsComponent, DeviceForApprovalComponent, DashboardAvailableRoomComponent, PIBDeviceStatusListComponent, MeetingRoomDisplayComponent,
    ReservationListComponent, ReservationListEditorComponent,
    CalendarComponent, CalendarHeaderComponent, DayViewSchedulerComponent, CalendarLandingPageComponent,
    BookingComponent, BookingEditorComponent,
    BookingGridComponent, BookingGridEditorComponent,
    CalendarBookingGridComponent, CalendarBookingGridEditorComponent,
    SettingsComponent,
    UsersManagementComponent, UserInfoComponent, UserPreferencesComponent,
    RolesManagementComponent, RoleEditorComponent,
    NotFoundComponent,
    NotificationsViewerComponent,
    SearchBoxComponent,
    StatisticsDemoComponent, TodoDemoComponent, BannerDemoComponent,
    EqualValidator,
    LastElementDirective,
    AutofocusDirective,
    BootstrapTabDirective,
    BootstrapToggleDirective,
    BootstrapSelectDirective,
    BootstrapDatepickerDirective,
    SignageComponentTypeDirective,
    GroupByPipe,
    OrderByPipe,
    DateTimeOnlyPipe, DateOnlyPipe,
    FacilitiesManagementComponent, FacilityEditorComponent,
    FacilityTypesManagementComponent, FacilityTypeEditorComponent,
    SignageComponentsManagementComponent, SignageComponentEditorComponent,
    SignageCompilationsManagementComponent, SignageCompilationEditorComponent,
    SignagePublicationsManagementComponent, SignagePublicationEditorComponent,
    DeviceManagerComponent,
    DeviceEditorComponent, DevicesManagementComponent,
    LocationEditorComponent, LocationsManagementComponent,
    LocationTreeEditorComponent, LocationsTreeManagementComponent,
    InstitutionsManagementComponent, InstitutionEditorComponent,
    DepartmentsManagementComponent, DepartmentEditorComponent,
    ContactGroupsManagementComponent, ContactGroupEditorComponent, CGNewContactEmailEditorComponent,
    FileUploadComponent, ImageUploadComponent, ContactGroupSelectorComponent, UserSelectorComponent,
    UserPhonebookEditorComponent, UserPhonebooksManagementComponent, UserPhonebookSelectorComponent,
    AssetSelectorComponent,
    ReportsReservationListComponent,
    PIBDevicesManagementComponent, PIBDeviceEditorComponent, PIBDeviceManagerComponent,
    ImageUploadsComponent, ImageListsComponent, PlaylistCreatorComponent, PlaylistsManagementComponent, FeedbackComponent, PictureEditorComponent,
    EpaperDevicesManagementComponent, EpaperDeviceEditorComponent, EpaperDeviceManagerComponent, UserVehicleEditorComponent, UserCardIdEditorComponent, UserVehiclesManagementComponent, SignageComponentTypeWrapperComponent, VjsPlayerComponent, FreeHTMLConfiguration, FreeHTML,
    DeviceDisplayWarningComponent,
    ReportContainerComponent,
    KioskSettingsListComponent, KioskSettingsEditComponent,
    VideoConfiguration, Video,
    SignagePlaylistConfiguration, SignagePlaylist,
    ClockConfiguration, Clock,
    EmergencyMessageConfiguration, EmergencyMessage,
    SingleQueueConfiguration, SingleQueue,
    EmployeeNameConfiguration, EmployeeName,
    LocationNameConfiguration, LocationName,
    FreeTextConfiguration, FreeText,
    SingleBookingConfiguration, SingleBooking,
    MultiBookingConfiguration, MultiBooking,
    WebFrameConfiguration, WebFrame,
    MultiQueueConfiguration, MultiQueue, NgInit,
    MissedQueueConfiguration, MissedQueue,
    SignageDisplayComponent,
    TextConfiguration,
    TextDisplay,
    ApplicationSettingEditorComponent, ApplicationSettingsManagementComponent,
    DirectoryListingCategoryEditorComponent, DirectoryListingCategorysManagementComponent,

    MediaExtensionEditorComponent, MediaExtensionsManagementComponent,
    EmployeeDesignationEditorComponent, EmployeeDesignationsManagementComponent,
    EmployeeScheduleEditorComponent, EmployeeScheduleAssignComponent, EmployeeSchedulesManagementComponent,
    EmployeeDataEditorComponent, EmployeeDatasManagementComponent,
    QueueTableMapEditorComponent, QueueTableMapsManagementComponent, QueueTableMapSimulator, NewMultipleBentoAsset, CreateAccountMultiple,
    OccupancyLogEditorComponent, OccupancyLogsManagementComponent,
    DirectoryListingEditorComponent, DirectoryListingsManagementComponent,
    BuildingEditorComponent, BuildingsManagementComponent,
    FloorEditorComponent, FloorsManagementComponent,
    MapEditorComponent, MapsManagementComponent,
    MediasManagementComponent, MediaEditorComponent,
    DeviceDisplayWarningComponent,
    ReportsVehicleLogListComponent,
    ModulesManagementComponent, ModuleEditorComponent,
    WayfinderComponent, WayfinderLandscapeComponent,
    EmsProfilesComponent, EmsProfileEditorComponent,
    SignageDashboardContainerComponent,
    UserGroupsManagementComponent, UserGroupEditorComponent,
    EmsSchedulesComponent, EmsScheduleEditorComponent,
    EmsesManagementComponent, EmsEditorComponent,
    LocationPanelComponent,
    DevicePushMessageDialog,
    MenuPanelComponent, SubMenuPanelComponent,
    ImageViewerComponent,
    AuthLogsManagementComponent, UpDownTimeLogsManagementComponent, DataLogsManagementComponent, DataLogViewerComponent,
    ImageReferenceTypesManagementComponent, ImageReferenceTypeEditorComponent,
    ImageReferenceColorsManagementComponent, ImageReferenceColorEditorComponent,
    SmartRoomSchedulerLogsManagementComponent, SmartRoomSchedulerDetailsViewerComponent,
    AssetTypesManagementComponent, AssetTypeEditorComponent,
    AssetModelsManagementComponent, AssetModelEditorComponent,
    AssetsManagementComponent, AssetEditorComponent, AssetQrCodeComponent,
    ServiceContractsManagementComponent, ServiceContractEditorComponent,
    EmailQueuesManagementComponent,
    DeviceTypesManagementComponent, DeviceTypeEditorComponent,
    ChannelInfosManagementComponent, ChannelInfoEditorComponent,
    StudentsManagementComponent, StudentEditorComponent,
    StudentGroupsManagementComponent, StudentGroupEditorComponent, StudentSelectorComponent, ScanTagComponent,
    ClassBatchesManagementComponent, ClassBatchEditorComponent,
    ClassLevelsManagementComponent, ClassLevelEditorComponent,
    DispenserOutletManagementComponent, DispenserOutletEditorComponent,
    ClassesManagementComponent, ClassEditorComponent,ClassTransferComponent,
    StaffTypesManagementComponent, StaffTypeEditorComponent,
    StaffsManagementComponent, StaffEditorComponent,
    RestrictionTypesManagementComponent, RestrictionTypeEditorComponent,
    RestrictionsManagementComponent, RestrictionEditorComponent,
    DishTypeEditorComponent, DishTypesManagementComponent,
    CuisineEditorComponent, CuisinesManagementComponent,
    CatererInfoEditorComponent, CatererInfosManagementComponent,
    OutletEditorComponent, OutletsManagementComponent, OutletProfileEditorComponent, OutletProfilesManagementComponent,
    BentoAssetEditorComponent, BentoAssetsManagementComponent, DishingProcesssManagementComponent, PackingProcesssManagementComponent, DoProcesssManagementComponent,
    CartonAssetEditorComponent, CartonAssetsManagementComponent, BentoUsageManagementComponent, CartonAssetQrCodeComponent,
    BentoBoxTypeEditorComponent, BentoBoxTypesManagementComponent,
    CartonTypeEditorComponent, CartonTypesManagementComponent,
    DeliveryOrderEditorComponent, DeliveryOrdersManagementComponent,
    DeliveryOrderNewEditorComponent, DeliveryOrdersNewManagementComponent,
    TrackingStatusEditorComponent, TrackingStatussManagementComponent,
    StoreInfoEditorComponent, StoreInfosManagementComponent,
    MealTypeEditorComponent, MealTypesManagementComponent,
    MealPeriodEditorComponent, MealPeriodsManagementComponent,
    DishEditorComponent, DishesManagementComponent, DishSelectorComponent, DishPreviewComponent, TokenOrderEditorComponent, TokenOrderBulkEditorComponent, TokenOrdersBulkManagementComponent,
    MenuEditorComponent, MenusManagementComponent, TokenOrdersManagementComponent, MenuCycleEditorComponent, MenuCyclesManagementComponent, MealAllocationssManagementComponent, PrintAllocationComponent,
    CatererSelectorComponent, CatererApprovalComponent, DOSelectorComponent, ReceiveDOComponent, InventorySelectorComponent, InventoryDetailComponent, StoreSelectorComponent, StoreSelectorDetailsComponent,
    DriverEditorComponent, DriversManagementComponent, RouteEditorComponent, RoutesManagementComponent, OrderReportComponent, OrderMealSummaryComponent, PrintLabelComponent, PackingAllocationssManagementComponent, PackingAllocationEditorComponent,
    InterestGroupEditorComponent, InterestGroupsManagementComponent, MealSessionEditorComponent, OrderDeliveryReportComponent, PackingTagsManagementComponent, PackingTagEditorComponent,
    MenuCycleCalendarComponent, ViewMenuCycleMenuComponent, OutletMenuCycleCalendarComponent, OutletViewMenuCycleMenuComponent,
    OutletClassRosterEditorComponent, MenuDishSelectorEditorComponent, OutletClassRostersManagementComponent, 
    StudentMenuCycleCalendarComponent, TokenOrderPaymentComponent, TokenOrderPaymentThankyouComponent ,
    DishCycleEditorComponent, DishCyclesManagementComponent, DishCycleCalendarComponent, ViewDishCycleMenuComponent, OutletDishCycleCalendarComponent, OutletViewDishCycleMenuComponent,
    PaymentTypesManagementComponent, PaymentTypeEditorComponent,
    TransactionFeesManagementComponent, TransactionFeeEditorComponent,
    VoucherTypesManagementComponent, VoucherTypeEditorComponent, VouchersManagementComponent,VouchersUnassignManagementComponent, VoucherEditorComponent,
    WaiversManagementComponent, WaiverEditorComponent,
    ContactUsDetailsManagementComponent, ContactUsDetailEditorComponent,
    ContactUsSubjectEditorComponent, ContactUsSubjectsManagementComponent,
    EmailTemplatesManagementComponent, EmailTemplateEditorComponent,
    InterestGroupEmailComponent,
    NotificationEventEditorComponent, NotificationEventsManagementComponent,
    StudentNotificationDialogComponent,
    CancelOrderRequestsManagementComponent, CancelOrderRequestEditorComponent,
    OrderLogsManagementComponent, ExternalLoginLogsManagementComponent,
    UserChangePasswordComponent, FasMealOrderSummaryComponent,
    UserAccountsReportManagementComponent,
    MenuGroupEditorComponent, MenuGroupsManagementComponent,
    OrderCancellationsManagementComponent,
    UserRolesReportManagementComponent,
    OrderCancellationReportManagementComponent,
    NotificationSettingComponent, MealPlanSummaryComponent,
    MultiFactorLoginComponent,
    OrderPortalContentEditorComponent, OrderPortalContentsManagementComponent,
    StudentGroupOrderSummaryComponent,
    UserActivityLogsManagementComponent, UserActivityLogViewerComponent,
    OutletTermsManagementComponent, OutletTermEditorComponent,
    StudentOrderManagementComponent, StatusDropdownComponent, StudentOrderDetailComponent,
    MatStatusDropdownComponent,
    StudentEditorComponent, StudentOrderEditorComponent,
    DefaultPipe,
    OrderCollectionLogsManagementComponent,
    PaymentsReportManagementComponent,

    FaqSubjectsManagementComponent, FaqSubjectEditorComponent,
    FaqDetailsManagementComponent, FaqDetailEditorComponent,
    VoucherUtilisationsReportManagementComponent
  ],
  providers: [
    { provide: 'BASE_URL', useFactory: getBaseUrl },
    { provide: ErrorHandler, useClass: AppErrorHandler },
    {
      provide: MatDialogRef,
      useValue: {}
    }, {
      provide: MAT_DIALOG_DATA,
      useValue: {} // Add any data you wish to test if it is passed/used correctly
    },
    { provide: MAT_LABEL_GLOBAL_OPTIONS, useValue: { float: 'always' } },
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: LOCALE_ID, useValue: "en-SG" },
    {
      provide: CalendarUtils,
      useClass: DayViewSchedulerCalendarUtils
    },
    {
      provide: APP_INITIALIZER,
      useFactory: configFactory,
      deps: [ConfigurationService],
      multi: true
    },
    FRSHubConnections,
    ShapeService,
    TextNodeService,
    AlertService,
    ConfigurationService,
    AppTitleService,
    AppTranslationService,
    NotificationService,
    NotificationEndpoint,
    AccountService,
    AccountEndpoint,
    LocalStoreManager,
    EndpointFactory,
    FacilityService,
    FacilityTypeService,
    SignageComponentService,
    SignageComponentTypeService,
    SignageCompilationService,
    SignagePublicationService,
    CommonEndpoint,
    ReservationService,
    BookingService,
    DeviceService,
    ChannelInfoService,
    InstitutionService,
    DepartmentService,
    LocationService,
    FileService,
    ImageFileService,
    PlaylistService,
    KioskSettingsService,
    DashboardService,
    ContactGroupService,
    PIBService,
    ApplicationSettingService,
    DirectoryListingCategoryService,
    MediaExtensionService,
    EmployeeDesignationService,
    EmployeeScheduleService,
    EmployeeDataService,
    OccupancyLogService,
    QueueTableMapService,
    BuildingService,
    FloorService,
    MapService,
    DirectoryListingService,
    MediaService,
    EpaperService,
    EpaperService,
    ModuleService,
    UserGroupService,
    EmsProfileService,
    EmsScheduleService,
    EmsService,
    AuditService,
    ImageReferenceTypeService,
    ImageReferenceColorService,
    SmartRoomSchedulerService,
    AssetService,
    ServiceContractService,
    EmailQueueService,
    ClassService, StudentService, UserService, StaffService, DeliveryService, RestrictionService,
    MealService, DishService, MenuService, PaymentService, EmailService,
    OrderService,
    ReportService,
    OrderPortalService,
    FaqService
  ],
  bootstrap: [RootAppComponent],
  exports: [CalendarComponent, CalendarHeaderComponent, BookingComponent, CalendarLandingPageComponent, MenuPanelComponent, SubMenuPanelComponent],
  entryComponents: [LocationEditorComponent, LocationsManagementComponent, LocationTreeEditorComponent, LocationsTreeManagementComponent, BookingEditorComponent, BookingComponent,
    DeviceEditorComponent, DevicesManagementComponent, DeviceManagerComponent, BookingGridEditorComponent, BookingGridComponent,
    InstitutionEditorComponent, InstitutionsManagementComponent,
    FacilityTypeEditorComponent, FacilityTypesManagementComponent,
    SignageComponentEditorComponent, SignageComponentsManagementComponent,
    DepartmentEditorComponent, DepartmentsManagementComponent,
    ContactGroupEditorComponent, ContactGroupsManagementComponent, CGNewContactEmailEditorComponent,
    FacilityEditorComponent, FacilitiesManagementComponent, CalendarBookingGridEditorComponent, CalendarBookingGridComponent, ContactGroupSelectorComponent, UserSelectorComponent, AssetSelectorComponent,
    UserPhonebookEditorComponent, UserPhonebooksManagementComponent, UserPhonebookSelectorComponent, TemplateEditorPreviewDialog, TemplateSaveDialogPreviewDialog, PIBDeviceEditorComponent,
    ImageUploadsComponent, ImageListsComponent, PlaylistCreatorComponent, PlaylistsManagementComponent, FRSTemplateEditorPreviewDialog, FRSTemplateSaveDialogPreviewDialog,
    ApplicationSettingEditorComponent, ApplicationSettingsManagementComponent,
    MediasManagementComponent, MediaEditorComponent,
    DirectoryListingEditorComponent, DirectoryListingsManagementComponent,
    DirectoryListingCategoryEditorComponent, DirectoryListingCategorysManagementComponent,
    MediaExtensionEditorComponent, MediaExtensionsManagementComponent,
    EmployeeDesignationEditorComponent, EmployeeDesignationsManagementComponent,
    EmployeeScheduleEditorComponent, EmployeeScheduleAssignComponent, EmployeeSchedulesManagementComponent,
    EmployeeDataEditorComponent, EmployeeDatasManagementComponent,
    OccupancyLogEditorComponent, OccupancyLogsManagementComponent,
    QueueTableMapEditorComponent, QueueTableMapsManagementComponent, QueueTableMapSimulator, NewMultipleBentoAsset, CreateAccountMultiple,
    BuildingEditorComponent, BuildingsManagementComponent,
    FloorEditorComponent, FloorsManagementComponent,
    MapEditorComponent, MapsManagementComponent,
    EpaperDeviceEditorComponent, PIBDisplayComponent,
    UserVehicleEditorComponent, UserCardIdEditorComponent, FreeHTMLConfiguration, FreeHTML, PictureEditorComponent,
    VideoConfiguration, Video,
    SignagePlaylistConfiguration, SignagePlaylist,
    KioskSettingsEditComponent, KioskSettingsListComponent,
    ClockConfiguration, Clock,
    SingleQueueConfiguration, SingleQueue,
    EmployeeNameConfiguration, EmployeeName,
    LocationNameConfiguration, LocationName,
    FreeTextConfiguration, FreeText,
    SingleBookingConfiguration, SingleBooking,
    MultiBookingConfiguration, MultiBooking,
    WebFrameConfiguration, WebFrame,
    MultiQueueConfiguration, MultiQueue,
    MissedQueueConfiguration, MissedQueue,
    EmsesManagementComponent, EmsEditorComponent,

    EmergencyMessageConfiguration, EmergencyMessage,
    SignageDisplayComponent,
    UserInfoComponent,
    ModuleEditorComponent,
    SignageDashboardContainerComponent,
    UserGroupsManagementComponent, UserGroupEditorComponent,
    LocationPanelComponent, DevicePushMessageDialog, ImageViewerComponent,
    AuthLogsManagementComponent, UpDownTimeLogsManagementComponent, DataLogsManagementComponent, DataLogViewerComponent,
    ImageReferenceTypeEditorComponent, ImageReferenceColorEditorComponent,
    SmartRoomSchedulerDetailsViewerComponent,
    AssetTypeEditorComponent,
    AssetModelEditorComponent,
    AssetEditorComponent,
    AssetQrCodeComponent, CartonAssetQrCodeComponent,
    ServiceContractEditorComponent,
    DeviceTypeEditorComponent,
    ChannelInfoEditorComponent,
    ClassBatchEditorComponent, ClassLevelEditorComponent, ClassEditorComponent,ClassTransferComponent, StudentEditorComponent, StaffTypeEditorComponent, StaffEditorComponent, DispenserOutletEditorComponent,
    RestrictionTypeEditorComponent, RestrictionEditorComponent, StudentGroupEditorComponent, StudentSelectorComponent, BentoUsageManagementComponent, ScanTagComponent,
    MealTypeEditorComponent, DishTypeEditorComponent, CatererInfoEditorComponent, BentoBoxTypeEditorComponent, CartonTypeEditorComponent, DeliveryOrderEditorComponent, DeliveryOrderNewEditorComponent, TrackingStatusEditorComponent, StoreInfoEditorComponent, BentoAssetEditorComponent, CartonAssetEditorComponent, MealPeriodEditorComponent,
    DishEditorComponent, DishSelectorComponent, MenuEditorComponent, MenuCycleEditorComponent, TokenOrderEditorComponent, TokenOrdersManagementComponent, TokenOrderBulkEditorComponent, TokenOrdersBulkManagementComponent, MealAllocationssManagementComponent, PrintAllocationComponent,
    OutletEditorComponent, OutletProfileEditorComponent, CatererSelectorComponent, DOSelectorComponent, ReceiveDOComponent, InventorySelectorComponent, InventoryDetailComponent, CatererApprovalComponent, StoreSelectorComponent, DriverEditorComponent, RouteEditorComponent, StoreSelectorDetailsComponent,
    CuisineEditorComponent, InterestGroupEditorComponent, MealSessionEditorComponent, OrderReportComponent, OrderMealSummaryComponent, PrintLabelComponent, PackingAllocationssManagementComponent, PackingAllocationEditorComponent,
    ViewMenuCycleMenuComponent, OutletViewMenuCycleMenuComponent, TokenOrderPaymentComponent, OrderDeliveryReportComponent, PackingTagsManagementComponent, PackingTagEditorComponent,
    OutletClassRosterEditorComponent, MenuDishSelectorEditorComponent, StudentMenuCycleCalendarComponent, OutletClassRostersManagementComponent,
    DishCycleEditorComponent, ViewDishCycleMenuComponent, OutletDishCycleCalendarComponent, OutletViewDishCycleMenuComponent,
    PaymentTypeEditorComponent,
    TransactionFeeEditorComponent,
    VoucherTypeEditorComponent, VoucherEditorComponent, WaiverEditorComponent,
    ContactUsDetailEditorComponent, ContactUsSubjectEditorComponent, EmailTemplateEditorComponent,
    InterestGroupEmailComponent,
    NotificationEventEditorComponent, StudentNotificationDialogComponent, CancelOrderRequestEditorComponent,
    FasMealOrderSummaryComponent, MenuGroupEditorComponent, MenuGroupsManagementComponent,
    OrderCancellationsManagementComponent,
    UserRolesReportManagementComponent,
    OrderCancellationReportManagementComponent,
    NotificationSettingComponent, MealPlanSummaryComponent,
    MultiFactorLoginComponent,
    OrderPortalContentEditorComponent, OrderPortalContentsManagementComponent,
    StudentGroupOrderSummaryComponent,
    UserActivityLogsManagementComponent, UserActivityLogViewerComponent,
    OutletTermsManagementComponent, OutletTermEditorComponent,
    MatStatusDropdownComponent,
    StudentOrderManagementComponent, StatusDropdownComponent, StudentOrderDetailComponent,
    StudentEditorComponent, StudentOrderEditorComponent,
    OrderCollectionLogsManagementComponent,
    PaymentsReportManagementComponent,
    FaqSubjectsManagementComponent, FaqSubjectEditorComponent,
    FaqDetailsManagementComponent, FaqDetailEditorComponent,
    VoucherUtilisationsReportManagementComponent
  ],
})
export class AppModule {
}




export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}
