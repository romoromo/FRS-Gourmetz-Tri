using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Helpers;
using DAL.Models;
using DAL.Models.MealOrder;
using FRS.Helpers;
using FRS.ViewModels.MealOrder;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DAL.Core.Helpers.TreeExtensions;

namespace FRS.ViewModels
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(d => d.Roles, map => map.MapFrom(f => f.Roles.Where(e => e.Role.IsActive).Select(e => e.Role.Name)))
                .ForMember(d => d.UserGroupIds, map => map.MapFrom(f => f.UserGroupMembers.Select(e => e.UserGroupId)))
                .ForMember(d => d.InstitutionCode, map => map.MapFrom(s => s.Institution != null && s.Institution.IsActive ? s.Institution.Name : string.Empty))
                .ForMember(d => d.DirectoryListingCode, map => map.MapFrom(s => s.DirectoryListing != null && s.DirectoryListing.IsActive ? s.DirectoryListing.Code : string.Empty))
                .ForMember(d => d.DirectoryListingLabel, map => map.MapFrom(s => s.DirectoryListing != null && s.DirectoryListing.IsActive ? s.DirectoryListing.Label : string.Empty))
                .ForMember(d => d.Phonebooks, map => map.MapFrom(s => s.UserPhonebooks.Where(f => f.IsActive)))
                .ForMember(d => d.UserVehicles, map => map.MapFrom(s => s.UserVehicles.Where(f => f.IsActive)))
                .ForMember(d => d.UserCardIds, map => map.MapFrom(s => s.UserCardIds.Where(f => f.IsActive)))
                .ForMember(d => d.UserWallets, map => map.MapFrom(s => s.UserWallets.Where(f => f.IsActive)))
                .ForMember(d => d.UserRewards, map => map.MapFrom(s => s.UserRewards.Where(f => f.IsActive)))
                .ForMember(d => d.Students, map => map.MapFrom(s => s.Students.Where(f => f.IsActive)))
                .ForMember(d => d.DepartmentName, map => map.MapFrom(s => s.Department != null && s.Department.IsActive ? s.Department.Name : string.Empty))
                .ForMember(d => d.FilePath, map => map.MapFrom(s => s.Icon != null ? s.Icon.Path : Utilities.DefaultProfilePhotoPath()))
                .ForMember(d => d.Status, map => map.MapFrom(s => !string.IsNullOrEmpty(s.Status) ? s.Status : UserConnectionStatus.OFFDUTY.ToString()))
                .ForMember(d => d.IsConnected, map => map.MapFrom(s => s.UserConnections != null && s.UserConnections.Any(e => e.IsActive)))
                //.ForMember(d => d.Students, map => map.MapFrom(s => s.Students != null ? s.Students.Where(f => f.Student != null && f.Student.IsActive && f.IsActive) : null))
                .ForMember(d => d.UserOutlets, map => map.MapFrom(s => s.UserOutlets.Where(f => f.IsActive)))
                .ForMember(d => d.UserCaterers, map => map.MapFrom(s => s.UserCaterers.Where(f => f.IsActive)))
                ;

            CreateMap<ApplicationUser, UserSimpleViewModel>();

            CreateMap<StudentManageAccount, StudentManageAccountViewModel>()
                .ForMember(d => d.Name, map => map.MapFrom(s => s.Student != null ? s.Student.Name : string.Empty))
                .ForMember(d => d.OutletName, map => map.MapFrom(s => s.Student != null && s.Student.Outlet != null ? s.Student.Outlet.Name : string.Empty))
                .ForMember(d => d.Year, map => map.MapFrom(s => s.Student != null && s.Student.ClassBatch != null ? s.Student.ClassBatch.Year : 0))
                .ForMember(d => d.ClassName, map => map.MapFrom(s => s.Student != null && s.Student.Class != null ? s.Student.Class.Name : string.Empty))
                ;
            CreateMap<StudentManageAccountViewModel, StudentManageAccount>();

            CreateMap<UserOutlet, UserOutletViewModel>()
                .ForMember(d => d.OutletName, map => map.MapFrom(s => s.Outlet != null ? s.Outlet.Name : string.Empty));
            CreateMap<UserOutletViewModel, UserOutlet>();

            CreateMap<UserCaterer, UserCatererViewModel>()
                .ForMember(d => d.CatererName, map => map.MapFrom(s => s.Caterer != null ? s.Caterer.Name : string.Empty));
            CreateMap<UserCatererViewModel, UserCaterer>();

            CreateMap<UserViewModel, ApplicationUser>()
                .ForMember(d => d.Roles, map => map.Ignore())
                .ForMember(d => d.UserConnections, map => map.Ignore())
                //.ForMember(d => d.Students, map => map.Ignore())
                //.ForMember(d => d.UserGroupMembers, map => map.MapFrom(f => f.UserGroups.Select(s => new UserGroupMember
                //{
                //    UserGroupId = s.Id,
                //    UserId = f.Id
                //})))
                .ForPath(d => d.Icon.Path, map => map.MapFrom(s => s.FilePath));

            CreateMap<ApplicationUser, UserEditViewModel>()
                .ForMember(d => d.FilePath, map => map.MapFrom(s => s.Icon != null ? s.Icon.Path : Utilities.DefaultProfilePhotoPath()))
                .ForMember(d => d.UserGroupIds, map => map.MapFrom(f => f.UserGroupMembers.Where(e => e.UserGroup.IsActive).Select(e => e.UserGroupId)))

                .ForMember(d => d.Roles, map => map.MapFrom(f => f.Roles.Where(e => e.Role.IsActive).Select(e => e.Role.Name)));

            CreateMap<UserEditViewModel, ApplicationUser>()
                .ForMember(d => d.Roles, map => map.Ignore())
                .ForMember(d => d.UserConnections, map => map.Ignore())
                .ForPath(d => d.Icon.Path, map => map.MapFrom(s => s.FilePath));

            CreateMap<ApplicationUser, UserPatchViewModel>()
                .ForMember(d => d.FilePath, map => map.MapFrom(s => s.Icon != null ? s.Icon.Path : Utilities.DefaultProfilePhotoPath()))
                .ReverseMap();

            CreateMap<ApplicationRole, RoleViewModel>()
                .ForMember(d => d.Permissions, map => map.MapFrom(s => s.Claims))
                .ForMember(d => d.InstitutionCode, map => map.MapFrom(s => s.Institution != null && s.Institution.IsActive ? s.Institution.Name : string.Empty))
                .ForMember(d => d.UsersCount, map => map.MapFrom(s => s.UserRoles != null ? s.UserRoles.Count : 0))
                .ReverseMap();
            CreateMap<RoleViewModel, ApplicationRole>();

            CreateMap<UserRoleClaim, ClaimViewModel>()
                .ForMember(d => d.Type, map => map.MapFrom(s => s.ClaimType))
                .ForMember(d => d.Value, map => map.MapFrom(s => s.ClaimValue))
                .ReverseMap();

            //CreateMap<ApplicationPermission, PermissionViewModel>()
            //    .ReverseMap();

            //CreateMap<UserRoleClaim, PermissionViewModel>()
            //    .ConvertUsing(s => _mapper.Map<PermissionViewModel>(ApplicationPermissionsTrees.GetPermissionByValue(s.ClaimValue)));

            //CreateMap<ApplicationPermissionsTree, PermissionTreeViewModel>()
            //    .ReverseMap();

            //CreateMap<UserRoleClaim, PermissionTreeViewModel>()
            //    .ConvertUsing(s => _mapper.Map<PermissionTreeViewModel>(ApplicationPermissionsTrees.GetPermissionByValue(s.ClaimValue)));

            CreateMap<UserRoleClaim, PermissionViewModel>()
                .ConvertUsing<UserRoleClaimToPermissionViewModelConverter>();

            CreateMap<ApplicationPermissionsTree, PermissionTreeViewModel>()
                .ReverseMap();

            CreateMap<ApplicationPermissionsTree, PermissionViewModel>()
                .ReverseMap();

            CreateMap<UserRoleClaim, PermissionTreeViewModel>()
                .ConvertUsing<UserRoleClaimToPermissionTreeViewModelConverter>();

            CreateMap<Media, MediaViewModel>()
                .ForMember(d => d.InstitutionName, map => map.MapFrom(s => s.Institution != null && s.Institution.IsActive ? s.Institution.Name : string.Empty))
                .ForMember(d => d.RolesArr, map => map.MapFrom(s => s.Roles != null ? s.Roles.Where(r => r.IsActive).Select(r => r.Role != null ? r.Role.Name : "") : new String[0]))

                .ForMember(d => d.UserGroupsArr, map => map.MapFrom(s => s.UserGroups != null ? s.UserGroups.Where(r => r.IsActive).Select(r => r.UserGroup != null ? r.UserGroup.Name : "") : new String[0]))
                //.ForMember(d => d.DirectorySize, opt => opt.MapFrom<DirectorySizeResolver>())
                //.ForMember(
                //    dest => dest.DirectorySize,
                //    opt =>
                //    {
                //        opt.MapFrom(src => Utilities.GetDirectorySize(new DirectoryInfo(Utilities.GetMediaFolder(src.Id)), true));
                //    })
                .ReverseMap();

            CreateMap<MediaViewModel, Media>();

            CreateMap<Map, MapViewModel>()
                .ForMember(d => d.FloorLabel, map => map.MapFrom(s => s.Floor != null ? s.Floor.Label : null))
                .ForMember(d => d.Points, map => map.MapFrom(s => s.Points != null ? s.Points.Where(z => z.IsActive) : null))
                .ForMember(d => d.Lines, map => map.MapFrom(s => s.Lines != null ? s.Lines.Where(z => z.IsActive) : null))
                   .ReverseMap();
            CreateMap<MapViewModel, Map>();

            CreateMap<Point, PointViewModel>()
                .ForMember(d => d.Directorys, map => map.MapFrom(s => s.Directorys != null ? s.Directorys.Where(z => z.IsActive) : null))
                .ReverseMap();
            CreateMap<PointViewModel, Point>();

            CreateMap<PointDirectoryListing, DirectoryListingViewModel>()
                .ForMember(d => d.Code, map => map.MapFrom(s => s.DirectoryListing != null ? s.DirectoryListing.Code : null))
                .ForMember(d => d.Label, map => map.MapFrom(s => s.DirectoryListing != null ? s.DirectoryListing.Label : null))
                .ReverseMap();
            CreateMap<DirectoryListingViewModel, PointDirectoryListing>();

            CreateMap<Line, LineViewModel>()
                .ForMember(d => d.Code0, map => map.MapFrom(s => s.Point0 != null ? s.Point0.Code : null))
                .ForMember(d => d.Code1, map => map.MapFrom(s => s.Point1 != null ? s.Point1.Code : null))
                .ForMember(d => d.Label0, map => map.MapFrom(s => s.Point0 != null ? s.Point0.Label : null))
                .ForMember(d => d.Label1, map => map.MapFrom(s => s.Point1 != null ? s.Point1.Label : null))
                   .ReverseMap();
            CreateMap<LineViewModel, Line>();

            CreateMap<OccupancyLog, OccupancyLogViewModel>();
            CreateMap<OccupancyLogViewModel, OccupancyLog>();

            CreateMap<DirectoryListingCategory, DirectoryListingCategoryViewModel>();
            CreateMap<DirectoryListingCategoryViewModel, DirectoryListingCategory>();

            CreateMap<MediaExtension, MediaExtensionViewModel>();
            CreateMap<MediaExtensionViewModel, MediaExtension>();

            CreateMap<EmployeeDesignation, EmployeeDesignationViewModel>();
            CreateMap<EmployeeDesignationViewModel, EmployeeDesignation>();


            CreateMap<EmployeeSchedule, EmployeeScheduleViewModel>()
                .ForMember(d => d.shifts, map => map.MapFrom(s => s.Shifts != null ? s.Shifts.Where(z => z.IsActive) : null))
                .ForMember(d => d.locations, map => map.MapFrom(s => s.Locations != null ? s.Locations.Where(z => z.IsActive && z.Location != null && z.Location.IsActive) : null))
                .ForMember(d => d.slots, map => map.MapFrom(s => s.Slots != null ? s.Slots.Where(z => z.IsActive && z.EmployeeData != null && z.EmployeeData.IsActive && z.Location != null && z.Location.IsActive && z.Shift != null && z.Shift.IsActive) : null))
                .ForMember(d => d.infos, map => map.MapFrom(s => s.Infos != null ? s.Infos.Where(z => z.IsActive && z.Location != null && z.Location.IsActive && z.Shift != null && z.Shift.IsActive) : null))
                   .ReverseMap();
            CreateMap<EmployeeScheduleViewModel, EmployeeSchedule>();

            CreateMap<EmployeeScheduleInfo, EmployeeScheduleInfoViewModel>();
            CreateMap<EmployeeScheduleInfoViewModel, EmployeeScheduleInfo>();

            CreateMap<EmployeeScheduleSlot, EmployeeScheduleSlotViewModel>()
                .ForMember(d => d.Name, map => map.MapFrom(s => s.EmployeeData.Name))
                .ForMember(d => d.CoverName, map => map.MapFrom(s => s.CoveringEmployeeData.Name))
                    .ReverseMap();
            CreateMap<EmployeeScheduleSlotViewModel, EmployeeScheduleSlot>();

            CreateMap<EmployeeScheduleLocation, LocationViewModel>()
                    .ForMember(d => d.Name, map => map.MapFrom(s => s.Location.Name))
                    .ReverseMap();
            CreateMap<LocationViewModel, EmployeeScheduleLocation>();

            CreateMap<EmployeeScheduleShift, EmployeeScheduleShiftViewModel>();
            CreateMap<EmployeeScheduleShiftViewModel, EmployeeScheduleShift>();

            CreateMap<EmployeeData, EmployeeDataViewModel>()
                .ForMember(d => d.EmployeeDesignationCode, map => map.MapFrom(s => s.EmployeeDesignation != null && s.EmployeeDesignation.IsActive ? s.EmployeeDesignation.Code : string.Empty))
                .ForMember(d => d.EmployeeDesignationLabel, map => map.MapFrom(s => s.EmployeeDesignation != null && s.EmployeeDesignation.IsActive ? s.EmployeeDesignation.Label : string.Empty))
                .ReverseMap();
            CreateMap<EmployeeDataViewModel, EmployeeData>();

            CreateMap<EmsProfile, EmsProfileViewModel>();
            CreateMap<EmsProfileViewModel, EmsProfile>();

            CreateMap<EmsSchedule, EmsScheduleViewModel>()
                .ForMember(d => d.LocationCode, map => map.MapFrom(s => s.Location != null && s.Location.IsActive ? s.Location.code : string.Empty))
                .ForMember(d => d.LocationLabel, map => map.MapFrom(s => s.Location != null && s.Location.IsActive ? s.Location.label : string.Empty))
                .ForMember(d => d.EmsProfileCode, map => map.MapFrom(s => s.EmsProfile != null && s.EmsProfile.IsActive ? s.EmsProfile.Code : string.Empty))
                .ForMember(d => d.EmsProfileLabel, map => map.MapFrom(s => s.EmsProfile != null && s.EmsProfile.IsActive ? s.EmsProfile.Label : string.Empty))
                .ReverseMap();
            CreateMap<EmsScheduleViewModel, EmsSchedule>();

            CreateMap<Ems, EmsViewModel>()
                .ForMember(d => d.InstitutionName, map => map.MapFrom(s => s.Institution != null && s.Institution.IsActive ? s.Institution.Name : string.Empty))
                .ReverseMap();

            CreateMap<EmsViewModel, Ems>();

            CreateMap<QueueTableMap, QueueTableMapViewModel>()
                .ForMember(d => d.DeviceId, map => map.MapFrom(s => s.Device != null ? s.Device.Code : string.Empty))
                .ReverseMap();
            CreateMap<QueueTableMapViewModel, QueueTableMap>();

            CreateMap<QueueLog, QueueLogViewModel>();
            CreateMap<QueueLogViewModel, QueueLog>();

            CreateMap<QueueServiceParam, QueueLogViewModel>();
            CreateMap<QueueLogViewModel, QueueServiceParam>();

            CreateMap<DirectoryListing, DirectoryListingViewModel>()
                .ForMember(d => d.DirectoryListingCategoryCode, map => map.MapFrom(s => s.DirectoryListingCategory != null && s.DirectoryListingCategory.IsActive ? s.DirectoryListingCategory.Code : string.Empty))
                .ForMember(d => d.DirectoryListingCategoryLabel, map => map.MapFrom(s => s.DirectoryListingCategory != null && s.DirectoryListingCategory.IsActive ? s.DirectoryListingCategory.Label : string.Empty))
                .ForMember(d => d.FloorLabel, map => map.MapFrom(s => s.Floor != null && s.Floor.IsActive ? s.Floor.Label : string.Empty))
                .ReverseMap();
            CreateMap<DirectoryListingViewModel, DirectoryListing>();

            CreateMap<Building, BuildingViewModel>()
                .ForMember(d => d.floors, map => map.MapFrom(s => s.Floors != null ? s.Floors.Where(z => z.IsActive) : null))
                .ReverseMap();
            CreateMap<BuildingViewModel, Building>();

            CreateMap<BuildingFloor, FloorViewModel>()
                    .ForMember(d => d.Code, map => map.MapFrom(s => s.Floor.Code))
                    .ForMember(d => d.Label, map => map.MapFrom(s => s.Floor.Label))
                    .ForMember(d => d.Id, map => map.Ignore())
                    .ReverseMap();
            CreateMap<FloorViewModel, BuildingFloor>();

            CreateMap<Floor, FloorViewModel>();
            CreateMap<FloorViewModel, Floor>();

            CreateMap<ApplicationSettingViewModel, ApplicationSetting>();

            CreateMap<ApplicationSetting, ApplicationSettingViewModel>()
                .ForMember(d => d.InstitutionName, map => map.MapFrom(s => s.Institution != null && s.Institution.IsActive ? s.Institution.Name : string.Empty));

            CreateMap<Facility, FacilityViewModel>()
                .ForMember(d => d.FilePath, map => map.MapFrom(s => s.Icon.Path))
                .ForMember(d => d.InstitutionName, map => map.MapFrom(s => s.Institution != null && s.Institution.IsActive ? s.Institution.Name : string.Empty))
                .ForMember(d => d.InstitutionDescription, map => map.MapFrom(s => s.Institution != null && s.Institution.IsActive ? s.Institution.Description : string.Empty))
                .ReverseMap();

            CreateMap<FacilityViewModel, Facility>();
            CreateMap<FacilityType, FacilityTypeViewModel>()
               .ReverseMap();
            CreateMap<FacilityTypeViewModel, FacilityType>();

            CreateMap<SignageComponent, SignageComponentViewModel>()
                .ForMember(d => d.CreatedByName, map => map.MapFrom(s => s.CreatedByUser != null ?  s.CreatedByUser.FullName : null))
               .ReverseMap();
            CreateMap<SignageComponentViewModel, SignageComponent>();



            CreateMap<SignageCompilationComponent, SignageComponentViewModel>()
                    .ForMember(d => d.Name, map => map.MapFrom(s => s.Component.Name))
                    .ForMember(d => d.Description, map => map.MapFrom(s => s.Component.Description))
                    .ForMember(d => d.FixWidth, map => map.MapFrom(s => s.Component.FixWidth))
                    .ForMember(d => d.FixHeight, map => map.MapFrom(s => s.Component.FixHeight))
                    .ForMember(d => d.IsRatio, map => map.MapFrom(s => s.Component.IsRatio))
                    .ForMember(d => d.BackgroundColor, map => map.MapFrom(s => s.Component.BackgroundColor))
                    .ForMember(d => d.BackgroundImage, map => map.MapFrom(s => s.Component.BackgroundImage))
                    .ForMember(d => d.ComponentType, map => map.MapFrom(s => s.Component.ComponentType))
                    .ForMember(d => d.Configurations, map => map.MapFrom(s => s.Component.Configurations))
                    .ForMember(d => d.PreviewWidth, map => map.MapFrom(s => s.Component.PreviewWidth))
                    .ForMember(d => d.PreviewHeight, map => map.MapFrom(s => s.Component.PreviewHeight))
                    .ForMember(d => d.CreatedByName, map => map.MapFrom(s => s.Component.CreatedByUser != null ? s.Component.CreatedByUser.FullName : null))
                    .ForMember(d => d.UpdatedDate, map => map.MapFrom(s => s.Component.UpdatedDate))
                    .ReverseMap();
            CreateMap<SignageComponentViewModel, SignageCompilationComponent>();

            CreateMap<SignagePublication, SignagePublicationViewModel>()
                    .ForMember(d => d.schedules, map => map.MapFrom(s => s.Schedules != null ? s.Schedules.Where(z => z.IsActive) : null))
                    .ForMember(d => d.ApprovedByName, map => map.MapFrom(s => s.ApprovedByUser != null ? s.ApprovedByUser.FullName : null))
                    .ForMember(d => d.RejectedByName, map => map.MapFrom(s => s.RejectedByUser != null ? s.RejectedByUser.FullName : null))
                    .ForMember(d => d.CreatedByName, map => map.MapFrom(s => s.CreatedByUser != null ? s.CreatedByUser.FullName : null))
                   .ForMember(d => d.UpdatedByName, map => map.MapFrom(s => s.UpdatedByUser != null ? s.UpdatedByUser.FullName : null))
                    .ReverseMap();
            CreateMap<SignagePublicationViewModel, SignagePublication>();

            CreateMap<SignageSchedule, SignageScheduleViewModel>()
                .ForMember(d => d.compilations, map => map.MapFrom(s => s.Compilations != null ? s.Compilations.Where(z => z.IsActive && z.Compilation != null && z.Compilation.IsActive) : null))
                .ReverseMap();
            CreateMap<SignageScheduleViewModel, SignageSchedule>();

            CreateMap<SignageScheduleCompilation, SignageCompilationViewModel>()
                    .ForMember(d => d.Name, map => map.MapFrom(s => s.Compilation.Name))
                    .ForMember(d => d.Description, map => map.MapFrom(s => s.Compilation.Description))
                    .ForMember(d => d.width, map => map.MapFrom(s => s.Compilation.width))
                    .ForMember(d => d.height, map => map.MapFrom(s => s.Compilation.height))
                    .ForMember(d => d.BackgroundColor, map => map.MapFrom(s => s.Compilation.BackgroundColor))
                    .ForMember(d => d.BackgroundImage, map => map.MapFrom(s => s.Compilation.BackgroundImage))
                    .ForMember(d => d.components, map => map.MapFrom(s => s.Compilation != null && s.Compilation.Components != null ? s.Compilation.Components.Where(c => c.IsActive && c.Component != null && c.Component.IsActive) : null))

                   .ReverseMap();
            CreateMap<SignageCompilationViewModel, SignageScheduleCompilation>();

            CreateMap<SignageCompilation, SignageCompilationViewModel>()
                .ForMember(d => d.components, map => map.MapFrom(s => s.Components != null ? s.Components.Where(z => z.IsActive && z.Component != null && z.Component.IsActive) : null))
                   .ReverseMap();
            CreateMap<SignageCompilationViewModel, SignageCompilation>();

            CreateMap<Reservation, ReservationViewModel>()
                //.ForMember(d => d.FilePath, map => map.MapFrom(s => s.kioskImage.Path))
                .ForMember(d => d.RequestedBy, map => map.MapFrom(s => s.SmartRoomSchedule != null ? s.SmartRoomSchedule.Requestor : string.Empty))
                .ForMember(d => d.CreatedOn, map => map.MapFrom(s => s.SmartRoomSchedule != null ? s.SmartRoomSchedule.DateCreated : s.CreatedDate))
                .ForMember(d => d.LastUpdatedOn, map => map.MapFrom(s => s.UpdatedDate))
                .ForMember(d => d.Status, map => map.MapFrom(s => ((s.Status == BookingStatus.Pending.ToString() || s.Status == BookingStatus.CheckedIn.ToString() ||
                        string.IsNullOrEmpty(s.Status)) && DateTime.Now >= s.StartDateTime && DateTime.Now <= s.EndDateTime) ? BookingStatus.InProgress.ToString() : (string.IsNullOrEmpty(s.Status) ? BookingStatus.Pending.ToString() : s.Status)))
                .ForMember(d => d.BookedBy, map => map.MapFrom(s => s.SmartRoomSchedule != null ? s.SmartRoomSchedule.UserName : (s.CreatedByUser != null ? s.CreatedByUser.FullName : "FRS Admin")))
                .ForMember(d => d.BookedEmail, map => map.MapFrom(s => (s.CreatedByUser != null ? s.CreatedByUser.Email : "FRS Admin")))
                .ForMember(d => d.BookedUserName, map => map.MapFrom(s => (s.CreatedByUser != null ? s.CreatedByUser.UserName : "FRS Admin")))
                .ForMember(d => d.LastUpdatedByName, map => map.MapFrom(s => (s.UpdatedByUser != null ? s.UpdatedByUser.FullName : "FRS Admin")))
                //.ForMember(d => d.LocationName, map => map.MapFrom(s => s.Location !=null ? s.Location.Name: s.OtherLocation))
                .ForMember(d => d.LocationName, map => map.MapFrom(s => s.Location != null && s.IsActive ? s.Location.Name : string.Empty))
                .ForMember(d => d.LocationObj, map => map.MapFrom(s => s.Location != null && s.IsActive ? s.Location : null))
                .ForMember(d => d.Facilities, map => map.MapFrom(s => s.Location.LocationFacilities.Where(a => a.IsActive && a.Facility != null).Select(e => e.Facility).Where(e => e.IsActive)))
                .ForMember(d => d.FacilityTypes, map => map.MapFrom(s => s.Location.LocationFacilityTypes.Where(a => a.IsActive && a.FacilityType != null).Select(e => e.FacilityType).Where(e => e.IsActive)))
                //.ForMember(d => d.TimeIntervals, map => map.MapFrom(s => s.ReservationTimes.Select(f => f.TimeInterval)))
                .ForMember(d => d.ContactGroups, map => map.MapFrom(s => s.ReservationContactGroups.Where(a => a.IsActive && a.ContactGroup != null).Select(e => e.ContactGroup).Where(e => e.IsActive)))
                .ForMember(d => d.ReservationPictures, map => map.MapFrom(s => s.ReservationPictures.Where(a => a.IsActive && a.PictureUrl != null)))
                .ForMember(d => d.Invitees, map => map.MapFrom(s => s.ReservationInvitees.Where(e => e.IsActive).Select(f =>
                  new ReservationInviteeViewModel
                  {
                      Email = f.Email,
                      Id = f.Id,
                      Name = f.User != null ? f.User.FullName : f.Name,
                      UserId = f.User != null ? f.UserId.ToString() : string.Empty,
                      Department = f.Department,
                      Company = f.Company,
                      Designation = f.Designation,
                      PhoneNumber = f.PhoneNumber,
                      Status = string.IsNullOrEmpty(f.Status) ? ParticipantStatus.Pending.ToString() : f.Status,
                      PersonName = f.User != null ? f.User.FullName : f.Name, //intended for vmms
                      PlateNumber = f.PlateNumber,
                      Vehicle = f.PlateNumber,
                      IssueDate = f.IssueDate,
                      ExpiryDate = f.ExpiryDate,
                      CardType = f.CardType,
                      VehicleStatus = f.VehicleStatus,
                      ContactGroupId = f.ContactGroupId,
                      FilePath = f.User != null && f.User.Icon != null ? f.User.Icon.Path : Utilities.DefaultProfilePhotoPath()
                  })));

            CreateMap<ReservationViewModel, Reservation>()
                .ForMember(d => d.Status, map => map.MapFrom(s => string.IsNullOrEmpty(s.Status) ? BookingStatus.Pending.ToString() : s.Status))
                .ForMember(d => d.ReservationTimes, map => map.MapFrom(s => s.TimeIntervals.Select(f => new ReservationTime() { TimeIntervalId = f.Id })))
                //.ForMember(d => d.OtherLocation, map => map.MapFrom(s => !s.LocationId.HasValue && string.IsNullOrEmpty(s.OtherLocation) ? s.LocationName : s.OtherLocation))
                .ForMember(d => d.ReservationInvitees, map => map.MapFrom(s => s.Invitees))
                .ForMember(d => d.ReservationPictures, map => map.MapFrom(s => s.ReservationPictures))
                .ForMember(d => d.ReservationContactGroups, map => map.MapFrom(s => s.ContactGroups.Select(e => new ReservationContactGroup
                {
                    ReservationId = s.Id,
                    ContactGroupId = e.Id
                }).Distinct()));


            CreateMap<Device, DeviceViewModel>()
                .ForMember(d => d.LocationName, map => map.MapFrom(s => s.Location != null && s.Location.IsActive ? s.Location.Name : string.Empty))
                .ForMember(d => d.LocationColorTheme, map => map.MapFrom(s => s.Location != null && s.Location.IsActive ? s.Location.ColorTheme : string.Empty))
                .ForMember(d => d.LocationGroup, map => map.MapFrom(s => s.Location != null && s.Location.IsActive ? s.Location.LocationGroup : string.Empty))
                //.ForMember(d => d.InstitutionName, map => map.MapFrom(s => s.Institution != null && s.Institution.IsActive ? s.Institution.Name : string.Empty))
                .ForMember(d => d.module_path, map => map.MapFrom(f => string.IsNullOrEmpty(f.module_path_value) ? string.Format("{0}/{1}/{2}/{3}", "/devicewarning", "adminreq", f.Id, f.MacAddress) : string.Format("{0}/{1}", f.module_path_value, f.MacAddress.Trim())))
                .ForMember(d => d.Code, map => map.MapFrom(s => s.Code.Trim()))
                .ForMember(d => d.MacAddress, map => map.MapFrom(s => s.MacAddress.Trim()))
                .ForMember(d => d.ModuleParameters, map => map.MapFrom(f => f.Module.ModuleParameters))
                .ForMember(d => d.PublicationName, map => map.MapFrom(f => f.Publication.Name))
                .ForMember(d => d.DeviceTypeName, map => map.MapFrom(s => s.DeviceType != null && s.DeviceType.IsActive ? s.DeviceType.Name : string.Empty))
                .ReverseMap();
            CreateMap<DeviceViewModel, Device>()
                .ForMember(d => d.module_path, map => map.MapFrom(f => string.IsNullOrEmpty(f.module_path_value) ? string.Format("{0}/{1}/{2}/{3}", "/devicewarning", "adminreq", f.Id, f.MacAddress) : string.Format("{0}/{1}", f.module_path_value, f.MacAddress.Trim())))
                .ForMember(d => d.Code, map => map.MapFrom(s => s.Code.Trim()))
                .ForMember(d => d.MacAddress, map => map.MapFrom(s => s.MacAddress.Trim()));

            CreateMap<Institution, InstitutionViewModel>()
                .ReverseMap();
            CreateMap<InstitutionViewModel, Institution>();

            CreateMap<Location, LocationViewModel>()
                .ForMember(d => d.FilePath, map => map.MapFrom(s => s.Icon.Path))
                .ForMember(d => d.InstitutionNames, map => map.MapFrom(s => string.Join(",", s.LocationInstitutions.Where(e => e.IsActive).Select(e => e.Institution).Where(e => e.IsActive).Select(e => e.Name))))
                .ForMember(d => d.FacilityIds, map => map.MapFrom(s => s.LocationFacilities.Where(e => e.IsActive).Select(e => e.Facility).Where(e => e.IsActive).Select(e => e.Id).ToList()))
                .ForMember(d => d.FacilityTypeIds, map => map.MapFrom(s => s.LocationFacilityTypes.Where(e => e.IsActive).Select(e => e.FacilityType).Where(e => e.IsActive).Select(e => e.Id).ToList()))
                .ForMember(d => d.InstitutionIds, map => map.MapFrom(s => s.LocationInstitutions.Where(e => e.IsActive).Select(e => e.Institution).Where(e => e.IsActive).Select(e => e.Id).ToList()))
                .ForMember(d => d.FacilityNames, map => map.MapFrom(s => string.Join(",", s.LocationFacilities.Where(e => e.IsActive).Select(e => e.Facility).Where(e => e.IsActive).Select(e => e.Name))))
                .ForMember(d => d.Facilities, map => map.MapFrom(s => s.LocationFacilities.Where(e => e.IsActive).Select(e => e.Facility).Where(e => e.IsActive).ToList()))
                .ForMember(d => d.FacilityTypeNames, map => map.MapFrom(s => string.Join(",", s.LocationFacilityTypes.Where(e => e.IsActive).Select(e => e.FacilityType).Where(e => e.IsActive).Select(e => e.Name))))
                .ForMember(d => d.FacilityTypes, map => map.MapFrom(s => s.LocationFacilityTypes.Where(e => e.IsActive).Select(e => e.FacilityType).Where(e => e.IsActive).ToList()))
                .ForMember(d => d.ImageReferences, map => map.MapFrom(s => s.LocationImageReferences.Where(e => e.IsActive).OrderByDescending(e => e.ReferenceDate).ToList()))
                .ForMember(d => d.DirectoryName, map => map.MapFrom(s => s.MappedDirectory != null && s.IsActive ? s.MappedDirectory.Label : string.Empty))
                .ForMember(d => d.LocationAssets, map => map.MapFrom(s => s.LocationAssets.Where(e => (e.Asset == null || (e.Asset != null && e.Asset.IsActive)) && e.IsActive).OrderByDescending(e => e.Asset.PurchaseDate).ToList()))
               .ReverseMap();
            CreateMap<LocationViewModel, Location>();

            CreateMap<TimeInterval, TimeIntervalViewModel>()
                .ForMember(d => d.TotalMinutes, map => map.MapFrom(s => (s.Hour * 60) + s.Minutes))
                .ForMember(d => d.ToTimeIntervalId, map => map.MapFrom(s => s.ToTimeInterval.Id))
                .ForMember(d => d.ToTimeIntervalDescription, map => map.MapFrom(s => s.ToTimeInterval.Description))
                .ForMember(d => d.ToTimeIntervalHour, map => map.MapFrom(s => s.ToTimeInterval.Hour))
                .ForMember(d => d.ToTimeIntervalMinutes, map => map.MapFrom(s => s.ToTimeInterval.Minutes))
                .ForMember(d => d.ToTimeIntervalValue, map => map.MapFrom(s => s.ToTimeInterval.Value))
                .MaxDepth(2)
                .ReverseMap();

            CreateMap<TimeIntervalViewModel, TimeInterval>();

            CreateMap<ReservationInvitee, ReservationInviteeViewModel>()
                .ForMember(d => d.Name, map => map.MapFrom(s => s.User.FullName))
                .ForMember(d => d.Email, map => map.MapFrom(s => s.User.Email))
                .ForMember(d => d.FilePath, map => map.MapFrom(f => f.User != null && f.User.Icon != null ? f.User.Icon.Path : Utilities.DefaultProfilePhotoPath()))
                .ReverseMap();
            CreateMap<ReservationInviteeViewModel, ReservationInvitee>();

            CreateMap<ReservationPictureViewModel, ReservationPicture>();
            CreateMap<ReservationPicture, ReservationPictureViewModel>();

            CreateMap<ReservationFeedbackViewModel, ReservationFeedback>();
            CreateMap<ReservationFeedback, ReservationFeedbackViewModel>();

            CreateMap<BookingGridRow, BookingGridRowViewModel>().MaxDepth(3)
                .ReverseMap();
            CreateMap<BookingGridRowViewModel, BookingGridRow>();

            CreateMap<LocationTimeInterval, LocationTimeIntervalViewModel>()
                .ReverseMap();
            CreateMap<LocationTimeIntervalViewModel, LocationTimeInterval>();

            CreateMap<BookingGrid, BookingGridViewModel>().MaxDepth(3)
                .ReverseMap();

            CreateMap<LocationTimeSlot, LocationTimeSlotViewModel>()
                .ForMember(d => d.Id, map => map.MapFrom(s => s.Location.Id))
                .ForMember(d => d.Name, map => map.MapFrom(s => s.Location.Name))
                .ForMember(d => d.Description, map => map.MapFrom(s => s.Location.Description))
                .ForMember(d => d.Status, map => map.MapFrom(s => s.Location.Status))
                .ForMember(d => d.Capacity, map => map.MapFrom(s => s.Location.Capacity))
                .ForMember(d => d.LocationTypeId, map => map.MapFrom(s => s.Location.LocationTypeId))
                .ForMember(d => d.ParentLocationId, map => map.MapFrom(s => s.Location.ParentLocationId))
                .ForMember(d => d.InstitutionIds, map => map.MapFrom(s => s.Location.LocationInstitutions.Where(e => e.IsActive).Select(e => e.Institution).Where(e => e.IsActive).Select(e => e.Id)))
                .ForMember(d => d.InstitutionNames, map => map.MapFrom(s => string.Join(",", s.Location.LocationInstitutions.Where(e => e.IsActive).Select(e => e.Institution).Where(e => e.IsActive).Select(e => e.Name))))
                .ForMember(d => d.FacilityIds, map => map.MapFrom(s => s.Location.LocationFacilities.Where(e => e.IsActive).Select(e => e.Facility).Where(e => e.IsActive).Select(e => e.Id).ToList()))
                .ForMember(d => d.FacilityNames, map => map.MapFrom(s => string.Join(",", s.Location.LocationFacilities.Where(a => a.IsActive).Select(e => e.Facility).Where(e => e.IsActive).Select(e => e.Name))))
                .ForMember(d => d.Facilities, map => map.MapFrom(s => s.Location.LocationFacilities.Where(a => a.IsActive).Select(e => e.Facility).Where(e => e.IsActive).ToList()));

            CreateMap<Department, DepartmentViewModel>()
                .ForMember(d => d.InstitutionName, map => map.MapFrom(s => s.Institution.Name));

            CreateMap<DepartmentViewModel, Department>();

            CreateMap<ImageFile, ImageViewModel>()
                .ForMember(d => d.InstitutionName, map => map.MapFrom(s => s.Institution.Name));

            CreateMap<ImageViewModel, ImageFile>();

            CreateMap<ContactGroup, ContactGroupSimpleViewModel>()
                .ForMember(d => d.InstitutionName, map => map.MapFrom(s => s.Institution != null && s.Institution.IsActive ? s.Institution.Name : string.Empty))
                .ForMember(d => d.DepartmentNames, map => map.MapFrom(s => string.Join(",", s.ContactGroupDepartments.Where(e => e.IsActive && e.Department != null).Select(e => e.Department).Where(e => e.IsActive).Select(e => e.Name))));

            CreateMap<ContactGroup, ContactGroupViewModel>()
                .ForMember(d => d.InstitutionName, map => map.MapFrom(s => s.Institution != null && s.Institution.IsActive ? s.Institution.Name : string.Empty))
                .ForMember(d => d.Departments, map => map.MapFrom(s => s.ContactGroupDepartments.Where(e => e.IsActive).Select(e => e.Department).Where(e => e.IsActive)))
                .ForMember(d => d.DepartmentNames, map => map.MapFrom(s => string.Join(",", s.ContactGroupDepartments.Where(e => e.IsActive && e.Department != null).Select(e => e.Department).Where(e => e.IsActive).Select(e => e.Name))))
                .ForMember(d => d.DepartmentIds, map => map.MapFrom(s => s.ContactGroupDepartments.Where(e => e.IsActive).Select(e => e.Department).Where(e => e.IsActive).Select(e => e.Id).Distinct()))
                .ForMember(d => d.Members, map => map.MapFrom(s => s.Members.Where(e => e.IsActive)));

            CreateMap<ContactGroupViewModel, ContactGroup>()
                .ForMember(d => d.ContactGroupDepartments, map => map.MapFrom(s => s.DepartmentIds.Select(e => new ContactGroupDepartment
                {
                    DepartmentId = e,
                    ContactGroupId = s.Id
                }).Distinct()));

            CreateMap<PlaylistImage, ImageViewModel>()
                    .ForMember(d => d.Title, map => map.MapFrom(s => s.ImageFile.Title))
                    .ForMember(d => d.ImageLocation, map => map.MapFrom(s => s.ImageFile.ImageLocation))
                    .ForMember(d => d.InstitutionId, map => map.MapFrom(s => s.ImageFile.InstitutionId))
                    .ForMember(d => d.InstitutionName, map => map.MapFrom(s => s.ImageFile.Institution.Name))
                    .ForMember(d => d.IsVideo, map => map.MapFrom(s => s.ImageFile.IsVideo))
                    .ForMember(d => d.MediaId, map => map.MapFrom(s => s.ImageFile.MediaId))
                   .ReverseMap();
            CreateMap<ImageViewModel, PlaylistImage>();

            CreateMap<Playlist, PlaylistViewModel>()
                 .ForMember(d => d.InstitutionName, map => map.MapFrom(s => s.Institution != null && s.Institution.IsActive ? s.Institution.Name : string.Empty))
                 .ForMember(d => d.Images, map => map.MapFrom(s => s.PlaylistImages.Where(e => e.IsActive).OrderBy(e => e.imageIndex)))
                 .ForMember(d => d.ImageTitles, map => map.MapFrom(s => string.Join(",", s.PlaylistImages.Where(e => e.IsActive && e.ImageFile != null).Select(e => e.ImageFile).Select(e => e.Title))))
                 .ForMember(d => d.ImageIds, map => map.MapFrom(s => s.PlaylistImages.Where(e => e.IsActive && e.ImageFile != null).Select(e => new imageIndex(e.ImageFile.Id, e.imageIndex, e.duration, e.animation))))
                 ;

            CreateMap<PlaylistViewModel, Playlist>()
                .ForMember(d => d.PlaylistImages, map => map.MapFrom(s => s.ImageIds.Select(e => new PlaylistImage
                {
                    ImageId = e.imageId,
                    PlaylistId = s.Id,
                    imageIndex = e.imgIndex,
                    duration = e.duration,
                    animation = e.animation
                })));


            CreateMap<KioskSettings, KioskSettingsViewModel>();

            CreateMap<KioskSettingsViewModel, KioskSettings>();

            CreateMap<ContactGroupMember, ContactGroupMemberViewModel>()
                .ForMember(d => d.ContactGroupName, map => map.MapFrom(s => s.ContactGroup.Name))
                .ForMember(d => d.FilePath, map => map.MapFrom(s => s.User != null && s.User.Icon != null ? s.User.Icon.Path : (s.UserId.HasValue ? Utilities.DefaultProfilePhotoPath() : (s.Icon != null ? s.Icon.Path : Utilities.DefaultProfilePhotoPath()))))
                .ForMember(d => d.Status, map => map.MapFrom(s => s.User != null ? s.User.Status : UserConnectionStatus.OFFDUTY.ToString()))
                .ForMember(d => d.IsConnected, map => map.MapFrom(s => s.User != null && s.User.UserConnections != null && s.User.UserConnections.Any(e => e.IsActive)));

            CreateMap<ContactGroupMemberViewModel, ContactGroupMember>()
                .ForPath(d => d.Icon.Path, map => map.MapFrom(s => s.UserId.HasValue ? string.Empty : s.FilePath));

            CreateMap<UserPhonebook, UserPhonebookViewModel>()
                .ForMember(d => d.FilePath, map => map.MapFrom(s => s.Icon.Path))
                .ForMember(d => d.Vehicles, map => map.MapFrom(s => s.User.UserVehicles))
                .ForMember(d => d.CardIds, map => map.MapFrom(s => s.User.UserCardIds));

            CreateMap<UserPhonebookViewModel, UserPhonebook>();

            CreateMap<UserVehicle, UserVehicleViewModel>()
                .ForMember(d => d.UserName, map => map.MapFrom(s => s.User.FriendlyName));

            CreateMap<UserVehicleViewModel, UserVehicle>();

            CreateMap<UserCardId, UserCardIdViewModel>()
                .ForMember(d => d.UserObj, map => map.MapFrom(s => s.User != null && s.IsActive ? s.User : null));

            CreateMap<UserCardIdViewModel, UserCardId>();

            CreateMap<PIBTemplateViewModel, PIBTemplate>();

            CreateMap<PIBTemplate, PIBTemplateViewModel>()
                .ForMember(d => d.LocationIds, map => map.MapFrom(s => s.Locations.Where(a => a.IsActive).Select(e => e.location_id).ToList()));

            CreateMap<PIBTemplateLocationViewModel, PIBTemplateLocation>();

            CreateMap<PIBTemplateLocation, PIBTemplateLocationViewModel>().MaxDepth(2);

            CreateMap<PIBDeviceViewModel, PIBDevice>();

            CreateMap<PIBDevice, PIBDeviceViewModel>()
                .ForMember(d => d.device_status, map => map.MapFrom(f => (f.last_heartbeat == null || DateTime.Now.Subtract(f.last_heartbeat.Value).TotalMinutes > 1.5) ? 0 : 1))
                .MaxDepth(2);

            CreateMap<JObject, PatientInfo>()
            .ForMember("identifier", cfg => { cfg.MapFrom(jo => jo["profile"]["identifier"]); })
            .ForMember("profile_id", cfg => { cfg.MapFrom(jo => jo["profile"]["profile_id"]); })
            .ForMember("name", cfg => { cfg.MapFrom(jo => jo["profile"]["name"]); })
            .ForMember("preferred_name", cfg => { cfg.MapFrom(jo => jo["profile"]["preferred_name"]); })
            .ForMember("gender", cfg => { cfg.MapFrom(jo => jo["profile"]["gender"]); })
            .ForMember("vip", cfg => { cfg.MapFrom(jo => jo["profile"]["vip"]); })
            .ForMember("case_number", cfg => { cfg.MapFrom(jo => jo["case_number"]); })
            .ForMember("status", cfg => { cfg.MapFrom(jo => jo["status"]); })
            .ForMember("location_code", cfg => { cfg.MapFrom(jo => jo["location"]["code"]); })
            .ForMember("location_label", cfg => { cfg.MapFrom(jo => jo["location"]["label"]); })
            .ForMember("location_alias", cfg => { cfg.MapFrom(jo => jo["location"]["alias"]); })
            .ForMember("attending_doctor", cfg => { cfg.MapFrom(jo => jo["attending_doctor"]); })
            .ForMember("attending_nurse", cfg => { cfg.MapFrom(jo => jo["attending_nurse"]); })
            .ForMember("admission_date", cfg => { cfg.MapFrom(jo => jo["admission_date"]); })
            .ForMember("ongoing_remarks", cfg => { cfg.MapFrom(jo => jo["ongoing_remarks"]); })
            .ForMember("ahp_remarks", cfg => { cfg.MapFrom(jo => jo["ahp_remarks"]); })
            .ForMember("pib_location", cfg => { cfg.MapFrom(jo => jo["pib_location"]); })
            .ForMember("restrictions", cfg => { cfg.MapFrom(jo => jo["restrictions"]); })
            .ForMember("languages", cfg => { cfg.MapFrom(jo => jo["profile"]["spoken_languages"]); })
            .ForMember("drug_allergies", cfg => { cfg.MapFrom(jo => jo["profile"]["drug_allergies"]); });

            CreateMap<JObject, RestrictionType>()
            .ForMember("restriction_type_id", cfg => { cfg.MapFrom(jo => jo["restriction_type_id"]); })
            .ForMember("tenant_id", cfg => { cfg.MapFrom(jo => jo["tenant_id"]); })
            .ForMember("code", cfg => { cfg.MapFrom(jo => jo["code"]); })
            .ForMember("label", cfg => { cfg.MapFrom(jo => jo["label"]); });

            CreateMap<JObject, Restriction>()
            .ForMember("restriction_id", cfg => { cfg.MapFrom(jo => jo["restriction_id"]); })
            .ForMember("type", cfg => { cfg.MapFrom(jo => jo["type"]); })
            .ForMember("code", cfg => { cfg.MapFrom(jo => jo["code"]); })
            .ForMember("label", cfg => { cfg.MapFrom(jo => jo["label"]); })
            .ForMember("picture", cfg => { cfg.MapFrom(jo => jo["picture"]); });

            CreateMap<JObject, Language>()
            .ForMember("language_id", cfg => { cfg.MapFrom(jo => jo["language_id"]); })
            .ForMember("code", cfg => { cfg.MapFrom(jo => jo["code"]); })
            .ForMember("label", cfg => { cfg.MapFrom(jo => jo["label"]); });

            CreateMap<JObject, PIBTemplateLocationViewModel>()
            .ForMember("location_id", cfg => { cfg.MapFrom(jo => jo["location_id"]); })
            .ForMember("code", cfg => { cfg.MapFrom(jo => jo["code"]); })
            .ForMember("label", cfg => { cfg.MapFrom(jo => jo["label"]); })
            .ForMember("hospital_id", cfg => { cfg.MapFrom(jo => jo["hospital_id"]); })
            .ForMember("alias", cfg => { cfg.MapFrom(jo => jo["alias"]); })
            .ForMember("sequence", cfg => { cfg.MapFrom(jo => jo["sequence"]); })
            .ForMember("ward", cfg => { cfg.MapFrom(jo => jo["ward"]); })
            .ForMember("bed", cfg => { cfg.MapFrom(jo => jo["bed"]); })
            .ForMember("ancestor", cfg => { cfg.MapFrom(jo => jo["ancestor"]); });

            CreateMap<EpaperTemplateViewModel, EpaperTemplate>();

            CreateMap<EpaperTemplate, EpaperTemplateViewModel>()
                .ForMember(d => d.LocationIds, map => map.MapFrom(s => s.Locations.Where(a => a.IsActive).Select(e => e.LocationId).ToList()));

            CreateMap<EpaperTemplateLocationViewModel, EpaperTemplateLocation>();

            CreateMap<EpaperTemplateLocation, EpaperTemplateLocationViewModel>().MaxDepth(2);

            CreateMap<EpaperDeviceViewModel, EpaperDevice>();

            CreateMap<EpaperDevice, EpaperDeviceViewModel>()
                .ForMember(d => d.device_status, map => map.MapFrom(f => (f.last_heartbeat == null || DateTime.Now.Subtract(f.last_heartbeat.Value).TotalMinutes > 1.5) ? 0 : 1))
                .MaxDepth(2);

            CreateMap<VehicleQueryModel, VehicleEntry>();
            CreateMap<VehicleEntry, VehicleQueryModel>();
            CreateMap<VehicleEntry, VMSVehicleLog>()
                .ForMember(d => d.BookingDescription, map => map.MapFrom(s => s.Reservation != null ? s.Reservation.ShortDescription : string.Empty));

            CreateMap<Module, ModuleViewModel>();
            CreateMap<ModuleViewModel, Module>();

            CreateMap<ModuleParameter, ModuleParameterViewModel>();
            CreateMap<ModuleParameterViewModel, ModuleParameter>();

            CreateMap<UserGroup, UserGroupViewModel>()
                .ForMember(d => d.Locations, map => map.MapFrom(s => s.Locations.Where(a => a.IsActive).ToList()));

            CreateMap<UserGroupViewModel, UserGroup>();

            CreateMap<UserGroupMember, UserGroupMemberViewModel>()
                .ForMember(d => d.UserGroupName, map => map.MapFrom(s => s.UserGroup.Name));
            CreateMap<UserGroupMemberViewModel, UserGroupMember>();

            CreateMap<UserGroupLocation, UserGroupLocationViewModel>();
            CreateMap<UserGroupLocationViewModel, UserGroupLocation>();


            #region For Token Payments 
            CreateMap<TokenPaymentRequestViewModel, TokenPaymentRequest>();
            CreateMap<TokenPaymentResponseViewModel, TokenPaymentResponse>();
            #endregion



            #region DEVICE Management Usages

            CreateMap<SignagePublication, SimpleApiResult>()
                .ForMember(d => d.Id, map => map.MapFrom(s => s.Id))
                .ForMember(d => d.Label, map => map.MapFrom(s => string.Format("{0} - {1}", s.Name, s.Description)));

            CreateMap<Institution, SimpleApiResult>()
                .ForMember(d => d.Id, map => map.MapFrom(s => s.Id))
                .ForMember(d => d.Label, map => map.MapFrom(s => string.Format("{0} - {1}", s.Name, s.Description)));

            CreateMap<Location, SimpleApiResult>()
                .ForMember(d => d.Id, map => map.MapFrom(s => s.Id))
                .ForMember(d => d.Label, map => map.MapFrom(s => string.Format("{0} - {1}", s.Name, s.Description)));

            #endregion

            CreateMap(typeof(PagedEntity<>), typeof(PagedEntityViewModel<>));
            CreateMap(typeof(PagedEntityViewModel<>), typeof(PagedEntityViewModel<>));

            CreateMap<Location, SimpleApiTreeResult>()
                .ForMember(d => d.Id, map => map.MapFrom(s => s.Id))
                .ForMember(d => d.Label, map => map.MapFrom(s => s.Name))
                .ForMember(d => d.Display, map => map.MapFrom(s => s.Name))
                .ForMember(dst => dst.Children, opt => opt.MapFrom(src => src.ChildLocations))
                .MaxDepth(5);

            CreateMap<SignageDashboardViewModel, SignageDashboardDTO>();

            CreateMap<ITree<ApplicationPermissionsTree>, SimpleApiTreeResult>()
                .ForMember(d => d.Id, map => map.MapFrom(s => s.Id))
                .ForMember(d => d.Label, map => map.MapFrom(s => s.Name))
                .ForMember(d => d.ParentId, map => map.MapFrom(s => s.ParentId))
                .ForMember(dst => dst.Children, opt => opt.MapFrom(src => src.Children))
                .MaxDepth(5);

            CreateMap<LocationImageReference, LocationImageReferenceDTO>()
                .ForMember(d => d.Id, map => map.MapFrom(s => s.FileId))
                .ForMember(d => d.FilePath, map => map.MapFrom(s => s.File != null ? s.File.Path : string.Empty))
                .ForMember(d => d.FileId, map => map.MapFrom(s => s.File != null ? s.File.Id : 0))
                .ForMember(d => d.FileName, map => map.MapFrom(s => s.File != null ? s.File.FileName : string.Empty))
                .ForMember(d => d.FileType, map => map.MapFrom(s => s.File != null ? s.File.Type : string.Empty))
                .ForMember(d => d.ImageReferenceColorCode, map => map.MapFrom(s => s.ImageReferenceColor != null ? s.ImageReferenceColor.ColorCode : string.Empty));

            CreateMap<LocationImageReferenceDTO, LocationImageReference>()
                .ForMember(d => d.LocationId, map => map.MapFrom(s => s.LocationId))
                .ForMember(d => d.File, map => map.MapFrom(s => new File() { Path = s.FilePath, Id = s.FileId, FileName = s.FileName, Type = s.FileType }));

            CreateMap<LocationAsset, LocationAssetDTO>()
                .ForMember(d => d.AssetModelName, map => map.MapFrom(s => s.Asset.AssetModel.Name))
                .ForMember(d => d.AssetTypeName, map => map.MapFrom(s => s.Asset.AssetModel.AssetType.Name))
                .ForMember(d => d.SerialNumber, map => map.MapFrom(s => s.Asset.SerialNumber))
                .ForMember(d => d.AssetModelId, map => map.MapFrom(s => s.Asset.AssetModelId))
                .ForMember(d => d.PurchaseDate, map => map.MapFrom(s => s.Asset.PurchaseDate))
                .ForMember(d => d.WarrantyStart, map => map.MapFrom(s => s.Asset.WarrantyEnd))
                .ForMember(d => d.WarrantyEnd, map => map.MapFrom(s => s.Asset.WarrantyEnd));

            CreateMap<LocationAssetDTO, LocationAsset>();

            CreateMap<NotificationDTO, NotificationViewModel>();
            CreateMap<NotificationViewModel, NotificationDTO>();

            CreateMap<NotificationEventDTO, NotificationEventViewModel>();
            CreateMap<NotificationEventViewModel, NotificationEventDTO>();

            CreateMap<NotificationSettingViewModel, NotificationSettingDTO>();

            CreateMap<OrderPortalContentViewModel, OrderPortalContentDTO>();
            CreateMap<OrderPortalBannerViewModel, OrderPortalBannerDTO>();
        }
    }

    public class UserRoleClaimToPermissionViewModelConverter : ITypeConverter<UserRoleClaim, PermissionViewModel>
    {
        public UserRoleClaimToPermissionViewModelConverter()
        {
        }

        public PermissionViewModel Convert(UserRoleClaim source, PermissionViewModel destination, ResolutionContext context)
        {
            var permission = ApplicationPermissionsTrees.GetPermissionByValue(source.ClaimValue);
            return context.Mapper.Map<PermissionViewModel>(permission);
        }
    }

    public class UserRoleClaimToPermissionTreeViewModelConverter : ITypeConverter<UserRoleClaim, PermissionTreeViewModel>
    {
        public UserRoleClaimToPermissionTreeViewModelConverter()
        {
        }

        public PermissionTreeViewModel Convert(UserRoleClaim source, PermissionTreeViewModel destination, ResolutionContext context)
        {
            var permission = ApplicationPermissionsTrees.GetPermissionByValue(source.ClaimValue);
            return context.Mapper.Map<PermissionTreeViewModel>(permission);
        }
    }
}
