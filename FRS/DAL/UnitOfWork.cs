using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Core.Interfaces;
using DAL.Core.Logging;
using DAL.Filters;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Interfaces.MealOrder;
using DAL.Repositories.MealOrder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sieve.Services;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;
        ISieveProcessor _sieveProcessor;
        private ILoggerFactory _loggerFactory;
        private readonly IMapper _mapper;
        IDepartmentRepository _departments;
        IImageRepository _images;
        IBookingRepository _bookings;
        IFacilityRepository _facilities;
        IFacilityTypeRepository _facilityTypes;
        ISignageComponentRepository _signageComponents;
        ISignageCompilationRepository _signageCompilations;
        ISignagePublicationRepository _signagePublications;
        IReservationRepository _reservations;
        IInstitutionRepository _institutions;
        IDeviceRepository _devices;
        ILocationRepository _locations;
        IContactGroupRepository _contactGroups;
        IPlaylistRepository _playlists;
        IMediaRepository _medias;
        IMapRepository _maps;
        IDirectoryListingRepository _directoryListings;
        IDirectoryListingCategoryRepository _directoryListingCategorys;

        IMediaExtensionRepository _mediaExtensions;

        IEmployeeDesignationRepository _employeeDesignations;
        IEmployeeScheduleRepository _employeeSchedules;
        IEmployeeDataRepository _employeeDatas;

        IQueueTableMapRepository _queueTableMaps;

        IEmsRepository _emses;
        IEmsProfileRepository _emsProfiles;
        IEmsScheduleRepository _emsSchedules;

        IOccupancyLogRepository _occupancyLogs;
        IBuildingRepository _buildings;
        IFloorRepository _floors;
        IApplicationSettingRepository _applicationSettings;
        IConnectionRepository _connections;
        IUserConnectionRepository _userConnections;
        IModuleRepository _modules;
        IKioskSettingsRepository _kioskSettings;
        IUserGroupRepository _userGroups;

        private PIBTemplateRepository _pibTemplates;
        private EpaperTemplateRepository _epaperTemplates;
        private IImageReferenceTypeRepository _imageReferenceTypes;
        private IImageReferenceColorRepository _imageReferenceColors;
        private SmartRoomSchedulerLogRepository _smartRoomSchedulerLogs;
        private AssetTypeRepository _assetTypes;
        private AssetModelRepository _assetModels;
        private ServiceContractRepository _serviceContracts;
        private AssetRepository _assets;
        private EmailQueueRepository _emailQueues;
        private EmailTemplateRepository _emailTemplates;
        private WalletRepository _wallets;
        private RewardRepository _rewards;
        private WalletTransactionRepository _walletTransactions;
        private StudentWalletTransactionRepository _studentWalletTransactions;
        private StudentPointTransactionRepository _studentPointTransactions;
        private RewardTransactionRepository _rewardTransactions;
        private DeviceTypeRepository _deviceTypes;
        private ChannelInfoRepository _channelInfos;
        private ClassLevelRepository _classLevels;
        private DispenserOutletRepository _dispenserOutlets;
        private ClassBatchRepository _classBatches;
        private ClassRepository _classes;
        private IStudentRepository _students;
        private IStudentGroupRepository _studentGroups;
        private ITokenOrderRepository _tokenOrders;
        private ITokensOrderHistoryRepository _tokensOrderHistorys;
        private ITokenLabelRepository _tokenLabels;
        private IMealAllocationRepository _mealAllocations;
        private IPackingAllocationRepository _packingAllocations;
        private StaffTypeRepository _staffTypes;
        private StaffRepository _staffs;
        private RestrictionRepository _restrictions;
        private RestrictionTypeRepository _restrictionTypes;
        private DishTypeRepository _dishTypes;
        private MealPlanOrderRepository _mealPlanOrders;
        private CatererInfoRepository _catererInfos;
        private BentoAssetRepository _bentoAssets;
        private CartonAssetRepository _cartonAssets;
        private CartonDisposableBoxRepository _cartonDisposableBoxes;
        private BentoBoxTypeRepository _bentoBoxTypes;
        private CartonTypeRepository _cartonTypes;
        private DeliveryOrderRepository _deliveryOrders;
        private DeliveryOrderNewRepository _deliveryOrderNews;
        private StoreInventoryRepository _storeInventories;
        private TrackingStatusRepository _trackingStatuss;
        private StoreInfoRepository _storeInfos;
        private MealPeriodRepository _mealPeriods;
        private MealTypeRepository _mealTypes;
        private DishRepository _dishes;
        private MenuRepository _menus;
        private MenuCycleRepository _menuCycles;
        private StudentCardRepository _studentCards;
        private OutletRepository _outlets;
        private TokenOrderedRepository _tokenOrdereds;
        private CuisineRepository _cuisines;
        private OutletProfileRepository _outletProfiles;
        private DriverRepository _drivers;
        private InterestGroupRepository _interestGroups;
        private RouteRepository _routes;
        private MealSessionRepository _mealSessions;
        private MealSessionDetailRepository _mealSessionDetails;
        private MenuCycleCalendarRepository _menuCyclesCalendars;
        private OutletClassRosterRepository _outletClassRosters;
        private DishCycleRepository _dishCycles;
        private DishCycleCalendarRepository _dishCycleCalendars;
        private NotificationRepository _notifications;
        private PaymentTypeRepository _paymentTypes;
        private PaymentRepository _payments;
        private TransactionFeeRepository _TransactionFees;
        private VoucherTypeRepository _voucherTypes;
        private VoucherRepository _vouchers;
        private WaiverRepository _waivers;
        private ContactUsSubjectRepository _contactUsSubjects;
        private ContactUsDetailRepository _contactUsDetails;
        private NotificationEventRepository _notificationEvents;
        private CancelOrderRequestRepository _cancelOrderRequests;
        private readonly IConfiguration _configuration;

        private ITokenPaymentRepository _tokenPayments;                 //
        private ITokenPaymentResponseRepository _tokenPaymentResponse;  //
        private MenuGroupRepository _menuGroups;
        private INotificationSettingRepository _notificationSetting;
        private IOrderPortalContentRepository _orderPortalContents;
        private OutletTermRepository _outletTerms;
        private FaqSubjectRepository _faqSubjects;
        private FaqDetailRepository _faqDetails;
        IUserActivityRepository _userActivityRepository;

        public UnitOfWork(IConfiguration configuration, ApplicationDbContext context, ISieveProcessor sieveProcessor, ILoggerFactory loggerFactory, IMapper mapper,IUserActivityRepository userActivityRepository)
        {
            _context = context;
            _configuration = configuration;
            _sieveProcessor = sieveProcessor;
            _loggerFactory = loggerFactory;
            _userActivityRepository = userActivityRepository;
            _mapper = mapper;
            Logger.ConfigureLogger(loggerFactory, configuration);
        }



        public IFacilityRepository Facilities
        {
            get
            {
                if (_facilities == null)
                    _facilities = new FacilityRepository(_context);

                return _facilities;
            }
        }

        public IMediaRepository Medias
        {
            get
            {
                if (_medias == null)
                    _medias = new MediaRepository(_context);

                return _medias;
            }
        }

        public IMapRepository Maps
        {
            get
            {
                if (_maps == null)
                    _maps = new MapRepository(_context);

                return _maps;
            }
        }

        public IDirectoryListingCategoryRepository DirectoryListingCategorys
        {
            get
            {
                if (_directoryListingCategorys == null)
                    _directoryListingCategorys = new DirectoryListingCategoryRepository(_context);

                return _directoryListingCategorys;
            }
        }

        public IMediaExtensionRepository MediaExtensions
        {
            get
            {
                if (_mediaExtensions == null)
                    _mediaExtensions = new MediaExtensionRepository(_context);

                return _mediaExtensions;
            }
        }

        public IEmsRepository Emses
        {
            get
            {
                if (_emses == null)
                    _emses = new EmsRepository(_context);

                return _emses;
            }
        }

        public IEmsScheduleRepository EmsSchedules
        {
            get
            {
                if (_emsSchedules == null)
                    _emsSchedules = new EmsScheduleRepository(_context);

                return _emsSchedules;
            }
        }

        public IEmsProfileRepository EmsProfiles
        {
            get
            {
                if (_emsProfiles == null)
                    _emsProfiles = new EmsProfileRepository(_context);

                return _emsProfiles;
            }
        }

        public IEmployeeDesignationRepository EmployeeDesignations
        {
            get
            {
                if (_employeeDesignations == null)
                    _employeeDesignations = new EmployeeDesignationRepository(_context);

                return _employeeDesignations;
            }
        }

        public IEmployeeScheduleRepository EmployeeSchedules
        {
            get
            {
                if (_employeeSchedules == null)
                    _employeeSchedules = new EmployeeScheduleRepository(_context);

                return _employeeSchedules;
            }
        }

        public IEmployeeDataRepository EmployeeDatas
        {
            get
            {
                if (_employeeDatas == null)
                    _employeeDatas = new EmployeeDataRepository(_context);

                return _employeeDatas;
            }
        }

        public IQueueTableMapRepository QueueTableMaps
        {
            get
            {
                if (_queueTableMaps == null)
                    _queueTableMaps = new QueueTableMapRepository(_context);

                return _queueTableMaps;
            }
        }

        public IOccupancyLogRepository OccupancyLogs
        {
            get
            {
                if (_occupancyLogs == null)
                    _occupancyLogs = new OccupancyLogRepository(_context);

                return _occupancyLogs;
            }
        }

        public IDirectoryListingRepository DirectoryListings
        {
            get
            {
                if (_directoryListings == null)
                    _directoryListings = new DirectoryListingRepository(_context);

                return _directoryListings;
            }
        }

        public IBuildingRepository Buildings
        {
            get
            {
                if (_buildings == null)
                    _buildings = new BuildingRepository(_context);

                return _buildings;
            }
        }

        public IFloorRepository Floors
        {
            get
            {
                if (_floors == null)
                    _floors = new FloorRepository(_context);

                return _floors;
            }
        }

        public IApplicationSettingRepository ApplicationSettings
        {
            get
            {
                if (_applicationSettings == null)
                    _applicationSettings = new ApplicationSettingRepository(_context);

                return _applicationSettings;
            }
        }

        public IFacilityTypeRepository FacilityTypes
        {
            get
            {
                if (_facilityTypes == null)
                    _facilityTypes = new FacilityTypeRepository(_context);

                return _facilityTypes;
            }
        }

        public ISignageComponentRepository SignageComponents
        {
            get
            {
                if (_signageComponents == null)
                    _signageComponents = new SignageComponentRepository(_context);

                return _signageComponents;
            }
        }

        public ISignageCompilationRepository SignageCompilations
        {
            get
            {
                if (_signageCompilations == null)
                    _signageCompilations = new SignageCompilationRepository(_context);

                return _signageCompilations;
            }
        }

        public ISignagePublicationRepository SignagePublications
        {
            get
            {
                if (_signagePublications == null)
                    _signagePublications = new SignagePublicationRepository(_context);

                return _signagePublications;
            }
        }

        public IReservationRepository Reservations
        {
            get
            {
                if (_reservations == null)
                    _reservations = new ReservationRepository(_context, _mapper);

                return _reservations;
            }
        }

        public IInstitutionRepository Institutions
        {
            get
            {
                if (_institutions == null)
                    _institutions = new InstitutionRepository(_context);

                return _institutions;
            }
        }

        public IDeviceRepository Devices
        {
            get
            {
                if (_devices == null)
                    _devices = new DeviceRepository(_context, _sieveProcessor);

                return _devices;
            }
        }

        public ILocationRepository Locations
        {
            get
            {
                if (_locations == null)
                    _locations = new LocationRepository(_context);

                return _locations;
            }
        }

        public IBookingRepository Bookings
        {
            get
            {
                if (_bookings == null)
                    _bookings = new BookingRepository(_context);

                return _bookings;
            }
        }

        public IDepartmentRepository Departments
        {
            get
            {
                if (_departments == null)
                    _departments = new DepartmentRepository(_context, _sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _departments;
            }
        }

        public IImageRepository Images
        {
            get
            {
                if (_images == null)
                    _images = new ImageRepository(_context);

                return _images;
            }
        }

        public IContactGroupRepository ContactGroups
        {
            get
            {
                if (_contactGroups == null)
                    _contactGroups = new ContactGroupRepository(_configuration, _context);

                return _contactGroups;
            }
        }

        public IPlaylistRepository Playlists
        {
            get
            {
                if (_playlists == null)
                    _playlists = new PlaylistRepository(_configuration, _context);

                return _playlists;
            }
        }

        public IPIBTemplateRepository PIBTemplates
        {
            get
            {
                if (_pibTemplates == null)
                    _pibTemplates = new PIBTemplateRepository(_context);

                return _pibTemplates;
            }
        }

        public IEpaperTemplateRepository EpaperTemplates
        {
            get
            {
                if (_epaperTemplates == null)
                    _epaperTemplates = new EpaperTemplateRepository(_context);

                return _epaperTemplates;
            }
        }

        public IConnectionRepository Connections
        {
            get
            {
                if (_connections == null)
                    _connections = new ConnectionRepository(_context);

                return _connections;
            }
        }

        public INotificationRepository Notifications
        {
            get
            {
                if (_notifications == null)
                    _notifications = new NotificationRepository(_context, _sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _notifications;
            }
        }

        public INotificationEventRepository NotificationEvents
        {
            get
            {
                if (_notificationEvents == null)
                    _notificationEvents = new NotificationEventRepository(_context, _sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _notificationEvents;
            }
        }

        public IUserConnectionRepository UserConnections
        {
            get
            {
                if (_userConnections == null)
                    _userConnections = new UserConnectionRepository(_context);

                return _userConnections;
            }
        }

        public IModuleRepository Modules
        {
            get
            {
                if (_modules == null)
                    _modules = new ModuleRepository(_context);

                return _modules;
            }
        }


        public IKioskSettingsRepository KioskSettings
        {
            get
            {
                if (_kioskSettings == null)
                    _kioskSettings = new KioskSettingsRepository(_configuration, _context);

                return _kioskSettings;
            }
        }

        public IUserGroupRepository UserGroups
        {
            get
            {
                if (_userGroups == null)
                    _userGroups = new UserGroupRepository(_context, _sieveProcessor);

                return _userGroups;
            }
        }

        public int? CurrentUserId { get; set; }

        public int? CurrentInstitutionId
        {
            get
            {
                var currentUser = _context.Users.FirstOrDefault(e => e.Id == CurrentUserId);
                return currentUser != null ? currentUser.InstitutionId : null;
            }
        }

        public IImageReferenceTypeRepository ImageReferenceTypes
        {
            get
            {
                if (_imageReferenceTypes == null)
                    _imageReferenceTypes = new ImageReferenceTypeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _imageReferenceTypes;
            }
        }

        public IImageReferenceColorRepository ImageReferenceColors
        {
            get
            {
                if (_imageReferenceColors == null)
                    _imageReferenceColors = new ImageReferenceColorRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _imageReferenceColors;
            }
        }


        public ISmartRoomSchedulerLogRepository SmartRoomSchedulerLogs
        {
            get
            {
                if (_smartRoomSchedulerLogs == null)
                    _smartRoomSchedulerLogs = new SmartRoomSchedulerLogRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _smartRoomSchedulerLogs;
            }
        }

        public IAssetTypeRepository AssetTypes
        {
            get
            {
                if (_assetTypes == null)
                    _assetTypes = new AssetTypeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _assetTypes;
            }
        }

        public IAssetModelRepository AssetModels
        {
            get
            {
                if (_assetModels == null)
                    _assetModels = new AssetModelRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _assetModels;
            }
        }

        public IAssetRepository Assets
        {
            get
            {
                if (_assets == null)
                    _assets = new AssetRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _assets;
            }
        }

        public IServiceContractRepository ServiceContracts
        {
            get
            {
                if (_serviceContracts == null)
                    _serviceContracts = new ServiceContractRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _serviceContracts;
            }
        }

        public IEmailQueueRepository EmailQueues
        {
            get
            {
                if (_emailQueues == null)
                    _emailQueues = new EmailQueueRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _emailQueues;
            }
        }

        public IEmailTemplateRepository EmailTemplates
        {
            get
            {
                if (_emailTemplates == null)
                    _emailTemplates = new EmailTemplateRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _emailTemplates;
            }
        }

        public IWalletRepository Wallets
        {
            get
            {
                if (_wallets == null)
                    _wallets = new WalletRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _wallets;
            }
        }

        public IWalletTransactionRepository WalletTransactions
        {
            get
            {
                if (_walletTransactions == null)
                    _walletTransactions = new WalletTransactionRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _walletTransactions;
            }
        }

        public IStudentWalletTransactionRepository StudentWalletTransactions
        {
            get
            {
                if (_studentWalletTransactions == null)
                    _studentWalletTransactions = new StudentWalletTransactionRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _studentWalletTransactions;
            }
        }

        public IStudentPointTransactionRepository StudentPointTransactions
        {
            get
            {
                if (_studentPointTransactions == null)
                    _studentPointTransactions = new StudentPointTransactionRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _studentPointTransactions;
            }
        }

        public IRewardRepository Rewards
        {
            get
            {
                if (_rewards == null)
                    _rewards = new RewardRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _rewards;
            }
        }

        public IRewardTransactionRepository RewardTransactions
        {
            get
            {
                if (_rewardTransactions == null)
                    _rewardTransactions = new RewardTransactionRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _rewardTransactions;
            }
        }

        public IDeviceTypeRepository DeviceTypes
        {
            get
            {
                if (_deviceTypes == null)
                    _deviceTypes = new DeviceTypeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _deviceTypes;
            }
        }

        public IChannelInfoRepository ChannelInfos
        {
            get
            {
                if (_channelInfos == null)
                    _channelInfos = new ChannelInfoRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _channelInfos;
            }
        }


        #region Meal Order
        public IWaiverRepository Waivers
        {
            get
            {
                if (_waivers == null)
                    _waivers = new WaiverRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _waivers;
            }
        }

        public IVoucherTypeRepository VoucherTypes
        {
            get
            {
                if (_voucherTypes == null)
                    _voucherTypes = new VoucherTypeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _voucherTypes;
            }
        }

        public IVoucherRepository Vouchers
        {
            get
            {
                if (_vouchers == null)
                    _vouchers = new VoucherRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId,_userActivityRepository);

                return _vouchers;
            }
        }

        public IPaymentTypeRepository PaymentTypes
        {
            get
            {
                if (_paymentTypes == null)
                    _paymentTypes = new PaymentTypeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _paymentTypes;
            }
        }

        public IPaymentRepository Payments
        {
            get
            {
                if (_payments == null)
                    _payments = new PaymentRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _payments;
            }
        }

        public ITransactionFeeRepository TransactionFees
        {
            get
            {
                if (_TransactionFees == null)
                    _TransactionFees = new TransactionFeeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _TransactionFees;
            }
        }

        public IClassBatchRepository ClassBatches
        {
            get
            {
                if (_classBatches == null)
                    _classBatches = new ClassBatchRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _classBatches;
            }
        }

        public IClassLevelRepository ClassLevels
        {
            get
            {
                if (_classLevels == null)
                    _classLevels = new ClassLevelRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _classLevels;
            }
        }

        public IDispenserOutletRepository DispenserOutlets
        {
            get
            {
                if (_dispenserOutlets == null)
                    _dispenserOutlets = new DispenserOutletRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _dispenserOutlets;
            }
        }

        public IClassRepository Classes
        {
            get
            {
                if (_classes == null)
                    _classes = new ClassRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _classes;
            }
        }

        public IStudentRepository Students
        {
            get
            {
                if (_students == null)
                    _students = new StudentRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId,_configuration,_userActivityRepository);

                return _students;
            }
        }

        public IStudentGroupRepository StudentGroups
        {
            get
            {
                if (_studentGroups == null)
                    _studentGroups = new StudentGroupRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _studentGroups;
            }
        }

        public IStudentCardRepository StudentCards
        {
            get
            {
                if (_studentCards == null)
                    _studentCards = new StudentCardRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId,_userActivityRepository);

                return _studentCards;
            }
        }

        public ITokenOrderRepository TokenOrders
        {
            get
            {
                if (_tokenOrders == null)
                    _tokenOrders = new TokenOrderRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId,_userActivityRepository);

                return _tokenOrders;
            }
        }

        public ITokensOrderHistoryRepository TokensOrderHistorys
        {
            get
            {
                if (_tokensOrderHistorys == null)
                    _tokensOrderHistorys = new TokensOrderHistoryRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _tokensOrderHistorys;
            }
        }

        public ITokenLabelRepository TokenLabels
        {
            get
            {
                if (_tokenLabels == null)
                    _tokenLabels = new TokenLabelRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _tokenLabels;
            }
        }

        public IMealAllocationRepository MealAllocations
        {
            get
            {
                if (_mealAllocations == null)
                    _mealAllocations = new MealAllocationRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _mealAllocations;
            }
        }

        public IPackingAllocationRepository PackingAllocations
        {
            get
            {
                if (_packingAllocations == null)
                    _packingAllocations = new PackingAllocationRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _packingAllocations;
            }
        }

        public ITokenOrderedRepository TokenOrdereds
        {
            get
            {
                if (_tokenOrdereds == null)
                    _tokenOrdereds = new TokenOrderedRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _tokenOrdereds;
            }
        }

        public IStaffTypeRepository StaffTypes
        {
            get
            {
                if (_staffTypes == null)
                    _staffTypes = new StaffTypeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _staffTypes;
            }
        }

        public IStaffRepository Staffs
        {
            get
            {
                if (_staffs == null)
                    _staffs = new StaffRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _staffs;
            }
        }

        public IRestrictionTypeRepository RestrictionTypes
        {
            get
            {
                if (_restrictionTypes == null)
                    _restrictionTypes = new RestrictionTypeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _restrictionTypes;
            }
        }

        public IRestrictionRepository Restrictions
        {
            get
            {
                if (_restrictions == null)
                    _restrictions = new RestrictionRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _restrictions;
            }
        }

        public IDishTypeRepository DishTypes
        {
            get
            {
                if (_dishTypes == null)
                    _dishTypes = new DishTypeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _dishTypes;
            }
        }

        public IMealPlanOrderRepository MealPlanOrders
        {
            get
            {
                if (_mealPlanOrders == null)
                    _mealPlanOrders = new MealPlanOrderRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _mealPlanOrders;
            }
        }


        public ICatererInfoRepository CatererInfos
        {
            get
            {
                if (_catererInfos == null)
                    _catererInfos = new CatererInfoRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _catererInfos;
            }
        }

        public IOutletRepository Outlets
        {
            get
            {
                if (_outlets == null)
                    _outlets = new OutletRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _outlets;
            }
        }

        public IOutletTermRepository OutletTerms
        {
            get
            {
                if (_outletTerms == null)
                    _outletTerms = new OutletTermRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _outletTerms;
            }
        }

        public IBentoAssetRepository BentoAssets
        {
            get
            {
                if (_bentoAssets == null)
                    _bentoAssets = new BentoAssetRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _bentoAssets;
            }
        }

        public ICartonAssetRepository CartonAssets
        {
            get
            {
                if (_cartonAssets == null)
                    _cartonAssets = new CartonAssetRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _cartonAssets;
            }
        }

        public ICartonDisposableBoxRepository CartonDisposableBoxes
        {
            get
            {
                if (_cartonDisposableBoxes == null)
                    _cartonDisposableBoxes = new CartonDisposableBoxRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _cartonDisposableBoxes;
            }
        }

        public IBentoBoxTypeRepository BentoBoxTypes
        {
            get
            {
                if (_bentoBoxTypes == null)
                    _bentoBoxTypes = new BentoBoxTypeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _bentoBoxTypes;
            }
        }

        public ICartonTypeRepository CartonTypes
        {
            get
            {
                if (_cartonTypes == null)
                    _cartonTypes = new CartonTypeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _cartonTypes;
            }
        }

        public IDeliveryOrderRepository DeliveryOrders
        {
            get
            {
                if (_deliveryOrders == null)
                    _deliveryOrders = new DeliveryOrderRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _deliveryOrders;
            }
        }

        public IDeliveryOrderNewRepository DeliveryOrderNews
        {
            get
            {
                if (_deliveryOrderNews == null)
                    _deliveryOrderNews = new DeliveryOrderNewRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _deliveryOrderNews;
            }
        }

        public IStoreInventoryRepository StoreInventories
        {
            get
            {
                if (_storeInventories == null)
                    _storeInventories = new StoreInventoryRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _storeInventories;
            }
        }

        public ITrackingStatusRepository TrackingStatuss
        {
            get
            {
                if (_trackingStatuss == null)
                    _trackingStatuss = new TrackingStatusRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _trackingStatuss;
            }
        }

        public IStoreInfoRepository StoreInfos
        {
            get
            {
                if (_storeInfos == null)
                    _storeInfos = new StoreInfoRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _storeInfos;
            }
        }

        public IMealPeriodRepository MealPeriods
        {
            get
            {
                if (_mealPeriods == null)
                    _mealPeriods = new MealPeriodRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _mealPeriods;
            }
        }

        public IMealSessionRepository MealSessions
        {
            get
            {
                if (_mealSessions == null)
                    _mealSessions = new MealSessionRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _mealSessions;
            }
        }

        public IMealSessionDetailRepository MealSessionDetails
        {
            get
            {
                if (_mealSessionDetails == null)
                    _mealSessionDetails = new MealSessionDetailRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _mealSessionDetails;
            }
        }

        public IMealTypeRepository MealTypes
        {
            get
            {
                if (_mealTypes == null)
                    _mealTypes = new MealTypeRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _mealTypes;
            }
        }

        public IDishRepository Dishes
        {
            get
            {
                if (_dishes == null)
                    _dishes = new DishRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _dishes;
            }
        }

        public IMenuRepository Menus
        {
            get
            {
                if (_menus == null)
                    _menus = new MenuRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _menus;
            }
        }

        public IMenuCycleRepository MenuCycles
        {
            get
            {
                if (_menuCycles == null)
                    _menuCycles = new MenuCycleRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _menuCycles;
            }
        }

        public ICuisineRepository Cuisines
        {
            get
            {
                if (_cuisines == null)
                    _cuisines = new CuisineRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _cuisines;
            }
        }

        public IOutletProfileRepository OutletProfiles
        {
            get
            {
                if (_outletProfiles == null)
                    _outletProfiles = new OutletProfileRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _outletProfiles;
            }
        }

        public IDriverRepository Drivers
        {
            get
            {
                if (_drivers == null)
                    _drivers = new DriverRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);


                return _drivers;
            }
        }
        
        public IInterestGroupRepository InterestGroups
        {
            get
            {
                if (_interestGroups == null)
                    _interestGroups = new InterestGroupRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _interestGroups;
            }
        }

        public IRouteRepository Routes
        {
            get
            {
                if (_routes == null)
                    _routes = new RouteRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _routes;
            }
        }

        public IMenuCycleCalendarRepository MenuCycleCalendars
        {
            get
            {
                if (_menuCyclesCalendars == null)
                    _menuCyclesCalendars = new MenuCycleCalendarRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _menuCyclesCalendars;
            }
        }

        public IOutletClassRosterRepository OutletClassRosters
        {
            get
            {
                if (_outletClassRosters == null)
                    _outletClassRosters = new OutletClassRosterRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _outletClassRosters;
            }
        }

        public IDishCycleRepository DishCycles
        {
            get
            {
                if (_dishCycles == null)
                    _dishCycles = new DishCycleRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _dishCycles;
            }
        }

        public IDishCycleCalendarRepository DishCycleCalendars
        {
            get
            {
                if (_dishCycleCalendars == null)
                    _dishCycleCalendars = new DishCycleCalendarRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _dishCycleCalendars;
            }
        }

        public IContactUsSubjectRepository ContactUsSubjects
        {
            get
            {
                if (_contactUsSubjects == null)
                    _contactUsSubjects = new ContactUsSubjectRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _contactUsSubjects;
            }
        }

        public IContactUsDetailRepository ContactUsDetails
        {
            get
            {
                if (_contactUsDetails == null)
                    _contactUsDetails = new ContactUsDetailRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _contactUsDetails;
            }
        }

        public ICancelOrderRequestRepository CancelOrderRequests
        {
            get
            {
                if (_cancelOrderRequests == null)
                    _cancelOrderRequests = new CancelOrderRequestRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _cancelOrderRequests;
            }
        }

        public IMenuGroupRepository MenuGroups
        {
            get
            {
                if (_menuGroups == null)
                    _menuGroups = new MenuGroupRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _menuGroups;
            }
        }

        public IFaqSubjectRepository FaqSubjects
        {
            get
            {
                if (_faqSubjects == null)
                    _faqSubjects = new FaqSubjectRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _faqSubjects;
            }
        }

        public IFaqDetailRepository FaqDetails
        {
            get
            {
                if (_faqDetails == null)
                    _faqDetails = new FaqDetailRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _faqDetails;
            }
        }
        #endregion

        #region TokenPayments section
        public ITokenPaymentRepository SubmittedPayments
        {
            get
            {
                if (_tokenPayments == null)
                    _tokenPayments = new TokenPaymentRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _tokenPayments;
            }
        }


        public ITokenPaymentResponseRepository ProcessedPayments
        {
            get
            {
                if (_tokenPaymentResponse == null)
                    _tokenPaymentResponse = new TokenPaymentResponseRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _tokenPaymentResponse;
            }
        }

        public INotificationSettingRepository NotificationSettings
        {
            get
            {
                if (_notificationSetting == null)
                    _notificationSetting = new NotificationSettingRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _notificationSetting;
            }
        }

        public IOrderPortalContentRepository OrderPortalContents
        {
            get
            {
                if (_orderPortalContents == null)
                    _orderPortalContents = new OrderPortalContentRepository(_context, this._sieveProcessor, CurrentUserId, CurrentInstitutionId);

                return _orderPortalContents;
            }
        }




        #endregion




    }
}
