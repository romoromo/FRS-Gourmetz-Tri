import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';


import { fadeInOut } from '../../services/animations';
import { BootstrapTabDirective } from "../../directives/bootstrap-tab.directive";
import { AccountService } from "../../services/account.service";
import { Permission } from '../../models/permission.model';


@Component({
  selector: 'settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css'],
  animations: [fadeInOut]
})
export class SettingsComponent implements OnInit, OnDestroy {

  isProfileActivated = true;
  isPhonebookActivated = false;
  isPreferencesActivated = false;
  isUsersActivated = false;
  isUserGroupsActivated = false;
  isRolesActivated = false;
  isFacilitiesActivated = false;
  isFacilityTypesActivated = false;
  isLocationsActivated = false;
  isLocationsTreeActivated = false;
  isInstitutionsActivated = false;
  isDepartmentsActivated = false;
  isApplicationSettingActivated = false;
  isMediaActivated = false;
  isUploadImagesActivated = false;
  isPlaylistsActivated = false;
  isContactGroupsActivated = false;
  isEpaperTemplatesActivated = false;
  isEpaperDevicesActivated = false;
  isDirectoryListingCategoryActivated = false;
  isEmployeeDesignationActivated = false;
  isEmployeeScheduleActivated = false;
  isEmployeeDataActivated = false;
  isOccupancyLogActivated = false;
  isDirectoryListingActivated = false;
  isBuildingActivated = false;
  isFloorActivated = false;
  fragmentSubscription: any;
  isMapActivated = false;
  isModulesActivated = false;
  isKioskSettingsActivated = false;
  isEmsActivated = false;
  isEmsProfileActivated = false;
  isEmsScheduleActivated = false;

  readonly profileTab = "profile";
  readonly phonebookTab = "phonebook";
  readonly preferencesTab = "preferences";
  readonly usersTab = "users";
  readonly userGroupsTab = "userGroups";
  readonly rolesTab = "roles";
  readonly facilitiesTab = "facilities";
  readonly facilityTypesTab = "facilityTypes";
  readonly locationsTab = "locations";
  readonly locationsTreeTab = "locationsTree";
  readonly institutionsTab = "institutions";
  readonly departmentsTab = "departments";
  readonly applicationSettingTab = "applicationSetting";
  readonly mediaTab = "medias";
  readonly imageUploadsTab = "imageUploads";
  readonly playlistsTab = "playlists";
  readonly contactGroupsTab = "contactGroups";
  readonly epaperTemplatesTab = "epaperTemplates";
  readonly epaperDevicesTab = "epaperDevices";
  readonly directoryListingCategoryTab = "directoryListingCategory";
  readonly employeeDesignationTab = "employeeDesignation";
  readonly employeeScheduleTab = "employeeSchedule";
  readonly employeeDataTab = "employeeData";
  readonly occupancyLogTab = "occupancyLog";
  readonly directoryListingTab = "directoryListing";
  readonly buildingTab = "building";
  readonly floorTab = "floor";
  readonly mapTab = "map";
  readonly moduleTab = "modules";
  readonly kioskSettingsTab = "kioskSettings";
  readonly emsTab = "emses";
  readonly emsProfileTab = "emsProfile";
  readonly emsScheduleTab = "emsSchedule";

  @ViewChild("tab")
  tab: BootstrapTabDirective;
    isSignageComponentsActivated: boolean;
  signageComponentsTab: any = "signageComponents";
  isSignageCompilationsActivated: boolean;
  signageCompilationsTab: any = "signageCompilations";
  isSignagePublicationsActivated: boolean;
  signagePublicationsTab: any = "signagePublications";


  constructor(private route: ActivatedRoute, private accountService: AccountService) {
  }


  ngOnInit() {
    this.fragmentSubscription = this.route.fragment.subscribe(anchor => this.showContent(anchor));
  }


  ngOnDestroy() {
    if (this.fragmentSubscription) {
      this.fragmentSubscription.unsubscribe();
    }
  }

  showContent(anchor: string) {
    if ((this.isFragmentEquals(anchor, this.usersTab) && !this.canViewUsers) ||
      (this.isFragmentEquals(anchor, this.rolesTab) && !this.canViewRoles))
      return;

    this.tab.show(`#${anchor || this.profileTab}Tab`);
  }


  isFragmentEquals(fragment1: string, fragment2: string) {

    if (fragment1 == null)
      fragment1 = "";

    if (fragment2 == null)
      fragment2 = "";

    return fragment1.toLowerCase() == fragment2.toLowerCase();
  }


  onShowTab(event) {
    let activeTab = event.target.hash.split("#", 2).pop();

    this.isProfileActivated = activeTab == this.profileTab;
    this.isPhonebookActivated = activeTab == this.phonebookTab;
    this.isPreferencesActivated = activeTab == this.preferencesTab;
    this.isUsersActivated = activeTab == this.usersTab;
    this.isUserGroupsActivated = activeTab == this.userGroupsTab;
    this.isRolesActivated = activeTab == this.rolesTab;
    this.isFacilitiesActivated = activeTab == this.facilitiesTab;
    this.isFacilityTypesActivated = activeTab == this.facilityTypesTab;
    this.isLocationsActivated = activeTab == this.locationsTab;
    this.isLocationsTreeActivated = activeTab == this.locationsTreeTab;
    this.isInstitutionsActivated = activeTab == this.institutionsTab;
    this.isDepartmentsActivated = activeTab == this.departmentsTab;
    this.isApplicationSettingActivated = activeTab == this.applicationSettingTab;
    this.isMediaActivated = activeTab == this.mediaTab;
    this.isUploadImagesActivated = activeTab == this.imageUploadsTab;
    this.isPlaylistsActivated = activeTab == this.playlistsTab;
    this.isContactGroupsActivated = activeTab == this.contactGroupsTab;
    this.isEpaperDevicesActivated = activeTab == this.epaperDevicesTab;
    this.isEpaperTemplatesActivated = activeTab == this.epaperTemplatesTab;
    this.isSignageComponentsActivated = activeTab == this.signageComponentsTab;
    this.isSignageCompilationsActivated = activeTab == this.signageCompilationsTab;
    this.isSignagePublicationsActivated = activeTab == this.signagePublicationsTab;
    this.isDirectoryListingCategoryActivated = activeTab == this.directoryListingCategoryTab;
    this.isEmployeeDesignationActivated = activeTab == this.employeeDesignationTab;
    this.isEmployeeScheduleActivated = activeTab == this.employeeScheduleTab;
    this.isEmployeeDataActivated = activeTab == this.employeeDataTab;
    this.isOccupancyLogActivated = activeTab == this.occupancyLogTab;
    this.isDirectoryListingActivated = activeTab == this.directoryListingTab;
    this.isBuildingActivated = activeTab == this.buildingTab;
    this.isFloorActivated = activeTab == this.floorTab;
    this.isMapActivated = activeTab == this.mapTab;
    this.isModulesActivated = activeTab == this.moduleTab;
    this.isKioskSettingsActivated = activeTab == this.kioskSettingsTab;
    this.isEmsActivated = activeTab == this.emsTab;
    this.isEmsProfileActivated = activeTab == this.emsProfileTab;
    this.isEmsScheduleActivated = activeTab == this.emsScheduleTab;
  }


  get canViewUsers() {
    return this.accountService.userHasPermission(Permission.viewUsersPermission);
  }

  get canViewRoles() {
    return this.accountService.userHasPermission(Permission.viewRolesPermission);
  }

  get canViewFacilities() {
    return this.accountService.userHasPermission(Permission.viewFacilitiesPermission);
  }

  get canViewLocations() {
    return this.accountService.userHasPermission(Permission.viewLocationsPermission);
  }

  get canViewFacilityTypes() {
    return this.accountService.userHasPermission(Permission.viewFacilityTypesPermission);
  }

  get canManageApplicationSettings() {
    return true;//this.accountService.userHasPermission(Permission.manageApplicationSettingPermission)
  }

  get canManageMedias() {
    return true; //this.accountService.userHasPermission(Permission.manageApplicationSettingPermission)
  }

  get canViewInstitutions() {
    return this.accountService.userHasPermission(Permission.viewInstitutionsPermission);
  }

  get canManageDepartments() {
    return this.accountService.userHasPermission(Permission.manageDepartmentsPermission);
  }

  get canManageContactGroups() {
    return this.accountService.userHasPermission(Permission.manageContactGroupsPermission);
  }

  get canManagePhonebooks() {
    return this.accountService.userHasPermission(Permission.manageUserPhonebooksPermission);
  }

  get canManageEpaperTemplates() {
    return this.accountService.userHasPermission(Permission.manageEpaperTemplatesPermission)
  }

  get canManageEpaperDevices() {
    return this.accountService.userHasPermission(Permission.manageEpaperDevicesPermission)
  }

  get canManageSignageComponents() {
    return true;// this.accountService.userHasPermission(Permission.manageSignageComponentsPermission)
  }

  get canManageSignageCompilations() {
    return true;// this.accountService.userHasPermission(Permission.manageSignageCompilationsPermission)
  }

  get canManageSignagePublications() {
    return true;// this.accountService.userHasPermission(Permission.manageSignagePublicationsPermission)
  }

  get canManageDirectoryListingCategorys() {
    return true;//this.accountService.userHasPermission(Permission.manageDirectoryListingCategoryPermission)
  }

  get canManageEmployeeDesignations() {
    return true;//this.accountService.userHasPermission(Permission.manageDirectoryListingCategoryPermission)
  }

  get canManageEmployeeSchedules() {
    return true;//this.accountService.userHasPermission(Permission.manageDirectoryListingCategoryPermission)
  }

  get canManageEmployeeDatas() {
    return true;//this.accountService.userHasPermission(Permission.manageDirectoryListingCategoryPermission)
  }

  get canManageDirectoryListings() {
    return true;//this.accountService.userHasPermission(Permission.manageDirectoryListingPermission)
  }

  get canManageOccupancyLogs() {
    return true;//this.accountService.userHasPermission(Permission.manageOccupancyLogPermission)
  }

  get canManageBuildings() {
    return true;//this.accountService.userHasPermission(Permission.manageBuildingPermission)
  }

  get canManageFloors() {
    return true;//this.accountService.userHasPermission(Permission.manageFloorPermission)
  }

  get canManageMaps() {
    return true;//this.accountService.userHasPermission(Permission.manageMapPermission)
  }

  get canManageModules() {
    return true;
  }

  get canViewUserGroups() {
    return this.accountService.userHasPermission(Permission.viewUserGroupsPermission);
  }
  
  get canManageEmses() {
    return true; //this.accountService.userHasPermission(Permission.manageApplicationSettingPermission)
  }

  get canManageEmsProfiles() {
    return true;
  }

  get canManageEmsSchedules() {
    return true;
  }

  get canManageKioskSettings() {
    return true;//this.accountService.userHasPermission(Permission.maanageKioskSettingsPermission)
  }
}
