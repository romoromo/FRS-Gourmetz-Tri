using DAL.Repositories.Interfaces;
using DAL.Repositories.Interfaces.MealOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IUnitOfWork
    {
        IFacilityRepository Facilities { get; }
        IFacilityTypeRepository FacilityTypes { get; }
        ISignageComponentRepository SignageComponents { get; }
        ISignageCompilationRepository SignageCompilations { get; }
        ISignagePublicationRepository SignagePublications { get; }
        IReservationRepository Reservations { get; }
        IBookingRepository Bookings { get; }
        IDeviceRepository Devices { get; }
        IConnectionRepository Connections { get; }
        IInstitutionRepository Institutions { get; }
        ILocationRepository Locations { get; }
        IDepartmentRepository Departments { get; }
        IImageRepository Images { get; }
        IContactGroupRepository ContactGroups { get; }
        IPIBTemplateRepository PIBTemplates { get; }
        IPlaylistRepository Playlists { get; }
        IMediaRepository Medias { get; }
        IMapRepository Maps { get; }
        IDirectoryListingCategoryRepository DirectoryListingCategorys { get; }
        IMediaExtensionRepository MediaExtensions { get; }
        IEmployeeDesignationRepository EmployeeDesignations { get; }
        IEmployeeScheduleRepository EmployeeSchedules { get; }
        IEmployeeDataRepository EmployeeDatas { get; }
        IQueueTableMapRepository QueueTableMaps { get; }
        IOccupancyLogRepository OccupancyLogs { get; }
        IBuildingRepository Buildings { get; }
        IFloorRepository Floors { get; }
        IDirectoryListingRepository DirectoryListings { get; }
        IApplicationSettingRepository ApplicationSettings { get; }
        IEpaperTemplateRepository EpaperTemplates { get; }
        IUserConnectionRepository UserConnections { get; }
        IModuleRepository Modules { get; }
	    IKioskSettingsRepository KioskSettings { get; }
        IUserGroupRepository UserGroups { get; }
        IEmsRepository Emses { get; }
        IEmsProfileRepository EmsProfiles { get; }
        IEmsScheduleRepository EmsSchedules { get; }

        IImageReferenceTypeRepository ImageReferenceTypes { get; }
        IImageReferenceColorRepository ImageReferenceColors { get; }
        ISmartRoomSchedulerLogRepository SmartRoomSchedulerLogs { get; }
        IAssetModelRepository AssetModels { get; }
        IAssetTypeRepository AssetTypes { get; }
        IAssetRepository Assets { get; }
        IServiceContractRepository ServiceContracts { get; }
        IEmailQueueRepository EmailQueues { get; }
        IEmailTemplateRepository EmailTemplates { get; }
        IWalletRepository Wallets { get; }
        IWalletTransactionRepository WalletTransactions { get; }
        IStudentWalletTransactionRepository StudentWalletTransactions { get; }
        IStudentPointTransactionRepository StudentPointTransactions { get; }
        IRewardRepository Rewards { get; }
        IRewardTransactionRepository RewardTransactions { get; }
        IDeviceTypeRepository DeviceTypes { get; }
        IChannelInfoRepository ChannelInfos { get; }
        IClassBatchRepository ClassBatches { get; }
        IClassLevelRepository ClassLevels { get; }

        IDispenserOutletRepository DispenserOutlets { get; }
        IClassRepository Classes { get; }
        IStudentRepository Students { get; }
        IStudentCardRepository StudentCards { get; }
        IStudentGroupRepository StudentGroups { get; }
        ITokenOrderRepository TokenOrders { get; }
        ITokensOrderHistoryRepository TokensOrderHistorys { get; }
        ITokenLabelRepository TokenLabels { get; }
        IMealAllocationRepository MealAllocations { get; }
        IPackingAllocationRepository PackingAllocations { get; }
        ITokenOrderedRepository TokenOrdereds { get; }
        IStaffTypeRepository StaffTypes { get; }
        IStaffRepository Staffs { get; }
        IRestrictionTypeRepository RestrictionTypes { get; }
        IRestrictionRepository Restrictions { get; }
        IDishTypeRepository DishTypes { get; }
        IMealPlanOrderRepository MealPlanOrders { get; }
        ICatererInfoRepository CatererInfos { get; }
        IOutletRepository Outlets { get; }
        IOutletProfileRepository OutletProfiles { get; }
        IOutletTermRepository OutletTerms { get; }
        IBentoBoxTypeRepository BentoBoxTypes { get; }
        ICartonTypeRepository CartonTypes { get; }
        IDeliveryOrderRepository DeliveryOrders { get; }
        IDeliveryOrderNewRepository DeliveryOrderNews { get; }
        IStoreInventoryRepository StoreInventories { get; }
        ITrackingStatusRepository TrackingStatuss { get; }
        IStoreInfoRepository StoreInfos { get; }
        IBentoAssetRepository BentoAssets { get; }
        ICartonAssetRepository CartonAssets { get; }
        ICartonDisposableBoxRepository CartonDisposableBoxes { get; }
        IMealPeriodRepository MealPeriods { get; }
        IMealTypeRepository MealTypes { get; }
        IDishRepository Dishes { get; }
        IMenuRepository Menus { get; }
        IMenuGroupRepository MenuGroups { get; }
        IMenuCycleRepository MenuCycles { get; }
        ICuisineRepository Cuisines { get; }
        IDriverRepository Drivers { get; }
        IEmailConfirmRepository EmailConfirms { get; }
        IInterestGroupRepository InterestGroups { get; }
        IRouteRepository Routes { get; }
        IMealSessionRepository MealSessions { get; }
        IMealSessionDetailRepository MealSessionDetails { get; }
        IMenuCycleCalendarRepository MenuCycleCalendars { get; }
        IOutletClassRosterRepository OutletClassRosters { get; }
        IDishCycleRepository DishCycles { get; }
        IDishCycleCalendarRepository DishCycleCalendars { get; }
        INotificationRepository Notifications { get; }
        INotificationSettingRepository NotificationSettings { get; }
        IPaymentTypeRepository PaymentTypes { get; }
        IPaymentRepository Payments { get; }
        ITransactionFeeRepository TransactionFees { get; }
        IVoucherTypeRepository VoucherTypes { get; }
        IVoucherRepository Vouchers { get; }
        IWaiverRepository Waivers { get; }
        IContactUsSubjectRepository ContactUsSubjects { get; }
        IContactUsDetailRepository ContactUsDetails { get; }
        INotificationEventRepository NotificationEvents { get; }
        ICancelOrderRequestRepository CancelOrderRequests { get; }
        IOrderPortalContentRepository OrderPortalContents { get; }
        int? CurrentUserId { get; set; }
        int? CurrentInstitutionId { get; }
        IFaqSubjectRepository FaqSubjects { get; }
        IFaqDetailRepository FaqDetails { get; }
        IPLCRepository PLCRepository { get; }
        ICatererAssetTypeRepository CatererAssetType { get; }
        ICatererAssetRepository CatererAssetRepository { get; }
        IAssetComponentRepository AssetComponentRepository { get; }

        #region For token order payment processing
        ITokenPaymentRepository SubmittedPayments { get; }
        ITokenPaymentResponseRepository ProcessedPayments { get; }
        #endregion


    }
}
