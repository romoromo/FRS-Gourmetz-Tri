using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.Interfaces;

namespace DAL
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync(bool applyMigration = true, bool seed = true);
        Task RemoveUserConnectionsAsync();
        Task RemoveDeviceConnectionsAsync();
    }




    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountManager _accountManager;
        private readonly ILogger _logger;

        public DatabaseInitializer(ApplicationDbContext context, IAccountManager accountManager, ILogger<DatabaseInitializer> logger)
        {
            _accountManager = accountManager;
            _context = context;
            _logger = logger;
        }

        #region SEED
        public async Task SeedAsync(bool applyMigration = true, bool seed = true)
        {
            if(applyMigration)
                await _context.Database.MigrateAsync().ConfigureAwait(false);

            if (seed)
            {
                if (!await _context.Institutions.AnyAsync())
                {
                    this.CreateInstitutions();
                    int times = await _context.SaveChangesAsync();
                }

                var institution = _context.Institutions.FirstOrDefault(e => e.Name == "KTPH");
                institution = institution ?? _context.Institutions.FirstOrDefault();

                //Departments
                if (!await _context.Departments.AnyAsync())
                {
                    this.CreateDepartments(institution);
                    int types = await _context.SaveChangesAsync();
                }

                if (!await _context.Users.AnyAsync())
                {
                    var department = _context.Departments.FirstOrDefault(e => e.Name == "IT Department");
                    department = department ?? _context.Departments.FirstOrDefault();

                    _logger.LogInformation("Generating inbuilt accounts");

                    const string adminRoleName = "administrator";
                    const string userRoleName = "user";

                    await EnsureRoleAsync(adminRoleName, "Default administrator", ApplicationPermissionsTrees.GetAllPermissionValues());
                    await EnsureRoleAsync(userRoleName, "Default user", new string[] { });

                    await CreateUserAsync("superadmin", "smv888", "Administrator", "superadmin@serialmultivision.com", "+1 (123) 000-0000", new string[] { adminRoleName }, null, null);
                    await CreateUserAsync("admin", "smv888", "Administrator", "admin@serialmultivision.com", "+1 (123) 000-0000", new string[] { adminRoleName }, institution, department);
                    await CreateUserAsync("user", "demo123", "Standard User", "user@serialmultivision.com", "+1 (123) 000-0001", new string[] { userRoleName }, institution, department);

                    _logger.LogInformation("Inbuilt account generation completed");
                }

                if (!await _context.TimeIntervals.AnyAsync())
                {
                    this.CreateTimingAsync();
                    int times = await _context.SaveChangesAsync();
                }

                //Facility Type
                if (!await _context.FacilityTypes.AnyAsync())
                {
                    this.CreateFacilityTypes(institution);
                    int types = await _context.SaveChangesAsync();
                }

                //Facilities
                if (!await _context.Facilities.AnyAsync())
                {
                    this.CreateFacilities(institution);
                    int types = await _context.SaveChangesAsync();
                }

                if (!await _context.LocationTypes.AnyAsync())
                {
                    this.CreateLocationTypes();
                    int types = await _context.SaveChangesAsync();
                }

                if (!await _context.Locations.AnyAsync())
                {
                    this.CreateLocations();
                    int types = await _context.SaveChangesAsync();
                }
            }
        }


        private async Task EnsureRoleAsync(string roleName, string description, string[] claims)
        {
            if ((await _accountManager.GetRoleByNameAsync(roleName)) == null)
            {
                ApplicationRole applicationRole = new ApplicationRole(roleName, description);

                var result = await this._accountManager.CreateRoleAsync(applicationRole, claims);

                if (!result.Item1)
                    throw new Exception($"Seeding \"{description}\" role failed. Errors: {string.Join(Environment.NewLine, result.Item2)}");
            }
        }

        private async Task<ApplicationUser> CreateUserAsync(string userName, string password, string fullName, string email, string phoneNumber, string[] roles, Institution institution, Department department)
        {
            //var institution = _context.Institutions.FirstOrDefault();
            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = userName,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                IsEnabled = true
            };

            if (institution != null) applicationUser.InstitutionId = institution.Id;
            if (department != null) applicationUser.DepartmentId = department.Id;
            var result = await _accountManager.CreateUserAsync(applicationUser, roles, password);

            if (!result.Item1)
                throw new Exception($"Seeding \"{userName}\" user failed. Errors: {string.Join(Environment.NewLine, result.Item2)}");


            return applicationUser;
        }

        private async Task CreateTimingAsync()
        {
            var list = new List<TimeInterval>();
            list.AddRange(new List<TimeInterval> {
                  new TimeInterval("12:00 AM", "12:00", 0, 0),
                  new TimeInterval("12:30 AM", "12:30", 0, 30),
                  new TimeInterval("01:00 AM", "01:00", 1, 00),
                  new TimeInterval("01:30 AM", "01:30", 1, 30),
                  new TimeInterval("02:00 AM", "02:00", 2, 00),
                  new TimeInterval("02:30 AM", "02:30", 2, 30),
                  new TimeInterval("03:00 AM", "03:00", 3, 00),
                  new TimeInterval("03:30 AM", "03:30", 3, 30),
                  new TimeInterval("04:00 AM", "04:00", 4, 00),
                  new TimeInterval("04:30 AM", "04:30", 4, 30),
                  new TimeInterval("05:00 AM", "05:00", 5, 00),
                  new TimeInterval("05:30 AM", "05:30", 5, 30),
                  new TimeInterval("06:00 AM", "06:00", 6, 00),
                  new TimeInterval("06:30 AM", "06:30", 6, 30),
                  new TimeInterval("07:00 AM", "07:00", 7, 00),
                  new TimeInterval("07:30 AM", "07:30", 7, 30),
                  new TimeInterval("08:00 AM", "08:00", 8, 00),
                  new TimeInterval("08:30 AM", "08:30", 8, 30),
                  new TimeInterval("09:00 AM", "09:00", 9, 00),
                  new TimeInterval("09:30 AM", "09:30", 9, 30),
                  new TimeInterval("10:00 AM", "10:00", 10, 00),
                  new TimeInterval("10:30 AM", "10:30", 10, 30),
                  new TimeInterval("11:00 AM", "11:00", 11, 00),
                  new TimeInterval("11:30 AM", "11:30", 11, 30),
                  new TimeInterval("12:00 PM", "12:00", 12, 00),
                  new TimeInterval("12:30 PM", "12:30", 12, 30),
                  new TimeInterval("01:00 PM", "01:00", 13, 00),
                  new TimeInterval("01:30 PM", "01:30", 13, 30),
                  new TimeInterval("02:00 PM", "02:00", 14, 00),
                  new TimeInterval("02:30 PM", "02:30", 14, 30),
                  new TimeInterval("03:00 PM", "03:00", 15, 00),
                  new TimeInterval("03:30 PM", "03:30", 15, 30),
                  new TimeInterval("04:00 PM", "04:00", 16, 00),
                  new TimeInterval("04:30 PM", "04:30", 16, 30),
                  new TimeInterval("05:00 PM", "05:00", 17, 00),
                  new TimeInterval("05:30 PM", "05:30", 17, 30),
                  new TimeInterval("06:00 PM", "06:00", 18, 00),
                  new TimeInterval("06:30 PM", "06:30", 18, 30),
                  new TimeInterval("07:00 PM", "07:00", 19, 00),
                  new TimeInterval("07:30 PM", "07:30", 19, 30),
                  new TimeInterval("08:00 PM", "08:00", 20, 00),
                  new TimeInterval("08:30 PM", "08:30", 20, 30),
                  new TimeInterval("09:00 PM", "09:00", 21, 00),
                  new TimeInterval("09:30 PM", "09:30", 21, 30),
                  new TimeInterval("10:00 PM", "10:00", 22, 00),
                  new TimeInterval("10:30 PM", "10:30", 22, 30),
                  new TimeInterval("11:00 PM", "11:00", 22, 00),
                  new TimeInterval("11:30 PM", "11:30", 23, 30) });
            await _context.TimeIntervals.AddRangeAsync(list.OrderBy(e => e.Hour).ThenBy(e => e.Minutes)
                    );
        }

        private async Task CreateDepartments(Institution institution)
        {
            var list = new List<Department>();
            list.AddRange(new List<Department>
            {
                new Department { Name = "IT Department", Description = "IT Department", InstitutionId = institution.Id },
                new Department { Name = "HR Department", Description = "HR Department", InstitutionId = institution.Id }
            });

            await _context.Departments.AddRangeAsync(list);
        }

        private async Task CreateFacilityTypes(Institution institution)
        {
            var list = new List<FacilityType>();
            list.AddRange(new List<FacilityType>
            {
                new FacilityType { Name = "Equipment", Description = "Equipment", InstitutionId = institution.Id }
            });

            await _context.FacilityTypes.AddRangeAsync(list);
        }

        private async Task CreateFacilities(Institution institution)
        {
            var facilityType = _context.FacilityTypes.FirstOrDefault(e => e.InstitutionId == institution.Id && e.Name == "Equipment");
            var list = new List<Facility>();
            list.AddRange(new List<Facility>
            {
                new Facility { Name = "Equipment", Description = "Equipment", InstitutionId = institution.Id }
            });

            await _context.Facilities.AddRangeAsync(list);
        }

        private async Task CreateLocationTypes()
        {
            var list = new List<LocationType>();
            list.AddRange(new List<LocationType>
            {
                new LocationType { Name = "Building", Description = "Building" },
                new LocationType { Name = "Room", Description="Room" },
                new LocationType { Name = "Level", Description="Level" },
                new LocationType { Name = "Hospital", Description="Hospital" }
            });

            await _context.LocationTypes.AddRangeAsync(list);
        }

        private async Task CreateLocations()
        {
            var institution = _context.Institutions.FirstOrDefault(e => e.IsActive && e.Name == "KTPH");
            var hospital = _context.LocationTypes.FirstOrDefault(e => e.IsActive && e.Name == "Hospital");
            var building = _context.LocationTypes.FirstOrDefault(e => e.IsActive && e.Name == "Building");
            var room = _context.LocationTypes.FirstOrDefault(e => e.IsActive && e.Name == "Room");
            var rootLocation = new Location
            {
                Name = "KTPH",
                Description = "Khoo Teck Phua Hospital",
                InstitutionId = institution.Id,
                LocationTypeId = hospital.Id,
                IsActive = true
            };

            var bldg = new Location
            {
                Name = "BLDG 1",
                Description = "BLDG 1",
                InstitutionId = institution.Id,
                LocationTypeId = building.Id,
                IsActive = true,
                ParentLocation = rootLocation                 
            };

            var r = new Location
            {
                Name = "Room 1",
                Description = "Room 1",
                InstitutionId = institution.Id,
                LocationTypeId = room.Id,
                IsActive = true,
                ParentLocation = bldg
            };

            await _context.Locations.AddAsync(r);
        }

        private async Task CreateInstitutions()
        {
            var list = new List<Institution>();
            list.AddRange(new List<Institution>
            {
                new Institution { Name = "KTPH", Description = "Khoo Teck Phua Hospital", StartTime = 8, EndTime = 20 },
                new Institution { Name = "YCH", Description="Yishun Community Hospital", StartTime = 8, EndTime = 20 }
            });

            await _context.Institutions.AddRangeAsync(list);
        }

        #endregion

        #region User Connections
        public async Task RemoveUserConnectionsAsync()
        {
            var connections = _context.UserConnections.Where(e => e.IsActive);
            foreach(var conn in connections)
            {
                //conn.Status = UserConnectionStatus.OFFLINE.ToString();
                conn.IsActive = false;
                _context.Update(conn);
            }

            var users = _context.Users.Where(e => e.IsActive && e.IsConnected);
            foreach (var user in users)
            {
                //user.Status = UserConnectionStatus.OFFLINE.ToString();
                user.IsConnected = false;
                _context.Update(user);
            }

            await _context.SaveChangesAsync();
        }
        #endregion

        #region Device Connections
        public async Task RemoveDeviceConnectionsAsync()
        {
            var connections = _context.Connections.Where(e => e.IsActive);
            foreach (var conn in connections)
            {
                //conn.Status = UserConnectionStatus.OFFLINE.ToString();
                conn.IsActive = false;
                _logger.LogInformation(string.Format("connection updated from RemoveDeviceConnectionsAsync for CONNECTION ID {0}", conn.Identifier));
                _context.Update(conn);
            }

            var devices = _context.Devices.Where(e => e.IsActive && e.device_status != 0);
            foreach (var device in devices)
            {
                device.device_status = 0;
                _logger.LogInformation(string.Format("device_status updated from RemoveDeviceConnectionsAsync for DEVICE ID {0}", device.Id));
                _context.Update(device);
            }

            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
