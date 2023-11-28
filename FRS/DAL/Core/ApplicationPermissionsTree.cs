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
        //public static ApplicationPermissionsTree ViewAccountsMenu = new ApplicationPermissionsTree(2, "View Account Management Menu", "account.view","Permission to view Account Management menu", Root);

        //public const string UsersPermissionGroupName = "User";
        //public static ApplicationPermissionsTree UsersMenu = new ApplicationPermissionsTree(3, "Users Menu", "accountsmenu.users.view","Permission to view Users menu", ViewAccountsMenu);

        //public static ApplicationPermissionsTree ViewUsers = new ApplicationPermissionsTree(4, "View Users", "users.view", "Permission to view other users account details", UsersMenu);
        //public static ApplicationPermissionsTree ManageUsers = new ApplicationPermissionsTree(5, "Manage Users", "users.manage", "Permission to create, delete and modify other users account details", UsersMenu);

        //public const string RolesPermissionGroupName = "Role";
        //public static ApplicationPermissionsTree RolesMenu = new ApplicationPermissionsTree(6, "Roles Menu", "accountsmenu.roles.view",  "Permission to view Roles menu", ViewAccountsMenu);

        //public static ApplicationPermissionsTree ViewRoles = new ApplicationPermissionsTree(7, "View Roles", "roles.view",  "Permission to view available roles", RolesMenu);
        //public static ApplicationPermissionsTree ManageRoles = new ApplicationPermissionsTree(8, "Manage Roles", "roles.manage",  "Permission to create, delete and modify roles", RolesMenu);
        //public static ApplicationPermissionsTree AssignRoles = new ApplicationPermissionsTree(9, "Assign Roles", "roles.assign",  "Permission to assign roles to users", RolesMenu);

        //#endregion

        #region Facility Reservation Management ACL
        public static ApplicationPermissionsTree ViewFRSMenu = new ApplicationPermissionsTree(id++, "View Facilities Management Menu", "frsmgt.view", "Permission to view Facilities Management menu", Root);

        #region Resource Booking
        public static ApplicationPermissionsTree ViewResourceBookingMenu = new ApplicationPermissionsTree(id++, "View Resource Booking Menu", "frsmgt.resourcebooking.view", "Permission to view Resource Booking menu", ViewFRSMenu);

        public static ApplicationPermissionsTree RBFacilitiesMenu = new ApplicationPermissionsTree(id++, "Equipment Menu", "frsmgt.equipment.view", "Permission to view equipments menu", ViewResourceBookingMenu);
        //public static ApplicationPermissionsTree RBViewFacilities = new ApplicationPermissionsTree(id++, "View Equipments", "frsmgt.equipment.view", "Permission to view available equipments", RBFacilitiesMenu);
        public static ApplicationPermissionsTree RBManageFacilities = new ApplicationPermissionsTree(id++, "Manage Equipments", "frsmgt.equipment.manage", "Permission to create, delete and modify equipments", RBFacilitiesMenu);

        public static ApplicationPermissionsTree RBFacilityTypesMenu = new ApplicationPermissionsTree(id++, "Equipment Type Menu", "frsmgt.equipmenttype.view", "Permission to view equipment types menu", ViewResourceBookingMenu);
        //public static ApplicationPermissionsTree RBViewFacilityTypes = new ApplicationPermissionsTree(id++, "View Equipment Types", "frsmgt.equipmenttype.view", "Permission to view equipment types", RBFacilityTypesMenu);
        public static ApplicationPermissionsTree RBManageFacilityTypes = new ApplicationPermissionsTree(id++, "Manage Equipment Types", "frsmgt.equipmenttype.manage", "Permission to create, delete and modify equipment type", RBFacilityTypesMenu);

        public static ApplicationPermissionsTree RBCalendarMenu = new ApplicationPermissionsTree(id++, "Calendar", "frsmgt.calendar.view", "Permission to book facilities and events", ViewResourceBookingMenu);

        public static ApplicationPermissionsTree RBSmartRoomSchedulerLogsMenu = new ApplicationPermissionsTree(id++, "Scheduler Logs", "frsmgt.smartroomschedulerlog.view", "Permission to view scheduler logs", ViewResourceBookingMenu);
        #endregion

        #region Access Control
        public static ApplicationPermissionsTree ViewAccessControlMenu = new ApplicationPermissionsTree(id++, "View Access Control Menu", "frsmgt.accesscontrol.view", "Permission to view FRS Access Control menu", ViewFRSMenu);

        public static ApplicationPermissionsTree ACVehiclesMenu = new ApplicationPermissionsTree(id++, "Vehicle Management Menu", "frsmgt.accesscontrol.uservehicle.view", "Permission to view vehicle management menu", ViewAccessControlMenu);
        public static ApplicationPermissionsTree ACManageUserVehicles = new ApplicationPermissionsTree(id++, "Manage Vehicles", "frsmgt.accesscontrol.uservehicle.manage", "Permission to create, delete and modify vehicles", ACVehiclesMenu);

        public static ApplicationPermissionsTree ACContactGroupMenu = new ApplicationPermissionsTree(id++, "Contact Group Management Menu", "frsmgt.accesscontrol.contactgroup.view", "Permission to view contact groups management menu", ViewAccessControlMenu);
        //public static ApplicationPermissionsTree ACContactGroupView = new ApplicationPermissionsTree(id++, "View Contact Groups", "frsmgt.accesscontrol.contactgroup.view", "Permission to view groups", ACContactGroupMenu);
        public static ApplicationPermissionsTree ACContactGroupManage = new ApplicationPermissionsTree(id++, "Manage Contact Groups", "frsmgt.accesscontrol.contactgroup.manage", "Permission to create, delete and modify contact groups", ACContactGroupMenu);


        public static ApplicationPermissionsTree ACUserPhonebookMenu = new ApplicationPermissionsTree(id++, "User Phonebook Management Menu", "frsmgt.accesscontrol.phonebook.view", "Permission to view user phonebook management menu", ViewAccessControlMenu);
        //public static ApplicationPermissionsTree ACUserPhonebookView = new ApplicationPermissionsTree(id++, "View User Phonebook", "frsmgt.accesscontrol.phonebook.view", "Permission to view user phonebooks", ACUserPhonebookMenu);
        public static ApplicationPermissionsTree ACUserPhonebookManage = new ApplicationPermissionsTree(id++, "Manage User Phonebook", "frsmgt.accesscontrol.phonebook.manage", "Permission to create, delete and modify user phonebooks", ACUserPhonebookMenu);


        #endregion

        #region Workforce Management
        public static ApplicationPermissionsTree ViewWorkforceManagementMenu = new ApplicationPermissionsTree(id++, "Workforce Management Menu", "frsmgt.workforcemanagement.view", "Permission to view FRS Workforce Management menu", ViewFRSMenu);

        public static ApplicationPermissionsTree WFDesignationMenu = new ApplicationPermissionsTree(id++, "View Designation", "frsmgt.workforcemanagement.designation.view", "Permission to view designation menu", ViewWorkforceManagementMenu);
        public static ApplicationPermissionsTree WFEmployeeRosterMenu = new ApplicationPermissionsTree(id++, "View Employee Roster", "frsmgt.workforcemanagement.employeeroster.view", "Permission to view employee master menu", ViewWorkforceManagementMenu);
        public static ApplicationPermissionsTree WFEmployeeMasterMenu = new ApplicationPermissionsTree(id++, "View Employee Master", "frsmgt.workforcemanagement.employeemaster.view", "Permission to view employee roster menu", ViewWorkforceManagementMenu);
        #endregion

        //public const string FacilityTypesPermissionGroupName = "Facility Types";
        //public static ApplicationPermissionsTree FacilityTypesMenu = new ApplicationPermissionsTree(14, "Facility Types Menu", "frsmgt.facilitytype.view", "Permission to view facility types menu", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewFacilityTypes = new ApplicationPermissionsTree(47, "View Facility Types", "frsmgt.facilitytype.view", "Permission to view available facility types", FacilityTypesMenu);
        //public static ApplicationPermissionsTree ManageFacilityTypes = new ApplicationPermissionsTree(48, "Manage Facility Types", "frsmgt.facilitytype.manage", "Permission to create, delete and modify facility types", FacilityTypesMenu);

        //public const string LocationsPermissionGroupName = "Locations";
        //public static ApplicationPermissionsTree LocationsMenu = new ApplicationPermissionsTree(49, "Locations Menu", "frsmgt.location.view", "Permission to view locations menu", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewLocations = new ApplicationPermissionsTree(15, "View Locations", "frsmgt.location.view", "Permission to view available locations", LocationsMenu);
        //public static ApplicationPermissionsTree ManageLocations = new ApplicationPermissionsTree(16, "Manage Locations", "frsmgt.location.manage", "Permission to create, delete and modify locations", LocationsMenu);

        //public const string InstitutionsPermissionGroupName = "Institutions";
        //public static ApplicationPermissionsTree InstitutionsMenu = new ApplicationPermissionsTree(17, "Institutions Menu", "frsmgt.institution.view", "Permission to view institutions menu", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewInstitutions = new ApplicationPermissionsTree(18, "View Institutions", "frsmgt.institution.view", "Permission to view available institutions", InstitutionsMenu);
        //public static ApplicationPermissionsTree ManageInstitutions = new ApplicationPermissionsTree(19, "Manage Institutions", "frsmgt.institution.manage", "Permission to create, delete and modify institutions", InstitutionsMenu);

        //public const string DepartmentsPermissionGroupName = "Departments";
        //public static ApplicationPermissionsTree DepartmentsMenu = new ApplicationPermissionsTree(20, "Departments Menu", "frsmgt.department.view", "Permission to view departments menu", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewDepartments = new ApplicationPermissionsTree(21, "View Departments", "frsmgt.department.view", "Permission to view available departments", DepartmentsMenu);
        //public static ApplicationPermissionsTree ManageDepartments = new ApplicationPermissionsTree(22, "Manage Departments", "frsmgt.department.manage", "Permission to create, delete and modify departments", DepartmentsMenu);

        //public const string ContactGroupsPermissionGroupName = "Contact Groups";
        //public static ApplicationPermissionsTree ContactGroupsMenu = new ApplicationPermissionsTree(23, "Contact Groups Menu", "frsmgt.contactgroup.view", "Permission to view contact groups menu", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewContactGroups = new ApplicationPermissionsTree(24, "View Contact Groups", "frsmgt.contactgroup.view", "Permission to view available contact groups", ContactGroupsMenu);
        //public static ApplicationPermissionsTree ManageContactGroups = new ApplicationPermissionsTree(25, "Manage Contact Groups", "frsmgt.contactgroup.manage", "Permission to create, delete and modify contact groups", ContactGroupsMenu);

        //public const string UserPhonebooksPermissionGroupName = "Phonebooks";
        //public static ApplicationPermissionsTree PhonebooksMenu = new ApplicationPermissionsTree(26, "Phonebooks Menu", "frsmgt.userphonebook.view", "Permission to view user phonebooks menu", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewUserPhonebooks = new ApplicationPermissionsTree(27, "View Phonebooks", "frsmgt.userphonebook.view", "Permission to view available phone book", PhonebooksMenu);
        //public static ApplicationPermissionsTree ManageUserPhonebooks = new ApplicationPermissionsTree(28, "Manage Phonebooks", "frsmgt.userphonebook.manage", "Permission to create, delete and modify phone book", PhonebooksMenu);

        //public const string UserGroupPermissionGroupName = "User Group";
        //public static ApplicationPermissionsTree UserGroupsMenu = new ApplicationPermissionsTree(31, "User Groups Menu", "frsmenu.usergroups.view", "Permission to view user groups menu", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewUserGroups = new ApplicationPermissionsTree(32, "View User Group", "frsmgt.usergroups.view", "Permission to view user groups", UserGroupsMenu);
        //public static ApplicationPermissionsTree ManageUserGroups = new ApplicationPermissionsTree(33, "Manage User Group", "frsmgt.usergroups.manage", "Permission to create, delete and modify user groups", UserGroupsMenu);

        //public const string PublicationsPermissionGroupName = "Publications";
        //public static ApplicationPermissionsTree PublicationsMenu = new ApplicationPermissionsTree(34, "Publications Menu", "frsmenu.publications.view", "Permission to view departments menu", ViewFRSMenu);

        //public static ApplicationPermissionsTree ApprovePublications = new ApplicationPermissionsTree(35, "Approve Signage Publications", "frsmgt.publications.approve", "Permission to approve signage publications", PublicationsMenu);

        //public const string PIBTemplatesPermissionGroupName = "PIB Templates";
        //public static ApplicationPermissionsTree PublicationsMenu = new ApplicationPermissionsTree(36, "Publications Menu", "frsmenu.publications.view", "Permission to view departments menu", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewPIBTemplates = new ApplicationPermissionsTree(37, "View PIB Templates", "pibtemplates.view", PIBTemplatesPermissionGroupName, "Permission to view available pib templates");
        //public static ApplicationPermissionsTree ManagePIBTemplates = new ApplicationPermissionsTree(38, "Manage PIB Templates", "pibtemplates.manage", PIBTemplatesPermissionGroupName, "Permission to create, delete and modify pib templates");

        //public const string PIBDevicesPermissionGroupName = "PIB Devices";
        //public static ApplicationPermissionsTree ViewPIBDevices = new ApplicationPermissionsTree(39, "View PIB Devices", "pibdevices.view", PIBDevicesPermissionGroupName, "Permission to view available pib devices");
        //public static ApplicationPermissionsTree ManagePIBDevices = new ApplicationPermissionsTree(40, "Manage PIB Devices", "pibdevices.manage", PIBDevicesPermissionGroupName, "Permission to create, delete and modify pib devices");

        //public const string EpaperTemplatesPermissionGroupName = "Epaper Templates";
        //public static ApplicationPermissionsTree EpaperTemplatesMenu = new ApplicationPermissionsTree(41, "Epaper Templates Menu", "frsmenu.epapertemplates.view", "Permission to view epaper templates menu", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewEpaperTemplates = new ApplicationPermissionsTree(42, "View Epaper Templates", "epapertemplates.view", "Permission to view available epaper templates", EpaperTemplatesMenu);
        //public static ApplicationPermissionsTree ManageEpaperTemplates = new ApplicationPermissionsTree(43, "Manage Epaper Templates", "epapertemplates.manage", "Permission to create, delete and modify epaper templates", EpaperTemplatesMenu);

        //public const string EpaperDevicesPermissionGroupName = "Epaper Devices";
        //public static ApplicationPermissionsTree EpaperDevicesMenu = new ApplicationPermissionsTree(44, "Epaper Devices Menu", "frsmenu.epaperdevices.view", "Permission to view epaper devices menu", ViewFRSMenu);

        //public static ApplicationPermissionsTree ViewEpaperDevices = new ApplicationPermissionsTree(45, "View Epaper Devices", "epaperdevices.view", "Permission to view available epaper devices", EpaperDevicesMenu);
        //public static ApplicationPermissionsTree ManageEpaperDevices = new ApplicationPermissionsTree(46, "Manage Epaper Devices", "epaperdevices.manage", "Permission to create, delete and modify epaper devices", EpaperDevicesMenu);

        #endregion

        #region Signages and Kiosks ACL
        public static ApplicationPermissionsTree ViewSignageMenu = new ApplicationPermissionsTree(id++, "View Signages and Kiosks Menu", "sgnmanagement.view", "Permission to view Signages and Kiosks menu", Root);

        #region Content Management
        public static ApplicationPermissionsTree ViewContentManagementMenu = new ApplicationPermissionsTree(id++, "View Content Management Menu", "sgnmanagement.contentmanagement.view", "Permission to view Content Management menu", ViewSignageMenu);

        public static ApplicationPermissionsTree CMMediaGroupsMenu = new ApplicationPermissionsTree(id++, "Media Group", "sgnmanagement.contentmanagement.mediagroup.view", "Permission to view Media Group menu", ViewContentManagementMenu);
        public static ApplicationPermissionsTree CMMediaLibrariesMenu = new ApplicationPermissionsTree(id++, "Media Library", "sgnmanagement.contentmanagement.medialibrary.view", "Permission to view Media Library menu", ViewContentManagementMenu);
        public static ApplicationPermissionsTree CMPlaylistsMenu = new ApplicationPermissionsTree(id++, "Playlists", "sgnmanagement.contentmanagement.playlist.view", "Permission to view Playlists menu", ViewContentManagementMenu);
        public static ApplicationPermissionsTree CMComponentsMenu = new ApplicationPermissionsTree(id++, "Components", "sgnmanagement.contentmanagement.component.view", "Permission to view Components menu", ViewContentManagementMenu);

        public static ApplicationPermissionsTree CMComponentCreate = new ApplicationPermissionsTree(id++, "Create Component", "sgnmanagement.contentmanagement.component.create", "Permission to create Components", CMComponentsMenu);
        public static ApplicationPermissionsTree CMComponentUpdate = new ApplicationPermissionsTree(id++, "Update Component", "sgnmanagement.contentmanagement.component.update", "Permission to update Components", CMComponentsMenu);
        public static ApplicationPermissionsTree CMComponentDelete = new ApplicationPermissionsTree(id++, "Delete Component", "sgnmanagement.contentmanagement.component.delete", "Permission to delete Components", CMComponentsMenu);

        public static ApplicationPermissionsTree CMCompilationsMenu = new ApplicationPermissionsTree(id++, "Compilations", "sgnmanagement.contentmanagement.compilation.view", "Permission to view Compilations menu", ViewContentManagementMenu);

        public static ApplicationPermissionsTree CMCompilationCreate = new ApplicationPermissionsTree(id++, "Create Compilation", "sgnmanagement.contentmanagement.compilation.create", "Permission to create Compilations", CMCompilationsMenu);
        public static ApplicationPermissionsTree CMCompilationUpdate = new ApplicationPermissionsTree(id++, "Update Compilation", "sgnmanagement.contentmanagement.compilation.update", "Permission to update Compilations", CMCompilationsMenu);
        public static ApplicationPermissionsTree CMCompilationDelete = new ApplicationPermissionsTree(id++, "Delete Compilation", "sgnmanagement.contentmanagement.compilation.delete", "Permission to delete Compilations", CMCompilationsMenu);

        public static ApplicationPermissionsTree CMPublicationsMenu = new ApplicationPermissionsTree(id++, "Publications", "sgnmanagement.contentmanagement.publication.view", "Permission to view Publications menu", ViewContentManagementMenu);
        public static ApplicationPermissionsTree CMPublicationApprove = new ApplicationPermissionsTree(id++, "Publications Approval", "frsmgt.accesscontrol.publication.approve", "Permission to approve publications", CMPublicationsMenu);
        public static ApplicationPermissionsTree CMPublicationEmail = new ApplicationPermissionsTree(id++, "Approval Email Alert", "frsmgt.accesscontrol.publication.email", "Receive email alert when publication created", CMPublicationsMenu);
        public static ApplicationPermissionsTree CMPublicationCreate = new ApplicationPermissionsTree(id++, "Create Publication", "sgnmanagement.contentmanagement.publication.create", "Permission to create Publications", CMPublicationsMenu);
        public static ApplicationPermissionsTree CMPublicationUpdate = new ApplicationPermissionsTree(id++, "Update Publication", "sgnmanagement.contentmanagement.publication.update", "Permission to update Publications", CMPublicationsMenu);
        public static ApplicationPermissionsTree CMPublicationDelete = new ApplicationPermissionsTree(id++, "Delete Publication", "sgnmanagement.contentmanagement.publication.delete", "Permission to delete Publications", CMPublicationsMenu);


        #endregion

        #region Directory Listing
        public static ApplicationPermissionsTree ViewDirectoryListingMenu = new ApplicationPermissionsTree(id++, "View Directory Listing Menu", "sgnmanagement.directorylisting.view", "Permission to view Directory Listing menu", ViewSignageMenu);

        public static ApplicationPermissionsTree DLDirectoryCategoryMenu = new ApplicationPermissionsTree(id++, "Directory Category", "sgnmanagement.directorylisting.directorycategory.view", "Permission to view Directory Category menu", ViewDirectoryListingMenu);
        public static ApplicationPermissionsTree DLDirectoryListingMenu = new ApplicationPermissionsTree(id++, "Directory Listing", "sgnmanagement.directorylisting.directorylisting.view", "Permission to view Directory Listing menu", ViewDirectoryListingMenu);
        public static ApplicationPermissionsTree DLDLBuildingMenu = new ApplicationPermissionsTree(id++, "Building", "sgnmanagement.directorylisting.building.view", "Permission to view Building menu", ViewDirectoryListingMenu);
        public static ApplicationPermissionsTree DLFloorMenu = new ApplicationPermissionsTree(id++, "Floor", "sgnmanagement.directorylisting.floor.view", "Permission to view Floor menu", ViewDirectoryListingMenu);
        public static ApplicationPermissionsTree DLMapMenu = new ApplicationPermissionsTree(id++, "Map", "sgnmanagement.directorylisting.map.view", "Permission to view Map menu", ViewDirectoryListingMenu);


        #endregion

        #region Queue Display Interface
        public static ApplicationPermissionsTree ViewQueueDisplayInterfaceMenu = new ApplicationPermissionsTree(id++, "View Queue Display Interface Menu", "sgnmanagement.qdi.view", "Permission to view Queue Display Interface menu", ViewSignageMenu);

        public static ApplicationPermissionsTree QDIQueueMatrixMenu = new ApplicationPermissionsTree(id++, "Queue Matrix Menu", "sgnmanagement.qdi.queuematrix.view", "Permission to view Queue Matrix menu", ViewQueueDisplayInterfaceMenu);
        public static ApplicationPermissionsTree QDIQueueDisplayLogsMenu = new ApplicationPermissionsTree(id++, "Queue Display Logs Menu", "sgnmanagement.qdi.queuedisplaylog.view", "Permission to view Queue Display Logs menu", ViewQueueDisplayInterfaceMenu);


        #endregion

        #region Facility Booking Interface
        public static ApplicationPermissionsTree ViewFacilityBookingInterfaceMenu = new ApplicationPermissionsTree(id++, "View Facility Booking Interface Menu", "sgnmanagement.fbi.view", "Permission to view Facility Booking Interface menu", ViewSignageMenu);

        public static ApplicationPermissionsTree FBIResourceMatrixMenu = new ApplicationPermissionsTree(id++, "Resource Matrix", "sgnmanagement.fbi.resourcematrix.view", "Permission to view Resource Matrix menu", ViewFacilityBookingInterfaceMenu);
        public static ApplicationPermissionsTree FBIInteraceLogsMenu = new ApplicationPermissionsTree(id++, "Interface Logs", "sgnmanagement.fbi.interfacelog.view", "Permission to view Interface Logs menu", ViewFacilityBookingInterfaceMenu);


        #endregion

        #region Device Management
        public static ApplicationPermissionsTree ViewDeviceManagementMenu = new ApplicationPermissionsTree(id++, "View Device Management Menu", "sgnmanagement.devicemgt.view", "Permission to view Device Management menu", ViewSignageMenu);

        public static ApplicationPermissionsTree DMDevicesMenu = new ApplicationPermissionsTree(id++, "Devices", "sgnmanagement.devicemgt.device.view", "Permission to view Devices", ViewDeviceManagementMenu);

        public static ApplicationPermissionsTree DMDevicesManageMenu = new ApplicationPermissionsTree(id++, "Manage Devices", "sgnmanagement.devicemgt.device.manage", "Permission to manage Devices", DMDevicesMenu);
        public static ApplicationPermissionsTree DMApprovalMenu = new ApplicationPermissionsTree(id++, "Approval", "sgnmanagement.devicemgt.device.approve", "Permission to approve Devices", DMDevicesMenu);

        public static ApplicationPermissionsTree DMEMSMenu = new ApplicationPermissionsTree(id++, "EMS", "sgnmanagement.devicemgt.ems.view", "Permission to view EMS menu", ViewDeviceManagementMenu);
        public static ApplicationPermissionsTree DMEMSProfileMenu = new ApplicationPermissionsTree(id++, "EMS Profile", "sgnmanagement.devicemgt.emsprofile.view", "Permission to view EMS Profile menu", ViewDeviceManagementMenu);
        public static ApplicationPermissionsTree DMEMSScheduleMenu = new ApplicationPermissionsTree(id++, "EMS Schedule", "sgnmanagement.devicemgt.emsschedule.view", "Permission to view EMS Schedule menu", ViewDeviceManagementMenu);

        #endregion

        #region Epaper Management
        public static ApplicationPermissionsTree ViewEpaperManagementMenu = new ApplicationPermissionsTree(id++, "View Epaper Management Menu", "sgnmanagement.epapermgt.view", "Permission to view Epaper Management menu", ViewSignageMenu);

        public static ApplicationPermissionsTree EPTemplateMenu = new ApplicationPermissionsTree(id++, "Epaper Template", "sgnmanagement.epapermgt.template.view", "Permission to view Epaper Template menu", ViewEpaperManagementMenu);

        #endregion


        #endregion

        #region Reports
        public static ApplicationPermissionsTree ViewReportsMenu = new ApplicationPermissionsTree(id++, "View Reports Menu", "reportmgt.view", "Permission to view Reports menu", Root);

        #region Facility Management Reports
        public static ApplicationPermissionsTree ViewFacilityManagementMenu = new ApplicationPermissionsTree(id++, "View Facility Management", "reportmgt.facilitymgt.view", "Permission to view Facilitiy Management Reports menu", ViewReportsMenu);

        public static ApplicationPermissionsTree RVehicleLogsMenu = new ApplicationPermissionsTree(id++, "Vehicle Logs", "reportmgt.facilitymgt.vehiclelog.view", "Permission to view Vehicle Logs menu", ViewFacilityManagementMenu);
        public static ApplicationPermissionsTree ROccupancyLogsMenu = new ApplicationPermissionsTree(id++, "Occupancy Logs", "reportmgt.facilitymgt.occupancylog.view", "Permission to view Occupancy Logs menu", ViewFacilityManagementMenu);


        #endregion

        #region Signages and Kiosks Reports
        public static ApplicationPermissionsTree ViewSgnKiosksMenu = new ApplicationPermissionsTree(id++, "Signages and Kiosks", "reportmgt.sgn.view", "Permission to view Signages and Kiosks Reports menu", ViewReportsMenu);

        public static ApplicationPermissionsTree SKUpDownTimeLogsMenu = new ApplicationPermissionsTree(id++, "Up/Downtime Logs", "reportmgt.sgn.updownlog.view", "Permission to view Up/Downtime Logs menu", ViewSgnKiosksMenu);


        #endregion

        #endregion

        #region Asset Management
        public static ApplicationPermissionsTree ViewAssetManagementMenu = new ApplicationPermissionsTree(id++, "View Asset Management Menu", "assetmgt.view", "Permission to view System Settings menu", Root);
        public static ApplicationPermissionsTree AMAssetTypeMenu = new ApplicationPermissionsTree(id++, "Asset Type", "assetmgt.assettype.view", "Permission to view Asset Type menu", ViewAssetManagementMenu);
        public static ApplicationPermissionsTree AMAssetTypeManage = new ApplicationPermissionsTree(id++, "Manage Asset Type", "assetmgt.assettype.manage", "Permission to manage Asset Type menu", AMAssetTypeMenu);

        public static ApplicationPermissionsTree AMAssetModelMenu = new ApplicationPermissionsTree(id++, "Asset Model", "assetmgt.assetmodel.view", "Permission to view Asset Model menu", ViewAssetManagementMenu);
        public static ApplicationPermissionsTree AMAssetModelManage = new ApplicationPermissionsTree(id++, "Manage Asset Model", "assetmgt.assetmodel.manage", "Permission to manage Asset Model menu", AMAssetModelMenu);

        public static ApplicationPermissionsTree AMAssetMenu = new ApplicationPermissionsTree(id++, "Asset", "assetmgt.asset.view", "Permission to view Asset menu", ViewAssetManagementMenu);
        public static ApplicationPermissionsTree AMAssetManage = new ApplicationPermissionsTree(id++, "Manage Asset", "assetmgt.asset.manage", "Permission to manage Asset menu", AMAssetMenu);

        public static ApplicationPermissionsTree AMServiceContractMenu = new ApplicationPermissionsTree(id++, "Service Contract", "assetmgt.servicecontract.view", "Permission to view Service Contract menu", ViewAssetManagementMenu);
        public static ApplicationPermissionsTree AMServiceContractManage = new ApplicationPermissionsTree(id++, "Manage Service Contract", "assetmgt.servicecontract.manage", "Permission to manage Service Contract menu", AMServiceContractMenu);

        #endregion

        #region System Settings
        public static ApplicationPermissionsTree ViewSystemSettingsMenu = new ApplicationPermissionsTree(id++, "View System Settings Menu", "systemsetting.view", "Permission to view System Settings menu", Root);

        public static ApplicationPermissionsTree SSLocationTreeMenu = new ApplicationPermissionsTree(id++, "Location Tree", "systemsetting.locationtree.view", "Permission to view Location Tree menu", ViewSystemSettingsMenu);
        //public static ApplicationPermissionsTree SSLocationTreeView = new ApplicationPermissionsTree(id++, "View Location Tree", "systemsetting.locationtree.view", "Permission to view Location Tree menu", SSLocationTreeMenu);
        public static ApplicationPermissionsTree SSLocationTreeManage = new ApplicationPermissionsTree(id++, "Manage Location Tree", "systemsetting.locationtree.manage", "Permission to manage Location Tree menu", SSLocationTreeMenu);

        public static ApplicationPermissionsTree SSInstitutionMenu = new ApplicationPermissionsTree(id++, "Institution", "systemsetting.institution.view", "Permission to view Institution menu", ViewSystemSettingsMenu);
        //public static ApplicationPermissionsTree SSInstitutionView = new ApplicationPermissionsTree(id++, "View Institution", "systemsetting.institution.view", "Permission to view Institution menu", SSInstitutionMenu);
        public static ApplicationPermissionsTree SSInstitutionManage = new ApplicationPermissionsTree(id++, "Manage Institution", "systemsetting.institution.manage", "Permission to manage Institution menu", SSInstitutionMenu);


        public static ApplicationPermissionsTree SSDepartmentMenu = new ApplicationPermissionsTree(id++, "Department", "systemsetting.department.view", "Permission to view Department menu", ViewSystemSettingsMenu);
        //public static ApplicationPermissionsTree SSDepartmentView = new ApplicationPermissionsTree(id++, "View Department", "systemsetting.department.view", "Permission to view Department menu", SSDepartmentMenu);
        public static ApplicationPermissionsTree SSDepartmentManage = new ApplicationPermissionsTree(id++, "Manage Department", "systemsetting.department.manage", "Permission to manage Department menu", SSDepartmentMenu);

        public static ApplicationPermissionsTree SSBuildingMenu = new ApplicationPermissionsTree(id++, "Building", "systemsetting.building.view", "Permission to view Building menu", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSUserGroupMenu = new ApplicationPermissionsTree(id++, "User Group", "systemsetting.usergroup.view", "Permission to view User Group menu", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSUserGroupManage = new ApplicationPermissionsTree(id++, "Manage User Group", "systemsetting.usergroup.manage", "Permission to view User Group menu", SSUserGroupMenu);

        public static ApplicationPermissionsTree SSImageReferenceTypeMenu = new ApplicationPermissionsTree(id++, "Image Reference Type", "systemsetting.imagereferencetype.view", "Permission to view Image Reference Type menu", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSImageReferenceTypeManage = new ApplicationPermissionsTree(id++, "Manage Image Reference Type", "systemsetting.imagereferencetype.manage", "Permission to view Image Reference Type menu", SSImageReferenceTypeMenu);

        public static ApplicationPermissionsTree SSImageReferenceColorMenu = new ApplicationPermissionsTree(id++, "Image Reference Color", "systemsetting.imagereferencecolor.view", "Permission to view Image Reference Color menu", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSImageReferenceColorManage = new ApplicationPermissionsTree(id++, "Manage Image Reference Color", "systemsetting.imagereferencecolor.manage", "Permission to view Image Reference Color menu", SSImageReferenceColorMenu);

        public static ApplicationPermissionsTree SSDeviceTypeMenu = new ApplicationPermissionsTree(id++, "Device Type", "systemsetting.devicetype.view", "Permission to view Device Type menu", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSDeviceTypeManage = new ApplicationPermissionsTree(id++, "Manage Device Type", "systemsetting.devicetype.manage", "Permission to manage Device Type menu", SSDeviceTypeMenu);


        public static ApplicationPermissionsTree SSUserMenu = new ApplicationPermissionsTree(id++, "User Management", "systemsetting.user.view", "Permission to view Users menu", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSManageUserMenu = new ApplicationPermissionsTree(id++, "User", "systemsetting.user.manage", "Permission to view Users menu", SSUserMenu);

        public static ApplicationPermissionsTree SSRoleMenu = new ApplicationPermissionsTree(id++, "Role Management Menu", "systemsetting.role.view", "Permission to view Role menu", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSManageRoleMenu = new ApplicationPermissionsTree(id++, "Manage Role", "systemsetting.role.manage", "Permission to create, delete and modify roles", SSRoleMenu);
        public static ApplicationPermissionsTree SSAssignRoleMenu = new ApplicationPermissionsTree(id++, "Assign Role", "systemsetting.role.assign", "Permission to assign roles to users", SSRoleMenu);

        public static ApplicationPermissionsTree SSAuditMenu = new ApplicationPermissionsTree(id++, "Audit Logs Menu", "systemsetting.audit.view", "Permission to view Audit Logs menu", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSAuthLogMenu = new ApplicationPermissionsTree(id++, "Authentication Logs", "systemsetting.audit.authlog.view", "Permission to view authentication logs", SSAuditMenu);
        public static ApplicationPermissionsTree SSDataLogMenu = new ApplicationPermissionsTree(id++, "Data Logs", "systemsetting.audit.datalog.view", "Permission to view data logs", SSAuditMenu);

        public static ApplicationPermissionsTree SSApplicationSettingMenu = new ApplicationPermissionsTree(id++, "Application Setting Menu", "systemsetting.applicationsetting.view", "Permission to view Application Setting menu", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSModuleSettingMenu = new ApplicationPermissionsTree(id++, "Module Setting Menu", "systemsetting.modulesetting.view", "Permission to view Module Setting menu", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSEmailQueueMenu = new ApplicationPermissionsTree(id++, "Email Queue Menu", "systemsetting.emailqueue.view", "Permission to view Email Queue menu", ViewSystemSettingsMenu);

        public static ApplicationPermissionsTree SSDashboardMenu = new ApplicationPermissionsTree(id++, "Dashboard Menu", "systemsetting.dashboard.view", "Permission to view Dashboard menu", ViewSystemSettingsMenu);
        public static ApplicationPermissionsTree SSDSignage = new ApplicationPermissionsTree(id++, "Signage Dashboard", "systemsetting.dashboard.sgn.view", "Permission to view Signage Dashboard menu", SSDashboardMenu);

        public static ApplicationPermissionsTree SSDCPreviewSignage = new ApplicationPermissionsTree(id++, "Preview", "systemsetting.dashboard.sgn.controls.preview", "Permission to allow preview of signage", SSDSignage);
        public static ApplicationPermissionsTree SSDCRebootSignage = new ApplicationPermissionsTree(id++, "Reboot", "systemsetting.dashboard.sgn.controls.reboot", "Permission to allow rebooting of devices", SSDSignage);
        public static ApplicationPermissionsTree SSDCRefreshSignage = new ApplicationPermissionsTree(id++, "Refresh", "systemsetting.dashboard.sgn.controls.refresh", "Permission to allow refreshing of devices", SSDSignage);
        public static ApplicationPermissionsTree SSDCScreenshotSignage = new ApplicationPermissionsTree(id++, "Screenshot", "systemsetting.dashboard.sgn.controls.screenshot", "Permission to download screenshot", SSDSignage);
        public static ApplicationPermissionsTree SSDCPushMessageSignage = new ApplicationPermissionsTree(id++, "Push Message", "systemsetting.dashboard.sgn.controls.pushmessage", "Permission to push messages", SSDSignage);
        public static ApplicationPermissionsTree SSDCDeviceInfoSignage = new ApplicationPermissionsTree(id++, "Modify device information", "systemsetting.dashboard.sgn.controls.deviceinfo", "Permission to change device info", SSDSignage);
        public static ApplicationPermissionsTree SSDCPublicationSignage = new ApplicationPermissionsTree(id++, "Publication", "systemsetting.dashboard.sgn.controls.publication", "Permission to change publication", SSDSignage);

        public static ApplicationPermissionsTree SSDUpcomingEvent = new ApplicationPermissionsTree(id++, "Upcoming Events", "systemsetting.dashboard.upcomingevent.view", "Permission to view Upcoming Events menu", SSDashboardMenu);
        public static ApplicationPermissionsTree SSDAvailableRoom = new ApplicationPermissionsTree(id++, "Available Locations", "systemsetting.dashboard.availablelocation.view", "Permission to view Available Locations menu", SSDashboardMenu);
        public static ApplicationPermissionsTree SSDApproval = new ApplicationPermissionsTree(id++, "Device Approval List", "systemsetting.dashboard.deviceapproval.view", "Permission to view Device Approval List menu", SSDashboardMenu);
        

        #endregion

        #region MEAL ORDER SYSTEM
        public static ApplicationPermissionsTree ViewMOSAccountMgtMenu = new ApplicationPermissionsTree(id++, "View Meal Order Account menu", "mosmgt.accountmgt.view", "Permission to view Meal Order Account menu", Root);
        public static ApplicationPermissionsTree ViewMOSMealMgtAccountMgtMenu = new ApplicationPermissionsTree(id++, "View Order Meals", "mosmgt.mealmgt.order.view", "Permission to view Order Meals menu", Root);
        public static ApplicationPermissionsTree ViewMOSOrganisationMgtMenu = new ApplicationPermissionsTree(id++, "View Meal Order Organisation menu", "mosmgt.orgmgt.view", "Permission to view Meal Order Organisation menu", Root);
        public static ApplicationPermissionsTree ViewMOSSettingMgtMenu = new ApplicationPermissionsTree(id++, "View Meal Order Setting menu", "mosmgt.settingmgt.view", "Permission to view Meal Order Setting menu", Root);
        public static ApplicationPermissionsTree ViewMOSreportMgtMenu = new ApplicationPermissionsTree(id++, "View Meal Order Report menu", "mosmgt.reportmgt.view", "Permission to view Meal Order Report menu", Root);
        public static ApplicationPermissionsTree ViewMOSSystemMgtMenu = new ApplicationPermissionsTree(id++, "View Meal Order System Admin menu", "mosmgt.sysadminmgt.view", "Permission to view Meal Order System Admin menu", Root);

        public static ApplicationPermissionsTree ViewMOSOrderMgtRestrictionTypesMenu = new ApplicationPermissionsTree(id++, "View Restriction Types menu", "mosmgt.mealmgt.order.restrictiontypes.view", "Permission to view Restriction Types menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtRestrictionTypesMenu = new ApplicationPermissionsTree(id++, "Manage Restriction Types menu", "mosmgt.mealmgt.order.restrictiontypes.manage", "Permission to view Restriction Types menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtRestrictionsMenu = new ApplicationPermissionsTree(id++, "View Restrictions menu", "mosmgt.mealmgt.order.restrictions.view", "Permission to view Restrictions menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtRestrictionsMenu = new ApplicationPermissionsTree(id++, "Manage Restrictions menu", "mosmgt.mealmgt.order.restrictions.manage", "Permission to view Restrictions menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtCuisinesMenu = new ApplicationPermissionsTree(id++, "View Cuisines menu", "mosmgt.mealmgt.order.cuisines.view", "Permission to view Cuisines menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCuisinesMenu = new ApplicationPermissionsTree(id++, "Manage Cuisines menu", "mosmgt.mealmgt.order.cuisines.manage", "Permission to view Cuisines menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtDeliveryMenu = new ApplicationPermissionsTree(id++, "View Delivery menu", "mosmgt.mealmgt.order.delivery.view", "Permission to view Delivery menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtDeliveryMenu = new ApplicationPermissionsTree(id++, "Manage Delivery menu", "mosmgt.mealmgt.order.delivery.manage", "Permission to view Delivery menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtBentoBoxTypesMenu = new ApplicationPermissionsTree(id++, "View Benton Box Types menu", "mosmgt.mealmgt.order.delivery.bentoboxtypes.view", "Permission to view Benton Box Types menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtBentoBoxTypesMenu = new ApplicationPermissionsTree(id++, "Manage Benton Box Types menu", "mosmgt.mealmgt.order.delivery.bentoboxtypes.manage", "Permission to view Benton Box Types menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtBentoAssetsMenu = new ApplicationPermissionsTree(id++, "View Bento Assets menu", "mosmgt.mealmgt.order.delivery.bentoassets.view", "Permission to view Bento Assets menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtBentoAssetsMenu = new ApplicationPermissionsTree(id++, "Manage Bento Assets menu", "mosmgt.mealmgt.order.delivery.bentoassets.manage", "Permission to view Bento Assets menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtCartonTypesMenu = new ApplicationPermissionsTree(id++, "View Carton Types menu", "mosmgt.mealmgt.order.delivery.cartontypes.view", "Permission to view Carton Types menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCartonTypesMenu = new ApplicationPermissionsTree(id++, "Manage Carton Types menu", "mosmgt.mealmgt.order.delivery.cartontypes.manage", "Permission to view Carton Types menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtCartonAssetsMenu = new ApplicationPermissionsTree(id++, "View Carton Assets menu", "mosmgt.mealmgt.order.delivery.cartonassets.view", "Permission to view Carton Assets menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCartonAssetsMenu = new ApplicationPermissionsTree(id++, "Manage Carton Assets menu", "mosmgt.mealmgt.order.delivery.cartonassets.manage", "Permission to view Carton Assets menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtTrackingStatusMenu = new ApplicationPermissionsTree(id++, "View Tracking Status menu", "mosmgt.mealmgt.order.delivery.trackingstatus.view", "Permission to view Tracking Status menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtTrackingStatusMenu = new ApplicationPermissionsTree(id++, "Manage Tracking Status menu", "mosmgt.mealmgt.order.delivery.trackingstatus.manage", "Permission to view Tracking Status menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtDeliveryOrdersMenu = new ApplicationPermissionsTree(id++, "View Delivery Order menu", "mosmgt.mealmgt.order.delivery.deliveryorders.view", "Permission to view Delivery Order menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtDeliveryOrdersMenu = new ApplicationPermissionsTree(id++, "Manage Delivery Order menu", "mosmgt.mealmgt.order.delivery.deliveryorders.manage", "Permission to view Delivery Order menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtDishingProcessMenu = new ApplicationPermissionsTree(id++, "View Dishing Process menu", "mosmgt.mealmgt.order.delivery.dishingprocess.view", "Permission to view Dishing Process menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtDishingProcessMenu = new ApplicationPermissionsTree(id++, "Manage Dishing Process menu", "mosmgt.mealmgt.order.delivery.dishingprocess.manage", "Permission to view Dishing Process menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtPackingProcessMenu = new ApplicationPermissionsTree(id++, "View Packing Process menu", "mosmgt.mealmgt.order.delivery.packingprocess.view", "Permission to view Packing Process menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtPackingProcessMenu = new ApplicationPermissionsTree(id++, "Manage Packing Process menu", "mosmgt.mealmgt.order.delivery.packingprocess.manage", "Permission to view Packing Process menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtDriversMenu = new ApplicationPermissionsTree(id++, "View Drivers menu", "mosmgt.mealmgt.order.delivery.drivers.view", "Permission to view Drivers menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtDriversMenu = new ApplicationPermissionsTree(id++, "Manage Drivers menu", "mosmgt.mealmgt.order.delivery.drivers.manage", "Permission to view Drivers menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtRoutesMenu = new ApplicationPermissionsTree(id++, "View Routes menu", "mosmgt.mealmgt.order.delivery.routes.view", "Permission to view Routes menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtRoutesMenu = new ApplicationPermissionsTree(id++, "Manage Routes menu", "mosmgt.mealmgt.order.delivery.routes.manage", "Permission to view Routes menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtCaterersMenu = new ApplicationPermissionsTree(id++, "View Caterers menu", "mosmgt.settingmgt.caterers.view", "Permission to view Caterers menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCaterersMenu = new ApplicationPermissionsTree(id++, "Manage Caterers menu", "mosmgt.settingmgt.caterers.manage", "Permission to view Caterers menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtOutletProfilesMenu = new ApplicationPermissionsTree(id++, "View Outlet Profiles menu", "mosmgt.settingmgt.outletprofiles.view", "Permission to view Outlet Profiles menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtOutletProfilesMenu = new ApplicationPermissionsTree(id++, "Manage Outlet Profiles menu", "mosmgt.settingmgt.outletprofiles.manage", "Permission to view Outlet Profiles menu", ViewMOSMealMgtAccountMgtMenu);

        #region Outlets menu

        public static ApplicationPermissionsTree ViewMOSOrderMgtOutletsMenu = new ApplicationPermissionsTree(id++, "View Outlets menu", "mosmgt.settingmgt.outlets.view", "Permission to view Outlets menu", ViewMOSSettingMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtOutletsMenu = new ApplicationPermissionsTree(id++, "Manage Outlets menu", "mosmgt.settingmgt.outlets.manage", "Permission to view Outlets menu", ViewMOSSettingMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtTermsPermission = new ApplicationPermissionsTree(id++, "View Meal Plan Groupings menu", "mosmgt.outletmgt.terms.view", "Permission to view Meal Plan Groupings menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtTermsPermission = new ApplicationPermissionsTree(id++, "Manage Meal Plan Groupings menu", "mosmgt.outletmgt.terms.manage", "Permission to manage Meal Plan Groupings menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtStoresPermission = new ApplicationPermissionsTree(id++, "View Outlet Stores menu", "mosmgt.outletmgt.stores.view", "Permission to view stores menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtStoresPermission = new ApplicationPermissionsTree(id++, "Manage Outlet Stores menu", "mosmgt.outletmgt.stores.manage", "Permission to manage stores menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtCaterersPermission = new ApplicationPermissionsTree(id++, "View Outlet Caterers menu", "mosmgt.outletmgt.caterers.view", "Permission to view Caterers menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtCaterersPermission = new ApplicationPermissionsTree(id++, "Manage Outlet Caterers menu", "mosmgt.outletmgt.caterers.manage", "Permission to manage Caterers menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtReportsPermission = new ApplicationPermissionsTree(id++, "View Outlet Reports menu", "mosmgt.outletmgt.reports.view", "Permission to view Reports menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ManageMOSOutletMgtMealSummaryPermission = new ApplicationPermissionsTree(id++, "Manage Outlet Meal Summary menu", "mosmgt.outletmgt.mealsummary.manage", "Permission to manage Meal Summary menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtClassBatchesPermission = new ApplicationPermissionsTree(id++, "View Class Batches menu", "mosmgt.outletmgt.classbatches.view", "Permission to view Class Batches menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtClassBatchesPermission = new ApplicationPermissionsTree(id++, "Manage Class Batches menu", "mosmgt.outletmgt.classbatches.manage", "Permission to manage Class Batches menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtClassLevelsPermission = new ApplicationPermissionsTree(id++, "View Class Levels menu", "mosmgt.outletmgt.levels.view", "Permission to view Class Levels menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtClassLevelsPermission = new ApplicationPermissionsTree(id++, "Manage Class Levels menu", "mosmgt.outletmgt.levels.manage", "Permission to manage Class Levels menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtClassesPermission = new ApplicationPermissionsTree(id++, "View Classes menu", "mosmgt.outletmgt.classes.view", "Permission to view Classes menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtClassesPermission = new ApplicationPermissionsTree(id++, "Manage Classes menu", "mosmgt.outletmgt.classes.manage", "Permission to manage Classes menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtStudentsPermission = new ApplicationPermissionsTree(id++, "View Students menu", "mosmgt.outletmgt.students.view", "Permission to view Students menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtStudentsPermission = new ApplicationPermissionsTree(id++, "Manage Students menu", "mosmgt.outletmgt.students.manage", "Permission to manage Students menu", ViewMOSOrderMgtOutletsMenu);

        #region Students Directory

        public static ApplicationPermissionsTree ViewMOSOutletStudentMgtImportPermission = new ApplicationPermissionsTree(id++, "Import Students", "mosmgt.outletmgt.students.import.view", "Permission to import Students menu", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionNew = new ApplicationPermissionsTree(id++, "Create Students", "mosmgt.outletmgt.students.manage.create", "Permission to manage Students menu", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionEdit = new ApplicationPermissionsTree(id++, "Update Students", "mosmgt.outletmgt.students.manage.edit", "Permission to manage Students menu", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionDelete = new ApplicationPermissionsTree(id++, "Delete Students", "mosmgt.outletmgt.students.manage.delete", "Permission to manage Students menu", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionCreateAccount = new ApplicationPermissionsTree(id++, "Create Student Accounts", "mosmgt.outletmgt.students.manage.account.create", "Permission to manage Student Accounts Creation", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionOrderMgt = new ApplicationPermissionsTree(id++, "Student Orders Management", "mosmgt.outletmgt.students.manage.order", "Permission to manage Student Orders", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionVoucher = new ApplicationPermissionsTree(id++, "Student Vouchers Management", "mosmgt.outletmgt.students.manage.voucher", "Permission to manage Student Vouchers", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionNotification = new ApplicationPermissionsTree(id++, "Student Notifications Management", "mosmgt.outletmgt.students.manage.notification", "Permission to manage Student Notifications", ViewMOSOutletMgtStudentsPermission);
        public static ApplicationPermissionsTree ManageMOSOutletStudentMgtPermissionCalendar = new ApplicationPermissionsTree(id++, "Student Calendars Management", "mosmgt.outletmgt.students.manage.calendar", "Permission to manage Student Calendars", ViewMOSOutletMgtStudentsPermission);

        #endregion

        public static ApplicationPermissionsTree ViewMOSOutletMgtFasPermission = new ApplicationPermissionsTree(id++, "View FAS menu", "mosmgt.outletmgt.fas.view", "Permission to view FAS menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtFasPermission = new ApplicationPermissionsTree(id++, "Manage FAS menu", "mosmgt.outletmgt.fas.manage", "Permission to manage FAS menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtStudentGroupsPermission = new ApplicationPermissionsTree(id++, "View Student Groups menu", "mosmgt.outletmgt.studentgroups.view", "Permission to view Student Groups menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtStudentGroupsPermission = new ApplicationPermissionsTree(id++, "Manage Student Groups menu", "mosmgt.outletmgt.studentgroups.manage", "Permission to manage Student Groups menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtMenusPermission = new ApplicationPermissionsTree(id++, "View Menu Groups menu", "mosmgt.outletmgt.menus.view", "Permission to view Menu Groups menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtMenusPermission = new ApplicationPermissionsTree(id++, "Manage Menu Groups menu", "mosmgt.outletmgt.menus.manage", "Permission to manage Menu Groups menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtCancellationsPermission = new ApplicationPermissionsTree(id++, "View Order Cancellations menu", "mosmgt.outletmgt.cancellations.view", "Permission to view Order Cancellations menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtCancellationsPermission = new ApplicationPermissionsTree(id++, "Manage Order Cancellations menu", "mosmgt.outletmgt.cancellations.manage", "Permission to manage Order Cancellations menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtPortalContentsPermission = new ApplicationPermissionsTree(id++, "View Portal Contents menu", "mosmgt.outletmgt.portalcontents.view", "Permission to view Portal Contents menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtPortalContentsPermission = new ApplicationPermissionsTree(id++, "Manage Portal Contents menu", "mosmgt.outletmgt.portalcontents.manage", "Permission to manage Portal Contents menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtEmailTemplatesPermission = new ApplicationPermissionsTree(id++, "View Email Templates menu", "mosmgt.outletmgt.emailtemplates.view", "Permission to view Email Templates menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtEmailTemplatesPermission = new ApplicationPermissionsTree(id++, "Manage Email Templates menu", "mosmgt.outletmgt.emailtemplates.manage", "Permission to manage Email Templates menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtMealAllocationsPermission = new ApplicationPermissionsTree(id++, "View Meal Allocations menu", "mosmgt.outletmgt.mealallocations.view", "Permission to view Meal Allocations menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtMealAllocationsPermission = new ApplicationPermissionsTree(id++, "Manage Meal Allocations menu", "mosmgt.outletmgt.mealallocations.manage", "Permission to view Meal Allocations menu", ViewMOSOrderMgtOutletsMenu);

        public static ApplicationPermissionsTree ViewMOSOutletMgtPackingAllocationsPermission = new ApplicationPermissionsTree(id++, "View Packing Allocations menu", "mosmgt.outletmgt.packingallocations.view", "Permission to view Packing Allocations menu", ViewMOSOrderMgtOutletsMenu);
        public static ApplicationPermissionsTree ManageMOSOutletMgtPackingAllocationsPermission = new ApplicationPermissionsTree(id++, "Manage Packing Allocations menu", "mosmgt.outletmgt.packingallocations.manage", "Permission to manage Packing Allocations menu", ViewMOSOrderMgtOutletsMenu);

        #endregion

        public static ApplicationPermissionsTree ViewMOSOrderMgtStoreInfoMenu = new ApplicationPermissionsTree(id++, "View Store Info menu", "mosmgt.settingmgt.storeinfo.view", "Permission to view Store Info menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtStoreInfoMenu = new ApplicationPermissionsTree(id++, "Manage Store Info menu", "mosmgt.settingmgt.storeinfo.manage", "Permission to view Store Info menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtPaymentTypesMenu = new ApplicationPermissionsTree(id++, "View Payment Type menu", "mosmgt.settingmgt.payment.paymenttype.view", "Permission to view Payment Type menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtPaymentTypesMenu = new ApplicationPermissionsTree(id++, "Manage Payment Type menu", "mosmgt.settingmgt.payment.paymenttype.manage", "Permission to view Payment Type menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtTransactionFeesMenu = new ApplicationPermissionsTree(id++, "View Transaction Fee menu", "mosmgt.settingmgt.payment.transactionfee.view", "Permission to view Transaction Fee menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtTransactionFeesMenu = new ApplicationPermissionsTree(id++, "Manage Transaction Fee menu", "mosmgt.settingmgt.payment.transactionfee.manage", "Permission to view Transaction Fee menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtVoucherTypesMenu = new ApplicationPermissionsTree(id++, "View Voucher Type menu", "mosmgt.settingmgt.payment.vouchertype.view", "Permission to view Voucher Type menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtVoucherTypesMenu = new ApplicationPermissionsTree(id++, "Manage Voucher Type menu", "mosmgt.settingmgt.payment.vouchertype.manage", "Permission to view Voucher Type menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtVouchersMenu = new ApplicationPermissionsTree(id++, "View Voucher menu", "mosmgt.settingmgt.payment.voucher.view", "Permission to view Voucher menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtVouchersMenu = new ApplicationPermissionsTree(id++, "Manage Voucher menu", "mosmgt.settingmgt.payment.voucher.manage", "Permission to view Voucher menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ViewMOSOrderMgtWaiversMenu = new ApplicationPermissionsTree(id++, "View Waiver menu", "mosmgt.settingmgt.payment.waiver.view", "Permission to view Waiver menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtWaiversMenu = new ApplicationPermissionsTree(id++, "Manage Waiver menu", "mosmgt.settingmgt.payment.waiver.manage", "Permission to view Waiver menu", ViewMOSMealMgtAccountMgtMenu);

        public static ApplicationPermissionsTree ManageMOSOrderMgtContactUsSubjectsPermission = new ApplicationPermissionsTree(id++, "Manage Contact Us Subjects menu", "mosmgt.settingmgt.contactus.subject.manage", "Permission to manage Contact Us Subjects menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtContactUsQuestionsPermission = new ApplicationPermissionsTree(id++, "Manage Contact Us Questions menu", "mosmgt.settingmgt.contactus.question.manage", "Permission to manage Contact Us Questions menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtNotificationSettingsPermission = new ApplicationPermissionsTree(id++, "Manage Notification Settings menu", "mosmgt.settingmgt.notifications.settings.manage", "Permission to manage Notification Settings menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtNotificationEventsPermission = new ApplicationPermissionsTree(id++, "Manage Notification Events menu", "mosmgt.settingmgt.notifications.events.manage", "Permission to manage Notification Events menu", ViewMOSMealMgtAccountMgtMenu);
        public static ApplicationPermissionsTree ManageMOSOrderMgtCancelRequestsPermission = new ApplicationPermissionsTree(id++, "Manage Cancel Requests menu", "mosmgt.settingmgt.orders.cancelrequest.manage", "Permission to manage Cancel Requests menu", ViewMOSMealMgtAccountMgtMenu);


        #region Outlets

        #endregion

        #endregion

        //#region Dashboard ACL
        //public const string DashboardMenuPermissionGroupName = "Dashboard Permissions";
        //public static ApplicationPermissionsTree ViewDashboardMenu = new ApplicationPermissionsTree(47, "View Dashboard Menu", "dashboardmenu.view", DashboardMenuPermissionGroupName, "Permission to view dashboard menu", Root);

        //public const string DashboardPermissionGroupName = "Dashboard";
        //public static ApplicationPermissionsTree ViewDashboard = new ApplicationPermissionsTree(48, "View Dashboard", "dashboard.view", DashboardPermissionGroupName, "Permission to view dashboard", ViewDashboardMenu);
        //public static ApplicationPermissionsTree ViewSignageDashboard = new ApplicationPermissionsTree(49, "View Signage Dashboard", "signagedashboard.view", DashboardPermissionGroupName, "Permission to view signage dashboard", ViewDashboardMenu);

        //#endregion

        //#region Devices ACL
        //public const string DeviceMenuPermissionGroupName = "Device Permissions";
        //public static ApplicationPermissionsTree ViewDeviceMenu = new ApplicationPermissionsTree(50, "View Device Menu", "devicemenu.view", DashboardMenuPermissionGroupName, "Permission to view device menu", Root);

        //public const string DevicesPermissionGroupName = "Devices";
        //public static ApplicationPermissionsTree ViewDevices = new ApplicationPermissionsTree(51, "View Devices", "devices.view", DevicesPermissionGroupName, "Permission to view available devices", ViewDeviceMenu);
        //public static ApplicationPermissionsTree ManageDevices = new ApplicationPermissionsTree(52, "Manage Devices", "devices.manage", DevicesPermissionGroupName, "Permission to create, delete and modify devices", ViewDeviceMenu);

        //#endregion

        //#region Reservation ACL
        //public const string ReservationMenuPermissionGroupName = "Reservation Menu Permissions";
        //public static ApplicationPermissionsTree ViewReservationMenu = new ApplicationPermissionsTree(53, "View Reservation Menu", "reservationmenu.view", ReservationMenuPermissionGroupName, "Permission to view reservation menu", Root);

        //public const string ReservationsPermissionGroupName = "Reservations";
        //public static ApplicationPermissionsTree AddReservations = new ApplicationPermissionsTree(54, "Add Reservations", "reservations.add", ReservationsPermissionGroupName, "Permission to add reservations", ViewReservationMenu);
        //public static ApplicationPermissionsTree ManageReservations = new ApplicationPermissionsTree(55, "Manage Reservations", "reservations.manage", ReservationsPermissionGroupName, "Permission to create, delete and modify reservations", ViewReservationMenu);

        //#endregion

        //#region Reports ACL
        //public const string ReportMenuPermissionGroupName = "Reports Menu Permissions";
        //public static ApplicationPermissionsTree ViewReportMenu = new ApplicationPermissionsTree(56, "View Reports Menu", "reportsmenu.view", ReportMenuPermissionGroupName, "Permission to view reports menu", Root);

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
                SSManageUserMenu,

                SSRoleMenu,
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
                ViewMOSAccountMgtMenu,
                ViewMOSMealMgtAccountMgtMenu,
                ViewMOSOrganisationMgtMenu,
                ViewMOSSettingMgtMenu,
                ViewMOSreportMgtMenu,
                ViewMOSSystemMgtMenu,

                ViewMOSOrderMgtRestrictionTypesMenu,
                ManageMOSOrderMgtRestrictionTypesMenu,

                ViewMOSOrderMgtRestrictionsMenu,
                ManageMOSOrderMgtRestrictionsMenu,

                ViewMOSOrderMgtCuisinesMenu,
                ManageMOSOrderMgtCuisinesMenu,

                ViewMOSOrderMgtDeliveryMenu,
                ManageMOSOrderMgtDeliveryMenu,

                ViewMOSOrderMgtBentoBoxTypesMenu,
                ManageMOSOrderMgtBentoBoxTypesMenu,

                ViewMOSOrderMgtBentoAssetsMenu,
                ManageMOSOrderMgtBentoAssetsMenu,

                ViewMOSOrderMgtCartonTypesMenu,
                ManageMOSOrderMgtCartonTypesMenu,

                ViewMOSOrderMgtCartonAssetsMenu,
                ManageMOSOrderMgtCartonAssetsMenu,

                ViewMOSOrderMgtTrackingStatusMenu,
                ManageMOSOrderMgtTrackingStatusMenu,

                ViewMOSOrderMgtDeliveryOrdersMenu,
                ManageMOSOrderMgtDeliveryOrdersMenu,

                ViewMOSOrderMgtDishingProcessMenu,
                ManageMOSOrderMgtDishingProcessMenu,

                ViewMOSOrderMgtPackingProcessMenu,
                ManageMOSOrderMgtPackingProcessMenu,

                ViewMOSOrderMgtDriversMenu,
                ManageMOSOrderMgtDriversMenu,

                ViewMOSOrderMgtRoutesMenu,
                ManageMOSOrderMgtRoutesMenu,

                ViewMOSOrderMgtCaterersMenu,
                ManageMOSOrderMgtCaterersMenu,

                ViewMOSOrderMgtOutletProfilesMenu,
                ManageMOSOrderMgtOutletProfilesMenu,

                ViewMOSOrderMgtOutletsMenu,
                ManageMOSOrderMgtOutletsMenu,

                ViewMOSOrderMgtStoreInfoMenu,
                ManageMOSOrderMgtStoreInfoMenu,

                ViewMOSOrderMgtPaymentTypesMenu,
                ManageMOSOrderMgtPaymentTypesMenu,

                ViewMOSOrderMgtTransactionFeesMenu,
                ManageMOSOrderMgtTransactionFeesMenu,

                ViewMOSOutletMgtStoresPermission,
                ManageMOSOutletMgtStoresPermission,

                ViewMOSOutletMgtCaterersPermission,
                ManageMOSOutletMgtCaterersPermission,


                ViewMOSOutletMgtReportsPermission,

                ManageMOSOutletMgtMealSummaryPermission,

                ViewMOSOrderMgtVoucherTypesMenu,
                ManageMOSOrderMgtVoucherTypesMenu,

                ViewMOSOrderMgtVouchersMenu,
                ManageMOSOrderMgtVouchersMenu,

                ViewMOSOrderMgtWaiversMenu,
                ManageMOSOrderMgtWaiversMenu,

                //ViewAccountsMenu,
                //ViewFRSMenu,
                //ViewDashboardMenu,
                //ViewDeviceMenu,
                //ViewReservationMenu,
                //ViewReportMenu,
                //UsersMenu,
                //RolesMenu,
                //FacilitiesMenu,
                //FacilityTypesMenu,
                //LocationsMenu,
                //InstitutionsMenu,
                //DepartmentsMenu,
                //ContactGroupsMenu,
                //PhonebooksMenu,
                //VehiclesMenu,
                //UserGroupsMenu,
                //PublicationsMenu,
                //EpaperTemplatesMenu,
                //EpaperDevicesMenu,

                //ViewUsers,
                //ManageUsers,

                //ViewRoles,
                //ManageRoles,
                //AssignRoles,

                //ViewFacilities,
                //ManageFacilities,

                //ViewFacilityTypes,
                //ManageFacilityTypes,

                //ViewDashboard,
                //ViewSignageDashboard,

                //AddReservations,
                //ManageReservations,

                //ViewDevices,
                //ManageDevices,

                //ViewLocations,
                //ManageLocations,

                //ViewInstitutions,
                //ManageInstitutions,

                //ViewDepartments,
                //ManageDepartments,

                //ViewContactGroups,
                //ManageContactGroups,

                //ViewUserPhonebooks,
                //ManageUserPhonebooks,

                //ViewReports,

                ////ViewPIBTemplates,
                ////ManagePIBTemplates,

                ////ViewPIBDevices,
                ////ManagePIBDevices,

                //ViewEpaperTemplates,
                //ManageEpaperTemplates,

                //ViewEpaperDevices,
                //ManageEpaperDevices,

                //ManageUserVehicles,

                //ApprovePublications,

                //ViewUserGroups,
                //ManageUserGroups

                 ViewMOSOutletMgtTermsPermission,
                 ManageMOSOutletMgtTermsPermission,

                 ViewMOSOutletMgtClassBatchesPermission,
                 ManageMOSOutletMgtClassBatchesPermission,

                 ViewMOSOutletMgtClassLevelsPermission,
                 ManageMOSOutletMgtClassLevelsPermission,

                 ViewMOSOutletMgtClassesPermission,
                 ManageMOSOutletMgtClassesPermission,

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

                 ViewMOSOutletMgtFasPermission,
                 ManageMOSOutletMgtFasPermission,

                 ViewMOSOutletMgtStudentGroupsPermission,
                 ManageMOSOutletMgtStudentGroupsPermission,

                 ViewMOSOutletMgtMenusPermission,
                 ManageMOSOutletMgtMenusPermission,

                 ViewMOSOutletMgtCancellationsPermission,
                 ManageMOSOutletMgtCancellationsPermission,

                 ViewMOSOutletMgtPortalContentsPermission,
                 ManageMOSOutletMgtPortalContentsPermission,

                 ViewMOSOutletMgtEmailTemplatesPermission,
                 ManageMOSOutletMgtEmailTemplatesPermission,

                 ViewMOSOutletMgtMealAllocationsPermission,
                 ManageMOSOutletMgtMealAllocationsPermission,

                 ViewMOSOutletMgtPackingAllocationsPermission,
                 ManageMOSOutletMgtPackingAllocationsPermission,

                 ManageMOSOrderMgtContactUsSubjectsPermission,
                 ManageMOSOrderMgtContactUsQuestionsPermission,
                 ManageMOSOrderMgtNotificationSettingsPermission,
                 ManageMOSOrderMgtNotificationEventsPermission,
                 ManageMOSOrderMgtCancelRequestsPermission
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
