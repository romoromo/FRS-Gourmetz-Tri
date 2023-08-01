//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Collections.ObjectModel;

//namespace DAL.Core
//{
//    public static class ApplicationPermissions
//    {
//        public static ReadOnlyCollection<ApplicationPermission> AllPermissions;


//        public const string UsersPermissionGroupName = "User Permissions";
//        public static ApplicationPermission ViewUsers = new ApplicationPermission("View Users", "users.view", UsersPermissionGroupName, "Permission to view other users account details");
//        public static ApplicationPermission ManageUsers = new ApplicationPermission("Manage Users", "users.manage", UsersPermissionGroupName, "Permission to create, delete and modify other users account details");

//        public const string RolesPermissionGroupName = "Role Permissions";
//        public static ApplicationPermission ViewRoles = new ApplicationPermission("View Roles", "roles.view", RolesPermissionGroupName, "Permission to view available roles");
//        public static ApplicationPermission ManageRoles = new ApplicationPermission("Manage Roles", "roles.manage", RolesPermissionGroupName, "Permission to create, delete and modify roles");
//        public static ApplicationPermission AssignRoles = new ApplicationPermission("Assign Roles", "roles.assign", RolesPermissionGroupName, "Permission to assign roles to users");

//        public const string FacilitiesPermissionGroupName = "Facilities Permissions";
//        public static ApplicationPermission ViewFacilities = new ApplicationPermission("View Facilities", "facilities.view", FacilitiesPermissionGroupName, "Permission to view available facilities");
//        public static ApplicationPermission ManageFacilities = new ApplicationPermission("Manage Facilities", "facilities.manage", FacilitiesPermissionGroupName, "Permission to create, delete and modify facilities");

//        public const string FacilityTypesPermissionGroupName = "Facility Types Permissions";
//        public static ApplicationPermission ViewFacilityTypes = new ApplicationPermission("View Facility Types", "facilitytypes.view", FacilitiesPermissionGroupName, "Permission to view available facility types");
//        public static ApplicationPermission ManageFacilityTypes = new ApplicationPermission("Manage Facility Types", "facilitytypes.manage", FacilitiesPermissionGroupName, "Permission to create, delete and modify facility types");

//        public const string ReservationsPermissionGroupName = "Reservations Permissions";
//        public static ApplicationPermission AddReservations = new ApplicationPermission("Add Reservations", "reservations.add", ReservationsPermissionGroupName, "Permission to add reservations");
//        public static ApplicationPermission ManageReservations = new ApplicationPermission("Manage Reservations", "reservations.manage", ReservationsPermissionGroupName, "Permission to create, delete and modify reservations");

//        public const string DevicesPermissionGroupName = "Devices Permissions";
//        public static ApplicationPermission ViewDevices = new ApplicationPermission("View Devices", "devices.view", DevicesPermissionGroupName, "Permission to view available devices");
//        public static ApplicationPermission ManageDevices = new ApplicationPermission("Manage Devices", "devices.manage", DevicesPermissionGroupName, "Permission to create, delete and modify devices");

//        public const string LocationsPermissionGroupName = "Locations Permissions";
//        public static ApplicationPermission ViewLocations = new ApplicationPermission("View Locations", "locations.view", LocationsPermissionGroupName, "Permission to view available locations");
//        public static ApplicationPermission ManageLocations = new ApplicationPermission("Manage Locations", "locations.manage", LocationsPermissionGroupName, "Permission to create, delete and modify locations");

//        public const string InstitutionsPermissionGroupName = "Institutions Permissions";
//        public static ApplicationPermission ViewInstitutions = new ApplicationPermission("View Institutions", "institutions.view", InstitutionsPermissionGroupName, "Permission to view available institutions");
//        public static ApplicationPermission ManageInstitutions = new ApplicationPermission("Manage Institutions", "institutions.manage", InstitutionsPermissionGroupName, "Permission to create, delete and modify institutions");

//        public const string DepartmentsPermissionGroupName = "Departments Permissions";
//        public static ApplicationPermission ViewDepartments = new ApplicationPermission("View Departments", "departments.view", DepartmentsPermissionGroupName, "Permission to view available departments");
//        public static ApplicationPermission ManageDepartments = new ApplicationPermission("Manage Departments", "departments.manage", DepartmentsPermissionGroupName, "Permission to create, delete and modify departments");

//        public const string ContactGroupsPermissionGroupName = "Contact Groups Permissions";
//        public static ApplicationPermission ViewContactGroups = new ApplicationPermission("View Contact Groups", "contactgroups.view", ContactGroupsPermissionGroupName, "Permission to view available contact groups");
//        public static ApplicationPermission ManageContactGroups = new ApplicationPermission("Manage Contact Groups", "contactgroups.manage", ContactGroupsPermissionGroupName, "Permission to create, delete and modify contact groups");

//        public const string UserPhonebooksPermissionGroupName = "Phonebooks Permissions";
//        public static ApplicationPermission ViewUserPhonebooks = new ApplicationPermission("View Phonebooks", "userphonebooks.view", UserPhonebooksPermissionGroupName, "Permission to view available phone book");
//        public static ApplicationPermission ManageUserPhonebooks = new ApplicationPermission("Manage Phonebooks", "userphonebooks.manage", UserPhonebooksPermissionGroupName, "Permission to create, delete and modify phone book");

//        public const string ReportsPermissionGroupName = "Reports Permissions";
//        public static ApplicationPermission ViewReports = new ApplicationPermission("View Reports", "reports.view", ReportsPermissionGroupName, "Permission to view reports");

//        public const string PIBTemplatesPermissionGroupName = "PIB Templates Permissions";
//        public static ApplicationPermission ViewPIBTemplates = new ApplicationPermission("View PIB Templates", "pibtemplates.view", PIBTemplatesPermissionGroupName, "Permission to view available pib templates");
//        public static ApplicationPermission ManagePIBTemplates = new ApplicationPermission("Manage PIB Templates", "pibtemplates.manage", PIBTemplatesPermissionGroupName, "Permission to create, delete and modify pib templates");

//        public const string PIBDevicesPermissionGroupName = "PIB Devices Permissions";
//        public static ApplicationPermission ViewPIBDevices = new ApplicationPermission("View PIB Devices", "pibdevices.view", PIBDevicesPermissionGroupName, "Permission to view available pib devices");
//        public static ApplicationPermission ManagePIBDevices = new ApplicationPermission("Manage PIB Devices", "pibdevices.manage", PIBDevicesPermissionGroupName, "Permission to create, delete and modify pib devices");

//        public const string EpaperTemplatesPermissionGroupName = "Epaper Templates Permissions";
//        public static ApplicationPermission ViewEpaperTemplates = new ApplicationPermission("View Epaper Templates", "epapertemplates.view", EpaperTemplatesPermissionGroupName, "Permission to view available epaper templates");
//        public static ApplicationPermission ManageEpaperTemplates = new ApplicationPermission("Manage Epaper Templates", "epapertemplates.manage", EpaperTemplatesPermissionGroupName, "Permission to create, delete and modify epaper templates");

//        public const string EpaperDevicesPermissionGroupName = "Epaper Devices Permissions";
//        public static ApplicationPermission ViewEpaperDevices = new ApplicationPermission("View Epaper Devices", "epaperdevices.view", EpaperDevicesPermissionGroupName, "Permission to view available epaper devices");
//        public static ApplicationPermission ManageEpaperDevices = new ApplicationPermission("Manage Epaper Devices", "epaperdevices.manage", EpaperDevicesPermissionGroupName, "Permission to create, delete and modify epaper devices");

//        //public const string CalendarPermissionGroupName = "Calendar Permissions";
//        //public static ApplicationPermission ViewCalendar = new ApplicationPermission("View Calendar", "calendar.view", CalendarPermissionGroupName, "Permission to view calendar");

//        public const string DashboardPermissionGroupName = "Dashboard Permissions";
//        public static ApplicationPermission ViewDashboard = new ApplicationPermission("View Dashboard", "dashboard.view", DashboardPermissionGroupName, "Permission to view dashboard");
//        public static ApplicationPermission ViewSignageDashboard = new ApplicationPermission("View Signage Dashboard", "signagedashboard.view", DashboardPermissionGroupName, "Permission to view signage dashboard");

//        public const string UserVehiclesPermissionGroupName = "Vehicles Permissions";
//        public static ApplicationPermission ManageUserVehicles = new ApplicationPermission("Manage Vehicles", "uservehicles.manage", UserVehiclesPermissionGroupName, "Permission to create, delete and modify vehicles");

//        public const string PublicationsPermissionGroupName = "Publications Permissions";
//        public static ApplicationPermission ApprovePublications = new ApplicationPermission("Approve Signage Publications", "publications.approve", PublicationsPermissionGroupName, "Permission to approve signage publications");

//        public const string UserGroupPermissionGroupName = "User Group Permissions";
//        public static ApplicationPermission ViewUserGroups = new ApplicationPermission("View User Group", "usergroups.view", UserGroupPermissionGroupName, "Permission to view user groups");
//        public static ApplicationPermission ManageUserGroups = new ApplicationPermission("Manage User Group", "usergroups.manage", UserGroupPermissionGroupName, "Permission to create, delete and modify user groups");


//        static ApplicationPermissions()
//        {
//            List<ApplicationPermission> allPermissions = new List<ApplicationPermission>()
//            {
//                ViewUsers,
//                ManageUsers,

//                ViewRoles,
//                ManageRoles,
//                AssignRoles,

//                ViewFacilities,
//                ManageFacilities,

//                ViewFacilityTypes,
//                ManageFacilityTypes,

//                //ViewCalendar,

//                ViewDashboard,
//                ViewSignageDashboard,

//                AddReservations,
//                ManageReservations,

//                ViewDevices,
//                ManageDevices,

//                ViewLocations,
//                ManageLocations,

//                ViewInstitutions,
//                ManageInstitutions,

//                ViewDepartments,
//                ManageDepartments,

//                ViewContactGroups,
//                ManageContactGroups,

//                ViewUserPhonebooks,
//                ManageUserPhonebooks,

//                ViewReports,

//                ViewPIBTemplates,
//                ManagePIBTemplates,

//                ViewPIBDevices,
//                ManagePIBDevices,

//                ViewEpaperTemplates,
//                ManageEpaperTemplates,

//                ViewEpaperDevices,
//                ManageEpaperDevices,

//                ManageUserVehicles,

//                ApprovePublications,

//                ViewUserGroups,
//                ManageUserGroups
//            };

//            AllPermissions = allPermissions.AsReadOnly();
//        }

//        public static ApplicationPermission GetPermissionByName(string permissionName)
//        {
//            return AllPermissions.Where(p => p.Name == permissionName).SingleOrDefault();
//        }

//        public static ApplicationPermission GetPermissionByValue(string permissionValue)
//        {
//            return AllPermissions.Where(p => p.Value == permissionValue).SingleOrDefault();
//        }

//        public static string[] GetAllPermissionValues()
//        {
//            return AllPermissions.Select(p => p.Value).ToArray();
//        }

//        public static string[] GetAdministrativePermissionValues()
//        {
//            return new string[] { ManageUsers, ManageRoles, AssignRoles };
//        }
//    }



//    public class ApplicationPermission
//    {
//        public ApplicationPermission()
//        { }

//        public ApplicationPermission(string name, string value, string groupName, string description = null)
//        {
//            Name = name;
//            Value = value;
//            GroupName = groupName;
//            Description = description;
//        }



//        public string Name { get; set; }
//        public string Value { get; set; }
//        public string GroupName { get; set; }
//        public string Description { get; set; }


//        public override string ToString()
//        {
//            return Value;
//        }


//        public static implicit operator string(ApplicationPermission permission)
//        {
//            return permission.Value;
//        }
//    }
//}
