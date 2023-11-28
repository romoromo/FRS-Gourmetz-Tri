using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DAL.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using DAL.Core;
using DAL.Core.Audit.Auditors;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using DAL.Core.Audit.Extensions;
using DAL.Models.MealOrder;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using DAL.Models.StoredProcedures;

namespace DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, UserClaim, ApplicationUserRole, IdentityUserLogin<int>, UserRoleClaim,
        IdentityUserToken<int>>
        , IDataProtectionKeyContext // Uncomment if cookie affinity doesn't fix the issue
    {
        public int? CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
        public int? CurrentInstitutionId { get; set; }
        public AuditUserActivityType AuditUserActivityType { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<FacilityType> FacilityTypes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationFacility> LocationFacilities { get; set; }
        public DbSet<LocationFacilityType> LocationFacilityTypes { get; set; }
        public DbSet<LocationInstitution> LocationInstitutions { get; set; }
        public DbSet<TimeInterval> TimeIntervals { get; set; }
        public DbSet<ReservationTime> ReservationTimes { get; set; }
        public DbSet<ReservationInvitee> ReservationInvitees { get; set; }
        public DbSet<LocationType> LocationTypes { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ContactGroup> ContactGroups { get; set; }
        public DbSet<ContactGroupMember> ContactGroupMembers { get; set; }
        public DbSet<ContactGroupDepartment> ContactGroupDepartments { get; set; }
        public DbSet<ReservationContactGroup> ReservationContactGroups { get; set; }
        public DbSet<UserPhonebook> UserPhonebooks { get; set; }
        public DbSet<PIBTemplate> PIBTemplates { get; set; }
        public DbSet<PIBTemplateLocation> PIBTemplateLocations { get; set; }
        public DbSet<PIBDevice> PIBDevices { get; set; }
        public DbSet<ImageFile> ImageFiles { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistImage> PlaylistImages { get; set; }
        public DbSet<ReservationFeedback> ReservationFeedbacks { get; set; }
        public DbSet<EpaperTemplate> EpaperTemplates { get; set; }
        public DbSet<EpaperDevice> EpaperDevices { get; set; }
        public DbSet<EpaperTemplateLocation> EpaperTemplateLocations { get; set; }
        public DbSet<UserVehicle> UserVehicles { get; set; }

        public DbSet<UserCardId> UserCardIds { get; set; }
        public DbSet<Connection> Connections { get; set; }

        public DbSet<SignageComponent> SignageComponents { get; set; }
        public DbSet<SignageCompilation> SignageCompilations { get; set; }
        public DbSet<SignagePublication> SignagePublications { get; set; }
        public DbSet<SignagePublicationHistory> SignagePublicationHistorys { get; set; }
        public DbSet<SignageSchedule> SignageSchedules { get; set; }
        public DbSet<SignageCompilationComponent> SignageCompilationComponents { get; set; }
        public DbSet<SignageScheduleCompilation> SignageScheduleCompilations { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<MediaExtension> MediaExtension { get; set; }
        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        public DbSet<MediaRole> MediaRole { get; set; }
        public DbSet<MediaUserGroup> MediaUserGroup { get; set; }
        public DbSet<DirectoryListing> DirectoryListing { get; set; }
        public DbSet<DirectoryListingCategory> DirectoryListingCategory { get; set; }
        public DbSet<EmployeeDesignation> EmployeeDesignation { get; set; }
        public DbSet<EmployeeSchedule> EmployeeSchedule { get; set; }
        public DbSet<EmployeeScheduleShift> EmployeeScheduleShift { get; set; }
        public DbSet<EmployeeScheduleLocation> EmployeeScheduleLocation { get; set; }
        public DbSet<EmployeeScheduleSlot> EmployeeScheduleSlot { get; set; }
        public DbSet<EmployeeScheduleInfo> EmployeeScheduleInfo { get; set; }
        public DbSet<EmployeeData> EmployeeData { get; set; }
        public DbSet<Building> Building { get; set; }
        public DbSet<BuildingFloor> BuildingFloor { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }
        public DbSet<VehicleEntry> VehicleEntries { get; set; }
        public DbSet<ReservationPicture> ReservationPictures { get; set; }
        public DbSet<Floor> Floor { get; set; }
        public DbSet<Map> Map { get; set; }
        public DbSet<Point> Point { get; set; }
        public DbSet<Line> Line { get; set; }
        public DbSet<PointDirectoryListing> PointDirectoryListing { get; set; }
        public DbSet<OccupancyLog> OccupancyLog { get; set; }
        public DbSet<QueueTableMap> QueueTableMap { get; set; }
        public DbSet<QueueLog> QueueLog { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleParameter> ModuleParameters { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupMember> UserGroupMembers { get; set; }
        public DbSet<UserGroupLocation> UserGroupLocations { get; set; }


        public DbSet<Ems> Emses { get; set; }
        public DbSet<EmsProfile> EmsProfiles { get; set; }
        public DbSet<EmsSchedule> EmsSchedules { get; set; }

        public DbSet<LocationImageReference> LocationImageReferences { get; set; }
        public DbSet<AuthenticationLog> AuthenticationLogs { get; set; }
        public DbSet<UpDownTimeLog> UpDownTimeLogs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AuditLogDetail> AuditLogDetails { get; set; }

        public DbSet<ImageReferenceType> ImageReferenceTypes { get; set; }
        public DbSet<ImageReferenceColor> ImageReferenceColors { get; set; }
        public DbSet<KioskSettings> KioskSettings { get; set; }
        public DbSet<SmartRoomSchedule> SmartRoomSchedules { get; set; }
        public DbSet<SmartRoomResource> SmartRoomResources { get; set; }
        public DbSet<SmartRoomSchedulerLog> SmartRoomSchedulerLogs { get; set; }

        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<AssetModel> AssetModels { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<LocationAsset> LocationAssets { get; set; }
        public DbSet<ServiceContract> ServiceContracts { get; set; }
        public DbSet<ServiceContractAsset> ServiceContractAssets { get; set; }
        public DbSet<EmailQueue> EmailQueues { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<RewardTransaction> RewardTransactions { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<ChannelInfo> ChannelInfos { get; set; }

        public DbSet<ClassLevel> ClassLevels { get; set; }
        public DbSet<ClassBatch> ClassBatches { get; set; } 
        public DbSet<Class> Classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCard> StudentCards { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<StudentGroupDetail> StudentGroupDetails { get; set; }
        public DbSet<TokenOrder> TokenOrders { get; set; }
        public DbSet<TokensOrderHistory> TokensOrderHistorys { get; set; }
        public DbSet<TokenOrdered> TokenOrdereds { get; set; }
        public DbSet<TokenOrderDish> TokenOrderDishes { get; set; }
        public DbSet<TokenAltDish> TokenAltDishes { get; set; }
        public DbSet<TokenLabel> TokenLabels { get; set; }
        public DbSet<MealAllocation> MealAllocations { get; set; }
        public DbSet<TokenDishLabel> TokenDishLabels { get; set; }
        public DbSet<PackingAllocation> PackingAllocations { get; set; }
        public DbSet<DishAllocation> DishAllocations { get; set; }
        public DbSet<StudentRestriction> StudentRestrictions { get; set; }
        public DbSet<StudentVoucher> StudentVouchers { get; set; }
        public DbSet<StudentInterestGroup> StudentInterestGroups { get; set; }
        public DbSet<RouteNode> RouteNodes { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<StaffType> StaffTypes { get; set; }
        public DbSet<StudentAccount> StudentAccounts { get; set; }
        public DbSet<StudentManageAccount> StudentManageAccounts { get; set; }
        public DbSet<StaffAccount> StaffAccounts { get; set; }
        public DbSet<Restriction> Restrictions { get; set; }
        public DbSet<RestrictionType> RestrictionTypes { get; set; }
        public DbSet<DishType> DishTypes { get; set; }
        public DbSet<MealPlanOrder> MealPlanOrders { get; set; }
        public DbSet<Cuisine> Cuisines { get; set; }
        public DbSet<CatererInfo> CatererInfos { get; set; }
        public DbSet<BentoBoxType> BentoBoxTypes { get; set; }
        public DbSet<CartonType> CartonTypes { get; set; }
        public DbSet<DeliveryOrder> DeliveryOrders { get; set; }
        public DbSet<DeliveryOrderNew> DeliveryOrderNews { get; set; }
        public DbSet<StoreInventory> StoreInventories { get; set; }
        public DbSet<DeliveryDetail> DeliveryDetails { get; set; }
        public DbSet<DeliveryDetailNew> DeliveryDetailNews { get; set; }
        public DbSet<StoreInventoryDetail> StoreInventoryDetails { get; set; }
        public DbSet<DeliveryBento> DeliveryBentos { get; set; }
        public DbSet<DeliveryBentoNew> DeliveryBentoNews { get; set; }
        public DbSet<TrackingStatus> TrackingStatuss { get; set; }
        public DbSet<StoreInfo> StoreInfos { get; set; }
        public DbSet<BentoAsset> BentoAssets { get; set; }
        public DbSet<CartonAsset> CartonAssets { get; set; }
        public DbSet<CartonDisposableBox> CartonDisposableBoxes { get; set; }
        public DbSet<MealType> MealTypes { get; set; }
        public DbSet<MealPeriod> MealPeriods { get; set; }
        public DbSet<DishTypePeriod> DishTypePeriods { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishPeriod> DishPeriods { get; set; }
        public DbSet<DishRestriction> DishRestrictions { get; set; }
        public DbSet<DishDetail> DishDetails { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuDish> MenuDishes { get; set; }
        public DbSet<MenuCycle> MenuCycles { get; set; }
        public DbSet<MenuCycleSchedule> MenuCycleSchedules { get; set; }
        public DbSet<MenuCycleSchedulePeriod> MenuCycleSchedulePeriods { get; set; }
        public DbSet<MenuCycleSchedulePeriodMenu> MenuCycleSchedulePeriodMenus { get; set; }
        public DbSet<OutletProfile> OutletProfiles { get; set; }
        public DbSet<Outlet> Outlets { get; set; }
        public DbSet<CatererOutlet> CatererOutlets { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<InterestGroup> InterestGroups { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<MealSession> MealSessions { get; set; }
        public DbSet<MealSessionDetail> MealSessionDetails { get; set; }
        public DbSet<MealTypeDish> MealTypeDishes { get; set; }
        public DbSet<MenuCycleCalendar> MenuCycleCalendars { get; set; }
        public DbSet<MenuCycleCalendarBlockedDate> MenuCycleCalendarBlockedDates { get; set; }
        public DbSet<MenuCycleBlockedDate> MenuCycleBlockedDates { get; set; }
        public DbSet<OutletBlockedDate> OutletBlockedDates { get; set; }
        public DbSet<OutletMenuDish> OutletMenuDishes { get; set; }
        public DbSet<OutletMenuCycleSchedulePeriodMenu> OutletMenuCycleSchedulePeriodMenus { get; set; }
        public DbSet<OutletClassRoster> OutletClassRosters { get; set; }
        public DbSet<OutletClassRosterSchedule> OutletClassRosterSchedules { get; set; }
        public DbSet<OutletClassRosterSchedulePeriod> OutletClassRosterSchedulePeriods { get; set; }
        public DbSet<OutletClassRosterSchedulePeriodClass> OutletClassRosterSchedulePeriodClasses { get; set; }
        public DbSet<DishCycle> DishCycles { get; set; }
        public DbSet<DishCycleSchedule> DishCycleSchedules { get; set; }
        public DbSet<DishCycleScheduleSet> DishCycleScheduleSets { get; set; }
        public DbSet<DishCycleScheduleDetail> DishCycleScheduleDetails { get; set; }
        public DbSet<DishCycleScheduleDetailMenu> DishCycleScheduleDetailMenus { get; set; }
        public DbSet<DishCycleCalendar> DishCycleCalendars { get; set; }
        public DbSet<DishCycleCalendarBlockedDate> DishCycleCalendarBlockedDates { get; set; }
        public DbSet<DishCycleBlockedDate> DishCycleBlockedDates { get; set; }
        public DbSet<OutletDishBlockedDate> OutletDishBlockedDates { get; set; }
        public DbSet<DishCyclePeriod> DishCyclePeriods { get; set; }
        public DbSet<OutletDishCyclePeriodMenu> OutletDishCyclePeriodMenus { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationEvent> NotificationEvents { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<TransactionFee> TransactionFees { get; set; }
        public DbSet<TransactionFeeDetail> TransactionFeeDetails { get; set; }
        public DbSet<VoucherType> VoucherTypes { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherMealPeriod> VoucherMealPeriods { get; set; }
        public DbSet<Waiver> Waivers { get; set; }
        public DbSet<ContactUsSubject> ContactUsSubjects { get; set; }
        public DbSet<ContactUsDetail> ContactUsDetails { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<CancelOrderRequest> CancelOrderRequests { get; set; }
        public DbSet<ExternalAppLoginLog> ExternalAppLoginLogs { get; set; }
        public DbSet<UsedPassword> UsedPasswords { get; set; }


        public DbSet<TokenPaymentRequest> TokenPaymentRequest { get; set; }

        public DbSet<TokenPaymentResponse> TokenPaymentResponse { get; set; }
        public DbSet<UserOrderAlert> UserOrderAlerts { get; set; }
        public DbSet<MenuGroup> MenuGroups { get; set; }
        public DbSet<MenuGroupDishCycle> MenuGroupDishCycles { get; set; }
        public DbSet<MenuGroupClass> MenuGroupClasses { get; set; }
        // Uncomment if cookie affinity doesn't fix the issue
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<DishComponent> DishComponents { get; set; }
        public DbSet<NotificationSetting> NotificationSettings { get; set; }
        public DbSet<StudentGroupMealPlan> StudentGroupMealPlans { get; set; }
        public DbSet<OrderPortalContent> OrderPortalContents { get; set; }
        public DbSet<OrderPortalBanner> OrderPortalBanners { get; set; }
        public DbSet<OutletTerm> OutletTerms { get; set; }
        public DbSet<StudentGroupSession> StudentGroupSessions { get; set; }
        public DbSet<UserOutlet> UserOutlets { get; set; }
        public DbSet<UserCaterer> UserCaterers { get; set; }

        #region Stored Procedures

        public DbSet<spSalesOrderReport> spSalesOrderReport { get; set; }
        public DbSet<spGetUserActivityLogHeader> spGetUserActivityLog { get; set; }

        #endregion

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var user = builder.Entity<ApplicationUser>();
            //user.HasIndex(a => new { a.InstitutionId, a.Email }).IsUnique();
            builder.Entity<ApplicationUser>().HasMany(u => u.Claims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ApplicationUser>().HasOne(e => e.Institution).WithMany(e => e.Users).HasForeignKey(e => e.InstitutionId);
            builder.Entity<ApplicationUser>().HasOne(e => e.DirectoryListing).WithMany(e => e.Users).HasForeignKey(e => e.DirectoryListingId);
            builder.Entity<ApplicationUser>().HasOne(e => e.Department).WithMany(e => e.Users).HasForeignKey(e => e.DepartmentId);
            builder.Entity<ApplicationUser>().HasOne(u => u.Icon).WithMany(e => e.Users).HasForeignKey(r => r.FileId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ApplicationUser>().HasMany(u => u.ContactGroupMembers).WithOne(e => e.User).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ApplicationUser>().HasIndex(e => new { e.IsActive, e.UserName, e.FullName, e.InstitutionId }).HasName("IX_User_ActiveUser");
            builder.Entity<ApplicationUser>().Property(u => u.ConcurrencyStamp).IsConcurrencyToken();
            builder.Entity<ApplicationUser>().HasMany(u => u.UserOutlets).WithOne(e => e.User).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasMany(u => u.UserCaterers).WithOne(e => e.User).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<ApplicationUser>()
            //.HasOne(b => b.Student)
            //.WithOne(i => i.User)
            //.HasForeignKey<Student>(b => b.UserId);

            builder.Entity<ApplicationUser>().ToTable("User");

            var role = builder.Entity<ApplicationRole>();
            role.HasIndex(a => new { a.InstitutionId, a.Name }).IsUnique();
            builder.Entity<ApplicationRole>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().HasMany(r => r.UserRoles).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().ToTable("Role");
            builder.Entity<ApplicationRole>().HasIndex(r => new { r.Name }).IsUnique(false);

            builder.Entity<ApplicationUserRole>().ToTable("UserRole");
            builder.Entity<UserClaim>().ToTable("UserClaim");
            builder.Entity<UserRoleClaim>().ToTable("UserRoleClaim");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogin");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserToken");

            builder.Entity<ApplicationUserRole>().HasKey(lf => new { lf.UserId, lf.RoleId });
            builder.Entity<ApplicationUserRole>().HasOne(e => e.Role).WithMany(e => e.UserRoles).HasForeignKey(e => e.RoleId);
            builder.Entity<ApplicationUserRole>().HasOne(e => e.User).WithMany(e => e.Roles).HasForeignKey(e => e.UserId);
            builder.Entity<UserClaim>().HasKey(lf => lf.Id);
            builder.Entity<UserRoleClaim>().HasKey(lf => lf.Id);

            builder.Entity<LocationFacility>().HasKey(lf => new { lf.LocationId, lf.FacilityId });
            builder.Entity<LocationFacilityType>().HasKey(lf => new { lf.LocationId, lf.FacilityTypeId });
            //builder.Entity<LocationImageReference>().HasKey(lf => new { lf.LocationId, lf.FileId });
            builder.Entity<LocationInstitution>().HasKey(lf => new { lf.LocationId, lf.InstitutionId });
            builder.Entity<ReservationTime>().HasKey(lf => new { lf.ReservationId, lf.TimeIntervalId });
            builder.Entity<ContactGroupDepartment>().HasKey(lf => new { lf.ContactGroupId, lf.DepartmentId });
            builder.Entity<EpaperTemplateLocation>().HasKey(lf => new { lf.LocationId, lf.EpaperTemplateId });

            builder.Entity<PlaylistImage>().HasKey(lf => new { lf.PlaylistId, lf.ImageId });

            builder.Entity<Location>().HasMany(s => s.LocationFacilities).WithOne(s => s.Location);
            builder.Entity<Location>().HasMany(s => s.LocationInstitutions).WithOne(s => s.Location);
            builder.Entity<Location>().HasMany(s => s.ChildLocations).WithOne(s => s.ParentLocation);
            builder.Entity<Location>().HasMany(s => s.LocationFacilityTypes).WithOne(s => s.Location);

            builder.Entity<Facility>().HasOne(e => e.Icon).WithMany(e => e.Facilities).HasForeignKey(e => e.FileId);
            builder.Entity<Location>().HasOne(e => e.Icon).WithMany(e => e.Locations).HasForeignKey(e => e.FileId);
            builder.Entity<Dish>().HasOne(e => e.Icon).WithMany(e => e.Dishes).HasForeignKey(e => e.FileId);
            builder.Entity<Dish>().HasOne(e => e.ProductionPicture).WithMany(e => e.ProductionDishes).HasForeignKey(e => e.ProductionPictureId);
            //builder.Entity<DishDetail>().HasKey(lf => new { lf.ParentDishId, lf.DishId });
            builder.Entity<DishDetail>().HasOne(e => e.ParentDish).WithMany(e => e.SubDishes).HasForeignKey(e => e.ParentDishId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ContactGroup>().HasMany(s => s.ContactGroupDepartments).WithOne(s => s.ContactGroup);

            builder.Entity<Playlist>().HasMany(s => s.PlaylistImages).WithOne(s => s.Playlist);

            builder.Entity<Reservation>().HasMany(s => s.ReservationContactGroups).WithOne(s => s.Reservation);
            builder.Entity<Reservation>().HasMany(s => s.ChildReservations).WithOne(s => s.ParentReservation);
            builder.Entity<Reservation>().HasMany(s => s.VehicleEntries).WithOne(s => s.Reservation);
            builder.Entity<Reservation>().HasMany(s => s.ReservationPictures).WithOne(s => s.Reservation);

            builder.Entity<ApplicationUser>().HasMany(s => s.UserPhonebooks).WithOne(s => s.User);
            builder.Entity<ApplicationUser>().HasMany(s => s.UserVehicles).WithOne(s => s.User);
            builder.Entity<ApplicationUser>().HasMany(s => s.UserCardIds).WithOne(s => s.User);
            builder.Entity<ApplicationUser>().HasMany(s => s.UserConnections).WithOne(s => s.User);
            builder.Entity<ApplicationUser>().HasMany(s => s.UserWallets).WithOne(s => s.User);
            builder.Entity<ApplicationUser>().HasMany(s => s.UserRewards).WithOne(s => s.User);
            builder.Entity<ApplicationUser>().HasMany(s => s.Students).WithOne(s => s.User);
            builder.Entity<ApplicationUser>().HasMany(s => s.UserOrderAlerts).WithOne(s => s.User);
            builder.Entity<UserGroupMember>().HasKey(sc => new { sc.UserGroupId, sc.UserId });
            

            builder.Entity<UserGroupMember>().HasOne(e => e.UserGroup).WithMany(e => e.Members).HasForeignKey(e => e.UserGroupId);
            builder.Entity<UserGroupMember>().HasOne(e => e.User).WithMany(e => e.UserGroupMembers).HasForeignKey(e => e.UserId);
            builder.Entity<PIBTemplateLocation>().Property(a => a.location_id).ValueGeneratedNever();

            builder.Entity<Device>().HasIndex(e =>
                new { e.IsActive, e.Code, e.device_label, e.MacAddress, e.SerialNumber }).HasName("IX_Device_ActiveDeviceWithSearchKeys");
            //builder.Entity<Device>().HasIndex(a => new { a.InstitutionId, a.Code, a.IsActive }).IsUnique(); //TODO: remove first before training

            builder.Entity<Wallet>().Property(u => u.ConcurrencyStamp).ValueGeneratedOnAddOrUpdate().IsConcurrencyToken(true).IsRowVersion();
            builder.Entity<Reward>().Property(u => u.ConcurrencyStamp).ValueGeneratedOnAddOrUpdate().IsConcurrencyToken(true).IsRowVersion();
            builder.Entity<Student>().HasMany(s => s.StudentCards).WithOne(s => s.Student);
            builder.Entity<Student>().HasMany(s => s.Restrictions).WithOne(s => s.Student);
            builder.Entity<Student>().HasMany(s => s.InterestGroups).WithOne(s => s.Student);
            builder.Entity<StudentGroup>().HasMany(s => s.Sgdetails).WithOne(s => s.StudentGroup);
            builder.Entity<StudentAccount>().HasKey(lf => new { lf.StudentId, lf.UserId });
            builder.Entity<TokenOrder>().HasMany(s => s.Tokens).WithOne(s => s.Order);
            builder.Entity<TokenOrdered>().HasMany(s => s.SelectedDishes).WithOne(s => s.TokenOrdered);
            builder.Entity<TokenOrdered>().HasMany(s => s.TokenAltDishes).WithOne(s => s.TokenOrdered);
            builder.Entity<TokenLabel>().HasMany(s => s.dishes).WithOne(s => s.TokenLabel);
            builder.Entity<MealAllocation>().HasMany(s => s.tokens).WithOne(s => s.Allocation);
            builder.Entity<PackingAllocation>().HasMany(s => s.Dishes).WithOne(s => s.Packing);
            builder.Entity<StaffAccount>().HasKey(lf => new { lf.StaffId, lf.UserId });
            builder.Entity<Staff>().HasOne(e => e.StaffType).WithMany(e => e.Staffs).HasForeignKey(e => e.StaffTypeId);

            builder.Entity<DishTypePeriod>().HasKey(lf => new { lf.DishTypeId, lf.PeriodId });
            builder.Entity<DishPeriod>().HasKey(lf => new { lf.DishId, lf.PeriodId });
            builder.Entity<MenuDish>().HasKey(lf => new { lf.DishId, lf.MenuId, lf.MealTypeId });
            //builder.Entity<MenuCycleSchedulePeriod>().HasKey(lf => new { lf.MenuCycleScheduleId, lf.MealPeriodId });
            builder.Entity<MenuCycleSchedulePeriodMenu>().HasKey(lf => new { lf.MenuCycleSchedulePeriodId, lf.MenuId });
            builder.Entity<OutletMenuCycleSchedulePeriodMenu>().HasKey(lf => new { lf.MenuCycleSchedulePeriodId, lf.MenuId, lf.OutletId });
            //builder.Entity<CatererOutlet>().HasKey(lf => new { lf.CatererInfoId, lf.OutletId, lf.OutletProfileId });
            //builder.Entity<OutletProfile>().HasMany(s => s.Outlets).WithOne(s => s.OutletProfile);
            builder.Entity<MealTypeDish>().HasKey(lf => new { lf.DishId, lf.MealTypeId });
            builder.Entity<OutletMenuDish>().HasKey(lf => new { lf.MenuCycleSchedulePeriodId, lf.DishId, lf.MenuId, lf.MealTypeId, lf.OutletId });
            builder.Entity<MealSessionDetail>().HasOne(e => e.MealSession).WithMany(e => e.Details).HasForeignKey(e => e.MealSessionId);
            builder.Entity<OutletClassRosterSchedulePeriodClass>().HasKey(lf => new { lf.OutletClassRosterSchedulePeriodId, lf.ClassId });
            builder.Entity<OutletDishCyclePeriodMenu>().HasKey(lf => new { lf.DishCyclePeriodId, lf.DishId, lf.OutletId, lf.DishCycleScheduleSetId });

            builder.Entity<DishCycle>().HasMany(lf => lf.Sets).WithOne(e => e.DishCycle);

            builder
            .Entity<NotificationSetting>()
            .Property(e => e.Type)
            .HasConversion(
                v => v.ToString(),
                v => (NotificationSettingType)Enum.Parse(typeof(NotificationSettingType), v));

            builder
            .Entity<NotificationSetting>()
            .Property(e => e.Type)
            .HasConversion(
                v => v.ToString(),
                v => (NotificationSettingType)Enum.Parse(typeof(NotificationSettingType), v));

            builder
            .Entity<NotificationSetting>()
            .Property(e => e.DayEnabled)
            .HasColumnType("nvarchar(24)");

            //builder.Entity<spSalesOrderReport>().Property(m => m.CollectionTime).IsRequired(false);
            //builder.Entity<spSalesOrderReport>().Property(m => m.ReturnTime).IsRequired(false);

            #region Audit Tables

            builder.Entity<Facility>().TrackAllProperties();
            builder.Entity<FacilityType>().TrackAllProperties();
            builder.Entity<Reservation>().TrackAllProperties();
            builder.Entity<Device>().TrackAllProperties();
            builder.Entity<Institution>().TrackAllProperties();
            builder.Entity<Location>().TrackAllProperties();
            builder.Entity<LocationFacility>().TrackAllProperties();
            builder.Entity<LocationFacilityType>().TrackAllProperties();
            builder.Entity<LocationInstitution>().TrackAllProperties();
            builder.Entity<ReservationInvitee>().TrackAllProperties();
            builder.Entity<Department>().TrackAllProperties();
            builder.Entity<ContactGroup>().TrackAllProperties();
            builder.Entity<ContactGroupMember>().TrackAllProperties();
            builder.Entity<ContactGroupDepartment>().TrackAllProperties();
            builder.Entity<ReservationContactGroup>().TrackAllProperties();
            builder.Entity<UserPhonebook>().TrackAllProperties();
            builder.Entity<PIBTemplate>().TrackAllProperties();
            builder.Entity<PIBDevice>().TrackAllProperties();
            builder.Entity<PIBTemplateLocation>().TrackAllProperties();
            builder.Entity<Playlist>().TrackAllProperties();
            builder.Entity<PlaylistImage>().TrackAllProperties();
            builder.Entity<ReservationFeedback>().TrackAllProperties();
            builder.Entity<EpaperTemplate>().TrackAllProperties();
            builder.Entity<EpaperDevice>().TrackAllProperties();
            builder.Entity<EpaperTemplateLocation>().TrackAllProperties();
            builder.Entity<UserVehicle>().TrackAllProperties();
            builder.Entity<UserCardId>().TrackAllProperties();
            builder.Entity<SignageComponent>().TrackAllProperties();
            builder.Entity<SignageCompilation>().TrackAllProperties();
            builder.Entity<SignagePublication>().TrackAllProperties();
            builder.Entity<SignagePublicationHistory>().TrackAllProperties();
            builder.Entity<SignageSchedule>().TrackAllProperties();
            builder.Entity<SignageCompilationComponent>().TrackAllProperties();
            builder.Entity<SignageScheduleCompilation>().TrackAllProperties();
            builder.Entity<Media>().TrackAllProperties();
            builder.Entity<MediaRole>().TrackAllProperties();
            builder.Entity<MediaUserGroup>().TrackAllProperties();
            builder.Entity<DirectoryListing>().TrackAllProperties();
            builder.Entity<DirectoryListingCategory>().TrackAllProperties();
            builder.Entity<EmployeeDesignation>().TrackAllProperties();
            builder.Entity<EmployeeSchedule>().TrackAllProperties();
            builder.Entity<EmployeeScheduleShift>().TrackAllProperties();
            builder.Entity<EmployeeScheduleLocation>().TrackAllProperties();
            builder.Entity<EmployeeScheduleSlot>().TrackAllProperties();
            builder.Entity<EmployeeScheduleInfo>().TrackAllProperties();
            builder.Entity<EmployeeData>().TrackAllProperties();
            builder.Entity<Building>().TrackAllProperties();
            builder.Entity<BuildingFloor>().TrackAllProperties();
            builder.Entity<VehicleEntry>().TrackAllProperties();
            builder.Entity<Floor>().TrackAllProperties();
            builder.Entity<Map>().TrackAllProperties();
            builder.Entity<QueueTableMap>().TrackAllProperties();
            builder.Entity<QueueLog>().TrackAllProperties();
            builder.Entity<UserGroup>().TrackAllProperties();
            builder.Entity<UserGroupMember>().TrackAllProperties();
            builder.Entity<UserGroupLocation>().TrackAllProperties();
            builder.Entity<Ems>().TrackAllProperties();
            builder.Entity<EmsProfile>().TrackAllProperties();
            builder.Entity<EmsSchedule>().TrackAllProperties();
            builder.Entity<LocationImageReference>().TrackAllProperties();
            builder.Entity<ApplicationUser>().TrackAllProperties().Except(e => e.PasswordHash).And(e => e.SecurityStamp);
            builder.Entity<ApplicationRole>().TrackAllProperties();
            builder.Entity<ApplicationUserRole>().TrackAllProperties();
            //builder.Entity<UserRoleClaim>().TrackAllProperties();
            builder.Entity<UserClaim>().TrackAllProperties();
            builder.Entity<ImageReferenceType>().TrackAllProperties();
            builder.Entity<ImageReferenceColor>().TrackAllProperties();
            builder.Entity<AssetModel>().TrackAllProperties();
            builder.Entity<AssetType>().TrackAllProperties();
            builder.Entity<LocationAsset>().TrackAllProperties();
            builder.Entity<Connection>().TrackAllProperties();
            builder.Entity<Wallet>().TrackAllProperties();
            builder.Entity<WalletTransaction>().TrackAllProperties();
            builder.Entity<Reward>().TrackAllProperties();
            builder.Entity<RewardTransaction>().TrackAllProperties();

            builder.Entity<ClassLevel>().TrackAllProperties();
            builder.Entity<Class>().TrackAllProperties();
            builder.Entity<Student>().TrackAllProperties();

            builder.Entity<TokenOrder>().TrackAllProperties();
            builder.Entity<TokensOrderHistory>().TrackAllProperties();
            builder.Entity<TokenOrdered>().TrackAllProperties();
            builder.Entity<TokenOrderDish>().TrackAllProperties();
            builder.Entity<TokenAltDish>().TrackAllProperties();
            builder.Entity<TokenLabel>().TrackAllProperties();
            builder.Entity<MealAllocation>().TrackAllProperties();
            builder.Entity<TokenDishLabel>().TrackAllProperties();
            builder.Entity<PackingAllocation>().TrackAllProperties();
            builder.Entity<DishAllocation>().TrackAllProperties();
            builder.Entity<Payment>().TrackAllProperties();
            builder.Entity<StudentCard>().TrackAllProperties();
            builder.Entity<StudentRestriction>().TrackAllProperties();
            builder.Entity<StudentVoucher>().TrackAllProperties();
            builder.Entity<StudentInterestGroup>().TrackAllProperties();
            builder.Entity<RouteNode>().TrackAllProperties();
            builder.Entity<Staff>().TrackAllProperties();
            builder.Entity<StaffType>().TrackAllProperties();
            builder.Entity<StudentAccount>().TrackAllProperties();
            builder.Entity<StudentManageAccount>().TrackAllProperties();
            builder.Entity<StaffAccount>().TrackAllProperties();
            builder.Entity<Restriction>().TrackAllProperties();
            builder.Entity<RestrictionType>().TrackAllProperties();
            builder.Entity<DishType>().TrackAllProperties();
            builder.Entity<MealPlanOrder>().TrackAllProperties();
            builder.Entity<Cuisine>().TrackAllProperties();
            builder.Entity<CatererInfo>().TrackAllProperties();
            builder.Entity<BentoBoxType>().TrackAllProperties();
            builder.Entity<CartonType>().TrackAllProperties();
            builder.Entity<DeliveryOrder>().TrackAllProperties();
            builder.Entity<StoreInventory>().TrackAllProperties();
            builder.Entity<DeliveryDetail>().TrackAllProperties();
            builder.Entity<StoreInventoryDetail>().TrackAllProperties();
            builder.Entity<DeliveryBento>().TrackAllProperties();
            builder.Entity<TrackingStatus>().TrackAllProperties();
            builder.Entity<StoreInfo>().TrackAllProperties();
            builder.Entity<BentoAsset>().TrackAllProperties();
            builder.Entity<CartonAsset>().TrackAllProperties();
            builder.Entity<CartonDisposableBox>().TrackAllProperties();
            builder.Entity<MealType>().TrackAllProperties();
            builder.Entity<MealPeriod>().TrackAllProperties();
            builder.Entity<DishTypePeriod>().TrackAllProperties();
            builder.Entity<Dish>().TrackAllProperties();
            builder.Entity<DishPeriod>().TrackAllProperties();
            builder.Entity<DishRestriction>().TrackAllProperties();
            builder.Entity<DishDetail>().TrackAllProperties();
            builder.Entity<Menu>().TrackAllProperties();
            builder.Entity<MenuDish>().TrackAllProperties();
            builder.Entity<MenuCycle>().TrackAllProperties();
            builder.Entity<MenuCycleSchedule>().TrackAllProperties();
            builder.Entity<MenuCycleSchedulePeriod>().TrackAllProperties();
            builder.Entity<MenuCycleSchedulePeriodMenu>().TrackAllProperties();
            builder.Entity<OutletProfile>().TrackAllProperties();
            builder.Entity<Outlet>().TrackAllProperties();
            builder.Entity<CatererOutlet>().TrackAllProperties();
            builder.Entity<Driver>().TrackAllProperties();
            builder.Entity<InterestGroup>().TrackAllProperties();
            builder.Entity<Route>().TrackAllProperties();
            builder.Entity<MealSession>().TrackAllProperties();
            builder.Entity<MealSessionDetail>().TrackAllProperties();
            builder.Entity<MealTypeDish>().TrackAllProperties();
            builder.Entity<MenuCycleCalendar>().TrackAllProperties();
            builder.Entity<MenuCycleCalendarBlockedDate>().TrackAllProperties();
            builder.Entity<MenuCycleBlockedDate>().TrackAllProperties();
            builder.Entity<OutletBlockedDate>().TrackAllProperties();
            builder.Entity<OutletMenuDish>().TrackAllProperties();
            builder.Entity<OutletMenuCycleSchedulePeriodMenu>().TrackAllProperties();
            builder.Entity<OutletClassRoster>().TrackAllProperties();
            builder.Entity<OutletClassRosterSchedule>().TrackAllProperties();
            builder.Entity<OutletClassRosterSchedulePeriod>().TrackAllProperties();
            builder.Entity<OutletClassRosterSchedulePeriodClass>().TrackAllProperties();
            builder.Entity<DishCycle>().TrackAllProperties();
            builder.Entity<DishCycleSchedule>().TrackAllProperties();
            builder.Entity<DishCycleScheduleSet>().TrackAllProperties();
            builder.Entity<DishCycleScheduleDetail>().TrackAllProperties();
            builder.Entity<DishCycleScheduleDetailMenu>().TrackAllProperties();
            builder.Entity<DishCycleCalendar>().TrackAllProperties();
            builder.Entity<DishCycleCalendarBlockedDate>().TrackAllProperties();
            builder.Entity<DishCycleBlockedDate>().TrackAllProperties();
            builder.Entity<OutletDishBlockedDate>().TrackAllProperties();
            builder.Entity<DishCyclePeriod>().TrackAllProperties();
            builder.Entity<OutletDishCyclePeriodMenu>().TrackAllProperties();
            builder.Entity<Notification>().TrackAllProperties();
            builder.Entity<NotificationEvent>().TrackAllProperties();
            builder.Entity<PaymentType>().TrackAllProperties();
            builder.Entity<TransactionFee>().TrackAllProperties();
            builder.Entity<TransactionFeeDetail>().TrackAllProperties();
            builder.Entity<VoucherType>().TrackAllProperties();
            builder.Entity<Voucher>().TrackAllProperties();
            builder.Entity<VoucherMealPeriod>().TrackAllProperties();
            builder.Entity<Waiver>().TrackAllProperties();
            builder.Entity<ContactUsSubject>().TrackAllProperties();
            builder.Entity<ContactUsDetail>().TrackAllProperties();
            builder.Entity<EmailTemplate>().TrackAllProperties();
            builder.Entity<UserOrderAlert>().TrackAllProperties();
            builder.Entity<MenuGroup>().TrackAllProperties();
            builder.Entity<MenuGroupDishCycle>().TrackAllProperties();
            builder.Entity<MenuGroupClass>().TrackAllProperties();
            builder.Entity<DishComponent>().TrackAllProperties();
            builder.Entity<StudentGroupMealPlan>().TrackAllProperties();
            builder.Entity<OrderPortalContent>().TrackAllProperties();
            builder.Entity<OrderPortalBanner>().TrackAllProperties();
            builder.Entity<OutletTerm>().TrackAllProperties();
            builder.Entity<StudentGroupSession>().TrackAllProperties();
            builder.Entity<UserOutlet>().TrackAllProperties();
            builder.Entity<UserCaterer>().TrackAllProperties();

            #endregion

        }

        #region Used Methods

        /// <summary>
        /// Resets the action type. Call only after the whole process is done
        /// So the group id retains
        /// </summary>
        public void ResetAuditUserAction()
        {
            AuditUserActivityType = null;
        }

        //public override int SaveChanges(bool acceptAllChangesOnSuccess)
        //{
        //    //UpdateAuditEntities();
        //    //return base.SaveChanges(acceptAllChangesOnSuccess);
        //    return SaveChanges(CurrentInstitutionId, CurrentUserId);
        //}

        public override int SaveChanges()
        {
            //UpdateAuditEntities();
            //return base.SaveChanges();
            return SaveChanges(CurrentInstitutionId, CurrentUserId);
        }

        public virtual int SaveChanges(int? institutionId, int? userId)
        {
            string userName = string.Empty;
            if (userId.HasValue)
            {
                var user = Users.FirstOrDefault(e => e.Id == userId);
                if (user != null)
                {
                    userName = user.UserName;
                    institutionId = institutionId > 0 ? institutionId : user.InstitutionId;
                }
            }

            string institutionName = string.Empty;
            if (institutionId.HasValue)
            {
                var institution = Institutions.FirstOrDefault(e => e.Id == institutionId);
                if (institution != null) institutionName = institution.Name;
            }

            this.AuditChanges(institutionId, userId, userName, institutionName);
            var addedEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
            // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
            int result = base.SaveChanges();
            // By now, we have got the primary keys of added entries of added entiries because of the call to savechanges.
            this.AuditAdditions(addedEntries, institutionId, userId, userName, institutionName);
            // Save changes to audit of added entries
            base.SaveChanges();
            return result;
        }

        //public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    //UpdateAuditEntities();
        //    //return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //    return SaveChangesAsync(CurrentInstitutionId, CurrentUserId, acceptAllChangesOnSuccess, cancellationToken);
        //}

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            //UpdateAuditEntities();
            //return base.SaveChangesAsync(cancellationToken);
            return SaveChangesAsync(CurrentInstitutionId, CurrentUserId, cancellationToken);
        }

        public virtual async Task<int> SaveChangesAsync(int? institutionId, int? userId, CancellationToken cancellationToken)
        {
            string userName = string.Empty;
            if (userId.HasValue)
            {
                var user = Users.FirstOrDefault(e => e.Id == userId);
                if (user != null)
                {
                    userName = user.UserName;
                    institutionId = institutionId > 0 ? institutionId : user.InstitutionId;
                }
            }

            string institutionName = string.Empty;
            if (institutionId.HasValue)
            {
                var institution = Institutions.FirstOrDefault(e => e.Id == institutionId);
                if (institution != null) institutionName = institution.Name;
            }

            this.AuditChanges(institutionId, userId, userName, institutionName);
            var addedEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
            // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
            int result = await base.SaveChangesAsync(cancellationToken);
            // By now, we have got the primary keys of added entries of added entiries because of the call to savechanges.
            this.AuditAdditions(addedEntries, institutionId, userId, userName, institutionName);
            // Save changes to audit of added entries
            await base.SaveChangesAsync(cancellationToken);
            return result;
        }

        //public virtual async Task<int> SaveChangesAsync(int? institutionId, int? userId, bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
        //{
        //    this.AuditChanges(institutionId, userId);
        //    var addedEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
        //    // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
        //    int result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //    // By now, we have got the primary keys of added entries of added entiries because of the call to savechanges.
        //    this.AuditAdditions(addedEntries, institutionId, userId);
        //    // Save changes to audit of added entries
        //    await base.SaveChangesAsync(cancellationToken);
        //    return result;
        //}

        #endregion

        #region Private Methods
        private void AuditChanges(int? institutionId, int? userId, string userName, string institutionName)
        {
            // Get all Deleted/Modified entities (not Unmodified or Detached or Added)
            var changedEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified).ToList();
            foreach (var ent in changedEntries)
            {
                var eventType = this.GetEventType(ent);
                if (eventType != AuditLogType.Deleted && ent.Entity is IAuditableEntity)
                {
                    //var auditedEntity = ent.Entity as IAuditableEntity;
                    //auditedEntity.UpdatedDate = DateTime.Now;
                    //auditedEntity.UpdatedBy = userId;
                    var entity = (IAuditableEntity)ent.Entity;
                    DateTime now = DateTime.Now;
                    if (ent.State == EntityState.Added)
                    {
                        entity.CreatedDate = now;
                        if (!entity.CreatedBy.HasValue)
                        {
                            if (CurrentUserId > 0) entity.CreatedBy = CurrentUserId;
                            else
                            {
                                var currentUser = Users.FirstOrDefault(e => e.InstitutionId == CurrentInstitutionId && e.UserName.Equals(this.CurrentUserName));
                                if (currentUser != null) entity.CreatedBy = currentUser.Id;
                            }
                        }

                        entity.IsActive = true;
                    }
                    else
                    {
                        base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                        base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                    }

                    entity.UpdatedDate = now;
                    if (!entity.UpdatedBy.HasValue)
                    {
                        if (CurrentUserId > 0) entity.UpdatedBy = CurrentUserId;
                        else
                        {
                            var currentUser = Users.FirstOrDefault(e => e.InstitutionId == CurrentInstitutionId && e.UserName.Equals(this.CurrentUserName));
                            if (currentUser != null) entity.UpdatedBy = currentUser.Id;
                        }
                    }
                }

                using (var auditor = new LogAuditor(ent, this))
                {
                    var record = auditor.CreateLogRecord(institutionId, userId, userName, institutionName, eventType, AuditUserActivityType);
                    if (record != null)
                        this.AuditLogs.Add(record);
                }
            }
        }

        private void AuditAdditions(IEnumerable<EntityEntry> addedEntries, int? institutionId, int? userId, string userName, string institutionName)
        {
            // Get all Added entities
            foreach (var ent in addedEntries)
            {
                if (ent.Entity is IAuditableEntity)
                {
                    var auditedEntity = ent.Entity as IAuditableEntity;
                    auditedEntity.CreatedDate = DateTime.Now;
                    auditedEntity.UpdatedDate = DateTime.Now;
                    auditedEntity.IsActive = true;
                    if (userId > 0)
                    {
                        auditedEntity.CreatedBy = userId;
                        auditedEntity.UpdatedBy = userId;
                    }
                    else
                    {
                        if (CurrentUserId > 0) auditedEntity.CreatedBy = CurrentUserId;
                        else
                        {
                            var currentUser = Users.FirstOrDefault(e => e.InstitutionId == CurrentInstitutionId && e.UserName.Equals(this.CurrentUserName));
                            if (currentUser != null) auditedEntity.CreatedBy = currentUser.Id;
                        }

                        auditedEntity.UpdatedBy = auditedEntity.CreatedBy;
                    }
                }

                using (var auditor = new LogAuditor(ent, this))
                {
                    var record = auditor.CreateLogRecord(institutionId, userId, userName, institutionName, AuditLogType.Added, AuditUserActivityType);
                    if (record != null)
                        this.AuditLogs.Add(record);
                }
            }
        }

        private AuditLogType GetEventType(EntityEntry entry)
        {
            if (entry.State == EntityState.Deleted) return AuditLogType.Deleted;
            if (entry.Metadata.FindProperty("IsActive") != null)
            {
                var previouslyActive = (bool)entry.Property("IsActive").OriginalValue;
                var nowInactive = (bool)entry.Property("IsActive").CurrentValue;
                if (previouslyActive && !nowInactive)
                    return AuditLogType.SoftDeleted;

                if (!previouslyActive && nowInactive)
                    return AuditLogType.UnDeleted;
            }

            return AuditLogType.Modified;
        }

        private void UpdateAuditEntities()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach (var entry in modifiedEntries)
            {
                var entity = (IAuditableEntity)entry.Entity;
                DateTime now = DateTime.Now;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = now;
                    if (!entity.CreatedBy.HasValue)
                    {
                        if (CurrentUserId > 0) entity.CreatedBy = CurrentUserId;
                        else
                        {
                            var currentUser = Users.FirstOrDefault(e => e.InstitutionId == CurrentInstitutionId && e.UserName.Equals(this.CurrentUserName));
                            if (currentUser != null) entity.CreatedBy = currentUser.Id;
                        }
                    }

                    entity.IsActive = true;
                }
                else
                {
                    base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }

                entity.UpdatedDate = now;
                if (!entity.UpdatedBy.HasValue)
                {
                    if (CurrentUserId > 0) entity.UpdatedBy = CurrentUserId;
                    else
                    {
                        var currentUser = Users.FirstOrDefault(e => e.InstitutionId == CurrentInstitutionId && e.UserName.Equals(this.CurrentUserName));
                        if (currentUser != null) entity.UpdatedBy = currentUser.Id;
                    }
                }
            }
        }
        #endregion
    }
}
