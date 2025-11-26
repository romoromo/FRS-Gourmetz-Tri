import { NgModule } from '@angular/core';
import { Routes, RouterModule, Router, ActivatedRoute } from '@angular/router';

import { LoginComponent } from "./components/login/login.component";
import { HomeComponent } from "./components/home/home.component";
import { ReservationsComponent } from "./components/reservations/reservations.component";
import { SettingsComponent } from "./components/settings/settings.component";
import { NotFoundComponent } from "./components/not-found/not-found.component";
import { FacilitiesComponent } from "./components/facilities/facilities.component";
import { ImageUploadsComponent } from "./components/imageUpload/image-upload.component";
import { CalendarComponent } from "./components/calendar/calendar.component";
import { BookingComponent } from "./components/calendar/booking/booking.component";
import { AuthService } from './services/auth.service';
import { AuthGuard } from './services/auth-guard.service';
import { DeviceManagerComponent } from './components/device-manager/device-manager.component';
import { BookingGridComponent } from './components/calendar/booking-grid/booking-grid.component';
import { RegisterComponent } from './components/register/register.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { ExternalLoginComponent } from './components/login/external-login.component';
import { AttendanceSignInComponent } from './components/attendance-signin/attendance-signin.component';
import { UserPhonebooksManagementComponent } from './components/controls/userphonebook/userphonebooks-management.component';
import { ContactGroupsManagementComponent } from './components/controls/contactgroup/contactgroups-management.component';
import { ReportsReservationListComponent } from './components/reports/reservations/reservations.component';
import { TemplateEditorComponent } from './components/template-editor/template-editor.component';
import { PIBDevicesManagementComponent } from './components/pib-device-manager/pib-unknown-device/pib-devices-management.component';
import { TemplateEditorPreviewDialog } from './components/template-editor/preview/preview.component';
import { AppComponent } from './components/app.component';
import { NoNavigationAppComponent } from './components/no_navigation_app.component';
import { FeedbackComponent } from './components/feedback/feedback.component';
import { FRSTemplateEditorComponent } from './components/frs-template-editor/frs-template-editor.component';
import { PIBDisplayComponent } from './components/template-editor/pibdisplay/pibdisplay.component';
import { UserVehiclesManagementComponent } from './components/controls/uservehicle/uservehicle-management.component';
import { SignageComponentsManagementComponent } from './components/signage/signage-component/signage-components-management.component';
import { SignageCompilationsManagementComponent } from './components/signage/signage-compilation/signage-compilations-management.component';
import { MeetingRoomDisplayComponent } from './components/meeting-room/meeting-room.component';
import { DeviceDisplayWarningComponent } from './components/device-warning/device-warning.component';
import { ReportContainerComponent } from './components/reports/report-container/report-container.component';
import { SignagePublicationsManagementComponent } from './components/signage/signage-publication/signage-publications-management.component';
import { SignageDisplayComponent } from './components/signage/signage-display/signage-display.component';
import { QueueTableMapsManagementComponent } from './components/queue-table-map/queue-table-maps-management.component';
import { WayfinderComponent } from './components/wayfinder-view/wayfinder-view.component'
import { WayfinderLandscapeComponent } from './components/wayfinder-landscape-view/wayfinder-landscape-view.component'
import { CalendarLandingPageComponent } from './components/calendar/landing-page/landing-page.component';
import { DevicesManagementComponent } from './components/device-manager/unknown-device/devices-management.component';
import { UsersManagementComponent } from './components/controls/users-management.component';
import { RolesManagementComponent } from './components/controls/roles-management.component';
import { UserGroupsManagementComponent } from './components/controls/user-group/user-groups-management.component';
import { DepartmentsManagementComponent } from './components/controls/department/departments-management.component';
import { LocationsTreeManagementComponent } from './components/controls/locations-tree/locations-tree-management.component';
import { InstitutionsManagementComponent } from './components/controls/institution/institutions-management.component';
import { BuildingsManagementComponent } from './components/building/buildings-management.component';
import { ApplicationSettingsManagementComponent } from './components/application-setting/application-settings-management.component';
import { FacilitiesManagementComponent } from './components/controls/facilities/facilities-management.component';
import { FacilityTypesManagementComponent } from './components/controls/facility-type/facility-types-management.component';
import { EmsesManagementComponent } from './components/controls/ems/emses-management.component';
import { EmsProfilesComponent } from './components/ems-profile/ems-profiles.component';
import { EmployeeSchedulesManagementComponent } from './components/employee-schedule/employee-schedules-management.component';
import { EmployeeDesignationsManagementComponent } from './components/employee-designation/employee-designations-management.component';
import { EmployeeDatasManagementComponent } from './components/employee-data/employee-datas-management.component';
import { MediasManagementComponent } from './components/controls/media/medias-management.component';
import { ImageListsComponent } from './components/imageUpload/image-list.component';
import { PlaylistsManagementComponent } from './components/playlistManagement/playlists-management.component';
import { DirectoryListingCategorysManagementComponent } from './components/directory-listing-category/directory-listing-categorys-management.component';
import { DirectoryListingsManagementComponent } from './components/directory-listing/directory-listings-management.component';
import { FloorsManagementComponent } from './components/floor/floors-management.component';
import { MapsManagementComponent } from './components/map/maps-management.component';
import { OccupancyLogsManagementComponent } from './components/occupancy-log/occupancy-logs-management.component';
import { ModulesManagementComponent } from './components/controls/module/modules-management.component';
import { MediaExtensionsManagementComponent } from './components/media-extension/media-extensions-management.component';
import { AuthLogsManagementComponent } from './components/audit/auth-log/auth-logs-management.component';
import { DataLogsManagementComponent } from './components/audit/data-log/data-logs-management.component';
import { ImageReferenceTypesManagementComponent } from './components/controls/image-reference-type/image-reference-types-management.component';
import { ImageReferenceColorsManagementComponent } from './components/controls/image-reference-color/image-reference-colors-management.component';
import { UserInfoComponent } from './components/controls/user-info.component';
import { SmartRoomSchedulerLogsManagementComponent } from './components/smartroom-scheduler-log/smartroom-scheduler-logs-management.component';
import { AssetTypesManagementComponent } from './components/controls/asset-type/asset-types-management.component';
import { AssetModelsManagementComponent } from './components/controls/asset-model/asset-models-management.component';
import { ServiceContractsManagementComponent } from './components/service-contract/service-contracts-management.component';
import { AssetsManagementComponent } from './components/controls/asset/assets-management.component';
import { EmsSchedulesComponent } from './components/ems-schedule/ems-schedules.component';
import { EmailQueuesManagementComponent } from './components/controls/email-queue/email-queues-management.component';
import { UpDownTimeLogsManagementComponent } from './components/audit/updowntime-log/updowntime-logs-management.component';
import { KioskSettingsListComponent } from './components/kioskSettings/kiosk-settings-list.component';
import { DeviceTypesManagementComponent } from './components/controls/device-type/device-types-management.component';
import { ClassLevelsManagementComponent } from './components/meal-order-system/class-level/class-levels-management.component';
import { DispenserOutletManagementComponent } from './components/meal-order-system/dispenser-outlet/dispenser-outlet-management.component';
import { ClassesManagementComponent } from './components/meal-order-system/class/classes-management.component';
import { StudentsManagementComponent } from './components/meal-order-system/student/students-management.component';
import { StudentGroupsManagementComponent } from './components/meal-order-system/student-group/student-groups-management.component';
import { StaffsManagementComponent } from './components/meal-order-system/staff/staffs-management.component';
import { StaffTypesManagementComponent } from './components/meal-order-system/staff-type/staff-types-management.component';
import { RestrictionTypesManagementComponent } from './components/meal-order-system/restriction-type/restriction-types-management.component';
import { RestrictionsManagementComponent } from './components/meal-order-system/restriction/restrictions-management.component';
import { DishTypesManagementComponent } from './components/meal-order-system/dish-type/dish-types-management.component';
import { MealTypesManagementComponent } from './components/meal-order-system/meal-type/meal-types-management.component';
import { MealPeriodsManagementComponent } from './components/meal-order-system/meal-period/meal-periods-management.component';
import { DishesManagementComponent } from './components/meal-order-system/dishes/dishes-management.component';
import { MenusManagementComponent } from './components/meal-order-system/menus/menus-management.component';
import { MenuCyclesManagementComponent } from './components/meal-order-system/menu-cycle/menu-cycles-management.component';
import { CatererInfosManagementComponent } from './components/meal-order-system/caterer-info/caterer-infos-management.component';
import { BentoBoxTypesManagementComponent } from './components/meal-order-system/bento-box-type/bento-box-types-management.component';
import { BentoAssetsManagementComponent } from './components/meal-order-system/bento-asset/bento-assets-management.component';
import { BentoUsageManagementComponent } from './components/meal-order-system/bento-usage/bento-usage-management.component';
import { CartonTypesManagementComponent } from './components/meal-order-system/carton-type/carton-types-management.component';
import { CartonAssetsManagementComponent } from './components/meal-order-system/carton-asset/carton-assets-management.component';
import { TrackingStatussManagementComponent } from './components/meal-order-system/tracking-status/tracking-statuss-management.component';
import { DeliveryOrdersManagementComponent } from './components/meal-order-system/delivery-order/delivery-orders-management.component';
import { DeliveryOrdersNewManagementComponent } from './components/meal-order-system/delivery-order-new/delivery-orders-new-management.component';
import { DishingProcesssManagementComponent } from './components/meal-order-system/dishing-process/dishing-processs-management.component';
import { ClassBatchesManagementComponent } from './components/meal-order-system/class-batch/class-batches-management.component';
import { DishPreviewComponent } from './components/meal-order-system/dishes/dish-preview/dish-preview.component';
import { OutletsManagementComponent } from './components/meal-order-system/outlet/outlets-management.component';
import { OutletProfilesManagementComponent } from './components/meal-order-system/outlet-profile/outlet-profiles-management.component';
import { CuisinesManagementComponent } from './components/meal-order-system/cuisine/cuisines-management.component';
import { TokenOrdersManagementComponent } from './components/meal-order-system/token-order/token-orders-management.component';
import { StoreInfosManagementComponent } from './components/meal-order-system/store-info/store-infos-management.component';
import { DoProcesssManagementComponent } from './components/meal-order-system/do-process/do-processs-management.component';
import { PackingProcesssManagementComponent } from './components/meal-order-system/packing-process/packing-processs-management.component';
import { InterestGroupsManagementComponent } from './components/meal-order-system/interest-group/interest-groups-management.component';
import { DriversManagementComponent } from './components/meal-order-system/driver/drivers-management.component';
import { RoutesManagementComponent } from './components/meal-order-system/route/routes-management.component';
import { MenuCycleCalendar } from './models/meal-order/menu-cycle-calendar.model';
import { MenuCycleCalendarComponent } from './components/meal-order-system/caterer-info/caterer-calendar/caterer-calendar.component';
import { OutletMenuCycleCalendarComponent } from './components/meal-order-system/outlet/outlet-calendar/outlet-calendar.component';
import { StudentMenuCycleCalendarComponent } from './components/meal-order-system/student/student-calendar/student-calendar.component';
import { DishCyclesManagementComponent } from './components/meal-order-system/dish-cycle/dish-cycles-management.component';
import { DishCycleCalendarComponent } from './components/meal-order-system/caterer-info/caterer-dish-calendar/caterer-dish-calendar.component';
import { OutletDishCycleCalendarComponent } from './components/meal-order-system/outlet/dish-calendar/dish-calendar.component';
import { ChannelInfosManagementComponent } from './components/controls/channel-info/channel-infos-management.component';

import { TokenOrderPaymentThankyouComponent } from './components/meal-order-system/token-order-payment/token-order-payment-thankyou/token-order-payment-thankyou.component';
import { TransactionFeesManagementComponent } from './components/meal-order-system/transaction-fee/transaction-fees-management.component';
import { PaymentTypesManagementComponent } from './components/meal-order-system/payment-type/payment-types-management.component';
import { VouchersManagementComponent } from './components/meal-order-system/voucher/vouchers-management.component';
import { VoucherTypesManagementComponent } from './components/meal-order-system/voucher-type/voucher-types-management.component';
import { WaiversManagementComponent } from './components/meal-order-system/waiver/waivers-management.component';
import { ContactUsDetailsManagementComponent } from './components/meal-order-system/contact-us-detail/contact-us-details-management.component';
import { ContactUsSubjectsManagementComponent } from './components/meal-order-system/contact-us-subject/contact-us-subjects-management.component';
import { EmailTemplatesManagementComponent } from './components/meal-order-system/email-template/email-templates-management.component';
import { NotificationEventsManagementComponent } from './components/meal-order-system/notification-event/notification-events-management.component';
import { CancelOrderRequestsManagementComponent } from './components/meal-order-system/cancel-order-request/cancel-order-requests-management.component';
import { ExternalLoginLogsManagementComponent } from './components/audit/external-login-log/external-login-logs-management.component';
import { OrderLogsManagementComponent } from './components/audit/order-log/order-logs-management.component';
import { UserChangePasswordComponent } from './components/user-change-password/user-change-password.component';
import { UserAccountsReportManagementComponent } from './components/reports/user-account/user-accounts-management.component';
import { MenuGroupsManagementComponent } from './components/meal-order-system/menu-group/menu-groups-management.component';
import { OrderCancellationsManagementComponent } from './components/meal-order-system/order-cancellation/order-cancellations-management.component';
import { UserRolesReportManagementComponent } from './components/reports/user-role/user-roles-management.component';
import { OrderCancellationReportManagementComponent } from './components/reports/order-cancellation/order-cancellations-management.component';
import { NotificationSettingComponent } from './components/notification-setting/notification-settings.component';
import { MultiFactorLoginComponent } from './components/multi-factor-login/multi-factor-login.component';
import { OrderPortalContent } from './models/meal-order/order-portal-content.model';
import { UserActivityLogsManagementComponent } from './components/audit/user-activity-log/user-activity-logs-management.component';
import { OutletTermsManagementComponent } from './components/meal-order-system/outlet/outlet-term/outlet-terms-management.component';
import { OrderCollectionLogsManagementComponent } from './components/meal-order-system/student-group/order-log/order-logs-management.component';
import { PaymentsReportManagementComponent } from './components/reports/payment/payments-management.component';
import { FaqSubjectsManagementComponent } from './components/meal-order-system/faq-subject/faq-subjects-management.component';
import { FaqDetailsManagementComponent } from './components/meal-order-system/faq-detail/faq-details-management.component';
import { VoucherUtilisationsReportManagementComponent } from './components/reports/voucher-utilisation/voucher-utilisations-management.component';
import { VouchersUnassignManagementComponent } from './components/meal-order-system/voucher-unassign/vouchers-unassign-management.component';
import { WalletTransactionLogManagementComponent } from './components/meal-order-system/wallet-transaction-log/wallet-transaction-log-management.component';
import { PlcManagementComponent } from './components/meal-order-system/outlet/plc/plc-management.component';
import { CatererAsssetTypeManagementComponent } from './components/meal-order-system/asset-type/caterer-asset-type-management.component';
import { CatererAssetsManagementComponent } from './components/meal-order-system/caterer-asset/caterer-asset-management.component';
import { title } from 'process';


const routes: Routes = [
  //{ path: "", component: HomeComponent, canActivate: [AuthGuard], data: { title: "Home" } },
  //{ path: "login", component: LoginComponent, data: { title: "Login" } },
  //{ path: "register", component: RegisterComponent, data: { title: "Register" } },
  //{ path: "resetpassword", component: ResetPasswordComponent, data: { title: "ResetPassword" } },
  //{ path: "forgotpassword", component: ForgotPasswordComponent, data: { title: "Forgot Password" } },
  //{ path: "reservations", component: ReservationsComponent, canActivate: [AuthGuard], data: { title: "Reservations" } },
  //{ path: "calendar", component: CalendarComponent, canActivate: [AuthGuard], data: { title: "Calendar" } },
  //{ path: "bookings", component: BookingGridComponent, canActivate: [AuthGuard], data: { title: "Book" } },
  //{ path: "facilities", component: FacilitiesComponent, canActivate: [AuthGuard], data: { title: "Facilities" } },
  //{ path: "settings", component: SettingsComponent, canActivate: [AuthGuard], data: { title: "Settings" } },
  //{ path: "devices", component: DeviceManagerComponent, canActivate: [AuthGuard], data: { title: "Devices" } },
  //{ path: "contactgroups", component: ContactGroupsManagementComponent, canActivate: [AuthGuard], data: { title: "Contact Groups" } },
  //{ path: "phonebooks", component: UserPhonebooksManagementComponent, canActivate: [AuthGuard], data: { title: "My Contacts" } },
  //{ path: "reports", component: ReportsReservationListComponent, canActivate: [AuthGuard], data: { title: "Reports" } },
  //{ path: "templateeditor", component: TemplateEditorComponent, canActivate: [AuthGuard], data: { title: "Template Editor" } },
  //{ path: "pibdevices", component: PIBDevicesManagementComponent, data: { title: "PIB Devices" } },
  //{ path: "home", redirectTo: "/", pathMatch: "full" },
  ////{ path: "signinwithgoogle", redirectTo: "/authorization/SignInWithGoogle", pathMatch: "full" },
  //{ path: "externallogin", component: ExternalLoginComponent, data: { title: "Login" } },
  ////{ path: "attendance", redirectTo: "/authorization/AttendanceSignIn", pathMatch: "full" },
  //{ path: "attendance", component: AttendanceSignInComponent, data: { title: "Attendance" } },
  //{ path: "pibdevicetemplate", component: TemplateEditorPreviewDialog, canActivate: [AuthGuard], data: { title: "PIB Display" } },
  ////{ path: "**", component: NotFoundComponent, data: { title: "Page Not Found" } }

  //-------------------------
  //Site routes goes here 
  {
    path: '',
    component: AppComponent,
    children: [
      { path: "", component: HomeComponent, canActivate: [AuthGuard], data: { title: "Home" } },
      
      
      { path: "reservations", component: ReservationsComponent, canActivate: [AuthGuard], data: { title: "Reservations" } },
      { path: "calendar", component: CalendarLandingPageComponent, canActivate: [AuthGuard], data: { title: "Calendar" } },
      { path: "bookings", component: BookingGridComponent, canActivate: [AuthGuard], data: { title: "Book" } },
      { path: "facilities", component: FacilitiesComponent, canActivate: [AuthGuard], data: { title: "Facilities" } },
      { path: "signagecomponents", component: SignageComponentsManagementComponent, canActivate: [AuthGuard], data: { title: "Signage Components" } },
      { path: "signagecompilations", component: SignageCompilationsManagementComponent, canActivate: [AuthGuard], data: { title: "Signage Compilations" } },
      { path: "signagepublications", component: SignagePublicationsManagementComponent, canActivate: [AuthGuard], data: { title: "Signage Publications" } },
      { path: "settings", component: SettingsComponent, canActivate: [AuthGuard], data: { title: "Settings" } },
      { path: "devices", component: DeviceManagerComponent, canActivate: [AuthGuard], data: { title: "Devices" } },
      { path: "devices/:type", component: DevicesManagementComponent, canActivate: [AuthGuard], data: { title: "Devices" } },
      { path: "uservehicles", component: UserVehiclesManagementComponent, canActivate: [AuthGuard], data: { title: "Vehicles" } },
      { path: "contactgroups", component: ContactGroupsManagementComponent, canActivate: [AuthGuard], data: { title: "Contact Groups" } },
      { path: "phonebooks", component: UserPhonebooksManagementComponent, canActivate: [AuthGuard], data: { title: "My Contacts" } },
      { path: "reports", component: ReportContainerComponent, canActivate: [AuthGuard], data: { title: "Reports" } },
      { path: "templateeditor", component: TemplateEditorComponent, canActivate: [AuthGuard], data: { title: "Template Editor" } },
      { path: "pibdevices", component: PIBDevicesManagementComponent, data: { title: "PIB Devices" } },
      { path: "queuemonitor", component: QueueTableMapsManagementComponent, data: { title: "Queue Monitor" } },
      { path: "frstemplateeditor", component: FRSTemplateEditorComponent, canActivate: [AuthGuard], data: { title: "FRS Template Editor" } },
      { path: "home", redirectTo: "/", pathMatch: "full" },
      { path: "imguploads", component: ImageUploadsComponent, canActivate: [AuthGuard], data: { title: "Image Uploads" } },
      { path: "users", component: UsersManagementComponent, canActivate: [AuthGuard], data: { title: "System Users" } },
      { path: "roles", component: RolesManagementComponent, canActivate: [AuthGuard], data: { title: "System Roles" } },
      { path: "usergroups", component: UserGroupsManagementComponent, canActivate: [AuthGuard], data: { title: "System User Groups" } },
      { path: "locations", component: LocationsTreeManagementComponent, canActivate: [AuthGuard], data: { title: "Locations" } },
      { path: "institutions", component: InstitutionsManagementComponent, canActivate: [AuthGuard], data: { title: "Institutions" } },
      { path: "departments", component: DepartmentsManagementComponent, canActivate: [AuthGuard], data: { title: "Departments" } },
      { path: "buildings", component: BuildingsManagementComponent, canActivate: [AuthGuard], data: { title: "Buildings" } },
      { path: "kiosksettings", component: KioskSettingsListComponent, canActivate: [AuthGuard], data: { title: "Kiosk Settings" } },
      { path: "applicationsettings", component: ApplicationSettingsManagementComponent, canActivate: [AuthGuard], data: { title: "Application Settings" } },
      { path: "equipments", component: FacilitiesManagementComponent, canActivate: [AuthGuard], data: { title: "Equipments" } },
      { path: "facilitytypes", component: FacilityTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Equipment Types" } },
      { path: "ems", component: EmsesManagementComponent, canActivate: [AuthGuard], data: { title: "EMS" } },
      { path: "emsprofile", component: EmsProfilesComponent, canActivate: [AuthGuard], data: { title: "EMS Profile" } },
      { path: "emsschedule", component: EmsSchedulesComponent, canActivate: [AuthGuard], data: { title: "EMS Schedule" } },
      { path: "employeedesignation", component: EmployeeDesignationsManagementComponent, canActivate: [AuthGuard], data: { title: "Employee Designation" } },
      { path: "employee", component: EmployeeDatasManagementComponent, canActivate: [AuthGuard], data: { title: "Employee" } },
      { path: "mediagroup", component: MediasManagementComponent, canActivate: [AuthGuard], data: { title: "Media Group" } },
      { path: "medialibrary", component: ImageListsComponent, canActivate: [AuthGuard], data: { title: "Media Library" } },
      { path: "playlist", component: PlaylistsManagementComponent, canActivate: [AuthGuard], data: { title: "Playlist" } },
      { path: "directorylistingcategory", component: DirectoryListingCategorysManagementComponent, canActivate: [AuthGuard], data: { title: "Directory Listing Category" } },
      { path: "mediaextension", component: MediaExtensionsManagementComponent, canActivate: [AuthGuard], data: { title: "Media Extension" } },
      { path: "directorylistings", component: DirectoryListingsManagementComponent, canActivate: [AuthGuard], data: { title: "Directory Listing" } },
      { path: "buildings", component: BuildingsManagementComponent, canActivate: [AuthGuard], data: { title: "Building" } },
      { path: "floors", component: FloorsManagementComponent, canActivate: [AuthGuard], data: { title: "Floor" } },
      { path: "maps", component: MapsManagementComponent, canActivate: [AuthGuard], data: { title: "Maps" } },
      { path: "occupancylogs", component: OccupancyLogsManagementComponent, canActivate: [AuthGuard], data: { title: "Occupancy Logs" } },
      { path: "modules", component: ModulesManagementComponent, canActivate: [AuthGuard], data: { title: "Module Settings" } },
      { path: "authlogs", component: AuthLogsManagementComponent, canActivate: [AuthGuard], data: { title: "Authentication Logs" } },
      { path: "updowntimelogs", component: UpDownTimeLogsManagementComponent, canActivate: [AuthGuard], data: { title: "Up Down Time Logs" } },
      { path: "auditlogs", component: DataLogsManagementComponent, canActivate: [AuthGuard], data: { title: "Audit Logs" } },
      { path: "imagereferencetypes", component: ImageReferenceTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Image Reference Types" } },
      { path: "imagereferencecolors", component: ImageReferenceColorsManagementComponent, canActivate: [AuthGuard], data: { title: "Image Reference Colors" } },
      { path: "profile", component: UserInfoComponent, canActivate: [AuthGuard], data: { title: "Profile" } },
      { path: "smartroomschedulerlogs", component: SmartRoomSchedulerLogsManagementComponent, canActivate: [AuthGuard], data: { title: "Smart Room Scheduler Logs" } },
      { path: "assettypes", component: AssetTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Asset Types" } },
      { path: "assetmodels", component: AssetModelsManagementComponent, canActivate: [AuthGuard], data: { title: "Asset Models" } },
      { path: "assets", component: AssetsManagementComponent, canActivate: [AuthGuard], data: { title: "Assets" } },
      { path: "servicecontracts", component: ServiceContractsManagementComponent, canActivate: [AuthGuard], data: { title: "Service Contracts" } },
      { path: "emailqueues", component: EmailQueuesManagementComponent, canActivate: [AuthGuard], data: { title: "Email Queues" } },
      { path: "devicetypes", component: DeviceTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Device Types" } },
      { path: "channelinfos", component: ChannelInfosManagementComponent, canActivate: [AuthGuard], data: { title: "Channel Infos" } },
      { path: "classbatches", component: ClassBatchesManagementComponent, canActivate: [AuthGuard], data: { title: "Class Batches" } },
      { path: "classlevels", component: ClassLevelsManagementComponent, canActivate: [AuthGuard], data: { title: "Class Levels" } },
      { path: "dispenseroutlets", component: DispenserOutletManagementComponent, canActivate: [AuthGuard], data: { title: "Dispensers" } },
      { path: "classes", component: ClassesManagementComponent, canActivate: [AuthGuard], data: { title: "Classes" } },
      { path: "students", component: StudentsManagementComponent, canActivate: [AuthGuard], data: { title: "Students" } },
      { path: "studentgroups", component: StudentGroupsManagementComponent, canActivate: [AuthGuard], data: { title: "Student Groups" } },
      { path: "stafftypes", component: StaffTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Staff Types" } },
      { path: "staffs", component: StaffsManagementComponent, canActivate: [AuthGuard], data: { title: "Staffs" } },
      { path: "restrictiontypes", component: RestrictionTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Restriction Types" } },
      { path: "restrictions", component: RestrictionsManagementComponent, canActivate: [AuthGuard], data: { title: "Restrictions" } },
      { path: "dishtypes", component: DishTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Dish Types" } },
      { path: "caterers", component: CatererInfosManagementComponent, canActivate: [AuthGuard], data: { title: "Caterers" } },
      { path: "bentoboxtypes", component: BentoBoxTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Bento Box Types" } },
      { path: "cartontypes", component: CartonTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Carton Types" } },
      { path: "deliveryorders", component: DeliveryOrdersManagementComponent, canActivate: [AuthGuard], data: { title: "Delivery Orders" } },
      { path: "deliveryordernews", component: DeliveryOrdersNewManagementComponent, canActivate: [AuthGuard], data: { title: "Delivery Orders" } },
      { path: "trackingstatus", component: TrackingStatussManagementComponent, canActivate: [AuthGuard], data: { title: "Tracking Status" } },
      { path: "storeinfo", component: StoreInfosManagementComponent, canActivate: [AuthGuard], data: { title: "Store" } },
      { path: "bentoassets", component: BentoAssetsManagementComponent, canActivate: [AuthGuard], data: { title: "Bento Assets" } },
      { path: "bentousages", component: BentoUsageManagementComponent, canActivate: [AuthGuard], data: { title: "Bento Usages" } },
      { path: "dishingprocess", component: DishingProcesssManagementComponent, canActivate: [AuthGuard], data: { title: "Dishing Process" } },
      { path: "packingprocess", component: PackingProcesssManagementComponent, canActivate: [AuthGuard], data: { title: "Packing Process" } },
      { path: "doprocess", component: DoProcesssManagementComponent, canActivate: [AuthGuard], data: { title: "Carton To DO" } },
      { path: "cartonassets", component: CartonAssetsManagementComponent, canActivate: [AuthGuard], data: { title: "Carton Assets" } },
      { path: "mealtypes", component: MealTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Meal Types" } },
      { path: "mealperiods", component: MealPeriodsManagementComponent, canActivate: [AuthGuard], data: { title: "Meal Periods" } },
      { path: "dishes", component: DishesManagementComponent, canActivate: [AuthGuard], data: { title: "Dishes" } },
      { path: "menus", component: MenusManagementComponent, canActivate: [AuthGuard], data: { title: "Menus" } },
      { path: "menucycles", component: MenuCyclesManagementComponent, canActivate: [AuthGuard], data: { title: "Menu Cyles" } },
      { path: "cuisines", component: CuisinesManagementComponent, canActivate: [AuthGuard], data: { title: "Cuisines" } },
      { path: "tokenorders", component: TokenOrdersManagementComponent, canActivate: [AuthGuard], data: { title: "Meal Orders" } },
      { path: "outlets", component: OutletsManagementComponent, canActivate: [AuthGuard], data: { title: "Outlets" } },
      { path: "outletprofiles", component: OutletProfilesManagementComponent, canActivate: [AuthGuard], data: { title: "Outlet Profiles" } },
      { path: "interestgroups", component: InterestGroupsManagementComponent, canActivate: [AuthGuard], data: { title: "Interest Groups" } },
      { path: "drivers", component: DriversManagementComponent, canActivate: [AuthGuard], data: { title: "Drivers" } },
      { path: "routes", component: RoutesManagementComponent, canActivate: [AuthGuard], data: { title: "Routes" } },
      { path: "menucyclecalendars", component: MenuCycleCalendarComponent, canActivate: [AuthGuard], data: { title: "Menu Cycle Calendar" } },
      { path: "menucyclecalendars/:catererId", component: MenuCycleCalendarComponent, canActivate: [AuthGuard], data: { title: "Menu Cycle Calendar" } },
      { path: "outletcalendars/:id/:catererId", component: OutletMenuCycleCalendarComponent, canActivate: [AuthGuard], data: { title: "Outlet Calendar" } },
      { path: "outletdishcalendars/:id/:catererId", component: OutletDishCycleCalendarComponent, canActivate: [AuthGuard], data: { title: "Outlet Calendar" } },
      { path: "studentcalendars/:id", component: StudentMenuCycleCalendarComponent, canActivate: [AuthGuard], data: { title: "Student Calendar" } },
      { path: "dishcycles", component: DishCyclesManagementComponent, canActivate: [AuthGuard], data: { title: "Menu Cyles" } },
      { path: "dishcyclecalendars", component: DishCycleCalendarComponent, canActivate: [AuthGuard], data: { title: "Dish Cycle Calendar" } },
      { path: "dishcyclecalendars/:catererId/:name", component: DishCycleCalendarComponent, canActivate: [AuthGuard], data: { title: "Dish Cycle Calendar" } },
      { path: "paymentthankyou", component: TokenOrderPaymentThankyouComponent, canActivate: [AuthGuard], data: { title: "Payment Thankyou" } },
      { path: "paymenttypes", component: PaymentTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Payment Types" } },
      { path: "transactionfees", component: TransactionFeesManagementComponent, canActivate: [AuthGuard], data: { title: "Transaction Fees" } },
      { path: "vouchertypes", component: VoucherTypesManagementComponent, canActivate: [AuthGuard], data: { title: "Voucher Types" } },
      { path: "vouchers", component: VouchersManagementComponent, canActivate: [AuthGuard], data: { title: "Vouchers" } },
      { path: "vouchers-administration", component: VouchersUnassignManagementComponent, canActivate: [AuthGuard], data: { title: "Vouchers Administration" } },
      { path: "wallet-transaction-log-management", component: WalletTransactionLogManagementComponent, canActivate: [AuthGuard], data: { title: "Ewallet Transactions" } },
      { path: "waivers", component: WaiversManagementComponent, canActivate: [AuthGuard], data: { title: "Waivers" } },
      { path: "contactussubjects", component: ContactUsSubjectsManagementComponent, canActivate: [AuthGuard], data: { title: "Contact Us Subjects" } },
      { path: "contactusdetails", component: ContactUsDetailsManagementComponent, canActivate: [AuthGuard], data: { title: "Contact Us Details" } },
      { path: "emailtemplates", component: EmailTemplatesManagementComponent, canActivate: [AuthGuard], data: { title: "Email Templates" } },
      { path: "notificationevents", component: NotificationEventsManagementComponent, canActivate: [AuthGuard], data: { title: "Notification Events" } },
      { path: "cancelorderrequests", component: CancelOrderRequestsManagementComponent, canActivate: [AuthGuard], data: { title: "Cancel Order Requests" } },
      { path: "externalapploginlogs", component: ExternalLoginLogsManagementComponent, canActivate: [AuthGuard], data: { title: "Order Portal Login Logs" } },
      { path: "orderlogs", component: OrderLogsManagementComponent, canActivate: [AuthGuard], data: { title: "Order Report" } },
      { path: "useraccountreport", component: UserAccountsReportManagementComponent, canActivate: [AuthGuard], data: { title: "User Account Report" } },
      { path: "rolereport", component: UserRolesReportManagementComponent, canActivate: [AuthGuard], data: { title: "Role Report" } },
      { path: "menugroup", component: MenuGroupsManagementComponent, canActivate: [AuthGuard], data: { title: "Menu Group" } },
      { path: "ordercancellation", component: OrderCancellationsManagementComponent, canActivate: [AuthGuard], data: { title: "Order Cancellation" } },
      { path: "ordercancellationreport", component: OrderCancellationReportManagementComponent, canActivate: [AuthGuard], data: { title: "Order Cancellation Report" } },
      { path: "notificationsettings", component: NotificationSettingComponent, canActivate: [AuthGuard], data: { title: "Notification Setting" } },
      { path: "useractivityreport", component: UserActivityLogsManagementComponent, canActivate: [AuthGuard], data: { title: "User Activity Report" } },
      { path: "ordercollectionreport", component: OrderCollectionLogsManagementComponent, canActivate: [AuthGuard], data: { title: "Order Collection Report" } },
      { path: "orderpaymentsreport", component: PaymentsReportManagementComponent, canActivate: [AuthGuard], data: { title: "Order Payments Report" } },
      { path: "voucherutilisationreport", component: VoucherUtilisationsReportManagementComponent, canActivate: [AuthGuard], data: { title: "Voucher Utilisation Report" } },
      { path: "outletterms", component: OutletTermsManagementComponent, canActivate: [AuthGuard], data: { title: "Terms" } },
      { path: "faqsubjects", component: FaqSubjectsManagementComponent, canActivate: [AuthGuard], data: { title: "FAQ Subjects" } },
      { path: "faqdetails", component: FaqDetailsManagementComponent, canActivate: [AuthGuard], data: { title: "FAQ Details" } },
      { path: "plcs", component: PlcManagementComponent, canActivate: [AuthGuard], data: { title: "PLCs" } },
      { path: "catererasssettypes", component: CatererAsssetTypeManagementComponent, canActivate: [AuthGuard], data: { title: "Asset Types" } },
      { path: "catererasset", component: CatererAssetsManagementComponent, canActivate: [AuthGuard], data: { title: "Assets" } }
      //{ path: "orderportalcontents", component: OrderPortalContentComponent, canActivate: [AuthGuard], data: { title: "Order Portal Content" } },

      //{ path: "display/:mac", redirectTo: "/display/:mac" },
      //{ path: "signinwithgoogle", redirectTo: "/authorization/SignInWithGoogle", pathMatch: "full" },
      
      //{ path: "attendance", redirectTo: "/authorization/AttendanceSignIn", pathMatch: "full" },
      //{ path: "attendance", component: AttendanceSignInComponent, data: { title: "Attendance" } },

      //{ path: "**", component: NotFoundComponent, data: { title: "Page Not Found" } }
    ]
  },
  // App routes goes here here
  {
    path: '',
    component: NoNavigationAppComponent,
    children: [
      { path: "pibdisplay", component: PIBDisplayComponent, data: { title: "PIB Display" } },
      { path: "pibdevicetemplate", component: TemplateEditorPreviewDialog, data: { title: "PIB Display" } },
      { path: "meetingroom", component: MeetingRoomDisplayComponent, data: { title: "Meeting Room Display" } },
      { path: "meetingroom/:mac_address", component: MeetingRoomDisplayComponent, data: { title: "Meeting Room Display" } },
      { path: "meetingroom/:mac_address/:server", component: MeetingRoomDisplayComponent, data: { title: "Meeting Room Display" } },
      { path: "attendance", component: AttendanceSignInComponent, data: { title: "Attendance" } },
      //{ path: "changepassword", component: UserChangePasswordComponent, data: { title: "Change Password" } },
      { path: "forgotpassword", component: ForgotPasswordComponent, data: { title: "Forgot Password" } },
      { path: "register", component: RegisterComponent, data: { title: "Register" } },
      { path: "resetpassword", component: ResetPasswordComponent, data: { title: "ResetPassword" } },
      { path: "login", component: LoginComponent, data: { title: "Login" } },
      { path: "multifactor", component: MultiFactorLoginComponent, data: { title: "Two-Factor Login" } },
      { path: "externallogin", component: ExternalLoginComponent, data: { title: "Login" } },
      { path: "feedback", component: FeedbackComponent, data: { title: "Feedback" } },
      { path: "signagedisplay/:mac_address", component: SignageDisplayComponent, data: { title: "Signage Display" } },
      { path: "signagedisplay/:mac_address/:publication_id", component: SignageDisplayComponent, data: { title: "Signage Display" } },
      { path: "kiosk", component: WayfinderComponent, data: { title: "Kiosk" } },
      { path: "kiosk/:point_id", component: WayfinderComponent, data: { title: "Kiosk" } },
      { path: "kioskL", component: WayfinderLandscapeComponent, data: { title: "Kiosk" } },
      { path: "kioskL/:point_id", component: WayfinderLandscapeComponent, data: { title: "Kiosk" } },
      { path: "devicewarning/:msg/", component: DeviceDisplayWarningComponent, data: { title: "Device" } },
      { path: "devicewarning/:msg/:id", component: DeviceDisplayWarningComponent, data: { title: "Device" } },
      { path: "devicewarning/:msg/:id/:address", component: DeviceDisplayWarningComponent, data: { title: "Device" } },
      { path: "preview/dish/:id", component: DishPreviewComponent, data: { title: "Dish Preview" } },

    ]
  }
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [AuthService, AuthGuard]
})
export class AppRoutingModule {
  
}
