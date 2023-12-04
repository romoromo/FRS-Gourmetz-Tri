using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DAL.Core.Helpers;
using static DAL.Core.Helpers.TreeExtensions;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;

namespace DAL.Core
{
    public static class ApplicationPermissionsTrees
    {
        static int id = 1; //This is just a holder for the tree. 
        public static readonly ApplicationPermissionsTree AllPermissionsTree;
        public static ReadOnlyCollection<ApplicationPermissionsTree> AllPermissions;
        public static string AclPath;
        private static string _aclPath;
        #region Root ACL
        public static ApplicationPermissionsTree Root = new ApplicationPermissionsTree(id++, "Root", "root.view", "Root");

        #endregion

        //#region Account ACL
        //public const string AccountPermissionGroupName = "Account Permissions";
        //public static ApplicationPermissionsTree ViewAccountsMenu = new ApplicationPermissionsTree(2, "View Account Management", "account.view","Permission to view Account Management", Root);

        //public const string UsersPermissionGroupName = "User";
        //public static ApplicationPermissionsTree UsersMenu = new ApplicationPermissionsTree(3, "Users", "accountsmenu.users.view","Permission to view Users", ViewAccountsMenu);

        //public static ApplicationPermissionsTree ViewUsers = new ApplicationPermissionsTree(4, "View Users", "users.view", "Permission to view other users account details", UsersMenu);
        //public static ApplicationPermissionsTree ManageUsers = new ApplicationPermissionsTree(5, "Manage Users", "users.manage", "Permission to create, delete and modify other users account details", UsersMenu);

        //public const string RolesPermissionGroupName = "Role";
        //public static ApplicationPermissionsTree RolesMenu = new ApplicationPermissionsTree(6, "Roles", "accountsmenu.roles.view",  "Permission to view Roles", ViewAccountsMenu);

        //public static ApplicationPermissionsTree ViewRoles = new ApplicationPermissionsTree(7, "View Roles", "roles.view",  "Permission to view available roles", RolesMenu);
        //public static ApplicationPermissionsTree ManageRoles = new ApplicationPermissionsTree(8, "Manage Roles", "roles.manage",  "Permission to create, delete and modify roles", RolesMenu);
        //public static ApplicationPermissionsTree AssignRoles = new ApplicationPermissionsTree(9, "Assign Roles", "roles.assign",  "Permission to assign roles to users", RolesMenu);

        //#endregion

        #region Facility Reservation Management ACL
        public static ApplicationPermissionsTree ViewFRSMenu = new ApplicationPermissionsTree(id++, "View Facilities Management", "frsmgt.view", "Permission to view Facilities Management", Root);

        #region Resource Booking
        public static ApplicationPermissionsTree ViewResourceBookingMenu = new ApplicationPermissionsTree(id++, "View Resource Booking", "frsmgt.resourcebooking.view", "Permission to view Resource Booking", ViewFRSMenu);

        public static ApplicationPermissionsTree RBFacilitiesMenu = new ApplicationPermissionsTree(id++, "Equipment", "frsmgt.equipment.view", "Permission to view equipments", ViewResourceBookingMenu);
        //public static ApplicationPermissionsTree RBViewFacilities = new ApplicationPermissionsTree(id++, "View Equipments", "frsmgt.equipment.view", "Permission to view available equipments", RBFacilitiesMenu);
        public static ApplicationPermissionsTree RBManageFacilities = new ApplicationPermissionsTree(id++, "Manage Equipments", "frsmgt.equipment.manage", "Permission to create, delete and modify equipments", RBFacilitiesMenu);

        public static ApplicationPermissionsTree RBFacilityTypesMenu = new ApplicationPermissionsTree(id++, "Equipment Type", "frsmgt.equipmenttype.view", "Permission to view equipment types", ViewResourceBookingMenu);
        //public static ApplicationPermissionsTree RBViewFacilityTypes = new ApplicationPermissionsTree(id++, "View Equipment Types", "frsmgt.equipmenttype.view", "Permission to view equipment types", RBFacilityTypesMenu);
        public static ApplicationPermissionsTree RBManageFacilityTypes = new ApplicationPermissionsTree(id++, "Manage Equipment Types", "frsmgt.equipmenttype.manage", "Permission to create, delete and modify equipment type", RBFacilityTypesMenu);

        public static ApplicationPermissionsTree RBCalendarMenu = new ApplicationPermissionsTree(id++, "Calendar", "frsmgt.calendar.view", "Permission to book facilities and events", ViewResourceBookingMenu);

        public static ApplicationPermissionsTree RBSmartRoomSchedulerLogsMenu = new ApplicationPermissionsTree(id++, "Scheduler Logs", "frsmgt.smartroomschedulerlog.view", "Permission to view scheduler logs", ViewResourceBookingMenu);
        #endregion

        #region Access Control
        public static ApplicationPermissionsTree ViewAccessControlMenu = new ApplicationPermissionsTree(id++, "View Access Control", "frsmgt.accesscontrol.view", "Permission to view FRS Access Control", ViewFRSMenu);

        public static ApplicationPermissionsTree ACVehiclesMenu = new ApplicationPermissionsTree(id++, "Vehicle Management", "frsmgt.accesscontrol.uservehicle.view", "Permission to view vehicle management", ViewAccessControlMenu);
        public static ApplicationPermissionsTree ACManageUserVehicles = new ApplicationPermissionsTree(id++, "Manage Vehicles", "frsmgt.accesscontrol.uservehicle.manage", "Permission to create, delete and modify vehicles", ACVehiclesMenu);

        public static ApplicationPermissionsTree ACContactGroupMenu = new ApplicationPermissionsTree(id++, "Contact Group Management", "frsmgt.accesscontrol.contactgroup.view", "Permission to view contact groups management", ViewAccessControlMenu);
        //public static ApplicationPermissionsTree ACContactGroupView = new ApplicationPermissionsTree(id++, "View Contact Groups", "frsmgt.accesscontrol.contactgroup.view", "Permission to view groups", ACContactGroupMenu);
        public static ApplicationPermissionsTree ACContactGroupManage = new ApplicationPermissionsTree(id++, "Manage Contact Groups", "frsmgt.accesscontrol.contactgroup.manage", "Permission to create, delete and modify contact groups", ACContactGroupMenu);


        public static ApplicationPermissionsTree ACUserPhonebookMenu = new ApplicationPermissionsTree(id++, "User Phonebook Management", "frsmgt.accesscontrol.phonebook.view", "Permission to view user phonebook management", ViewAccessControlMenu);
        //public static ApplicationPermissionsTree ACUserPhonebookView = new ApplicationPermissionsTree(id++, "View User Phonebook", "frsmgt.accesscontrol.phonebook.view", "Permission to view user phonebooks", ACUserPhonebookMenu);
        public static ApplicationPermissionsTree ACUserPhonebookManage = new ApplicationPermissionsTree(id++, "Manage User Phonebook", "frsmgt.accesscontrol.phonebook.manage", "Permission to create, delete and modify user phonebooks", ACUserPhonebookMenu);


        #endregion

        #region Workforce Management
        public static ApplicationPermissionsTree ViewWorkforceManagementMenu = new ApplicationPermissionsTree(id++, "Workforce Management", "frsmgt.workforcemanagement.view", "Permission to view FRS Workforce Management", ViewFRSMenu);

        public static ApplicationPermissionsTree WFDesignationMenu = new ApplicationPermissionsTree(id++, "View Designation", "frsmgt.workforcemanagement.designation.view", "Permission to view designation", ViewWorkforceManagementMenu);
        public static ApplicationPermissionsTree WFEmployeeRosterMenu = new ApplicationPermissionsTree(id++, "View Employee Roster", "frsmgt.workforcemanagement.employeeroster.view", "Permission to view employee master", ViewWorkforceManagementMenu);
        public static ApplicationPermissionsTree WFEmployeeMasterMenu = new ApplicationPermissionsTree(id++, "View Employee Master", "frsmgt.workforcemanagement.employeemaster.view", "Permission to view employee roster", ViewWorkforceManagementMenu);
        #endregion

        //public const string FacilityTypesPermissionGroupName = "Facility Types";
        //public static ApplicationPermissionsTree FacilityTypesMenu = new ApplicationPermissionsTree(14, "Facility Types", "frsmgt.facilitytype.view", "Permission to view facility types", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewFacilityTypes = new ApplicationPermissionsTree(47, "View Facility Types", "frsmgt.facilitytype.view", "Permission to view available facility types", FacilityTypesMenu);
        //public static ApplicationPermissionsTree ManageFacilityTypes = new ApplicationPermissionsTree(48, "Manage Facility Types", "frsmgt.facilitytype.manage", "Permission to create, delete and modify facility types", FacilityTypesMenu);

        //public const string LocationsPermissionGroupName = "Locations";
        //public static ApplicationPermissionsTree LocationsMenu = new ApplicationPermissionsTree(49, "Locations", "frsmgt.location.view", "Permission to view locations", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewLocations = new ApplicationPermissionsTree(15, "View Locations", "frsmgt.location.view", "Permission to view available locations", LocationsMenu);
        //public static ApplicationPermissionsTree ManageLocations = new ApplicationPermissionsTree(16, "Manage Locations", "frsmgt.location.manage", "Permission to create, delete and modify locations", LocationsMenu);

        //public const string InstitutionsPermissionGroupName = "Institutions";
        //public static ApplicationPermissionsTree InstitutionsMenu = new ApplicationPermissionsTree(17, "Institutions", "frsmgt.institution.view", "Permission to view institutions", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewInstitutions = new ApplicationPermissionsTree(18, "View Institutions", "frsmgt.institution.view", "Permission to view available institutions", InstitutionsMenu);
        //public static ApplicationPermissionsTree ManageInstitutions = new ApplicationPermissionsTree(19, "Manage Institutions", "frsmgt.institution.manage", "Permission to create, delete and modify institutions", InstitutionsMenu);

        //public const string DepartmentsPermissionGroupName = "Departments";
        //public static ApplicationPermissionsTree DepartmentsMenu = new ApplicationPermissionsTree(20, "Departments", "frsmgt.department.view", "Permission to view departments", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewDepartments = new ApplicationPermissionsTree(21, "View Departments", "frsmgt.department.view", "Permission to view available departments", DepartmentsMenu);
        //public static ApplicationPermissionsTree ManageDepartments = new ApplicationPermissionsTree(22, "Manage Departments", "frsmgt.department.manage", "Permission to create, delete and modify departments", DepartmentsMenu);

        //public const string ContactGroupsPermissionGroupName = "Contact Groups";
        //public static ApplicationPermissionsTree ContactGroupsMenu = new ApplicationPermissionsTree(23, "Contact Groups", "frsmgt.contactgroup.view", "Permission to view contact groups", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewContactGroups = new ApplicationPermissionsTree(24, "View Contact Groups", "frsmgt.contactgroup.view", "Permission to view available contact groups", ContactGroupsMenu);
        //public static ApplicationPermissionsTree ManageContactGroups = new ApplicationPermissionsTree(25, "Manage Contact Groups", "frsmgt.contactgroup.manage", "Permission to create, delete and modify contact groups", ContactGroupsMenu);

        //public const string UserPhonebooksPermissionGroupName = "Phonebooks";
        //public static ApplicationPermissionsTree PhonebooksMenu = new ApplicationPermissionsTree(26, "Phonebooks", "frsmgt.userphonebook.view", "Permission to view user phonebooks", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewUserPhonebooks = new ApplicationPermissionsTree(27, "View Phonebooks", "frsmgt.userphonebook.view", "Permission to view available phone book", PhonebooksMenu);
        //public static ApplicationPermissionsTree ManageUserPhonebooks = new ApplicationPermissionsTree(28, "Manage Phonebooks", "frsmgt.userphonebook.manage", "Permission to create, delete and modify phone book", PhonebooksMenu);

        //public const string UserGroupPermissionGroupName = "User Group";
        //public static ApplicationPermissionsTree UserGroupsMenu = new ApplicationPermissionsTree(31, "User Groups", "frsmenu.usergroups.view", "Permission to view user groups", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewUserGroups = new ApplicationPermissionsTree(32, "View User Group", "frsmgt.usergroups.view", "Permission to view user groups", UserGroupsMenu);
        //public static ApplicationPermissionsTree ManageUserGroups = new ApplicationPermissionsTree(33, "Manage User Group", "frsmgt.usergroups.manage", "Permission to create, delete and modify user groups", UserGroupsMenu);

        //public const string PublicationsPermissionGroupName = "Publications";
        //public static ApplicationPermissionsTree PublicationsMenu = new ApplicationPermissionsTree(34, "Publications", "frsmenu.publications.view", "Permission to view departments", ViewFRSMenu);

        //public static ApplicationPermissionsTree ApprovePublications = new ApplicationPermissionsTree(35, "Approve Signage Publications", "frsmgt.publications.approve", "Permission to approve signage publications", PublicationsMenu);

        //public const string PIBTemplatesPermissionGroupName = "PIB Templates";
        //public static ApplicationPermissionsTree PublicationsMenu = new ApplicationPermissionsTree(36, "Publications", "frsmenu.publications.view", "Permission to view departments", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewPIBTemplates = new ApplicationPermissionsTree(37, "View PIB Templates", "pibtemplates.view", PIBTemplatesPermissionGroupName, "Permission to view available pib templates");
        //public static ApplicationPermissionsTree ManagePIBTemplates = new ApplicationPermissionsTree(38, "Manage PIB Templates", "pibtemplates.manage", PIBTemplatesPermissionGroupName, "Permission to create, delete and modify pib templates");

        //public const string PIBDevicesPermissionGroupName = "PIB Devices";
        //public static ApplicationPermissionsTree ViewPIBDevices = new ApplicationPermissionsTree(39, "View PIB Devices", "pibdevices.view", PIBDevicesPermissionGroupName, "Permission to view available pib devices");
        //public static ApplicationPermissionsTree ManagePIBDevices = new ApplicationPermissionsTree(40, "Manage PIB Devices", "pibdevices.manage", PIBDevicesPermissionGroupName, "Permission to create, delete and modify pib devices");

        //public const string EpaperTemplatesPermissionGroupName = "Epaper Templates";
        //public static ApplicationPermissionsTree EpaperTemplatesMenu = new ApplicationPermissionsTree(41, "Epaper Templates", "frsmenu.epapertemplates.view", "Permission to view epaper templates", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewEpaperTemplates = new ApplicationPermissionsTree(42, "View Epaper Templates", "epapertemplates.view", "Permission to view available epaper templates", EpaperTemplatesMenu);
        //public static ApplicationPermissionsTree ManageEpaperTemplates = new ApplicationPermissionsTree(43, "Manage Epaper Templates", "epapertemplates.manage", "Permission to create, delete and modify epaper templates", EpaperTemplatesMenu);

        //public const string EpaperDevicesPermissionGroupName = "Epaper Devices";
        //public static ApplicationPermissionsTree EpaperDevicesMenu = new ApplicationPermissionsTree(44, "Epaper Devices", "frsmenu.epaperdevices.view", "Permission to view epaper devices", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewEpaperDevices = new ApplicationPermissionsTree(45, "View Epaper Devices", "epaperdevices.view", "Permission to view available epaper devices", EpaperDevicesMenu);
        //public static ApplicationPermissionsTree ManageEpaperDevices = new ApplicationPermissionsTree(46, "Manage Epaper Devices", "epaperdevices.manage", "Permission to create, delete and modify epaper devices", EpaperDevicesMenu);

        #endregion

        #region Signages and Kiosks ACL
        public static ApplicationPermissionsTree ViewSignageMenu = new ApplicationPermissionsTree(id++, "View Signages and Kiosks", "sgnmanagement.view", "Permission to view Signages and Kiosks", Root);

        #region Content Management
        public static ApplicationPermissionsTree ViewContentManagementMenu = new ApplicationPermissionsTree(id++, "View Content Management", "sgnmanagement.contentmanagement.view", "Permission to view Content Management", ViewSignageMenu);

        public static ApplicationPermissionsTree CMMediaGroupsMenu = new ApplicationPermissionsTree(id++, "Media Group", "sgnmanagement.contentmanagement.mediagroup.view", "Permission to view Media Group", ViewContentManagementMenu);
        public static ApplicationPermissionsTree CMMediaLibrariesMenu = new ApplicationPermissionsTree(id++, "Media Library", "sgnmanagement.contentmanagement.medialibrary.view", "Permission to view Media Library", ViewContentManagementMenu);
        public static ApplicationPermissionsTree CMPlaylistsMenu = new ApplicationPermissionsTree(id++, "Playlists", "sgnmanagement.contentmanagement.playlist.view", "Permission to view Playlists", ViewContentManagementMenu);
        public static ApplicationPermissionsTree CMComponentsMenu = new ApplicationPermissionsTree(id++, "Components", "sgnmanagement.contentmanagement.component.view", "Permission to view Components", ViewContentManagementMenu);

        public static ApplicationPermissionsTree CMComponentCreate = new ApplicationPermissionsTree(id++, "Create Component", "sgnmanagement.contentmanagement.component.create", "Permission to create Components", CMComponentsMenu);
        public static ApplicationPermissionsTree CMComponentUpdate = new ApplicationPermissionsTree(id++, "Update Component", "sgnmanagement.contentmanagement.component.update", "Permission to update Components", CMComponentsMenu);
        public static ApplicationPermissionsTree CMComponentDelete = new ApplicationPermissionsTree(id++, "Delete Component", "sgnmanagement.contentmanagement.component.delete", "Permission to delete Components", CMComponentsMenu);

        public static ApplicationPermissionsTree CMCompilationsMenu = new ApplicationPermissionsTree(id++, "Compilations", "sgnmanagement.contentmanagement.compilation.view", "Permission to view Compilations", ViewContentManagementMenu);

        public static ApplicationPermissionsTree CMCompilationCreate = new ApplicationPermissionsTree(id++, "Create Compilation", "sgnmanagement.contentmanagement.compilation.create", "Permission to create Compilations", CMCompilationsMenu);
        public static ApplicationPermissionsTree CMCompilationUpdate = new ApplicationPermissionsTree(id++, "Update Compilation", "sgnmanagement.contentmanagement.compilation.update", "Permission to update Compilations", CMCompilationsMenu);
        public static ApplicationPermissionsTree CMCompilationDelete = new ApplicationPermissionsTree(id++, "Delete Compilation", "sgnmanagement.contentmanagement.compilation.delete", "Permission to delete Compilations", CMCompilationsMenu);

        public static ApplicationPermissionsTree CMPublicationsMenu = new ApplicationPermissionsTree(id++, "Publications", "sgnmanagement.contentmanagement.publication.view", "Permission to view Publications", ViewContentManagementMenu);
        public static ApplicationPermissionsTree CMPublicationApprove = new ApplicationPermissionsTree(id++, "Publications Approval", "frsmgt.accesscontrol.publication.approve", "Permission to approve publications", CMPublicationsMenu);
        public static ApplicationPermissionsTree CMPublicationEmail = new ApplicationPermissionsTree(id++, "Approval Email Alert", "frsmgt.accesscontrol.publication.email", "Receive email alert when publication created", CMPublicationsMenu);
        public static ApplicationPermissionsTree CMPublicationCreate = new ApplicationPermissionsTree(id++, "Create Publication", "sgnmanagement.contentmanagement.publication.create", "Permission to create Publications", CMPublicationsMenu);
        public static ApplicationPermissionsTree CMPublicationUpdate = new ApplicationPermissionsTree(id++, "Update Publication", "sgnmanagement.contentmanagement.publication.update", "Permission to update Publications", CMPublicationsMenu);
        public static ApplicationPermissionsTree CMPublicationDelete = new ApplicationPermissionsTree(id++, "Delete Publication", "sgnmanagement.contentmanagement.publication.delete", "Permission to delete Publications", CMPublicationsMenu);


        #endregion

        #region Directory Listing
        public static ApplicationPermissionsTree ViewDirectoryListingMenu = new ApplicationPermissionsTree(id++, "View Directory Listing", "sgnmanagement.directorylisting.view", "Permission to view Directory Listing", ViewSignageMenu);

        public static ApplicationPermissionsTree DLDirectoryCategoryMenu = new ApplicationPermissionsTree(id++, "Directory Category", "sgnmanagement.directorylisting.directorycategory.view", "Permission to view Directory Category", ViewDirectoryListingMenu);
        public static ApplicationPermissionsTree DLDirectoryListingMenu = new ApplicationPermissionsTree(id++, "Directory Listing", "sgnmanagement.directorylisting.directorylisting.view", "Permission to view Directory Listing", ViewDirectoryListingMenu);
        public static ApplicationPermissionsTree DLDLBuildingMenu = new ApplicationPermissionsTree(id++, "Building", "sgnmanagement.directorylisting.building.view", "Permission to view Building", ViewDirectoryListingMenu);
        public static ApplicationPermissionsTree DLFloorMenu = new ApplicationPermissionsTree(id++, "Floor", "sgnmanagement.directorylisting.floor.view", "Permission to view Floor", ViewDirectoryListingMenu);
        public static ApplicationPermissionsTree DLMapMenu = new ApplicationPermissionsTree(id++, "Map", "sgnmanagement.directorylisting.map.view", "Permission to view Map", ViewDirectoryListingMenu);


        #endregion

        #region Queue Display Interface
        public static ApplicationPermissionsTree ViewQueueDisplayInterfaceMenu = new ApplicationPermissionsTree(id++, "View Queue Display Interface", "sgnmanagement.qdi.view", "Permission to view Queue Display Interface", ViewSignageMenu);

        public static ApplicationPermissionsTree QDIQueueMatrixMenu = new ApplicationPermissionsTree(id++, "Queue Matrix", "sgnmanagement.qdi.queuematrix.view", "Permission to view Queue Matrix", ViewQueueDisplayInterfaceMenu);
        public static ApplicationPermissionsTree QDIQueueDisplayLogsMenu = new ApplicationPermissionsTree(id++, "Queue Display Logs", "sgnmanagement.qdi.queuedisplaylog.view", "Permission to view Queue Display Logs", ViewQueueDisplayInterfaceMenu);


        #endregion

        #region Facility Booking Interface
        public static ApplicationPermissionsTree ViewFacilityBookingInterfaceMenu = new ApplicationPermissionsTree(id++, "View Facility Booking Interface", "sgnmanagement.fbi.view", "Permission to view Facility Booking Interface", ViewSignageMenu);

        public static ApplicationPermissionsTree FBIResourceMatrixMenu = new ApplicationPermissionsTree(id++, "Resource Matrix", "sgnmanagement.fbi.resourcematrix.view", "Permission to view Resource Matrix", ViewFacilityBookingInterfaceMenu);
        public static ApplicationPermissionsTree FBIInteraceLogsMenu = new ApplicationPermissionsTree(id++, "Interface Logs", "sgnmanagement.fbi.interfacelog.view", "Permission to view Interface Logs", ViewFacilityBookingInterfaceMenu);


        #endregion

        #region Device Management
        public static ApplicationPermissionsTree ViewDeviceManagementMenu = new ApplicationPermissionsTree(id++, "View Device Management", "sgnmanagement.devicemgt.view", "Permission to view Device Management", ViewSignageMenu);

        public static ApplicationPermissionsTree DMDevicesMenu = new ApplicationPermissionsTree(id++, "Devices", "sgnmanagement.devicemgt.device.view", "Permission to view Devices", ViewDeviceManagementMenu);

        public static ApplicationPermissionsTree DMDevicesManageMenu = new ApplicationPermissionsTree(id++, "Manage Devices", "sgnmanagement.devicemgt.device.manage", "Permission to add, edit and delete Devices", DMDevicesMenu);
        public static ApplicationPermissionsTree DMApprovalMenu = new ApplicationPermissionsTree(id++, "Approval", "sgnmanagement.devicemgt.device.approve", "Permission to approve Devices", DMDevicesMenu);

        public static ApplicationPermissionsTree DMEMSMenu = new ApplicationPermissionsTree(id++, "EMS", "sgnmanagement.devicemgt.ems.view", "Permission to view EMS", ViewDeviceManagementMenu);
        public static ApplicationPermissionsTree DMEMSProfileMenu = new ApplicationPermissionsTree(id++, "EMS Profile", "sgnmanagement.devicemgt.emsprofile.view", "Permission to view EMS Profile", ViewDeviceManagementMenu);
        public static ApplicationPermissionsTree DMEMSScheduleMenu = new ApplicationPermissionsTree(id++, "EMS Schedule", "sgnmanagement.devicemgt.emsschedule.view", "Permission to view EMS Schedule", ViewDeviceManagementMenu);

        #endregion

        #region Epaper Management
        public static ApplicationPermissionsTree ViewEpaperManagementMenu = new ApplicationPermissionsTree(id++, "View Epaper Management", "sgnmanagement.epapermgt.view", "Permission to view Epaper Management", ViewSignageMenu);

        public static ApplicationPermissionsTree EPTemplateMenu = new ApplicationPermissionsTree(id++, "Epaper Template", "sgnmanagement.epapermgt.template.view", "Permission to view Epaper Template", ViewEpaperManagementMenu);

        #endregion


        #endregion

        #region Reports
        public static ApplicationPermissionsTree ViewReportsMenu = new ApplicationPermissionsTree(id++, "View Reports", "reportmgt.view", "Permission to view Reports", Root);

        #region Facility Management Reports
        public static ApplicationPermissionsTree ViewFacilityManagementMenu = new ApplicationPermissionsTree(id++, "View Facility Management", "reportmgt.facilitymgt.view", "Permission to view Facilitiy Management Reports", ViewReportsMenu);

        public static ApplicationPermissionsTree RVehicleLogsMenu = new ApplicationPermissionsTree(id++, "Vehicle Logs", "reportmgt.facilitymgt.vehiclelog.view", "Permission to view Vehicle Logs", ViewFacilityManagementMenu);
        public static ApplicationPermissionsTree ROccupancyLogsMenu = new ApplicationPermissionsTree(id++, "Occupancy Logs", "reportmgt.facilitymgt.occupancylog.view", "Permission to view Occupancy Logs", ViewFacilityManagementMenu);


        #endregion

        #region Signages and Kiosks Reports
        public static ApplicationPermissionsTree ViewSgnKiosksMenu = new ApplicationPermissionsTree(id++, "Signages and Kiosks", "reportmgt.sgn.view", "Permission to view Signages and Kiosks Reports", ViewReportsMenu);

        public static ApplicationPermissionsTree SKUpDownTimeLogsMenu = new ApplicationPermissionsTree(id++, "Up/Downtime Logs", "reportmgt.sgn.updownlog.view", "Permission to view Up/Downtime Logs", ViewSgnKiosksMenu);


        #endregion

        #endregion

        #region Asset Management
        public static ApplicationPermissionsTree ViewAssetManagementMenu = new ApplicationPermissionsTree(id++, "View Asset Management", "assetmgt.view", "Permission to view System Settings", Root);
        public static ApplicationPermissionsTree AMAssetTypeMenu = new ApplicationPermissionsTree(id++, "Asset Type", "assetmgt.assettype.view", "Permission to view Asset Type", ViewAssetManagementMenu);
        public static ApplicationPermissionsTree AMAssetTypeManage = new ApplicationPermissionsTree(id++, "Manage Asset Type", "assetmgt.assettype.manage", "Permission to add, edit and delete Asset Type", AMAssetTypeMenu);

        public static ApplicationPermissionsTree AMAssetModelMenu = new ApplicationPermissionsTree(id++, "Asset Model", "assetmgt.assetmodel.view", "Permission to view Asset Model", ViewAssetManagementMenu);
        public static ApplicationPermissionsTree AMAssetModelManage = new ApplicationPermissionsTree(id++, "Manage Asset Model", "assetmgt.assetmodel.manage", "Permission to add, edit and delete Asset Model", AMAssetModelMenu);

        public static ApplicationPermissionsTree AMAssetMenu = new ApplicationPermissionsTree(id++, "Asset", "assetmgt.asset.view", "Permission to view Asset", ViewAssetManagementMenu);
        public static ApplicationPermissionsTree AMAssetManage = new ApplicationPermissionsTree(id++, "Manage Asset", "assetmgt.asset.manage", "Permission to add, edit and delete Asset", AMAssetMenu);

        public static ApplicationPermissionsTree AMServiceContractMenu = new ApplicationPermissionsTree(id++, "Service Contract", "assetmgt.servicecontract.view", "Permission to view Service Contract", ViewAssetManagementMenu);
        public static ApplicationPermissionsTree AMServiceContractManage = new ApplicationPermissionsTree(id++, "Manage Service Contract", "assetmgt.servicecontract.manage", "Permission to add, edit and delete Service Contract", AMServiceContractMenu);

        #endregion

        #region System Settings
        public static ApplicationPermissionsTree ViewSystemSettingsMenu = new ApplicationPermissionsTree(id++, "View System Settings", "systemsetting.view", "Permission to view System Settings", Root);

        public static ApplicationPermissionsTree SSLocationTreeMenu = new ApplicationPermissionsTree(id++, "Location Tree", "systemsetting.locationtree.view", "Permission to view Location Tree", ViewSystemSettingsMenu);
        //public static ApplicationPermissionsTree SSLocationTreeView = new ApplicationPermissionsTree(id++, "View Location Tree", "systemsetting.locationtree.view", "Permission to view Location Tree", SSLocationTreeMenu);
        public static ApplicationPermissionsTree SSLocationTreeManage = new ApplicationPermissionsTree(id++, "Manage Location Tree", "systemsetting.locationtree.manage", "Permission to add, edit and delete Location Tree", SSLocationTreeMenu);

        public static ApplicationPermissionsTree SSInstitutionMenu = new ApplicationPermissionsTree(id++, "Institution", "systemsetting.institution.view", "Permission to view Institution", ViewSystemSettingsMenu);
        //public static ApplicationPermissionsTree SSInstitutionView = new ApplicationPermissionsTree(id++, "View Institution", "systemsetting.institution.view", "Permission to view Institution", SSInstitutionMenu);
        public static ApplicationPermissionsTree SSInstitutionManage = new ApplicationPermissionsTree(id++, "Manage Institution", "systemsetting.institution.manage", "Permission to add, edit and delete Institution", SSInstitutionMenu);


        public static ApplicationPermissionsTree SSDepartmentMenu = new ApplicationPermissionsTree(id++, "Department", "systemsetting.department.view", "Permission to view Department", ViewSystemSettingsMenu);
        //public static ApplicationPermissionsTree SSDepartmentView = new ApplicationPermissionsTree(id++, "View Department", "systemsetting.department.view", "Permission to view Department", SSDepartmentMenu);
        public static ApplicationPermissionsTree SSDepartmentManage = new ApplicationPermissionsTree(id++, "Manage Department", "systemsetting.department.manage", "Permission to add, edit and delete Department", SSDepartmentMenu);

        public static ApplicationPermissionsTree SSBuildingMenu = new ApplicationPermissionsTree(id++, "Building", "systemsetting.building.view", "Permission to view Building", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSUserGroupMenu = new ApplicationPermissionsTree(id++, "User Group", "systemsetting.usergroup.view", "Permission to view User Group", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSUserGroupManage = new ApplicationPermissionsTree(id++, "Manage User Group", "systemsetting.usergroup.manage", "Permission to view User Group", SSUserGroupMenu);

        public static ApplicationPermissionsTree SSImageReferenceTypeMenu = new ApplicationPermissionsTree(id++, "Image Reference Type", "systemsetting.imagereferencetype.view", "Permission to view Image Reference Type", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSImageReferenceTypeManage = new ApplicationPermissionsTree(id++, "Manage Image Reference Type", "systemsetting.imagereferencetype.manage", "Permission to view Image Reference Type", SSImageReferenceTypeMenu);

        public static ApplicationPermissionsTree SSImageReferenceColorMenu = new ApplicationPermissionsTree(id++, "Image Reference Color", "systemsetting.imagereferencecolor.view", "Permission to view Image Reference Color", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSImageReferenceColorManage = new ApplicationPermissionsTree(id++, "Manage Image Reference Color", "systemsetting.imagereferencecolor.manage", "Permission to view Image Reference Color", SSImageReferenceColorMenu);

        public static ApplicationPermissionsTree SSDeviceTypeMenu = new ApplicationPermissionsTree(id++, "Device Type", "systemsetting.devicetype.view", "Permission to view Device Type", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSDeviceTypeManage = new ApplicationPermissionsTree(id++, "Manage Device Type", "systemsetting.devicetype.manage", "Permission to add, edit and delete Device Type", SSDeviceTypeMenu);


        public static ApplicationPermissionsTree SSUserMenu = new ApplicationPermissionsTree(id++, "User Management", "systemsetting.user", "Permission to view Users", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSViewUserMenu = new ApplicationPermissionsTree(id++, "View User", "systemsetting.user.view", "Permission to view Users", SSUserMenu);
        public static ApplicationPermissionsTree SSManageUserMenu = new ApplicationPermissionsTree(id++, "Manage User", "systemsetting.user.manage", "Permission to view Users", SSUserMenu);

        public static ApplicationPermissionsTree SSRoleMenu = new ApplicationPermissionsTree(id++, "Role Management", "systemsetting.role", "Permission to view Role", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSViewRoleMenu = new ApplicationPermissionsTree(id++, "View Role", "systemsetting.role.view", "Permission to view Role", SSRoleMenu);
        public static ApplicationPermissionsTree SSManageRoleMenu = new ApplicationPermissionsTree(id++, "Manage Role", "systemsetting.role.manage", "Permission to create, delete and modify roles", SSRoleMenu);
        public static ApplicationPermissionsTree SSAssignRoleMenu = new ApplicationPermissionsTree(id++, "Assign Role", "systemsetting.role.assign", "Permission to assign roles to users", SSRoleMenu);

        public static ApplicationPermissionsTree SSAuditMenu = new ApplicationPermissionsTree(id++, "Audit Logs", "systemsetting.audit.view", "Permission to view Audit Logs", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSAuthLogMenu = new ApplicationPermissionsTree(id++, "Authentication Logs", "systemsetting.audit.authlog.view", "Permission to view authentication logs", SSAuditMenu);
        public static ApplicationPermissionsTree SSDataLogMenu = new ApplicationPermissionsTree(id++, "Data Logs", "systemsetting.audit.datalog.view", "Permission to view data logs", SSAuditMenu);

        public static ApplicationPermissionsTree SSApplicationSettingMenu = new ApplicationPermissionsTree(id++, "Application Setting", "systemsetting.applicationsetting.view", "Permission to view Application Setting", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSModuleSettingMenu = new ApplicationPermissionsTree(id++, "Module Setting", "systemsetting.modulesetting.view", "Permission to view Module Setting", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSEmailQueueMenu = new ApplicationPermissionsTree(id++, "Email Queue", "systemsetting.emailqueue.view", "Permission to view Email Queue", ViewSystemSettingsMenu);

        public static ApplicationPermissionsTree SSDashboardMenu = new ApplicationPermissionsTree(id++, "Dashboard", "systemsetting.dashboard.view", "Permission to view Dashboard", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSDSignage = new ApplicationPermissionsTree(id++, "Signage Dashboard", "systemsetting.dashboard.sgn.view", "Permission to view Signage Dashboard", SSDashboardMenu);

        public static ApplicationPermissionsTree SSDCPreviewSignage = new ApplicationPermissionsTree(id++, "Preview", "systemsetting.dashboard.sgn.controls.preview", "Permission to allow preview of signage", SSDSignage);
        public static ApplicationPermissionsTree SSDCRebootSignage = new ApplicationPermissionsTree(id++, "Reboot", "systemsetting.dashboard.sgn.controls.reboot", "Permission to allow rebooting of devices", SSDSignage);
        public static ApplicationPermissionsTree SSDCRefreshSignage = new ApplicationPermissionsTree(id++, "Refresh", "systemsetting.dashboard.sgn.controls.refresh", "Permission to allow refreshing of devices", SSDSignage);
        public static ApplicationPermissionsTree SSDCScreenshotSignage = new ApplicationPermissionsTree(id++, "Screenshot", "systemsetting.dashboard.sgn.controls.screenshot", "Permission to download screenshot", SSDSignage);
        public static ApplicationPermissionsTree SSDCPushMessageSignage = new ApplicationPermissionsTree(id++, "Push Message", "systemsetting.dashboard.sgn.controls.pushmessage", "Permission to push messages", SSDSignage);
        public static ApplicationPermissionsTree SSDCDeviceInfoSignage = new ApplicationPermissionsTree(id++, "Modify device information", "systemsetting.dashboard.sgn.controls.deviceinfo", "Permission to change device info", SSDSignage);
        public static ApplicationPermissionsTree SSDCPublicationSignage = new ApplicationPermissionsTree(id++, "Publication", "systemsetting.dashboard.sgn.controls.publication", "Permission to change publication", SSDSignage);

        public static ApplicationPermissionsTree SSDUpcomingEvent = new ApplicationPermissionsTree(id++, "Upcoming Events", "systemsetting.dashboard.upcomingevent.view", "Permission to view Upcoming Events", SSDashboardMenu);
        public static ApplicationPermissionsTree SSDAvailableRoom = new ApplicationPermissionsTree(id++, "Available Locations", "systemsetting.dashboard.availablelocation.view", "Permission to view Available Locations", SSDashboardMenu);
        public static ApplicationPermissionsTree SSDApproval = new ApplicationPermissionsTree(id++, "Device Approval List", "systemsetting.dashboard.deviceapproval.view", "Permission to view Device Approval List", SSDashboardMenu);
        

        #endregion

        #region MEAL ORDER SYSTEM
        //public static ApplicationPermissionsTree ViewMOSAccountMgtMenu = new ApplicationPermissionsTree(id++, "View Meal Order Account", "mosmgt.accountmgt.view", "Permission to view Meal Order Account", Root);
        public static ApplicationPermissionsTree ViewMOSMealMgtAccountMgtMenu = new ApplicationPermissionsTree(id++, "Platform Setup", "mosmgt.mealmgt.order.view", "Permission to view platform setup", Root);
        //public static ApplicationPermissionsTree ViewMOSOrganisationMgtMenu = new ApplicationPermissionsTree(id++, "View Meal Order Organisation", "mosmgt.orgmgt.view", "Permission to view Meal Order Organisation", Root);

        public static ApplicationPermissionsTree ViewMOSCatererMgtMenu = new ApplicationPermissionsTree(id++, "Caterers", "mosmgt.caterermgt.view", "Permission to view Meal Order Setting", Root);
        public static ApplicationPermissionsTree ViewMOSOutletMgtMenu = new ApplicationPermissionsTree(id++, "Outlets", "mosmgt.outletmgt.view", "Permission to view Meal Order Setting", Root);

        public static ApplicationPermissionsTree ViewMOSSettingMgtMenu = new ApplicationPermissionsTree(id++, "Meal Order System Setting", "mosmgt.settingmgt.view", "Permission to view Meal Order Setting", Root);
        public static ApplicationPermissionsTree ViewMOSreportMgtMenu = new ApplicationPermissionsTree(id++, "Reports", "mosmgt.reportmgt.view", "Permission to view Meal Order Report", Root);
        //public static ApplicationPermissionsTree ViewMOSSystemMgtMenu = new ApplicationPermissionsTree(id++, "View Meal Order System Admin", "mosmgt.sysadminmgt.view", "Permission to view Meal Order System Admin", Root);

        #region Platform Setup

        #region Restriction Types

        public static ApplicationPermissionsTree MOSOrderMgtRestrictionTypesMenu = new ApplicationPermissionsTree(id++, "Restriction Types", "mosmgt.mealmgt.order.restrictiontypes", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtRestrictionTypesMenu = new ApplicationPermissionsTree(id++, "View Restriction Types", "mosmgt.mealmgt.order.restrictiontypes.view", "Permission to view Restriction Types", MOSOrderMgtRestrictionTypesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtRestrictionTypesMenu = new ApplicationPermissionsTree(id++, "Manage Restriction Types", "mosmgt.mealmgt.order.restrictiontypes.manage", "Permission to view Restriction Types", MOSOrderMgtRestrictionTypesMenu);

        #endregion

        #region Restrictions

        public static ApplicationPermissionsTree MOSOrderMgtRestrictionsMenu = new ApplicationPermissionsTree(id++, "Restrictions", "mosmgt.mealmgt.order.restrictions", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtRestrictionsMenu = new ApplicationPermissionsTree(id++, "View Restrictions", "mosmgt.mealmgt.order.restrictions.view", "Permission to view Restrictions", MOSOrderMgtRestrictionsMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtRestrictionsMenu = new ApplicationPermissionsTree(id++, "Manage Restrictions", "mosmgt.mealmgt.order.restrictions.manage", "Permission to view Restrictions", MOSOrderMgtRestrictionsMenu);

        #endregion

        #region Cuisines

        public static ApplicationPermissionsTree MOSOrderMgtCuisinesMenu = new ApplicationPermissionsTree(id++, "Cuisines", "mosmgt.mealmgt.order.cuisines", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtCuisinesMenu = new ApplicationPermissionsTree(id++, "View Cuisines", "mosmgt.mealmgt.order.cuisines.view", "Permission to view Cuisines", MOSOrderMgtCuisinesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCuisinesMenu = new ApplicationPermissionsTree(id++, "Manage Cuisines", "mosmgt.mealmgt.order.cuisines.manage", "Permission to view Cuisines", MOSOrderMgtCuisinesMenu);

        #endregion

        #region Delivery
        public static ApplicationPermissionsTree MOSOrderMgtDeliveryMenu = new ApplicationPermissionsTree(id++, "Delivery", "mosmgt.mealmgt.order.delivery", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtDeliveryMenu = new ApplicationPermissionsTree(id++, "View Delivery", "mosmgt.mealmgt.order.delivery.view", "Permission to view Delivery", MOSOrderMgtDeliveryMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtDeliveryMenu = new ApplicationPermissionsTree(id++, "Manage Delivery", "mosmgt.mealmgt.order.delivery.manage", "Permission to add, edit and delete Delivery", MOSOrderMgtDeliveryMenu);
        #endregion

        #region Bento Box Types
        public static ApplicationPermissionsTree MOSOrderMgtBentoBoxTypesMenu = new ApplicationPermissionsTree(id++, "Bento Box Types", "mosmgt.mealmgt.order.delivery.bentoboxtypes", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtBentoBoxTypesMenu = new ApplicationPermissionsTree(id++, "View Bento Box Types", "mosmgt.mealmgt.order.delivery.bentoboxtypes.view", "Permission to view Bento Box Types", MOSOrderMgtBentoBoxTypesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtBentoBoxTypesMenu = new ApplicationPermissionsTree(id++, "Manage Bento Box Types", "mosmgt.mealmgt.order.delivery.bentoboxtypes.manage", "Permission to add, edit and delete Bento Box Types", MOSOrderMgtBentoBoxTypesMenu);
        #endregion

        #region Bento Assets
        public static ApplicationPermissionsTree MOSOrderMgtBentoAssetsMenu = new ApplicationPermissionsTree(id++, "Bento Assets", "mosmgt.mealmgt.order.delivery.bentoassets", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtBentoAssetsMenu = new ApplicationPermissionsTree(id++, "View Bento Assets", "mosmgt.mealmgt.order.delivery.bentoassets.view", "Permission to view Bento Assets", MOSOrderMgtBentoAssetsMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtBentoAssetsMenu = new ApplicationPermissionsTree(id++, "Manage Bento Assets", "mosmgt.mealmgt.order.delivery.bentoassets.manage", "Permission to add, edit and delete Bento Assets", MOSOrderMgtBentoAssetsMenu);
        #endregion

        #region Carton Types
        public static ApplicationPermissionsTree MOSOrderMgtCartonTypesMenu = new ApplicationPermissionsTree(id++, "Carton Types", "mosmgt.mealmgt.order.delivery.cartontypes", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtCartonTypesMenu = new ApplicationPermissionsTree(id++, "View Carton Types", "mosmgt.mealmgt.order.delivery.cartontypes.view", "Permission to view Carton Types", MOSOrderMgtCartonTypesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCartonTypesMenu = new ApplicationPermissionsTree(id++, "Manage Carton Types", "mosmgt.mealmgt.order.delivery.cartontypes.manage", "Permission to add, edit and delete Carton Types", MOSOrderMgtCartonTypesMenu);
        #endregion

        #region Carton Assets
        public static ApplicationPermissionsTree MOSOrderMgtCartonAssetsMenu = new ApplicationPermissionsTree(id++, "Carton Assets", "mosmgt.mealmgt.order.delivery.cartonassets", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtCartonAssetsMenu = new ApplicationPermissionsTree(id++, "View Carton Assets", "mosmgt.mealmgt.order.delivery.cartonassets.view", "Permission to view Carton Assets", MOSOrderMgtCartonAssetsMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCartonAssetsMenu = new ApplicationPermissionsTree(id++, "Manage Carton Assets", "mosmgt.mealmgt.order.delivery.cartonassets.manage", "Permission to add, edit and delete Carton Assets", MOSOrderMgtCartonAssetsMenu);
        #endregion

        #region Tracking Status
        public static ApplicationPermissionsTree MOSOrderMgtTrackingStatusMenu = new ApplicationPermissionsTree(id++, "Tracking Status", "mosmgt.mealmgt.order.delivery.trackingstatus", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtTrackingStatusMenu = new ApplicationPermissionsTree(id++, "View Tracking Status", "mosmgt.mealmgt.order.delivery.trackingstatus.view", "Permission to view Tracking Status", MOSOrderMgtTrackingStatusMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtTrackingStatusMenu = new ApplicationPermissionsTree(id++, "Manage Tracking Status", "mosmgt.mealmgt.order.delivery.trackingstatus.manage", "Permission to add, edit and delete Tracking Status", MOSOrderMgtTrackingStatusMenu);
        #endregion

        #region Delivery Orders
        public static ApplicationPermissionsTree MOSOrderMgtDeliveryOrdersMenu = new ApplicationPermissionsTree(id++, "Delivery Order", "mosmgt.mealmgt.order.delivery.deliveryorders", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtDeliveryOrdersMenu = new ApplicationPermissionsTree(id++, "View Delivery Order", "mosmgt.mealmgt.order.delivery.deliveryorders.view", "Permission to view Delivery Order", MOSOrderMgtDeliveryOrdersMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtDeliveryOrdersMenu = new ApplicationPermissionsTree(id++, "Manage Delivery Order", "mosmgt.mealmgt.order.delivery.deliveryorders.manage", "Permission to add, edit and delete Delivery Order", MOSOrderMgtDeliveryOrdersMenu);
        #endregion

        #region Dishing Process
        public static ApplicationPermissionsTree MOSOrderMgtDishingProcessMenu = new ApplicationPermissionsTree(id++, "Dishing Process", "mosmgt.mealmgt.order.delivery.dishingprocess", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtDishingProcessMenu = new ApplicationPermissionsTree(id++, "View Dishing Process", "mosmgt.mealmgt.order.delivery.dishingprocess.view", "Permission to view Dishing Process", MOSOrderMgtDishingProcessMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtDishingProcessMenu = new ApplicationPermissionsTree(id++, "Manage Dishing Process", "mosmgt.mealmgt.order.delivery.dishingprocess.manage", "Permission to view Dishing Process", MOSOrderMgtDishingProcessMenu);
        #endregion

        #region Packing Process
        public static ApplicationPermissionsTree MOSOrderMgtPackingProcessMenu = new ApplicationPermissionsTree(id++, "Packing Process", "mosmgt.mealmgt.order.delivery.packingprocess", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtPackingProcessMenu = new ApplicationPermissionsTree(id++, "View Packing Process", "mosmgt.mealmgt.order.delivery.packingprocess.view", "Permission to view Packing Process", MOSOrderMgtPackingProcessMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtPackingProcessMenu = new ApplicationPermissionsTree(id++, "Manage Packing Process", "mosmgt.mealmgt.order.delivery.packingprocess.manage", "Permission to add, edit and delete Packing Process", MOSOrderMgtPackingProcessMenu);
        #endregion

        #region Packing Process
        public static ApplicationPermissionsTree MOSOrderMgtDriversMenu = new ApplicationPermissionsTree(id++, "Drivers", "mosmgt.mealmgt.order.delivery.drivers", "", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtDriversMenu = new ApplicationPermissionsTree(id++, "View Drivers", "mosmgt.mealmgt.order.delivery.drivers.view", "Permission to view Drivers", MOSOrderMgtDriversMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtDriversMenu = new ApplicationPermissionsTree(id++, "Manage Drivers", "mosmgt.mealmgt.order.delivery.drivers.manage", "Permission to add, edit and delete Drivers", MOSOrderMgtDriversMenu);
        #endregion

        #endregion

        #region Caterers Management

        #region Caterers
        public static ApplicationPermissionsTree MOSOrderMgtCaterersMenu = new ApplicationPermissionsTree(id++, "Caterers", "mosmgt.settingmgt.caterers", "", ViewMOSCatererMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtCaterersMenu = new ApplicationPermissionsTree(id++, "View Caterers", "mosmgt.settingmgt.caterers.view", "Permission to view Caterers", MOSOrderMgtCaterersMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCaterersMenu = new ApplicationPermissionsTree(id++, "Manage Caterers", "mosmgt.settingmgt.caterers.manage", "Permission to add, edit and delete Caterers", MOSOrderMgtCaterersMenu);
        #endregion

        #region Dish Calendar
        public static ApplicationPermissionsTree MOSOrderMgtCatererDishCalendarMenu = new ApplicationPermissionsTree(id++, "Dish Calendar", "mosmgt.settingmgt.caterers.dishcalendar", "", MOSOrderMgtCaterersMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtCatererDishCalendarMenu = new ApplicationPermissionsTree(id++, "View Dish Calendar", "mosmgt.settingmgt.caterers.dishcalendar.view", "Permission to view Dish Calendar", MOSOrderMgtCatererDishCalendarMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCatererDishCalendarMenu = new ApplicationPermissionsTree(id++, "Manage Dish Calendar", "mosmgt.settingmgt.caterers.dishcalendar.manage", "Permission to manage Dish Calendar", MOSOrderMgtCatererDishCalendarMenu);
        #endregion

        #region Caterer Outlets
        public static ApplicationPermissionsTree MOSOrderMgtCatererOutletsMenu = new ApplicationPermissionsTree(id++, "Outlets", "mosmgt.settingmgt.caterers.outlets", "", MOSOrderMgtCaterersMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtCatererOutletsMenu = new ApplicationPermissionsTree(id++, "View Outlets", "mosmgt.settingmgt.caterers.outlets.view", "Permission to view Outlets", MOSOrderMgtCatererOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCatererOutletsMenu = new ApplicationPermissionsTree(id++, "Manage Outlets", "mosmgt.settingmgt.caterers.outlets.manage", "Permission to manage Outlets", MOSOrderMgtCatererOutletsMenu);
        #endregion

        #region Outlet Profiles
        public static ApplicationPermissionsTree MOSOrderMgtOutletProfilesMenu = new ApplicationPermissionsTree(id++, "Outlet Profiles", "mosmgt.settingmgt.outletprofiles", "", MOSOrderMgtCaterersMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtOutletProfilesMenu = new ApplicationPermissionsTree(id++, "View Outlet Profiles", "mosmgt.settingmgt.outletprofiles.view", "Permission to view Outlet Profiles", MOSOrderMgtOutletProfilesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtOutletProfilesMenu = new ApplicationPermissionsTree(id++, "Manage Outlet Profiles", "mosmgt.settingmgt.outletprofiles.manage", "Permission to add, edit and delete Outlet Profiles", MOSOrderMgtOutletProfilesMenu);
        #endregion

        #region Meal Types
        public static ApplicationPermissionsTree MOSOrderMgtCatererMealTypesMenu = new ApplicationPermissionsTree(id++, "Meal Types", "mosmgt.settingmgt.caterers.mealtypes", "", MOSOrderMgtCaterersMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtCatererMealTypesMenu = new ApplicationPermissionsTree(id++, "View Meal Types", "mosmgt.settingmgt.caterers.mealtypes.view", "Permission to view Meal Types", MOSOrderMgtCatererMealTypesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCatererMealTypesMenu = new ApplicationPermissionsTree(id++, "Manage Meal Types", "mosmgt.settingmgt.caterers.mealtypes.manage", "Permission to add, edit and delete Meal Types", MOSOrderMgtCatererMealTypesMenu);
        #endregion

        #region Meal Periods
        public static ApplicationPermissionsTree MOSOrderMgtCatererMealPeriodsMenu = new ApplicationPermissionsTree(id++, "Meal Periods", "mosmgt.settingmgt.caterers.mealperiods", "", MOSOrderMgtCaterersMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtCatererMealPeriodsMenu = new ApplicationPermissionsTree(id++, "View Meal Periods", "mosmgt.settingmgt.caterers.mealperiods.view", "Permission to view Meal Types", MOSOrderMgtCatererMealPeriodsMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCatererMealPeriodsMenu = new ApplicationPermissionsTree(id++, "Manage Meal Periods", "mosmgt.settingmgt.caterers.mealperiods.manage", "Permission to add, edit and delete Meal Types", MOSOrderMgtCatererMealPeriodsMenu);
        #endregion

        #region Dish Types
        public static ApplicationPermissionsTree MOSOrderMgtCatererDishTypesMenu = new ApplicationPermissionsTree(id++, "Dish Types", "mosmgt.settingmgt.caterers.dishtypes", "", MOSOrderMgtCaterersMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtCatererDishTypesMenu = new ApplicationPermissionsTree(id++, "View Dish Types", "mosmgt.settingmgt.caterers.dishtypes.view", "Permission to view Dish Types", MOSOrderMgtCatererDishTypesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCatererDishTypesMenu = new ApplicationPermissionsTree(id++, "Manage Dish Types", "mosmgt.settingmgt.caterers.dishtypes.manage", "Permission to add, edit and delete Dish Types", MOSOrderMgtCatererDishTypesMenu);
        #endregion

        #region Dishes
        public static ApplicationPermissionsTree MOSOrderMgtCatererDishesMenu = new ApplicationPermissionsTree(id++, "Dishes", "mosmgt.settingmgt.caterers.dishes", "", MOSOrderMgtCaterersMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtCatererDishesMenu = new ApplicationPermissionsTree(id++, "View Dishes", "mosmgt.settingmgt.caterers.dishes.view", "Permission to view Dishes", MOSOrderMgtCatererDishesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCatererDishesMenu = new ApplicationPermissionsTree(id++, "Manage Dishes", "mosmgt.settingmgt.caterers.dishes.manage", "Permission to add, edit and delete Dishes", MOSOrderMgtCatererDishesMenu);
        #endregion

        #region Dish Cycles
        public static ApplicationPermissionsTree MOSOrderMgtCatererDishCyclesMenu = new ApplicationPermissionsTree(id++, "Dish Cycles", "mosmgt.settingmgt.caterers.dishcycles", "", MOSOrderMgtCaterersMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtCatererDishCyclesMenu = new ApplicationPermissionsTree(id++, "View Dish Cycles", "mosmgt.settingmgt.caterers.dishcycles.view", "Permission to view Dishes", MOSOrderMgtCatererDishCyclesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCatererDishCyclesMenu = new ApplicationPermissionsTree(id++, "Manage Dish Cycles", "mosmgt.settingmgt.caterers.dishcycles.manage", "Permission to add, edit and delete Dishes", MOSOrderMgtCatererDishCyclesMenu);
        #endregion

        #region Routes
        public static ApplicationPermissionsTree MOSOrderMgtRoutesMenu = new ApplicationPermissionsTree(id++, "Routes", "mosmgt.mealmgt.order.delivery.routes", "", ViewMOSCatererMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtRoutesMenu = new ApplicationPermissionsTree(id++, "View Routes", "mosmgt.mealmgt.order.delivery.routes.view", "Permission to view Routes", MOSOrderMgtRoutesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtRoutesMenu = new ApplicationPermissionsTree(id++, "Manage Routes", "mosmgt.mealmgt.order.delivery.routes.manage", "Permission to add, edit and delete Routes", MOSOrderMgtRoutesMenu);
        #endregion

        #endregion

        #region Outlets Management

        #region Outlets

        public static ApplicationPermissionsTree MOSOrderMgtOutletsMenu = new ApplicationPermissionsTree(id++, "Outlets", "mosmgt.settingmgt.outlets.outlets", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtOutletsMenu = new ApplicationPermissionsTree(id++, "View Outlets", "mosmgt.settingmgt.outlets.view", "Permission to view Outlets", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtOutletsMenu = new ApplicationPermissionsTree(id++, "Manage Outlets", "mosmgt.settingmgt.outlets.manage", "Permission to add, edit and delete Outlets", ViewMOSSettingMgtMenu);
        #endregion

        #region Outlet Caterers
        public static ApplicationPermissionsTree MOSOutletMgtCaterersPermission = new ApplicationPermissionsTree(id++, "Outlet Caterers", "mosmgt.outletmgt.caterers", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtCaterersPermission = new ApplicationPermissionsTree(id++, "View Outlet Caterers", "mosmgt.outletmgt.caterers.view", "Permission to view Caterers", MOSOutletMgtCaterersPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtCaterersPermission = new ApplicationPermissionsTree(id++, "Manage Outlet Caterers", "mosmgt.outletmgt.caterers.manage", "Permission to add, edit and delete Caterers", MOSOutletMgtCaterersPermission);
        #endregion

        #region Outlet Stores
        public static ApplicationPermissionsTree MOSOutletMgtStoresPermission = new ApplicationPermissionsTree(id++, "Outlet Stores", "mosmgt.outletmgt.stores", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtStoresPermission = new ApplicationPermissionsTree(id++, "View Outlet Stores", "mosmgt.outletmgt.stores.view", "Permission to view stores", MOSOutletMgtStoresPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtStoresPermission = new ApplicationPermissionsTree(id++, "Manage Outlet Stores", "mosmgt.outletmgt.stores.manage", "Permission to add, edit and delete stores", MOSOutletMgtStoresPermission);
        #endregion

        #region Outlet Reports
        public static ApplicationPermissionsTree MOSOutletMgtReportsPermission = new ApplicationPermissionsTree(id++, "Outlet Reports", "mosmgt.outletmgt.reports", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtReportsPermission = new ApplicationPermissionsTree(id++, "View Outlet Reports", "mosmgt.outletmgt.reports.view", "Permission to view Outlet Order and Delivery Reports", MOSOutletMgtReportsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtMealSummaryPermission = new ApplicationPermissionsTree(id++, "Manage Outlet Meal Summary", "mosmgt.outletmgt.mealsummary.manage", "Permission to view Meal Summary Report", MOSOutletMgtReportsPermission);

        #endregion

        #region Meal Plan Groupings
        public static ApplicationPermissionsTree MOSOutletMgtTermsPermission = new ApplicationPermissionsTree(id++, "Meal Plan Groupings", "mosmgt.outletmgt.terms", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtTermsPermission = new ApplicationPermissionsTree(id++, "View Meal Plan Groupings", "mosmgt.outletmgt.terms.view", "Permission to view Meal Plan Groupings", MOSOutletMgtTermsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtTermsPermission = new ApplicationPermissionsTree(id++, "Manage Meal Plan Groupings", "mosmgt.outletmgt.terms.manage", "Permission to add, edit and delete Meal Plan Groupings", MOSOutletMgtTermsPermission);
        #endregion

        #region Class Batches
        public static ApplicationPermissionsTree MOSOutletMgtClassBatchesPermission = new ApplicationPermissionsTree(id++, "Class Batches", "mosmgt.outletmgt.classbatches", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtClassBatchesPermission = new ApplicationPermissionsTree(id++, "View Class Batches", "mosmgt.outletmgt.classbatches.view", "Permission to view Class Batches", MOSOutletMgtClassBatchesPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtClassBatchesPermission = new ApplicationPermissionsTree(id++, "Manage Class Batches", "mosmgt.outletmgt.classbatches.manage", "Permission to add, edit and delete Class Batches", MOSOutletMgtClassBatchesPermission);
        #endregion

        #region Class Levels
        public static ApplicationPermissionsTree MOSOutletMgtClassLevelsPermission = new ApplicationPermissionsTree(id++, "Class Levels", "mosmgt.outletmgt.levels", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtClassLevelsPermission = new ApplicationPermissionsTree(id++, "View Class Levels", "mosmgt.outletmgt.levels.view", "Permission to view Class Levels", MOSOutletMgtClassLevelsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtClassLevelsPermission = new ApplicationPermissionsTree(id++, "Manage Class Levels", "mosmgt.outletmgt.levels.manage", "Permission to add, edit and delete Class Levels", MOSOutletMgtClassLevelsPermission);
        #endregion

        #region Classes
        public static ApplicationPermissionsTree MOSOutletMgtClassesPermission = new ApplicationPermissionsTree(id++, "Classes", "mosmgt.outletmgt.classes", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtClassesPermission = new ApplicationPermissionsTree(id++, "View Classes", "mosmgt.outletmgt.classes.view", "Permission to view Classes", MOSOutletMgtClassesPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtClassesPermission = new ApplicationPermissionsTree(id++, "Manage Classes", "mosmgt.outletmgt.classes.manage", "Permission to add, edit and delete Classes", MOSOutletMgtClassesPermission);
        #endregion

        #region Students
        public static ApplicationPermissionsTree MOSOutletMgtStudentsPermission = new ApplicationPermissionsTree(id++, "Students", "mosmgt.outletmgt.students", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtStudentsPermission = new ApplicationPermissionsTree(id++, "View Students", "mosmgt.outletmgt.students.view", "Permission to view Students", MOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtStudentsPermission = new ApplicationPermissionsTree(id++, "Manage Students", "mosmgt.outletmgt.students.manage", "Permission to manage Students", MOSOutletMgtStudentsPermission);

        #region Students Directory

        public static ApplicationPermissionsTree ViewMOSOutletStudentMgtImportPermission = new ApplicationPermissionsTree(id++, "Import Students", "mosmgt.outletmgt.students.import.view", "Permission to import Students", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionNew = new ApplicationPermissionsTree(id++, "Create Students", "mosmgt.outletmgt.students.manage.create", "Permission to add, edit and delete Students", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionEdit = new ApplicationPermissionsTree(id++, "Update Students", "mosmgt.outletmgt.students.manage.edit", "Permission to add, edit and delete Students", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionDelete = new ApplicationPermissionsTree(id++, "Delete Students", "mosmgt.outletmgt.students.manage.delete", "Permission to add, edit and delete Students", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionCreateAccount = new ApplicationPermissionsTree(id++, "Create Student Accounts", "mosmgt.outletmgt.students.manage.account.create", "Permission to add, edit and delete Student Accounts Creation", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionOrderMgt = new ApplicationPermissionsTree(id++, "Student Orders Management", "mosmgt.outletmgt.students.manage.order", "Permission to add, edit and delete Student Orders", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionVoucher = new ApplicationPermissionsTree(id++, "Student Vouchers Management", "mosmgt.outletmgt.students.manage.voucher", "Permission to add, edit and delete Student Vouchers", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionNotification = new ApplicationPermissionsTree(id++, "Student Notifications Management", "mosmgt.outletmgt.students.manage.notification", "Permission to add, edit and delete Student Notifications", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionCalendar = new ApplicationPermissionsTree(id++, "Student Calendars Management", "mosmgt.outletmgt.students.manage.calendar", "Permission to add, edit and delete Student Calendars", ViewMOSOutletMgtStudentsPermission);

        #endregion

        #endregion

        #region FAS
        public static ApplicationPermissionsTree MOSOutletMgtFasPermission = new ApplicationPermissionsTree(id++, "FAS", "mosmgt.outletmgt.fas", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtFasPermission = new ApplicationPermissionsTree(id++, "View FAS", "mosmgt.outletmgt.fas.view", "Permission to view FAS", MOSOutletMgtFasPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtFasPermission = new ApplicationPermissionsTree(id++, "Manage FAS", "mosmgt.outletmgt.fas.manage", "Permission to add, edit and delete FAS", MOSOutletMgtFasPermission);
        #endregion

        #region Student Groups
        public static ApplicationPermissionsTree MOSOutletMgtStudentGroupsPermission = new ApplicationPermissionsTree(id++, "Student Groups", "mosmgt.outletmgt.studentgroups", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtStudentGroupsPermission = new ApplicationPermissionsTree(id++, "View Student Groups", "mosmgt.outletmgt.studentgroups.view", "Permission to view Student Groups", MOSOutletMgtStudentGroupsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtStudentGroupsPermission = new ApplicationPermissionsTree(id++, "Manage Student Groups", "mosmgt.outletmgt.studentgroups.manage", "Permission to add, edit and delete Student Groups", MOSOutletMgtStudentGroupsPermission);
        #endregion

        #region Groups
        public static ApplicationPermissionsTree MOSOutletMgtMenusPermission = new ApplicationPermissionsTree(id++, "Menu Groups", "mosmgt.outletmgt.menus", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtMenusPermission = new ApplicationPermissionsTree(id++, "View Groups", "mosmgt.outletmgt.menus.view", "Permission to view Groups", MOSOutletMgtMenusPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtMenusPermission = new ApplicationPermissionsTree(id++, "Manage Groups", "mosmgt.outletmgt.menus.manage", "Permission to add, edit and delete Groups", MOSOutletMgtMenusPermission);
        #endregion

        #region Order Cancellations
        public static ApplicationPermissionsTree MOSOutletMgtCancellationsPermission = new ApplicationPermissionsTree(id++, "Order Cancellations", "mosmgt.outletmgt.cancellations", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtCancellationsPermission = new ApplicationPermissionsTree(id++, "View Order Cancellations", "mosmgt.outletmgt.cancellations.view", "Permission to view Order Cancellations", MOSOutletMgtCancellationsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtCancellationsPermission = new ApplicationPermissionsTree(id++, "Manage Order Cancellations", "mosmgt.outletmgt.cancellations.manage", "Permission to manage Order Cancellations", MOSOutletMgtCancellationsPermission);
        #endregion

        #region Portal Contents
        public static ApplicationPermissionsTree MOSOutletMgtPortalContentsPermission = new ApplicationPermissionsTree(id++, "Portal Contents", "mosmgt.outletmgt.portalcontents", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtPortalContentsPermission = new ApplicationPermissionsTree(id++, "View Portal Contents", "mosmgt.outletmgt.portalcontents.view", "Permission to view Portal Contents", MOSOutletMgtPortalContentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtPortalContentsPermission = new ApplicationPermissionsTree(id++, "Manage Portal Contents", "mosmgt.outletmgt.portalcontents.manage", "Permission to add, edit and delete Portal Contents", MOSOutletMgtPortalContentsPermission);
        #endregion

        #region Email Templates
        public static ApplicationPermissionsTree MOSOutletMgtEmailTemplatesPermission = new ApplicationPermissionsTree(id++, "Email Templates", "mosmgt.outletmgt.emailtemplates", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtEmailTemplatesPermission = new ApplicationPermissionsTree(id++, "View Email Templates", "mosmgt.outletmgt.emailtemplates.view", "Permission to view Email Templates", MOSOutletMgtEmailTemplatesPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtEmailTemplatesPermission = new ApplicationPermissionsTree(id++, "Manage Email Templates", "mosmgt.outletmgt.emailtemplates.manage", "Permission to add, edit and delete Email Templates", MOSOutletMgtEmailTemplatesPermission);
        #endregion

        #region Meal Allocations
        public static ApplicationPermissionsTree MOSOutletMgtMealAllocationsPermission = new ApplicationPermissionsTree(id++, "Meal Allocations", "mosmgt.outletmgt.mealallocations", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtMealAllocationsPermission = new ApplicationPermissionsTree(id++, "View Meal Allocations", "mosmgt.outletmgt.mealallocations.view", "Permission to view Meal Allocations", MOSOutletMgtMealAllocationsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletMgtMealAllocationsPermission = new ApplicationPermissionsTree(id++, "Manage Meal Allocations", "mosmgt.outletmgt.mealallocations.manage", "Permission to view Meal Allocations", MOSOutletMgtMealAllocationsPermission);
        #endregion

        #region Packing Allocations
        public static ApplicationPermissionsTree MOSOutletMgtPackingAllocationsPermission = new ApplicationPermissionsTree(id++, "Packing Allocations", "mosmgt.outletmgt.packingallocations", "", ViewMOSOutletMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOutletMgtPackingAllocationsPermission = new ApplicationPermissionsTree(id++, "View Packing Allocations", "mosmgt.outletmgt.packingallocations.view", "Permission to view Packing Allocations", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtPackingAllocationsPermission = new ApplicationPermissionsTree(id++, "Manage Packing Allocations", "mosmgt.outletmgt.packingallocations.manage", "Permission to add, edit and delete Packing Allocations", ViewMOSOrderMgtOutletsMenu);
        #endregion

        #endregion

        #region Settings Management

        #region Stores
        public static ApplicationPermissionsTree MOSOrderMgtStoreInfoMenu = new ApplicationPermissionsTree(id++, "Stores", "mosmgt.settingmgt.storeinfo", "", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtStoreInfoMenu = new ApplicationPermissionsTree(id++, "View Stores", "mosmgt.settingmgt.storeinfo.view", "Permission to view Store Info", MOSOrderMgtStoreInfoMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtStoreInfoMenu = new ApplicationPermissionsTree(id++, "Manage Stores", "mosmgt.settingmgt.storeinfo.manage", "Permission to view Store Info", MOSOrderMgtStoreInfoMenu);
        #endregion

        #region Payment Types
        public static ApplicationPermissionsTree MOSOrderMgtPaymentTypesMenu = new ApplicationPermissionsTree(id++, "Payment Types", "mosmgt.settingmgt.payment.paymenttype", "", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtPaymentTypesMenu = new ApplicationPermissionsTree(id++, "View Payment Type", "mosmgt.settingmgt.payment.paymenttype.view", "Permission to view Payment Type", MOSOrderMgtPaymentTypesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtPaymentTypesMenu = new ApplicationPermissionsTree(id++, "Manage Payment Type", "mosmgt.settingmgt.payment.paymenttype.manage", "Permission to view Payment Type", MOSOrderMgtPaymentTypesMenu);
        #endregion

        #region Transaction Fees
        public static ApplicationPermissionsTree MOSOrderMgtTransactionFeesMenu = new ApplicationPermissionsTree(id++, "Transaction Fees", "mosmgt.settingmgt.payment.transactionfee", "", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtTransactionFeesMenu = new ApplicationPermissionsTree(id++, "View Transaction Fee", "mosmgt.settingmgt.payment.transactionfee.view", "Permission to view Transaction Fee", MOSOrderMgtTransactionFeesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtTransactionFeesMenu = new ApplicationPermissionsTree(id++, "Manage Transaction Fee", "mosmgt.settingmgt.payment.transactionfee.manage", "Permission to add, edit and delete Transaction Fee", MOSOrderMgtTransactionFeesMenu);
        #endregion

        #region Transaction Fees
        public static ApplicationPermissionsTree MOSOrderMgtVoucherTypesMenu = new ApplicationPermissionsTree(id++, "Voucher Types", "mosmgt.settingmgt.payment.vouchertype", "", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtVoucherTypesMenu = new ApplicationPermissionsTree(id++, "View Voucher Type", "mosmgt.settingmgt.payment.vouchertype.view", "Permission to view Voucher Type", MOSOrderMgtVoucherTypesMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtVoucherTypesMenu = new ApplicationPermissionsTree(id++, "Manage Voucher Type", "mosmgt.settingmgt.payment.vouchertype.manage", "Permission to view Voucher Type", MOSOrderMgtVoucherTypesMenu);
        #endregion

        #region Vouchers
        public static ApplicationPermissionsTree MOSOrderMgtVouchersMenu = new ApplicationPermissionsTree(id++, "Voucher Types", "mosmgt.settingmgt.payment.voucher", "", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtVouchersMenu = new ApplicationPermissionsTree(id++, "View Voucher", "mosmgt.settingmgt.payment.voucher.view", "Permission to view Voucher", MOSOrderMgtVouchersMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtVouchersMenu = new ApplicationPermissionsTree(id++, "Manage Voucher", "mosmgt.settingmgt.payment.voucher.manage", "Permission to view Voucher", MOSOrderMgtVouchersMenu);
        #endregion

        #region Voucher Types
        public static ApplicationPermissionsTree MOSOrderMgtWaiversMenu = new ApplicationPermissionsTree(id++, "Voucher Types", "mosmgt.settingmgt.payment.waiver", "", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ViewMOSOrderMgtWaiversMenu = new ApplicationPermissionsTree(id++, "View Waiver", "mosmgt.settingmgt.payment.waiver.view", "Permission to view Waiver", MOSOrderMgtWaiversMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtWaiversMenu = new ApplicationPermissionsTree(id++, "Manage Waiver", "mosmgt.settingmgt.payment.waiver.manage", "Permission to view Waiver", MOSOrderMgtWaiversMenu);
        #endregion

        #region Contact Us Subjects
        public static ApplicationPermissionsTree MOSOrderMgtContactUsSubjectsMenu = new ApplicationPermissionsTree(id++, "Contact Us Subjects", "mosmgt.settingmgt.contactus.subject", "", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtContactUsSubjectsPermission = new ApplicationPermissionsTree(id++, "Manage Contact Us Subjects", "mosmgt.settingmgt.contactus.subject.manage", "Permission to add, edit and delete Contact Us Subjects", MOSOrderMgtContactUsSubjectsMenu);
        #endregion

        #region Contact Us Questions
        public static ApplicationPermissionsTree MOSOrderMgtContactUsQuestionsMenu = new ApplicationPermissionsTree(id++, "Contact Us Questions", "mosmgt.settingmgt.contactus.question", "", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtContactUsQuestionsPermission = new ApplicationPermissionsTree(id++, "Manage Contact Us Questions", "mosmgt.settingmgt.contactus.question.manage", "Permission to add, edit and delete Contact Us Questions", MOSOrderMgtContactUsQuestionsMenu);
        #endregion

        #region Notification Settings
        public static ApplicationPermissionsTree MOSOrderMgtNotificationSettingsMenu = new ApplicationPermissionsTree(id++, "Notification Settings", "mosmgt.settingmgt.notifications.settings", "", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtNotificationSettingsPermission = new ApplicationPermissionsTree(id++, "Manage Notification Settings", "mosmgt.settingmgt.notifications.settings.manage", "Permission to add, edit and delete Notification Settings", MOSOrderMgtNotificationSettingsMenu);
        #endregion

        #region Notification Events
        public static ApplicationPermissionsTree MOSOrderMgtNotificationEventsMenu = new ApplicationPermissionsTree(id++, "Notification Events", "mosmgt.settingmgt.notifications.events", "", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtNotificationEventsPermission = new ApplicationPermissionsTree(id++, "Manage Notification Events", "mosmgt.settingmgt.notifications.events.manage", "Permission to add, edit and delete Notification Events", MOSOrderMgtNotificationEventsMenu);
        #endregion

        #region Cancel Requests
        public static ApplicationPermissionsTree MOSOrderMgtCancelRequestsMenu = new ApplicationPermissionsTree(id++, "Cancel Requests", "mosmgt.settingmgt.orders.cancelrequest", "", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCancelRequestsPermission = new ApplicationPermissionsTree(id++, "Manage Cancel Requests", "mosmgt.settingmgt.orders.cancelrequest.manage", "Permission to add, edit and delete Cancel Requests", MOSOrderMgtCancelRequestsMenu);
        #endregion

        #endregion

        #region Reports Management

        #region Order Portal Login
        public static ApplicationPermissionsTree MOSreportMgtPortalLoginMenu = new ApplicationPermissionsTree(id++, "Portal Login", "mosmgt.reportmgt.portallogin", "", ViewMOSreportMgtMenu);
        public static ApplicationPermissionsTree ViewMOSreportMgtPortalLoginMenu = new ApplicationPermissionsTree(id++, "View Portal Login Report", "mosmgt.reportmgt.portallogin.view", "Permission to view Order Portal Login Report", MOSreportMgtPortalLoginMenu);
        #endregion

        #region Order Report
        public static ApplicationPermissionsTree MOSreportMgtOrderReportMenu = new ApplicationPermissionsTree(id++, "Order Report", "mosmgt.reportmgt.order", "", ViewMOSreportMgtMenu);
        public static ApplicationPermissionsTree ViewMOSreportMgtOrderReportMenu = new ApplicationPermissionsTree(id++, "View Order Report", "mosmgt.reportmgt.order.view", "Permission to view Order Report", MOSreportMgtOrderReportMenu);
        #endregion

        #region Order Cancellation Report
        public static ApplicationPermissionsTree MOSreportMgtOrderCancellationReportMenu = new ApplicationPermissionsTree(id++, "Order Cancellation Report", "mosmgt.reportmgt.ordercancellation", "", ViewMOSreportMgtMenu);
        public static ApplicationPermissionsTree ViewMOSreportMgtOrderCancellationReportMenu = new ApplicationPermissionsTree(id++, "View Order Cancellation Report", "mosmgt.reportmgt.ordercancellation.view", "Permission to view Order Cancellation Report", MOSreportMgtOrderCancellationReportMenu);
        #endregion

        #region User Account Report
        public static ApplicationPermissionsTree MOSreportMgtUserAccountReportMenu = new ApplicationPermissionsTree(id++, "User Account Report", "mosmgt.reportmgt.useraccount", "", ViewMOSreportMgtMenu);
        public static ApplicationPermissionsTree ViewMOSreportMgtUserAccountReportMenu = new ApplicationPermissionsTree(id++, "View User Account Report", "mosmgt.reportmgt.useraccount.view", "Permission to view User Account Report", MOSreportMgtUserAccountReportMenu);
        #endregion

        #region Role Report
        public static ApplicationPermissionsTree MOSreportMgtRoleReportMenu = new ApplicationPermissionsTree(id++, "User Role Report", "mosmgt.reportmgt.role", "", ViewMOSreportMgtMenu);
        public static ApplicationPermissionsTree ViewMOSreportMgtRoleReportMenu = new ApplicationPermissionsTree(id++, "View Role Report", "mosmgt.reportmgt.role.view", "Permission to view Role Report", MOSreportMgtRoleReportMenu);
        #endregion

        #region User Activity Report
        public static ApplicationPermissionsTree MOSreportMgtUserActivityReportMenu = new ApplicationPermissionsTree(id++, "User Activity Report", "mosmgt.reportmgt.useractivity", "", ViewMOSreportMgtMenu);
        public static ApplicationPermissionsTree ViewMOSreportMgtUserActivityReportMenu = new ApplicationPermissionsTree(id++, "View Activity Report", "mosmgt.reportmgt.useractivity.view", "Permission to view User Activity Report", MOSreportMgtUserActivityReportMenu);
        #endregion

        #region Order Collection Report
        public static ApplicationPermissionsTree MOSreportMgtOrderCollectionReportMenu = new ApplicationPermissionsTree(id++, "Order Collection Report", "mosmgt.outletmgt.reports.ordercollection", "", ViewMOSreportMgtMenu);
        public static ApplicationPermissionsTree ViewMOSreportMgtOrderCollectionReportMenu = new ApplicationPermissionsTree(id++, "View Order Collection Report", "mosmgt.outletmgt.reports.ordercollection.view", "Permission to view User Activity Report", MOSreportMgtOrderCollectionReportMenu);
        #endregion

        #endregion

        #endregion


        //#region Dashboard ACL
        //public const string DashboardMenuPermissionGroupName = "Dashboard Permissions";
        //public static ApplicationPermissionsTree ViewDashboardMenu = new ApplicationPermissionsTree(47, "View Dashboard", "dashboardmenu.view", DashboardMenuPermissionGroupName, "Permission to view dashboard", Root);

        //public const string DashboardPermissionGroupName = "Dashboard";
        //public static ApplicationPermissionsTree ViewDashboard = new ApplicationPermissionsTree(48, "View Dashboard", "dashboard.view", DashboardPermissionGroupName, "Permission to view dashboard", ViewDashboardMenu);
        //public static ApplicationPermissionsTree ViewSignageDashboard = new ApplicationPermissionsTree(49, "View Signage Dashboard", "signagedashboard.view", DashboardPermissionGroupName, "Permission to view signage dashboard", ViewDashboardMenu);

        //#endregion

        //#region Devices ACL
        //public const string DeviceMenuPermissionGroupName = "Device Permissions";
        //public static ApplicationPermissionsTree ViewDeviceMenu = new ApplicationPermissionsTree(50, "View Device", "devicemenu.view", DashboardMenuPermissionGroupName, "Permission to view device", Root);

        //public const string DevicesPermissionGroupName = "Devices";
        //public static ApplicationPermissionsTree ViewDevices = new ApplicationPermissionsTree(51, "View Devices", "devices.view", DevicesPermissionGroupName, "Permission to view available devices", ViewDeviceMenu);
        //public static ApplicationPermissionsTree ManageDevices = new ApplicationPermissionsTree(52, "Manage Devices", "devices.manage", DevicesPermissionGroupName, "Permission to create, delete and modify devices", ViewDeviceMenu);

        //#endregion

        //#region Reservation ACL
        //public const string ReservationMenuPermissionGroupName = "Reservation Permissions";
        //public static ApplicationPermissionsTree ViewReservationMenu = new ApplicationPermissionsTree(53, "View Reservation", "reservationmenu.view", ReservationMenuPermissionGroupName, "Permission to view reservation", Root);

        //public const string ReservationsPermissionGroupName = "Reservations";
        //public static ApplicationPermissionsTree AddReservations = new ApplicationPermissionsTree(54, "Add Reservations", "reservations.add", ReservationsPermissionGroupName, "Permission to add reservations", ViewReservationMenu);
        //public static ApplicationPermissionsTree ManageReservations = new ApplicationPermissionsTree(55, "Manage Reservations", "reservations.manage", ReservationsPermissionGroupName, "Permission to create, delete and modify reservations", ViewReservationMenu);

        //#endregion

        //#region Reports ACL
        //public const string ReportMenuPermissionGroupName = "Reports Permissions";
        //public static ApplicationPermissionsTree ViewReportMenu = new ApplicationPermissionsTree(56, "View Reports", "reportsmenu.view", ReportMenuPermissionGroupName, "Permission to view reports", Root);

        //public const string ReportsPermissionGroupName = "Reports Permissions";
        //public static ApplicationPermissionsTree ViewReports = new ApplicationPermissionsTree(57, "View Reports", "reports.view", ReportsPermissionGroupName, "Permission to view reports", ViewReportMenu);
        //public static ApplicationPermissionsTree ViewReservationReports = new ApplicationPermissionsTree(58, "View Reservation Reports", "reports.reservation.view", ReportsPermissionGroupName, "Permission to view reservation reports", ViewReportMenu);
        //public static ApplicationPermissionsTree ViewVehicleLogReports = new ApplicationPermissionsTree(59, "View Vehicle Log Reports", "reports.vehiclelog.view", ReportsPermissionGroupName, "Permission to view vehicle log reports", ViewReportMenu);

        //#endregion

        static ApplicationPermissionsTrees()
        {
            ApplicationPermissionsTree allPermissions = new ApplicationPermissionsTree();

            //Root.Children = new List<ApplicationPermissionsTree>
            //{

            //    ViewAccountsMenu,
            //    ViewFRSMenu,
            //    ViewDashboardMenu,
            //    ViewDeviceMenu,
            //    ViewReservationMenu,
            //    ViewReportMenu
            //};
            //Root.Children = PermissionsList();
            //allPermissions = Root;

            AllPermissions = PermissionsListFromJson().AsReadOnly();// PermissionsList().AsReadOnly();
        }

        public static ApplicationPermissionsTree GetPermissionByName(string permissionName)
        {
            return AllPermissions.Where(p => p.Name == permissionName).SingleOrDefault();
        }

        public static ApplicationPermissionsTree GetPermissionByValue(string permissionValue)
        {
            return AllPermissions.Where(p => p.Value == permissionValue).SingleOrDefault();
        }

        public static string[] GetAllPermissionValues()
        {
            return AllPermissions.Select(p => p.Value).ToArray();
        }

        public static string[] GetAdministrativePermissionValues()
        {
            return new string[] { SSManageUserMenu, SSManageRoleMenu, SSAssignRoleMenu };
        }

        static List<ApplicationPermissionsTree> PermissionsList()
        {
            return new List<ApplicationPermissionsTree>()
            {
                Root,
                ViewFRSMenu,
                ViewResourceBookingMenu,
                RBFacilitiesMenu,
                RBManageFacilities,
                RBFacilityTypesMenu,
                RBManageFacilityTypes,
                RBCalendarMenu,
                RBSmartRoomSchedulerLogsMenu,

                ACUserPhonebookMenu,
                ACUserPhonebookManage,

                ViewAccessControlMenu,
                ACVehiclesMenu,
                ACManageUserVehicles,

                ViewWorkforceManagementMenu,
                WFDesignationMenu,
                WFEmployeeRosterMenu,
                WFEmployeeMasterMenu,

                ViewSignageMenu,

                ViewContentManagementMenu,
                CMMediaGroupsMenu,
                CMMediaLibrariesMenu,
                CMPlaylistsMenu,
                CMComponentsMenu,
                CMComponentCreate,
                CMComponentUpdate,
                CMComponentDelete,
                CMCompilationsMenu,
                CMCompilationCreate,
                CMCompilationUpdate,
                CMCompilationDelete,
                CMPublicationsMenu,
                CMPublicationApprove,
                CMPublicationEmail,
                CMPublicationCreate,
                CMPublicationUpdate,
                CMPublicationDelete,

                ViewDirectoryListingMenu,
                DLDirectoryCategoryMenu,
                DLDirectoryListingMenu,
                DLDLBuildingMenu,
                DLFloorMenu,
                DLMapMenu,

                ViewQueueDisplayInterfaceMenu,
                QDIQueueMatrixMenu,
                QDIQueueDisplayLogsMenu,

                ViewFacilityBookingInterfaceMenu,
                FBIResourceMatrixMenu,
                FBIInteraceLogsMenu,

                ViewDeviceManagementMenu,
                DMDevicesMenu,
                DMDevicesManageMenu,
                DMApprovalMenu,
                DMEMSMenu,
                DMEMSProfileMenu,
                DMEMSScheduleMenu,

                ViewEpaperManagementMenu,
                EPTemplateMenu,

                ViewReportsMenu,
                ViewFacilityManagementMenu,
                RVehicleLogsMenu,
                ROccupancyLogsMenu,

                ViewSgnKiosksMenu,
                SKUpDownTimeLogsMenu,

                ViewAssetManagementMenu,
                AMAssetTypeMenu,
                AMAssetTypeManage,
                AMAssetModelMenu,
                AMAssetModelManage,
                AMAssetMenu,
                AMAssetManage,
                AMServiceContractMenu,
                AMServiceContractManage,

                ViewSystemSettingsMenu,
                SSLocationTreeMenu,
                SSLocationTreeManage,
                SSInstitutionMenu,
                SSInstitutionManage,
                SSDepartmentMenu,
                SSDepartmentManage,
                SSBuildingMenu,
                SSUserGroupMenu,
                SSUserGroupManage,
                SSImageReferenceTypeMenu,
                SSImageReferenceTypeManage,
                SSImageReferenceColorMenu,
                SSImageReferenceColorManage,
                SSDeviceTypeMenu,
                SSDeviceTypeManage,

                SSUserMenu,
                SSViewUserMenu,
                SSManageUserMenu,

                SSRoleMenu,
                SSViewRoleMenu,
                SSManageRoleMenu,
                SSAssignRoleMenu,

                SSAuditMenu,
                SSAuthLogMenu,
                SSDataLogMenu,

                SSApplicationSettingMenu,
                SSModuleSettingMenu,
                SSEmailQueueMenu,
                SSDashboardMenu,
                SSDSignage,

                SSDCPreviewSignage,
                SSDCRebootSignage,
                SSDCRefreshSignage,
                SSDCScreenshotSignage,
                SSDCPushMessageSignage,
                SSDCDeviceInfoSignage,
                SSDCPublicationSignage,

                SSDUpcomingEvent,
                SSDAvailableRoom,
                SSDApproval,


                //MEAL ORDER SYSTEM
                ViewMOSMealMgtAccountMgtMenu,

                ViewMOSCatererMgtMenu,
                ViewMOSOutletMgtMenu,

                ViewMOSSettingMgtMenu,
                ViewMOSreportMgtMenu,

                MOSOrderMgtRestrictionTypesMenu,
                ViewMOSOrderMgtRestrictionTypesMenu,
                ManageMOSOrderMgtRestrictionTypesMenu,

                MOSOrderMgtRestrictionsMenu,
                ViewMOSOrderMgtRestrictionsMenu,
                ManageMOSOrderMgtRestrictionsMenu,

                MOSOrderMgtCuisinesMenu,
                ViewMOSOrderMgtCuisinesMenu,
                ManageMOSOrderMgtCuisinesMenu,

                MOSOrderMgtDeliveryMenu,
                ViewMOSOrderMgtDeliveryMenu,
                ManageMOSOrderMgtDeliveryMenu,

                MOSOrderMgtBentoBoxTypesMenu,
                ViewMOSOrderMgtBentoBoxTypesMenu,
                ManageMOSOrderMgtBentoBoxTypesMenu,

                MOSOrderMgtBentoAssetsMenu,
                ViewMOSOrderMgtBentoAssetsMenu,
                ManageMOSOrderMgtBentoAssetsMenu,

                MOSOrderMgtCartonTypesMenu,
                ViewMOSOrderMgtCartonTypesMenu,
                ManageMOSOrderMgtCartonTypesMenu,

                MOSOrderMgtCartonAssetsMenu,
                ViewMOSOrderMgtCartonAssetsMenu,
                ManageMOSOrderMgtCartonAssetsMenu,

                MOSOrderMgtTrackingStatusMenu,
                ViewMOSOrderMgtTrackingStatusMenu,
                ManageMOSOrderMgtTrackingStatusMenu,

                MOSOrderMgtDeliveryOrdersMenu,
                ViewMOSOrderMgtDeliveryOrdersMenu,
                ManageMOSOrderMgtDeliveryOrdersMenu,

                MOSOrderMgtDishingProcessMenu,
                ViewMOSOrderMgtDishingProcessMenu,
                ManageMOSOrderMgtDishingProcessMenu,

                MOSOrderMgtPackingProcessMenu,
                ViewMOSOrderMgtPackingProcessMenu,
                ManageMOSOrderMgtPackingProcessMenu,

                MOSOrderMgtDriversMenu,
                ViewMOSOrderMgtDriversMenu,
                ManageMOSOrderMgtDriversMenu,

                MOSOrderMgtCaterersMenu,
                ViewMOSOrderMgtCaterersMenu,
                ManageMOSOrderMgtCaterersMenu,

                MOSOrderMgtCatererDishCalendarMenu,
                ViewMOSOrderMgtCatererDishCalendarMenu,
                ManageMOSOrderMgtCatererDishCalendarMenu,

                MOSOrderMgtCatererOutletsMenu,
                ViewMOSOrderMgtCatererOutletsMenu,
                ManageMOSOrderMgtCatererOutletsMenu,

                MOSOrderMgtCatererMealTypesMenu,
                ViewMOSOrderMgtCatererMealTypesMenu,
                ManageMOSOrderMgtCatererMealTypesMenu,

                MOSOrderMgtCatererMealPeriodsMenu,
                ViewMOSOrderMgtCatererMealPeriodsMenu,
                ManageMOSOrderMgtCatererMealPeriodsMenu,

                MOSOrderMgtCatererDishTypesMenu,
                ViewMOSOrderMgtCatererDishTypesMenu,
                ManageMOSOrderMgtCatererDishTypesMenu,

                MOSOrderMgtCatererDishesMenu,
                ViewMOSOrderMgtCatererDishesMenu,
                ManageMOSOrderMgtCatererDishesMenu,

                MOSOrderMgtCatererDishCyclesMenu,
                ViewMOSOrderMgtCatererDishCyclesMenu,
                ManageMOSOrderMgtCatererDishCyclesMenu,


                MOSOrderMgtRoutesMenu,
                ViewMOSOrderMgtRoutesMenu,
                ManageMOSOrderMgtRoutesMenu,

                MOSOrderMgtOutletProfilesMenu,
                ViewMOSOrderMgtOutletProfilesMenu,
                ManageMOSOrderMgtOutletProfilesMenu,

                MOSOrderMgtOutletsMenu,
                ViewMOSOrderMgtOutletsMenu,
                ManageMOSOrderMgtOutletsMenu,

                MOSOutletMgtCaterersPermission,
                ViewMOSOutletMgtCaterersPermission,
                ManageMOSOutletMgtCaterersPermission,

                MOSOutletMgtStoresPermission,
                ViewMOSOutletMgtStoresPermission,
                ManageMOSOutletMgtStoresPermission,

                MOSOutletMgtReportsPermission,
                ViewMOSOutletMgtReportsPermission,
                ManageMOSOutletMgtMealSummaryPermission,

                MOSOutletMgtTermsPermission,
                ViewMOSOutletMgtTermsPermission,
                ManageMOSOutletMgtTermsPermission,

                MOSOutletMgtClassBatchesPermission,
                ViewMOSOutletMgtClassBatchesPermission,
                ManageMOSOutletMgtClassBatchesPermission,

                MOSOutletMgtClassLevelsPermission,
                ViewMOSOutletMgtClassLevelsPermission,
                ManageMOSOutletMgtClassLevelsPermission,

                MOSOutletMgtClassesPermission,
                ViewMOSOutletMgtClassesPermission,
                ManageMOSOutletMgtClassesPermission,

                MOSOutletMgtStudentsPermission,
                ViewMOSOutletMgtStudentsPermission,
                ManageMOSOutletMgtStudentsPermission,

                ViewMOSOutletStudentMgtImportPermission,
                ManageMOSOutletStudentMgtPermissionNew,
                ManageMOSOutletStudentMgtPermissionEdit,
                ManageMOSOutletStudentMgtPermissionDelete,
                ManageMOSOutletStudentMgtPermissionCreateAccount,
                ManageMOSOutletStudentMgtPermissionOrderMgt,
                ManageMOSOutletStudentMgtPermissionVoucher,
                ManageMOSOutletStudentMgtPermissionNotification,
                ManageMOSOutletStudentMgtPermissionCalendar,

                MOSOutletMgtFasPermission,
                ViewMOSOutletMgtFasPermission,
                ManageMOSOutletMgtFasPermission,

                MOSOutletMgtStudentGroupsPermission,
                ViewMOSOutletMgtStudentGroupsPermission,
                ManageMOSOutletMgtStudentGroupsPermission,

                MOSOutletMgtMenusPermission,
                ViewMOSOutletMgtMenusPermission,
                ManageMOSOutletMgtMenusPermission,

                MOSOutletMgtCancellationsPermission,
                ViewMOSOutletMgtCancellationsPermission,
                ManageMOSOutletMgtCancellationsPermission,

                MOSOutletMgtPortalContentsPermission,
                ViewMOSOutletMgtPortalContentsPermission,
                ManageMOSOutletMgtPortalContentsPermission,

                MOSOutletMgtEmailTemplatesPermission,
                ViewMOSOutletMgtEmailTemplatesPermission,
                ManageMOSOutletMgtEmailTemplatesPermission,

                MOSOutletMgtMealAllocationsPermission,
                ViewMOSOutletMgtMealAllocationsPermission,
                ManageMOSOutletMgtMealAllocationsPermission,

                MOSOutletMgtPackingAllocationsPermission,
                ViewMOSOutletMgtPackingAllocationsPermission,
                ManageMOSOutletMgtPackingAllocationsPermission,

                MOSOrderMgtStoreInfoMenu,
                ViewMOSOrderMgtStoreInfoMenu,
                ManageMOSOrderMgtStoreInfoMenu,

                MOSOrderMgtPaymentTypesMenu,
                ViewMOSOrderMgtPaymentTypesMenu,
                ManageMOSOrderMgtPaymentTypesMenu,

                MOSOrderMgtTransactionFeesMenu,
                ViewMOSOrderMgtTransactionFeesMenu,
                ManageMOSOrderMgtTransactionFeesMenu,

                MOSOrderMgtVoucherTypesMenu,
                ViewMOSOrderMgtVoucherTypesMenu,
                ManageMOSOrderMgtVoucherTypesMenu,

                MOSOrderMgtVouchersMenu,
                ViewMOSOrderMgtVouchersMenu,
                ManageMOSOrderMgtVouchersMenu,

                MOSOrderMgtWaiversMenu,
                ViewMOSOrderMgtWaiversMenu,
                ManageMOSOrderMgtWaiversMenu,

                MOSOrderMgtContactUsSubjectsMenu,
                ManageMOSOrderMgtContactUsSubjectsPermission,

                MOSOrderMgtContactUsQuestionsMenu,
                ManageMOSOrderMgtContactUsQuestionsPermission,

                MOSOrderMgtNotificationSettingsMenu,
                ManageMOSOrderMgtNotificationSettingsPermission,

                MOSOrderMgtNotificationEventsMenu,
                ManageMOSOrderMgtNotificationEventsPermission,

                MOSOrderMgtCancelRequestsMenu,
                ManageMOSOrderMgtCancelRequestsPermission,

                MOSreportMgtPortalLoginMenu,
                ViewMOSreportMgtPortalLoginMenu,

                MOSreportMgtOrderReportMenu,
                ViewMOSreportMgtOrderReportMenu,

                MOSreportMgtOrderCancellationReportMenu,
                ViewMOSreportMgtOrderCancellationReportMenu,

                MOSreportMgtUserAccountReportMenu,
                ViewMOSreportMgtUserAccountReportMenu,

                MOSreportMgtRoleReportMenu,
                ViewMOSreportMgtRoleReportMenu,

                MOSreportMgtUserActivityReportMenu,
                ViewMOSreportMgtUserActivityReportMenu,

                MOSreportMgtOrderCollectionReportMenu,
                ViewMOSreportMgtOrderCollectionReportMenu
            };
        }

        public static ITree<ApplicationPermissionsTree> Tree()
        {
            return PermissionsList().ToTree((parent, child) => child.ParentId == parent.Id);
        }

        public static void SetAclPath(string aclPath)
        {
            _aclPath = aclPath;
        }

        static List<ApplicationPermissionsTree> PermissionsListFromJson()
        {
            if (!string.IsNullOrEmpty(_aclPath))
            {
                var tree = TreeFromJson(_aclPath);
                //convert tree to list;
                return ConvertTreeToList(tree);
            }
            else
            {
                //get the list from PermissionsList();
                return PermissionsList();
            }
        }

        public static List<ApplicationPermissionsTree> ConvertTreeToList(ApplicationPermissionsTree tree)
        {
            try
            {
                List<ApplicationPermissionsTree> children = new List<ApplicationPermissionsTree>();

                foreach(var child in tree.Children)
                {
                    if (child.Children.Any())
                    {
                        children.AddRange(ConvertTreeToList(child));
                    }
                    else
                    {
                        children.Add(child);
                    }
                }

                return children;
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public static ApplicationPermissionsTree TreeFromJson(string aclPath = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(aclPath))
                {
                    using (StreamReader r = new StreamReader(aclPath))
                    {
                        string json = r.ReadToEnd();
                        ApplicationPermissionsTree root = JsonConvert.DeserializeObject<ApplicationPermissionsTree>(json);
                        root = UpdateTreeWithId(root, 0);
                        return root;
                        //List<ApplicationPermissionsTree> items = JsonConvert.DeserializeObject<List<ApplicationPermissionsTree>>(json);
                    }
                }

            }
            catch (Exception ex)
            {
            }

            return null;
        }

        private static ApplicationPermissionsTree UpdateTreeWithId(ApplicationPermissionsTree root, int lastId, int? parentId = null)
        {
            root.Id = lastId;
            root.ParentId = parentId;

            if (root.Children != null && root.Children.Any())
            {
                foreach (var child in root.Children)
                {
                    UpdateTreeWithId(child, ++lastId, root.Id);
                }
            }

            return root;
        }
    }



    public class ApplicationPermissionsTree
    {
        public ApplicationPermissionsTree()
        { }

        public ApplicationPermissionsTree(int id, string name, string value, string description = null, ApplicationPermissionsTree parent = null, params ApplicationPermissionsTree[] children)
        {
            Id = id;
            Name = name;
            Value = value;
            Description = description;
            ParentId = parent?.Id;
            Children = children != null ? children.ToList() : new List<ApplicationPermissionsTree>();
        }


        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public List<ApplicationPermissionsTree> Children { get; set; }

        public override string ToString()
        {
            return Value;
        }


        public static implicit operator string(ApplicationPermissionsTree permission)
        {
            return permission.Value;
        }
    }

    public class ApplicationPermissionsTreeDTO
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public List<ApplicationPermissionsTreeDTO> Children { get; set; }

        public override string ToString()
        {
            return Value;
        }


        public static implicit operator string(ApplicationPermissionsTreeDTO permission)
        {
            return permission.Value;
        }
    }
}
