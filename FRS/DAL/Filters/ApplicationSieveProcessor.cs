using DAL.Models;
using DAL.Models.MealOrder;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Filters
{
    public class ApplicationSieveProcessor : SieveProcessor
    {
        //public ApplicationSieveProcessor(
        //    IOptions<SieveOptions> options)
        //    : base(options)
        //{
        //}

        public ApplicationSieveProcessor(
            IOptions<SieveOptions> options,
            ISieveCustomFilterMethods customFilterMethods)
            : base(options, customFilterMethods)
        {
        }

        //public ApplicationSieveProcessor(
        //    IOptions<SieveOptions> options,
        //    ISieveCustomSortMethods customSortMethods,
        //    ISieveCustomFilterMethods customFilterMethods)
        //    : base(options, customSortMethods, customFilterMethods)
        //{
        //}

        protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
        {
            UserMapping(ref mapper);
            RoleMapping(ref mapper);
            UserGroupMapping(ref mapper);
            DepartmentMapping(ref mapper);
            ImageReferenceTypeMapping(ref mapper);
            ImageReferenceColorMapping(ref mapper);
            DeviceMapping(ref mapper);
            SmartRoomSchedulerLogMapping(ref mapper);
            AuthLogMapping(ref mapper);
            UpDownTimeLogMapping(ref mapper);
            AuditLogMapping(ref mapper);
            AuditLogDetailMapping(ref mapper);
            AssetTypeMapping(ref mapper);
            AssetModelMapping(ref mapper);
            AssetMapping(ref mapper);
            ServiceContractMapping(ref mapper);
            EmailQueueMapping(ref mapper);
            DeviceTypeMapping(ref mapper);
            ChannelInfoMapping(ref mapper);
            ClassMapping(ref mapper);
            ClassBatchMapping(ref mapper);
            ClassLevelMapping(ref mapper);
            DispenserOutletMapping(ref mapper);
            StudentMapping(ref mapper);
            StudentWalletTransactionMapping(ref mapper);
            StudentPointTransactionMapping(ref mapper);
            StudentCardMapping(ref mapper);
            StudentRestrictionMapping(ref mapper);
            StaffTypeMapping(ref mapper);
            StaffMapping(ref mapper);
            RestrictionMapping(ref mapper);
            RestrictionTypeMapping(ref mapper);
            DishTypeMapping(ref mapper);
            InterestGroupMapping(ref mapper);
            CatererInfoMapping(ref mapper);
            OutletMapping(ref mapper);
            OutletProfileMapping(ref mapper);
            BentoBoxTypeMapping(ref mapper);
            CartonTypeMapping(ref mapper);
            DeliveryOrderMapping(ref mapper);
            DeliveryOrderNewMapping(ref mapper);
            StoreInventoryMapping(ref mapper);
            StoreInventoryDetailMapping(ref mapper);
            TrackingStatusMapping(ref mapper);
            StoreInfoMapping(ref mapper);
            BentoAssetMapping(ref mapper);
            CartonAssetMapping(ref mapper);
            CartonDisposableBoxMapping(ref mapper);
            MealTypeMapping(ref mapper);
            MealPeriodMapping(ref mapper);
            MealSessionMapping(ref mapper);
            MealSessionDetailMapping(ref mapper);
            DishMapping(ref mapper);
            MenuMapping(ref mapper);
            MenuCycleMapping(ref mapper);
            MenuCycleScheduleMapping(ref mapper);
            MenuCycleSchedulePeriodMapping(ref mapper);
            MenuCycleSchedulePeriodMenuMapping(ref mapper);
            CuisineMapping(ref mapper);
            DriverMapping(ref mapper);
            RouteMapping(ref mapper);
            TokenOrderMapping(ref mapper);
            MealPlanOrderMapping(ref mapper);
            TokensOrderHistoryMapping(ref mapper);
            TokenLabelMapping(ref mapper);
            MealAllocationMapping(ref mapper);
            PackingAllocationMapping(ref mapper);
            StudentGroupMapping(ref mapper);
            OutletClassRosterMapping(ref mapper);
            DishCycleMapping(ref mapper);
            DishCycleScheduleMapping(ref mapper);
            DishCycleScheduleDetailMapping(ref mapper);
            NotificationMapping(ref mapper);
            PaymentTypeMapping(ref mapper);
            PaymentMapping(ref mapper);
            TransactionFeeMapping(ref mapper);
            TransactionFeeDetailMapping(ref mapper);
            ContactUsSubjectMapping(ref mapper);
            ContactUsDetailMapping(ref mapper);
            EmailTemplateMapping(ref mapper);
            NotificationEventMapping(ref mapper);
            CancelOrderRequestMapping(ref mapper);
            ExternalAppLoginLogMapping(ref mapper);
            MenuGroupMapping(ref mapper);
            FaqSubjectMapping(ref mapper);
            FaqDetailMapping(ref mapper);
            CatererAssetTypeMapping(ref mapper);
            return mapper;
        }

        private void NotificationMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Notification>(p => p.Id)
                .CanFilter()
                .CanSort();
            mapper.Property<Notification>(p => p.Body)
                .CanFilter()
                .CanSort();
            mapper.Property<Notification>(p => p.Date)
                .CanFilter()
                .CanSort();
            mapper.Property<Notification>(p => p.EventId)
                .CanFilter()
                .CanSort();
            mapper.Property<Notification>(p => p.Header)
                .CanFilter()
                .CanSort();
            mapper.Property<Notification>(p => p.IsActive)
                .CanFilter()
                .CanSort();
            mapper.Property<Notification>(p => p.IsRead)
                .CanFilter()
                .CanSort();
            mapper.Property<Notification>(p => p.UserId)
                .CanFilter()
                .CanSort();

            //.HasName("institution_name");
        }

        private void SmartRoomSchedulerLogMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<SmartRoomSchedulerLog>(p => p.Details)
                .CanFilter()
                .CanSort();
            mapper.Property<SmartRoomSchedulerLog>(p => p.EventDateTime)
                .CanFilter()
                .CanSort();
            mapper.Property<SmartRoomSchedulerLog>(p => p.NoRecordsAffected)
                .CanFilter()
                .CanSort();
            mapper.Property<SmartRoomSchedulerLog>(p => p.Id)
                .CanFilter()
                .CanSort();
            mapper.Property<SmartRoomSchedulerLog>(p => p.Status)
                .CanFilter()
                .CanSort();
            //.HasName("institution_name");
        }

        private void AuthLogMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<AuthenticationLog>(p => p.CreatedDate)
                .CanFilter()
                .CanSort();
            mapper.Property<AuthenticationLog>(p => p.InstitutionCode)
                .CanFilter()
                .CanSort();
            mapper.Property<AuthenticationLog>(p => p.Message)
                .CanFilter()
                .CanSort();
            mapper.Property<AuthenticationLog>(p => p.UserName)
                .CanFilter()
                .CanSort();
            //.HasName("institution_name");
        }

        private void UpDownTimeLogMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<UpDownTimeLog>(p => p.CreatedDate)
                .CanFilter()
                .CanSort();
            mapper.Property<UpDownTimeLog>(p => p.DeviceIdentifier)
                .CanFilter()
                .CanSort();
            mapper.Property<UpDownTimeLog>(p => p.IsUp)
                .CanSort();
        }

        private void AuditLogMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<AuditLog>(p => p.EventDateTime)
                .CanFilter()
                .CanSort();
            mapper.Property<AuditLog>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();
            mapper.Property<AuditLog>(p => p.InstitutionName)
                .CanFilter()
                .CanSort();
            mapper.Property<AuditLog>(p => p.LogType)
                .CanFilter()
                .CanSort();
            mapper.Property<AuditLog>(p => p.RecordId)
                .CanFilter()
                .CanSort();
            mapper.Property<AuditLog>(p => p.TableName)
                .CanFilter()
                .CanSort();
            mapper.Property<AuditLog>(p => p.UserId)
                .CanFilter()
                .CanSort();
            mapper.Property<AuditLog>(p => p.UserName)
                .CanFilter()
                .CanSort();

            mapper.Property<AuditLog>(p => p.ActionName)
               .CanFilter()
               .CanSort();
            mapper.Property<AuditLog>(p => p.GroupId)
               .CanFilter()
               .CanSort();
            mapper.Property<AuditLog>(p => p.Remarks)
               .CanFilter()
               .CanSort();
            //.HasName("institution_name");
        }

        private void AuditLogDetailMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<AuditLogDetail>(p => p.AuditLogId)
                .CanFilter()
                .CanSort();
            mapper.Property<AuditLogDetail>(p => p.NewValue)
                .CanFilter()
                .CanSort();
            mapper.Property<AuditLogDetail>(p => p.OriginalValue)
                .CanFilter()
                .CanSort();
            mapper.Property<AuditLogDetail>(p => p.PropertyName)
                .CanFilter()
                .CanSort();

            //.HasName("institution_name");
        }

        private void DepartmentMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Department>(p => p.Institution.Name)
                .CanFilter()
                .CanSort();
            //.HasName("institution_name");
        }

        private void ImageReferenceTypeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<ImageReferenceType>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<ImageReferenceType>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<ImageReferenceType>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<ImageReferenceType>(p => p.Description)
                .CanFilter()
                .CanSort();

            mapper.Property<ImageReferenceType>(p => p.Institution.Name)
                .CanFilter()
                .CanSort();
        }


        private void ImageReferenceColorMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<ImageReferenceColor>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<ImageReferenceColor>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<ImageReferenceColor>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<ImageReferenceColor>(p => p.ColorCode)
                .CanFilter()
                .CanSort();

            mapper.Property<ImageReferenceColor>(p => p.Institution.Name)
                .CanFilter()
                .CanSort();
        }

        private void UserMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<ApplicationUser>(p => p.FullName)
                .CanFilter()
                .CanSort();
            mapper.Property<ApplicationUser>(p => p.JobTitle)
                .CanFilter()
                .CanSort();
            mapper.Property<ApplicationUser>(p => p.IsActive)
                .CanFilter()
                .CanSort();
            mapper.Property<ApplicationUser>(p => p.PhoneNumber)
                .CanFilter()
                .CanSort();

            mapper.Property<ApplicationUser>(p => p.IsConnected)
                .CanFilter()
                .CanSort();
            mapper.Property<ApplicationUser>(p => p.IsEnabled)
               .CanFilter()
               .CanSort();
            mapper.Property<ApplicationUser>(p => p.InstitutionId)
                .CanFilter();
            mapper.Property<ApplicationUser>(p => p.UserName)
                .CanFilter()
                .CanSort();
            //.HasName("institution_name");

            mapper.Property<ApplicationUser>(p => p.Email)
                .CanFilter()
                .CanSort();

            mapper.Property<ApplicationUser>(p => p.FirebaseToken)
                .CanFilter()
                .CanSort();

            mapper.Property<ApplicationUser>(p => p.LastLoginTime)
                .CanFilter()
                .CanSort();

            mapper.Property<ApplicationUser>(p => p.DeletedDate)
                .CanFilter()
                .CanSort();

            mapper.Property<ApplicationUser>(p => p.CreatedDate)
                .CanFilter()
                .CanSort();

            mapper.Property<ApplicationUser>(p => p.DepartmentId)
                .CanFilter()
                .CanSort();

            mapper.Property<ApplicationUser>(p => p.Department.Name)
                .CanFilter()
                .CanSort()
                .HasName("departmentName");

            mapper.Property<ApplicationUser>(p => p.Department.Name)
               .CanFilter()
               .CanSort()
               .HasName("department");

            mapper.Property<ApplicationUser>(p => p.JobTitle)
               .CanFilter()
               .CanSort()
               .HasName("designation");

            mapper.Property<ApplicationUser>(p => p.IsActive)
                .CanFilter()
                .CanSort()
                .HasName("status");
        }

        private void RoleMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<ApplicationRole>(p => p.Name)
                .CanFilter()
                .CanSort();
            mapper.Property<ApplicationRole>(p => p.Description)
                .CanFilter()
                .CanSort();
            mapper.Property<ApplicationRole>(p => p.IsActive)
                .CanFilter()
                .CanSort();
            mapper.Property<ApplicationRole>(p => p.InstitutionId)
                .CanFilter();
            mapper.Property<ApplicationRole>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionCode");
        }

        private void DeviceMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Device>(p => p.Code)
                .CanFilter()
                .CanSort();
            mapper.Property<Device>(p => p.MacAddress)
                .CanFilter()
                .CanSort();
            mapper.Property<Device>(p => p.IpAddress)
                .CanFilter()
                .CanSort();
            mapper.Property<Device>(p => p.device_status)
                .CanFilter()
                .CanSort()
                .HasName("status");

            mapper.Property<Device>(p => p.IsActive)
                .CanFilter()
                .CanSort();
            mapper.Property<Device>(p => p.SerialNumber)
                .CanFilter()
                .CanSort();

            mapper.Property<Device>(p => p.StartDate)
                .CanFilter()
                .CanSort();
            mapper.Property<Device>(p => p.EndDate)
               .CanFilter()
               .CanSort();
            mapper.Property<Device>(p => p.LocationId)
                .CanFilter();
            mapper.Property<Device>(p => p.InstitutionId)
                .CanFilter();
            mapper.Property<Device>(p => p.IsApproved)
                .CanFilter()
                .CanSort();
            mapper.Property<Device>(p => p.device_label)
                .CanFilter()
                .CanSort();
            mapper.Property<Device>(p => p.Location.Name)
                .CanFilter()
                .CanSort()
                .HasName("locationName");
            mapper.Property<Device>(p => p.Module.Route)
               .CanFilter()
               .CanSort()
               .HasName("moduleRoute");
            mapper.Property<Device>(p => p.DeviceTypeId)
                .CanFilter()
                .CanSort();
            //mapper.Property<Device>(p => p.Department.Name)
            //    .CanFilter()
            //    .CanSort()
            //    .HasName("departmentName");
        }


        private void UserGroupMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<UserGroup>(p => p.Name)
                .CanFilter()
                .CanSort();
            mapper.Property<UserGroup>(p => p.Description)
                .CanFilter()
                .CanSort();
            mapper.Property<UserGroup>(p => p.IsActive)
                .CanFilter()
                .CanSort();
            mapper.Property<UserGroup>(p => p.InstitutionId)
                .CanFilter();
        }

        private void AssetTypeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<AssetType>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<AssetType>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<AssetType>(p => p.Name)
                .CanFilter()
                .CanSort();
        }

        private void AssetModelMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<AssetModel>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<AssetModel>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<AssetModel>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<AssetModel>(p => p.AssetTypeId)
                .CanFilter()
                .CanSort();

            mapper.Property<AssetModel>(p => p.IsActive)
                .CanFilter()
                .CanSort();

            mapper.Property<AssetModel>(p => p.Notes)
                .CanFilter()
                .CanSort();
        }

        private void AssetMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Asset>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Asset>(p => p.PurchaseDate)
                .CanFilter()
                .CanSort();

            mapper.Property<Asset>(p => p.SerialNumber)
                .CanFilter()
                .CanSort();

            mapper.Property<Asset>(p => p.WarrantyEnd)
                .CanFilter()
                .CanSort();

            mapper.Property<Asset>(p => p.AssetModelId)
                .CanFilter()
                .CanSort();

            mapper.Property<Asset>(p => p.AssetModel.Name)
                .CanFilter()
                .CanSort()
                .HasName("assetModelName");

            mapper.Property<Asset>(p => p.AssetModel.AssetType.Name)
                .CanFilter()
                .CanSort()
                .HasName("assetTypeName");

            mapper.Property<Asset>(p => p.Location.Name)
                .CanFilter()
                .CanSort()
                .HasName("locationName");

            mapper.Property<Asset>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<Asset>(p => p.WarrantyStart)
                .CanFilter()
                .CanSort();
        }

        private void ServiceContractMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<ServiceContract>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<ServiceContract>(p => p.Coverage)
                .CanFilter()
                .CanSort();

            mapper.Property<ServiceContract>(p => p.Details)
                .CanFilter()
                .CanSort();

            mapper.Property<ServiceContract>(p => p.EndDate)
                .CanFilter()
                .CanSort();

            mapper.Property<ServiceContract>(p => p.Identifier)
                .CanFilter()
                .CanSort();

            mapper.Property<ServiceContract>(p => p.Reference)
                .CanFilter()
                .CanSort();

            mapper.Property<ServiceContract>(p => p.RenewalAlert)
                .CanFilter()
                .CanSort();

            mapper.Property<ServiceContract>(p => p.StartDate)
                .CanFilter()
                .CanSort();
        }

        private void EmailQueueMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<EmailQueue>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailQueue>(p => p.Action)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailQueue>(p => p.Cc)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailQueue>(p => p.FailMessage)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailQueue>(p => p.FromName)
               .CanFilter()
               .CanSort();


            mapper.Property<EmailQueue>(p => p.FromEmail)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailQueue>(p => p.IsFailed)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailQueue>(p => p.IsActive)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailQueue>(p => p.IsSent)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailQueue>(p => p.Subject)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailQueue>(p => p.ToEmail)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailQueue>(p => p.ToName)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailQueue>(p => p.SentDate)
                .CanFilter()
                .CanSort();
        }

        private void DeviceTypeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<DeviceType>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<DeviceType>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<DeviceType>(p => p.Name)
                .CanFilter()
                .CanSort();
        }

        private void ChannelInfoMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<ChannelInfo>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<ChannelInfo>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<ChannelInfo>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<ChannelInfo>(p => p.Number)
                .CanFilter()
                .CanSort();
        }

        private void ClassBatchMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<ClassBatch>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<ClassBatch>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<ClassBatch>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<ClassBatch>(p => p.Year)
                .CanFilter()
                .CanSort();

            mapper.Property<ClassBatch>(p => p.OutletId)
                .CanFilter()
                .CanSort();
        }

        private void ClassLevelMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<ClassLevel>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<ClassLevel>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<ClassLevel>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<ClassLevel>(p => p.Year)
                .CanFilter()
                .CanSort();

            mapper.Property<ClassLevel>(p => p.OutletId)
                .CanFilter()
                .CanSort();

            mapper.Property<ClassLevel>(p => p.Outlet.Name)
                .CanFilter()
                .CanSort()
                .HasName("outletProfileName");
        }

        private void DispenserOutletMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<DispenserOutlet>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<DispenserOutlet>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<DispenserOutlet>(p => p.DispenserCode)
                .CanFilter()
                .CanSort();

            mapper.Property<DispenserOutlet>(p => p.CounterName)
                .CanFilter()
                .CanSort();

            mapper.Property<DispenserOutlet>(p => p.OutletId)
                .CanFilter()
                .CanSort();

            mapper.Property<DispenserOutlet>(p => p.Outlet.Name)
                .CanFilter()
                .CanSort()
                .HasName("outletProfileName");
        }

        private void StudentWalletTransactionMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<StudentWalletTransaction>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentWalletTransaction>(p => p.TransactionType)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentWalletTransaction>(p => p.CreatedDate)
                .CanFilter()
                .CanSort()
                .HasName("transactionDate");

            mapper.Property<StudentWalletTransaction>(p => p.Description)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentWalletTransaction>(p => p.StudentId)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentWalletTransaction>(p => p.student.Name)
                .CanFilter()
                .CanSort()
                .HasName("studentName");
        }

        private void StudentPointTransactionMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<StudentPointTransaction>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentPointTransaction>(p => p.TransactionType)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentPointTransaction>(p => p.CreatedDate)
                .CanFilter()
                .CanSort()
                .HasName("transactionDate");

            mapper.Property<StudentPointTransaction>(p => p.Description)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentPointTransaction>(p => p.StudentId)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentPointTransaction>(p => p.student.Name)
                .CanFilter()
                .CanSort()
                .HasName("studentName");
        }

        private void ClassMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Class>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Class>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<Class>(p => p.ClassLevelId)
                .CanFilter()
                .CanSort();

            mapper.Property<Class>(p => p.ClassLevel.OutletId)
                .CanFilter()
                .CanSort()
                .HasName("classOutletId");

            mapper.Property<Class>(p => p.ClassLevel.Name)
                .CanFilter()
                .CanSort()
                .HasName("classLevelName");
        }

        private void StudentMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Student>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Student>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<Student>(p => p.Gender)
                .CanFilter()
                .CanSort();

            mapper.Property<Student>(p => p.ClassId)
                .CanFilter()
                .CanSort();

            mapper.Property<Student>(p => p.IsFAS)
                .CanFilter()
                .CanSort();

            mapper.Property<Student>(p => p.ClassBatchId)
                .CanFilter()
                .CanSort();

            mapper.Property<Student>(p => p.ClassBatch.Name)
                .CanFilter()
                .CanSort()
                .HasName("classBatchName");

            mapper.Property<Student>(p => p.ClassLevelId)
                .CanFilter()
                .CanSort();

            mapper.Property<Student>(p => p.Class.ClassLevel.Name)
                .CanFilter()
                .CanSort()
                .HasName("classLevelName");

            mapper.Property<Student>(p => p.ClassId)
               .CanFilter()
               .CanSort();

            mapper.Property<Student>(p => p.Class.Name)
                .CanFilter()
                .CanSort()
                .HasName("className");

            mapper.Property<Student>(p => p.IsActive)
                .CanFilter()
                .CanSort();

            mapper.Property<Student>(p => p.OutletId)
               .CanFilter()
               .CanSort();

            mapper.Property<Student>(p => p.Outlet.Name)
                .CanFilter()
                .CanSort()
                .HasName("outletProfileName");

            mapper.Property<Student>(p => p.Account.UserId)
                .CanFilter()
                .CanSort()
                .HasName("userId");

            mapper.Property<Student>(p => p.Account.User.UserName)
                .CanFilter()
                .CanSort()
                .HasName("username");
        }

        private void StudentRestrictionMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<StudentRestriction>(p => p.RestrictionId)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentRestriction>(p => p.StudentId)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentRestriction>(p => p.IsActive)
                .CanFilter()
                .CanSort();
        }

        private void StudentCardMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<StudentCard>(p => p.CardId)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentCard>(p => p.StudentId)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentCard>(p => p.Status)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentCard>(p => p.Remarks)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentCard>(p => p.IsActive)
                .CanFilter()
                .CanSort();
        }

        private void StaffTypeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<StaffType>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<StaffType>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<StaffType>(p => p.Name)
                .CanFilter()
                .CanSort();
        }

        private void StaffMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Staff>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Staff>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<Staff>(p => p.Gender)
                .CanFilter()
                .CanSort();

            mapper.Property<Staff>(p => p.Department.Name)
                .CanFilter()
                .CanSort()
                .HasName("departmentName");

            mapper.Property<Staff>(p => p.StaffType.Name)
                .CanFilter()
                .CanSort()
                .HasName("staffTypeName");
        }

        private void RestrictionMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Restriction>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Restriction>(p => p.Code)
                .CanFilter()
                .CanSort();

            mapper.Property<Restriction>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<Restriction>(p => p.IsHighPriority)
                .CanFilter()
                .CanSort();


            mapper.Property<Restriction>(p => p.IsIncludeHighPriorityFiltering)
                .CanFilter()
                .CanSort();

            mapper.Property<Restriction>(p => p.IsAvailableDateSpecific)
                .CanFilter()
                .CanSort();

            mapper.Property<Restriction>(p => p.IsMealFiltering)
                .CanFilter()
                .CanSort();

            mapper.Property<Restriction>(p => p.Sequence)
                .CanFilter()
                .CanSort();

            mapper.Property<Restriction>(p => p.RestrictionType.Label)
                .CanFilter()
                .CanSort()
                .HasName("restrictionTypeLabel");
        }

        private void RestrictionTypeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<RestrictionType>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<RestrictionType>(p => p.Code)
                .CanFilter()
                .CanSort();

            mapper.Property<RestrictionType>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<RestrictionType>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<RestrictionType>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<RestrictionType>(p => p.IsHandledEMR)
                .CanFilter()
                .CanSort();

            mapper.Property<RestrictionType>(p => p.DietCondition)
                .CanFilter()
                .CanSort();

            mapper.Property<RestrictionType>(p => p.IsDishTypeCustomisation)
                .CanFilter()
                .CanSort();

            mapper.Property<RestrictionType>(p => p.IsSpecialCondition)
                .CanFilter()
                .CanSort();

            mapper.Property<RestrictionType>(p => p.IsCarriedForward)
                .CanFilter()
                .CanSort();

            mapper.Property<RestrictionType>(p => p.IsMealFiltering)
                .CanFilter()
                .CanSort();

            mapper.Property<RestrictionType>(p => p.Sequence)
                .CanFilter()
                .CanSort();
        }

        private void DishTypeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<DishType>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<DishType>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<DishType>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<DishType>(p => p.CatererId)
                .CanFilter()
                .CanSort();

            mapper.Property<DishType>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");
        }

        private void InterestGroupMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<InterestGroup>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<InterestGroup>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<InterestGroup>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<InterestGroup>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");
        }
        

        private void CatererInfoMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<CatererInfo>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<CatererInfo>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<CatererInfo>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<CatererInfo>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");
        }

        private void OutletProfileMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<OutletProfile>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<OutletProfile>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<OutletProfile>(p => p.CatererId)
                .CanFilter()
                .CanSort();
        }

        private void OutletMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Outlet>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Outlet>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<Outlet>(p => p.Address)
                .CanFilter()
                .CanSort();

            mapper.Property<Outlet>(p => p.Location.Name)
                .CanFilter()
                .CanSort()
                .HasName("locationName");

            //mapper.Property<Outlet>(p => p.OutletProfile.Label)
            //    .CanFilter()
            //    .CanSort()
            //    .HasName("outletProfileName");
        }

        private void StudentGroupMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<StudentGroup>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentGroup>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentGroup>(p => p.Code)
                .CanFilter()
                .CanSort();

            mapper.Property<StudentGroup>(p => p.OutletId)
                .CanFilter()
                .CanSort();
        }

        private void BentoBoxTypeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<BentoBoxType>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoBoxType>(p => p.Code)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoBoxType>(p => p.Details)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoBoxType>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoBoxType>(p => p.CatererInfoId)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoBoxType>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<BentoBoxType>(p => p.CatererInfo.Name)
                .CanFilter()
                .CanSort()
                .HasName("catererInfoName");
        }


        private void CartonTypeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<CartonType>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonType>(p => p.Code)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonType>(p => p.Details)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonType>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonType>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<CartonType>(p => p.CatererInfo.Name)
                .CanFilter()
                .CanSort()
                .HasName("catererInfoName");
        }

        private void DeliveryOrderMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<DeliveryOrder>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrder>(p => p.DONumber)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrder>(p => p.DeliveryAddress)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrder>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrder>(p => p.ToStoreId)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrder>(p => p.Completed)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrderNew>(p => p.Completed)
               .CanFilter()
               .CanSort()
               .HasName("Status"); ;

            mapper.Property<DeliveryOrder>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<DeliveryOrder>(p => p.CatererInfo.Name)
                .CanFilter()
                .CanSort()
                .HasName("catererInfoName");

            mapper.Property<DeliveryOrder>(p => p.FromStore.Name)
                .CanFilter()
                .CanSort()
                .HasName("fromStoreName");

            mapper.Property<DeliveryOrder>(p => p.ToStore.Name)
                .CanFilter()
                .CanSort()
                .HasName("toStoreName");
        }

        private void DeliveryOrderNewMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<DeliveryOrderNew>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrderNew>(p => p.DONumber)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrderNew>(p => p.CreatedDate)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrderNew>(p => p.DeliveryAddress)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrderNew>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrderNew>(p => p.ToStoreId)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrderNew>(p => p.Completed)
                .CanFilter()
                .CanSort();

            mapper.Property<DeliveryOrderNew>(p => p.Completed)
                .CanFilter()
                .CanSort()
                .HasName("Status"); ;

            mapper.Property<DeliveryOrderNew>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<DeliveryOrderNew>(p => p.CatererInfo.Name)
                .CanFilter()
                .CanSort()
                .HasName("catererInfoName");

            mapper.Property<DeliveryOrderNew>(p => p.FromStore.Name)
                .CanFilter()
                .CanSort()
                .HasName("fromStoreName");

            mapper.Property<DeliveryOrderNew>(p => p.ToStore.Name)
                .CanFilter()
                .CanSort()
                .HasName("toStoreName");
        }

        private void StoreInventoryMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<StoreInventory>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInventory>(p => p.DeliveryOrderID)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInventory>(p => p.DeliveryOrderNewID)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInventory>(p => p.StoreInfoId)
                .CanFilter()
                .CanSort();
        }

        private void StoreInventoryDetailMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<StoreInventoryDetail>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInventoryDetail>(p => p.StoreInfoId)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInventoryDetail>(p => p.TimeReceived)
                .CanFilter()
                .CanSort();
        }

        private void TrackingStatusMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<TrackingStatus>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<TrackingStatus>(p => p.Status)
                .CanFilter()
                .CanSort();

            mapper.Property<TrackingStatus>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<TrackingStatus>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");
        }

        private void StoreInfoMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<StoreInfo>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInfo>(p => p.Code)
               .CanFilter()
               .CanSort();

            mapper.Property<StoreInfo>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInfo>(p => p.Address)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInfo>(p => p.StoreType)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInfo>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInfo>(p => p.CatererInfoId)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInfo>(p => p.OutletId)
                .CanFilter()
                .CanSort();

            mapper.Property<StoreInfo>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<StoreInfo>(p => p.CatererInfo.Name)
               .CanFilter()
               .CanSort()
               .HasName("catererInfoName");
        }


        private void BentoAssetMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<BentoAsset>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoAsset>(p => p.Code)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoAsset>(p => p.LastReturnTime)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoAsset>(p => p.UpdatedDate)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoAsset>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoAsset>(p => p.CartonAssetId)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoAsset>(p => p.RouteId)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoAsset>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<BentoAsset>(p => p.BentoBoxType.Code)
                .CanFilter()
                .CanSort()
                .HasName("bentoBoxTypeCode");

            mapper.Property<BentoAsset>(p => p.CartonAsset.Code)
                .CanFilter()
                .CanSort()
                .HasName("cartonAssetCode");

            mapper.Property<BentoAsset>(p => p.Dish.Code)
                .CanFilter()
                .CanSort()
                .HasName("dishCode");

            mapper.Property<BentoAsset>(p => p.Dish.Label)
                .CanFilter()
                .CanSort()
                .HasName("dishLabel");

            mapper.Property<BentoAsset>(p => p.StoreInfo.Code)
                .CanFilter()
                .CanSort()
                .HasName("storeInfoCode");

            mapper.Property<BentoAsset>(p => p.Route.Label)
               .CanFilter()
               .CanSort()
               .HasName("routeLabel");

            mapper.Property<BentoAsset>(p => p.LastUpdateTime)
                .CanFilter()
                .CanSort();

            mapper.Property<BentoAsset>(p => p.LastPackingTime)
                .CanFilter()
                .CanSort();

        }

        private void CartonAssetMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<CartonAsset>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonAsset>(p => p.Code)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonAsset>(p => p.UpdatedDate)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonAsset>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonAsset>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<CartonAsset>(p => p.CartonType.Code)
                .CanFilter()
                .CanSort()
                .HasName("cartonTypeCode");
        }

        private void CartonDisposableBoxMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<CartonDisposableBox>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonDisposableBox>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonDisposableBox>(p => p.RouteId)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonDisposableBox>(p => p.CartonAssetId)
                .CanFilter()
                .CanSort();

            mapper.Property<CartonDisposableBox>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<CartonDisposableBox>(p => p.CartonAsset.Code)
                .CanFilter()
                .CanSort()
                .HasName("cartonAssetCode");

            mapper.Property<CartonDisposableBox>(p => p.Dish.Code)
                .CanFilter()
                .CanSort()
                .HasName("dishCode");

            mapper.Property<CartonDisposableBox>(p => p.Route.Label)
                .CanFilter()
                .CanSort()
                .HasName("routeLabel");
        }

        private void MealTypeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<MealType>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<MealType>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<MealType>(p => p.Price)
                .CanFilter()
                .CanSort();

            mapper.Property<MealType>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealType>(p => p.CatererId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealType>(p => p.ChargeType)
                .CanFilter()
                .CanSort()
                .HasName("chargeBy");

            mapper.Property<MealType>(p => p.CuisineId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealType>(p => p.Cuisine.Name)
                .CanFilter()
                .CanSort()
                .HasName("cuisineName");

            mapper.Property<MealType>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");
        }

        private void DriverMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Driver>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Driver>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<Driver>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<Driver>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");
        }

        private void RouteMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Route>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Route>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<Route>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<Route>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");
        }

        private void MealPeriodMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<MealPeriod>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<MealPeriod>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<MealPeriod>(p => p.Sequence)
                .CanFilter()
                .CanSort();

            mapper.Property<MealPeriod>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealPeriod>(p => p.OutletProfileId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealPeriod>(p => p.StartDate)
                .CanFilter()
                .CanSort();

            mapper.Property<MealPeriod>(p => p.EndDate)
                .CanFilter()
                .CanSort();

            mapper.Property<MealPeriod>(p => p.OutletProfile.CatererId)
                .CanFilter()
                .CanSort()
                .HasName("CatererId");

            mapper.Property<MealPeriod>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<MealPeriod>(p => p.OutletProfile.Label)
                .CanFilter()
                .CanSort()
                .HasName("outletProfileName");
        }

        private void MealSessionMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<MealSession>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSession>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSession>(p => p.Sequence)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSession>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSession>(p => p.OutletId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSession>(p => p.StartDate)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSession>(p => p.EndDate)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSession>(p => p.MealPeriodId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSession>(p => p.CatererId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSession>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<MealSession>(p => p.Outlet.Name)
                .CanFilter()
                .CanSort()
                .HasName("outletName");

            mapper.Property<MealSession>(p => p.MealPeriod.Name)
                .CanFilter()
                .CanSort()
                .HasName("mealPeriodName");
        }

        private void MealSessionDetailMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<MealSessionDetail>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSessionDetail>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSessionDetail>(p => p.Sequence)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSessionDetail>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSessionDetail>(p => p.MealSessionId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSessionDetail>(p => p.StartDate)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSessionDetail>(p => p.EndDate)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSessionDetail>(p => p.RouteTime)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSessionDetail>(p => p.OverheadTime)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSessionDetail>(p => p.CalSourceTime)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSessionDetail>(p => p.RouteId)
                .CanFilter()
                .CanSort();

            mapper.Property<MealSessionDetail>(p => p.Route.Label)
                .CanFilter()
                .CanSort()
                .HasName("routeName");

            mapper.Property<MealSessionDetail>(p => p.MealSession.Name)
                .CanFilter()
                .CanSort()
                .HasName("mealSessionName");
        }

        private void DishMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Dish>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Dish>(p => p.Code)
                .CanFilter()
                .CanSort();

            mapper.Property<Dish>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<Dish>(p => p.DishTypeId)
                .CanFilter()
                .CanSort();

            mapper.Property<Dish>(p => p.DishType.Name)
                .CanFilter()
                .CanSort()
                .HasName("dishTypeName");

            mapper.Property<Dish>(p => p.RRPrice)
                .CanFilter()
                .CanSort();

            mapper.Property<Dish>(p => p.Cost)
                .CanFilter()
                .CanSort();

            mapper.Property<Dish>(p => p.IsEnabled)
                .CanFilter()
                .CanSort();

            mapper.Property<Dish>(p => p.FileId)
                .CanFilter()
                .CanSort();

            mapper.Property<Dish>(p => p.ProductionPictureId)
                .CanFilter()
                .CanSort();

            mapper.Property<Dish>(p => p.CuisineId)
                .CanFilter()
                .CanSort();

            mapper.Property<Dish>(p => p.CatererId)
                .CanFilter()
                .CanSort();

            mapper.Property<Dish>(p => p.Cuisine.Name)
               .CanFilter()
               .CanSort()
               .HasName("cuisineName");

            mapper.Property<Dish>(p => p.ProductionDescription)
                .CanFilter()
                .CanSort();

            //mapper.Property<Dish>(p => p.ParentDishId)
            //    .CanFilter()
            //    .CanSort();
        }

        private void CuisineMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Cuisine>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Cuisine>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<Cuisine>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();
        }

        

        private void MenuMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Menu>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Menu>(p => p.Code)
                .CanFilter()
                .CanSort();

            mapper.Property<Menu>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<Menu>(p => p.StartDate)
               .CanFilter()
               .CanSort();

            mapper.Property<Menu>(p => p.EndDate)
               .CanFilter()
               .CanSort();

            mapper.Property<Menu>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<Menu>(p => p.CatererId)
                .CanFilter()
                .CanSort();

            mapper.Property<Menu>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<Menu>(p => p.CuisineId)
                .CanFilter()
                .CanSort();

            mapper.Property<Menu>(p => p.Cuisine.Name)
                .CanFilter()
                .CanSort()
                .HasName("cuisineName");
        }

        private void TokenOrderMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<TokenOrder>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<TokenOrder>(p => p.TransactionTime)
                .CanFilter()
                .CanSort();

            mapper.Property<TokenOrder>(p => p.Status)
                .CanFilter()
                .CanSort();

            mapper.Property<TokenOrder>(p => p.IsFAS)
                .CanFilter()
                .CanSort();

            mapper.Property<TokenOrder>(p => p.Student.OutletId)
                .CanFilter()
                .CanSort()
                .HasName("outletId");

            mapper.Property<TokenOrder>(p => p.DeliveryDate)
                .CanFilter()
                .CanSort();

            mapper.Property<TokenOrder>(p => p.MealSessionDetailId)
                .CanFilter()
                .CanSort()
                .HasName("sessionId");

            mapper.Property<TokenOrder>(p => p.CancelRequestStatus)
                .CanFilter()
                .CanSort();

            mapper.Property<TokenOrder>(p => p.Student.Name)
                .CanFilter()
                .CanSort()
                .HasName("profileName");

            mapper.Property<TokenOrder>(p => p.Session.Name)
                .CanFilter()
                .CanSort()
                .HasName("mealSessionName");

            mapper.Property<TokenOrder>(p => p.Payment.PaymentType.Name)
                .CanFilter()
                .CanSort()
                .HasName("paymentTypeName");

            mapper.Property<TokenOrder>(p => p.Payment.PaymentNumber)
                .CanFilter()
                .CanSort()
                .HasName("paymentNumber");

            mapper.Property<TokenOrder>(p => p.Payment.fomoid)
                .CanFilter()
                .CanSort()
                .HasName("fomoId");

            mapper.Property<TokenOrder>(p => p.CreatedByUser.FriendlyName)
                .CanFilter()
                .CanSort()
                .HasName("processedBy");

            mapper.Property<TokenOrder>(p => p.Student.Class.Name)
                .CanFilter()
                .CanSort()
                .HasName("className");

            mapper.Property<TokenOrder>(p => p.Payment.InvoiceNumber)
                .CanFilter()
                .CanSort()
                .HasName("invoiceNumber");

            mapper.Property<TokenOrder>(p => p.Payment.Voucher.Code)
                .CanFilter()
                .CanSort()
                .HasName("voucherCode");

            mapper.Property<TokenOrder>(p => p.Payment.discount)
                .CanFilter()
                .CanSort()
                .HasName("discount");

            mapper.Property<TokenOrder>(p => p.Payment.total)
                .CanFilter()
                .CanSort()
                .HasName("total");

            mapper.Property<TokenOrder>(p => p.Student.Class.Name)
                .CanFilter()
                .CanSort()
                .HasName("className");
        }

        private void MealPlanOrderMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<MealPlanOrder>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<MealPlanOrder>(p => p.TransactionTime)
                .CanFilter()
                .CanSort();

            mapper.Property<MealPlanOrder>(p => p.Status)
                .CanFilter()
                .CanSort();

            mapper.Property<MealPlanOrder>(p => p.Student.OutletId)
                .CanFilter()
                .CanSort()
                .HasName("outletId");

            mapper.Property<MealPlanOrder>(p => p.Student.Name)
                .CanFilter()
                .CanSort()
                .HasName("profileName");


            mapper.Property<MealPlanOrder>(p => p.Payment.PaymentType.Name)
                .CanFilter()
                .CanSort()
                .HasName("paymentTypeName");

            mapper.Property<MealPlanOrder>(p => p.Payment.PaymentNumber)
                .CanFilter()
                .CanSort()
                .HasName("paymentNumber");

            mapper.Property<MealPlanOrder>(p => p.Payment.fomoid)
                .CanFilter()
                .CanSort()
                .HasName("fomoId");

            mapper.Property<MealPlanOrder>(p => p.CreatedByUser.FriendlyName)
                .CanFilter()
                .CanSort()
                .HasName("processedBy");

            mapper.Property<MealPlanOrder>(p => p.Student.Class.Name)
                .CanFilter()
                .CanSort()
                .HasName("className");

            mapper.Property<MealPlanOrder>(p => p.Payment.InvoiceNumber)
                .CanFilter()
                .CanSort()
                .HasName("invoiceNumber");

            mapper.Property<MealPlanOrder>(p => p.Payment.Voucher.Code)
                .CanFilter()
                .CanSort()
                .HasName("voucherCode");

            mapper.Property<MealPlanOrder>(p => p.Payment.discount)
                .CanFilter()
                .CanSort()
                .HasName("discount");

            mapper.Property<MealPlanOrder>(p => p.Payment.total)
                .CanFilter()
                .CanSort()
                .HasName("total");

            mapper.Property<MealPlanOrder>(p => p.Student.Class.Name)
                .CanFilter()
                .CanSort()
                .HasName("className");
        }

        private void TokensOrderHistoryMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<TokensOrderHistory>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<TokensOrderHistory>(p => p.TransactionTime)
                .CanFilter()
                .CanSort();

            mapper.Property<TokensOrderHistory>(p => p.Student.OutletId)
                .CanFilter()
                .CanSort()
                .HasName("outletId");

            mapper.Property<TokensOrderHistory>(p => p.DeliveryDate)
                .CanFilter()
                .CanSort();

            mapper.Property<TokensOrderHistory>(p => p.MealSessionDetailId)
                .CanFilter()
                .CanSort()
                .HasName("sessionId");

            mapper.Property<TokensOrderHistory>(p => p.CancelRequestStatus)
                .CanFilter()
                .CanSort();

            mapper.Property<TokensOrderHistory>(p => p.Student.Name)
                .CanFilter()
                .CanSort()
                .HasName("profileName");

            mapper.Property<TokensOrderHistory>(p => p.Session.Name)
                .CanFilter()
                .CanSort()
                .HasName("mealSessionName");

            mapper.Property<TokensOrderHistory>(p => p.Payment.PaymentType.Name)
                .CanFilter()
                .CanSort()
                .HasName("paymentTypeName");

            mapper.Property<TokensOrderHistory>(p => p.CreatedByUser.FriendlyName)
                .CanFilter()
                .CanSort()
                .HasName("processedBy");

            mapper.Property<TokensOrderHistory>(p => p.Student.Class.Name)
                .CanFilter()
                .CanSort()
                .HasName("className");
        }

        private void TokenLabelMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<TokenLabel>(p => p.Id)
                .CanFilter()
                .CanSort();

        }

        private void MealAllocationMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<MealAllocation>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<MealAllocation>(p => p.outletId)
                    .CanFilter()
                    .CanSort();

            mapper.Property<MealAllocation>(p => p.deliveryDate)
                    .CanFilter()
                    .CanSort();

            mapper.Property<MealAllocation>(p => p.periodId)
                   .CanFilter()
                   .CanSort();

            mapper.Property<MealAllocation>(p => p.RouteId)
                   .CanFilter()
                   .CanSort();

            mapper.Property<MealAllocation>(p => p.mealSessionId)
                    .CanFilter()
                    .CanSort()
                    .HasName("sessionId");

            //mapper.Property<MealAllocation>(p => p.MealSessionDetail.RouteId)
            //        .CanFilter()
            //        .CanSort()
            //        .HasName("routeId");

        }

        private void PackingAllocationMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<PackingAllocation>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<PackingAllocation>(p => p.OutletId)
                    .CanFilter()
                    .CanSort();

            mapper.Property<PackingAllocation>(p => p.PackingDate)
                    .CanFilter()
                    .CanSort();

            mapper.Property<PackingAllocation>(p => p.RouteId)
                    .CanFilter()
                    .CanSort();

        }

        private void OutletClassRosterMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<OutletClassRoster>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<OutletClassRoster>(p => p.IsActive)
                .CanFilter()
                .CanSort();

            mapper.Property<OutletClassRoster>(p => p.StartDate)
                .CanFilter()
                .CanSort();


            mapper.Property<OutletClassRoster>(p => p.EndDate)
                .CanFilter()
                .CanSort();

            mapper.Property<OutletClassRoster>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<OutletClassRoster>(p => p.MealSessionId)
                .CanFilter()
                .CanSort();

            mapper.Property<OutletClassRoster>(p => p.OutletProfileId)
                .CanFilter()
                .CanSort();
        }

        private void MenuCycleMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<MenuCycle>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<MenuCycle>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<MenuCycle>(p => p.StartDate)
                .CanFilter()
                .CanSort();

            mapper.Property<MenuCycle>(p => p.EndDate)
                .CanFilter()
                .CanSort();

            mapper.Property<MenuCycle>(p => p.OutletProfileId)
                .CanFilter()
                .CanSort();

            mapper.Property<MenuCycle>(p => p.OutletProfile.CatererId)
                .CanFilter()
                .CanSort()
                .HasName("CatererId");

            mapper.Property<MenuCycle>(p => p.Institution.Name)
                .CanFilter()
                .CanSort()
                .HasName("institutionName");

            mapper.Property<MenuCycle>(p => p.OutletProfile.Label)
                .CanFilter()
                .CanSort()
                .HasName("outletProfileName");

            
        }

        private void MenuCycleScheduleMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<MenuCycleSchedule>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<MenuCycleSchedule>(p => p.MenuCycleId)
                .CanFilter()
                .CanSort();

            mapper.Property<MenuCycleSchedule>(p => p.Day)
                .CanFilter()
                .CanSort();
        }

        private void MenuCycleSchedulePeriodMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<MenuCycleSchedulePeriod>(p => p.MenuCycleScheduleId)
                .CanFilter()
                .CanSort();

            mapper.Property<MenuCycleSchedulePeriod>(p => p.MealPeriodId)
                .CanFilter()
                .CanSort();
        }

        private void MenuCycleSchedulePeriodMenuMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<MenuCycleSchedulePeriodMenu>(p => p.MenuCycleSchedulePeriodId)
                .CanFilter()
                .CanSort();

            mapper.Property<MenuCycleSchedulePeriodMenu>(p => p.MenuId)
                .CanFilter()
                .CanSort();
        }

        private void DishCycleMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<DishCycle>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<DishCycle>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<DishCycle>(p => p.StartDate)
                .CanFilter()
                .CanSort();

            mapper.Property<DishCycle>(p => p.EndDate)
                .CanFilter()
                .CanSort();

            mapper.Property<DishCycle>(p => p.OutletProfileId)
                .CanFilter()
                .CanSort();

            mapper.Property<DishCycle>(p => p.MealTypeId)
                .CanFilter()
                .CanSort();

            mapper.Property<DishCycle>(p => p.OutletProfile.CatererId)
                .CanFilter()
                .CanSort()
                .HasName("CatererId");

            mapper.Property<DishCycle>(p => p.OutletProfile.Label)
                .CanFilter()
                .CanSort()
                .HasName("outletProfileName");


        }

        private void DishCycleScheduleMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<DishCycleSchedule>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<DishCycleSchedule>(p => p.DishCycleId)
                .CanFilter()
                .CanSort();

            mapper.Property<DishCycleSchedule>(p => p.Day)
                .CanFilter()
                .CanSort();
        }

        private void DishCycleScheduleDetailMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<DishCycleScheduleDetail>(p => p.DishCycleScheduleId)
                .CanFilter()
                .CanSort();

            mapper.Property<DishCycleScheduleDetail>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<DishCycleScheduleDetail>(p => p.Sequence)
                .CanFilter()
                .CanSort();
        }

        private void PaymentTypeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<PaymentType>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<PaymentType>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<PaymentType>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<PaymentType>(p => p.IsSystem)
                .CanFilter()
                .CanSort();
        }

        private void PaymentMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<Payment>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<Payment>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<Payment>(p => p.InvoiceNumber)
                .CanFilter()
                .CanSort();

            mapper.Property<Payment>(p => p.StudentId)
               .CanFilter()
               .CanSort();

            mapper.Property<Payment>(p => p.Status)
               .CanFilter()
               .CanSort();

            mapper.Property<Payment>(p => p.CreatedDate)
               .CanFilter()
               .CanSort();
        }

        private void TransactionFeeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<TransactionFee>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<TransactionFee>(p => p.PaymentTypeId)
                .CanFilter()
                .CanSort();

            mapper.Property<TransactionFee>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<TransactionFee>(p => p.Amount)
                .CanFilter()
                .CanSort();

            mapper.Property<TransactionFee>(p => p.IsFixed)
                .CanFilter()
                .CanSort();
        }

        private void TransactionFeeDetailMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<TransactionFeeDetail>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<TransactionFeeDetail>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<TransactionFeeDetail>(p => p.Amount)
                .CanFilter()
                .CanSort();

            mapper.Property<TransactionFeeDetail>(p => p.TransactionFeeId)
                .CanFilter()
                .CanSort();
        }

        private void ContactUsSubjectMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<ContactUsSubject>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<ContactUsSubject>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<ContactUsSubject>(p => p.Description)
                .CanFilter()
                .CanSort();

            mapper.Property<ContactUsSubject>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();
        }

        private void ContactUsDetailMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<ContactUsDetail>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<ContactUsDetail>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<ContactUsDetail>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<ContactUsDetail>(p => p.ContactUsSubjectId)
                .CanFilter()
                .CanSort();

            mapper.Property<ContactUsDetail>(p => p.ContactUsSubject.Name)
                .CanFilter()
                .CanSort()
                .HasName("contactUsSubjectName");

            mapper.Property<ContactUsDetail>(p => p.ContactUsSubject.Description)
               .CanFilter()
               .CanSort()
               .HasName("contactUsSubjectDescription");
        }

        private void EmailTemplateMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<EmailTemplate>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailTemplate>(p => p.Subject)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailTemplate>(p => p.Body)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailTemplate>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<EmailTemplate>(p => p.OutletId)
                .CanFilter()
                .CanSort();
        }

        private void NotificationEventMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<NotificationEvent>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<NotificationEvent>(p => p.Template)
                .CanFilter()
                .CanSort();

            mapper.Property<NotificationEvent>(p => p.Title)
                .CanFilter()
                .CanSort();

            mapper.Property<NotificationEvent>(p => p.Type)
                .CanFilter()
                .CanSort();
        }

        private void CancelOrderRequestMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<CancelOrderRequest>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<CancelOrderRequest>(p => p.FileName)
                .CanFilter()
                .CanSort();

            mapper.Property<CancelOrderRequest>(p => p.FilePath)
                .CanFilter()
                .CanSort();

            mapper.Property<CancelOrderRequest>(p => p.IsActive)
                .CanFilter()
                .CanSort();

            mapper.Property<CancelOrderRequest>(p => p.Note)
                .CanFilter()
                .CanSort();

            mapper.Property<CancelOrderRequest>(p => p.OrderId)
                .CanFilter()
                .CanSort();

            mapper.Property<CancelOrderRequest>(p => p.Reason)
                .CanFilter()
                .CanSort();

            mapper.Property<CancelOrderRequest>(p => p.Response)
                .CanFilter()
                .CanSort();

            mapper.Property<CancelOrderRequest>(p => p.Status)
                .CanFilter()
                .CanSort();

            mapper.Property<CancelOrderRequest>(p => p.UserId)
                .CanFilter()
                .CanSort();

            mapper.Property<CancelOrderRequest>(p => p.StudentId)
                .CanFilter()
                .CanSort();

            mapper.Property<CancelOrderRequest>(p => p.Student.Name)
                .CanFilter()
                .CanSort()
                .HasName("studentName");

            mapper.Property<CancelOrderRequest>(p => p.User.UserName)
                .CanFilter()
                .CanSort()
                .HasName("userName");

            mapper.Property<CancelOrderRequest>(p => p.CreatedDate)
                .CanFilter()
                .CanSort()
                .HasName("submissionDate"); 

            mapper.Property<CancelOrderRequest>(p => p.TokenOrder.DeliveryDate)
                .CanFilter()
                .CanSort()
                .HasName("cancellationDate");
        }

        private void ExternalAppLoginLogMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<ExternalAppLoginLog>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<ExternalAppLoginLog>(p => p.AppId)
                .CanFilter()
                .CanSort();

            mapper.Property<ExternalAppLoginLog>(p => p.Email)
                .CanFilter()
                .CanSort();

            mapper.Property<ExternalAppLoginLog>(p => p.EventDateTime)
                .CanFilter()
                .CanSort();

            mapper.Property<ExternalAppLoginLog>(p => p.Message)
                .CanFilter()
                .CanSort();

            mapper.Property<ExternalAppLoginLog>(p => p.UserId)
                .CanFilter()
                .CanSort();

            mapper.Property<ExternalAppLoginLog>(p => p.Username)
                .CanFilter()
                .CanSort();
        }

        private void MenuGroupMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<MenuGroup>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<MenuGroup>(p => p.OutletId)
                .CanFilter()
                .CanSort();
        }

        private void FaqSubjectMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<FaqSubject>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<FaqSubject>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<FaqSubject>(p => p.Description)
                .CanFilter()
                .CanSort();

            mapper.Property<FaqSubject>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();
            mapper.Property<FaqSubject>(p => p.Order)
                .CanFilter()
                .CanSort();
        }

        private void FaqDetailMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<FaqDetail>(p => p.Id)
                .CanFilter()
                .CanSort();

            mapper.Property<FaqDetail>(p => p.Label)
                .CanFilter()
                .CanSort();

            mapper.Property<FaqDetail>(p => p.Description)
                .CanFilter()
                .CanSort();

            mapper.Property<FaqDetail>(p => p.InstitutionId)
                .CanFilter()
                .CanSort();

            mapper.Property<FaqDetail>(p => p.FaqSubjectId)
                .CanFilter()
                .CanSort();

            mapper.Property<FaqDetail>(p => p.Order)
                .CanFilter()
                .CanSort();

            mapper.Property<FaqDetail>(p => p.FaqSubject.Name)
                .CanFilter()
                .CanSort()
                .HasName("faqSubjectName");

            mapper.Property<FaqDetail>(p => p.FaqSubject.Description)
               .CanFilter()
               .CanSort()
               .HasName("faqDescription");
        }

        private void CatererAssetTypeMapping(ref SievePropertyMapper mapper)
        {
            mapper.Property<CatererAssetType>(m => m.CatererInfoId)
                .CanFilter()
                .HasName("CatererInfoId");
        }
    }
}
