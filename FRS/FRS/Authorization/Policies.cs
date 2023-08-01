using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.Authorization
{
    public class Policies
    {
        ///<summary>Policy to allow viewing all user records.</summary>
        public const string ViewAllUsersPolicy = "View All Users";

        ///<summary>Policy to allow adding, removing and updating all user records.</summary>
        public const string ManageAllUsersPolicy = "Manage All Users";

        /// <summary>Policy to allow viewing details of all roles.</summary>
        public const string ViewAllRolesPolicy = "View All Roles";

        /// <summary>Policy to allow viewing details of all or specific roles (Requires roleName as parameter).</summary>
        public const string ViewRoleByRoleNamePolicy = "View Role by RoleName";

        /// <summary>Policy to allow adding, removing and updating all roles.</summary>
        public const string ManageAllRolesPolicy = "Manage All Roles";

        /// <summary>Policy to allow assigning roles the user has access to (Requires new and current roles as parameter).</summary>
        public const string AssignAllowedRolesPolicy = "Assign Allowed Roles";

        /// <summary>Policy to allow viewing details of all facilities.</summary>
        public const string ViewAllFacilitiesPolicy = "View All Facilities";

        /// <summary>Policy to allow viewing details of all or specific facilities (Requires facilityName as parameter).</summary>
        public const string ViewFacilityByFacilityNamePolicy = "View Facility by FacilityName";

        /// <summary>Policy to allow adding, removing and updating all facilities.</summary>
        public const string ManageAllFacilitiesPolicy = "Manage All Facilities";

        /// <summary>Policy to allow viewing details of all facility types.</summary>
        public const string ViewAllFacilityTypesPolicy = "View All Facility Types";

        /// <summary>Policy to allow viewing details of all or specific facility types (Requires facilityName as parameter).</summary>
        public const string ViewFacilityTypeByFacilityTypeNamePolicy = "View Facility Type by FacilityName";

        /// <summary>Policy to allow adding, removing and updating all facility types.</summary>
        public const string ManageAllFacilityTypesPolicy = "Manage All Facility Types";

        /// <summary>Policy to allow adding, removing and updating all reservations.</summary>
        public const string ManageAllReservationsPolicy = "Manage All Reservations";

        /// <summary>Policy to allow adding reservations.</summary>
        public const string AddReservationPolicy = "Add Reservation Policy";

        /// <summary>Policy to allow viewing details of all devices.</summary>
        public const string ViewAllDevicesPolicy = "View All Devices";

        /// <summary>Policy to allow adding, removing and updating all devices.</summary>
        public const string ManageAllDevicesPolicy = "Manage All Devices";

        /// <summary>Policy to allow viewing details of all locations.</summary>
        public const string ViewAllLocationsPolicy = "View All Locations";

        /// <summary>Policy to allow adding, removing and updating all locations.</summary>
        public const string ManageAllLocationsPolicy = "Manage All Locations";

        /// <summary>Policy to allow viewing details of all Institutions.</summary>
        public const string ViewAllInstitutionsPolicy = "View All Institutions";

        /// <summary>Policy to allow adding, removing and updating all Institutions.</summary>
        public const string ManageAllInstitutionsPolicy = "Manage All Institutions";

        /// <summary>Policy to allow viewing details of all Departments.</summary>
        public const string ViewAllDepartmentsPolicy = "View All Departments";

        /// <summary>Policy to allow adding, removing and updating all Departments.</summary>
        public const string ManageAllDepartmentsPolicy = "Manage All Departments";

        /// <summary>Policy to allow viewing details of all contact groups.</summary>
        public const string ViewAllContactGroupsPolicy = "View All Contact Groups";

        /// <summary>Policy to allow adding, removing and updating all contact groups.</summary>
        public const string ManageAllContactGroupsPolicy = "Manage All Contact Groups";

        /// <summary>Policy to allow viewing details of all contacts.</summary>
        public const string ViewAllUserPhonebooksPolicy = "View All Phonebooks";

        /// <summary>Policy to allow adding, removing and updating all contacts.</summary>
        public const string ManageAllUserPhonebooksPolicy = "Manage All Phonebooks";

        /// <summary>Policy to allow viewing details of all pib templates.</summary>
        public const string ViewAllPIBTemplatesPolicy = "View All PIB Templates";

        /// <summary>Policy to allow adding, removing and updating all pib templates.</summary>
        public const string ManageAllPIBTemplatesPolicy = "Manage All PIB Templates";
    }



    /// <summary>
    /// Operation Policy to allow adding, viewing, updating and deleting general or specific user records.
    /// </summary>
    public static class AccountManagementOperations
    {
        public const string CreateOperationName = "Create";
        public const string ReadOperationName = "Read";
        public const string UpdateOperationName = "Update";
        public const string DeleteOperationName = "Delete";

        public static UserAccountAuthorizationRequirement Create = new UserAccountAuthorizationRequirement(CreateOperationName);
        public static UserAccountAuthorizationRequirement Read = new UserAccountAuthorizationRequirement(ReadOperationName);
        public static UserAccountAuthorizationRequirement Update = new UserAccountAuthorizationRequirement(UpdateOperationName);
        public static UserAccountAuthorizationRequirement Delete = new UserAccountAuthorizationRequirement(DeleteOperationName);
    }
}
