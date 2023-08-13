using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static DAL.Core.Helpers.TreeExtensions;

using SMV.FOMOPay.Model;
using DAL.Models.StoredProcedures;

namespace BAL.Mapping
{
    public class EntityToDTOAutoMapperProfile : Profile
    {
        public EntityToDTOAutoMapperProfile()
        {
            CreateMap(typeof(PagedEntity<>), typeof(PagedEntityDTO<>));

            CreateMap<ExternalAppLoginLog, ExternalAppLoginLogDTO>();
            CreateMap<ExternalAppLoginLogDTO, ExternalAppLoginLog>();

            CreateMap<AuthenticationLog, AuthenticationLogDTO>();
            CreateMap<AuthenticationLogDTO, AuthenticationLog>();

            CreateMap<UpDownTimeLog, UpDownTimeLogDTO>();
            CreateMap<UpDownTimeLogDTO, UpDownTimeLog>();

            CreateMap<AuditLog, AuditLogDTO>()
                .ForMember(e => e.LogType, map => map.MapFrom(e => Enum.GetName(typeof(AuditLogType), e.LogType)))
                .ForMember(e => e.UserName, map => map.MapFrom(e => string.IsNullOrEmpty(e.UserName) ? "system" : e.UserName));

            CreateMap<AuditLogDetail, AuditLogDetailDTO>();

            CreateMap<ImageReferenceType, ImageReferenceTypeDTO>();
            CreateMap<ImageReferenceTypeDTO, ImageReferenceType>();

            CreateMap<ImageReferenceColor, ImageReferenceColorDTO>();
            CreateMap<ImageReferenceColorDTO, ImageReferenceColor>();

            CreateMap<AssetType, AssetTypeDTO>()
                .ForMember(d => d.FilePath, map => map.MapFrom(s => s.Icon.Path));
            CreateMap<AssetTypeDTO, AssetType>()
                .ForMember(d => d.Icon, map => map.MapFrom(s => new File { Path = s.FilePath, FileName = s.FileName, Type = FileType.Icon.ToString() }));

            CreateMap<AssetModel, AssetModelDTO>()
                .ForMember(e => e.AssetTypeName, map => map.MapFrom(e => e.AssetType.Name))
                .ForMember(e => e.InstitutionName, map => map.MapFrom(e => e.Institution.Name))
                .ForMember(d => d.FilePath, map => map.MapFrom(s => s.Photo.Path));

            CreateMap<AssetModelDTO, AssetModel>()
                .ForMember(d => d.Photo, map => map.MapFrom(s => new File { Path = s.FilePath, FileName = s.FileName, Type = FileType.JPG.ToString() }));

            CreateMap<ServiceContract, ServiceContractDTO>()
                .ForMember(d => d.ServiceContractAssets, map => map.MapFrom(f => f.ServiceContractAssets.Where(e => (e.Asset == null || (e.Asset != null && e.Asset.IsActive)) && e.IsActive)));

            CreateMap<ServiceContractDTO, ServiceContract>();

            CreateMap<ServiceContractAsset, ServiceContractAssetDTO>()
                .ForMember(d => d.AssetModelName, map => map.MapFrom(s => s.Asset.AssetModel.Name))
                .ForMember(d => d.AssetTypeName, map => map.MapFrom(s => s.Asset.AssetModel.AssetType.Name))
                .ForMember(d => d.SerialNumber, map => map.MapFrom(s => s.Asset.SerialNumber))
                .ForMember(d => d.AssetModelId, map => map.MapFrom(s => s.Asset.AssetModelId))
                .ForMember(d => d.PurchaseDate, map => map.MapFrom(s => s.Asset.PurchaseDate))
                .ForMember(d => d.WarrantyStart, map => map.MapFrom(s => s.Asset.WarrantyEnd))
                .ForMember(d => d.WarrantyEnd, map => map.MapFrom(s => s.Asset.WarrantyEnd));

            CreateMap<ServiceContractAssetDTO, ServiceContractAsset>();

            CreateMap<Asset, AssetDTO>()
                .ForMember(e => e.AssetModelName, map => map.MapFrom(e => e.AssetModel.Name))
                .ForMember(e => e.AssetTypeName, map => map.MapFrom(e => e.AssetModel.AssetType.Name))
                .ForMember(e => e.LocationName, map => map.MapFrom(e => e.Location.Name))
                .ForMember(e => e.InstitutionName, map => map.MapFrom(e => e.Institution.Name));

            CreateMap<AssetDTO, Asset>();


            CreateMap<ApplicationSetting, ApplicationSettingDTO>();

            CreateMap<EmailQueue, EmailQueueDTO>();
            CreateMap<EmailQueueDTO, EmailQueue>();

            CreateMap<Connection, ConnectionDTO>();

            CreateMap<Wallet, WalletDTO>()
                .ForMember(e => e.UserName, map => map.MapFrom(e => e.User.FriendlyName));
            CreateMap<WalletDTO, Wallet>();


            CreateMap<WalletTransaction, WalletTransactionDTO>()
                .ForMember(e => e.TransactionDateTime, map => map.MapFrom(e => e.CreatedDate));
            CreateMap<WalletTransactionDTO, WalletTransaction>();

            CreateMap<Reward, RewardDTO>()
                .ForMember(e => e.UserName, map => map.MapFrom(e => e.User.FriendlyName));
            CreateMap<RewardDTO, Reward>();

            CreateMap<RewardTransaction, RewardTransactionDTO>()
                .ForMember(e => e.TransactionDateTime, map => map.MapFrom(e => e.CreatedDate));
            CreateMap<RewardTransactionDTO, RewardTransaction>();

            CreateMap<DeviceType, DeviceTypeDTO>();
            CreateMap<DeviceTypeDTO, DeviceType>();

            CreateMap<ChannelInfo, ChannelInfoDTO>();
            CreateMap<ChannelInfoDTO, ChannelInfo>();

            CreateMap<ITree<ApplicationPermissionsTree>, ApplicationPermissionsTreeDTO>()
                .ForMember(d => d.Name, map => map.MapFrom(f => f.Data.Name))
                .ForMember(d => d.Description, map => map.MapFrom(f => f.Data.Description))
                .ForMember(d => d.Value, map => map.MapFrom(f => f.Data.Value));


            // For Token payments
            CreateMap<TokenPaymentRequestDTO, TokenPaymentRequest>();
            CreateMap<TokenPaymentResponseDTO, TokenPaymentResponse>();
            //


            MapMealOrderEntityProfile();
        }

        private void MapMealOrderEntityProfile()
        {
            CreateMap<ClassBatchDTO, ClassBatch>();
            CreateMap<ClassLevelDTO, ClassLevel>();
            CreateMap<ClassLevel, ClassLevelDTO>()
                .ForMember(e => e.OutletName, map => map.MapFrom(f => f.Outlet.Name));

            CreateMap<ClassDTO, Class>();
            CreateMap<StudentDTO, Student>();
            CreateMap<StudentDTO, ApplicationUser>()
                .ForMember(e => e.Id, map => map.MapFrom(e => e.UserId))
                .ForMember(e => e.UserType, map => map.MapFrom(e => e.UserType));

            CreateMap<Student, StudentDTO>()
                .ForMember(e => e.ClassLevelId, map => map.MapFrom(e => e.Class.ClassLevelId))
                .ForMember(e => e.ClassLevelName, map => map.MapFrom(e => e.Class.ClassLevel.Name))
                .ForMember(e => e.ClassBatchName, map => map.MapFrom(e => e.ClassBatch.Name))
                .ForMember(e => e.Year, map => map.MapFrom(e => e.ClassBatch.Year))
                .ForMember(e => e.ClassName, map => map.MapFrom(e => e.Class.Name))
                .ForMember(e => e.UserType, map => map.MapFrom(e => e.Account.User.UserType))
                .ForMember(e => e.UserId, map => map.MapFrom(e => e.Account.User.Id))
                .ForMember(e => e.StudentId, map => map.MapFrom(e => e.Id))
                .ForMember(e => e.Username, map => map.MapFrom(e => e.Account.User.UserName))
                //.ForMember(e => e.CurrentPassword, map => map.MapFrom(e => e.Account.User.))
                .ForMember(e => e.Email, map => map.MapFrom(e => e.Account.User.Email))
                .ForMember(e => e.FullName, map => map.MapFrom(e => e.Account.User.FullName))
                .ForMember(e => e.OutletName, map => map.MapFrom(e => e.Outlet.Name))
                .ForMember(e => e.Cards, map => map.MapFrom(e => e.Account != null ? e.Account.User.UserCardIds.Where(f => f.IsActive).ToList() : null))
                .ForMember(d => d.Users, map => map.MapFrom(s => s.Users.Where(f => f.IsActive)))
                .ForMember(e => e.StudentCards, map => map.MapFrom(e => e.StudentCards.Where(f => f.IsActive).ToList()))
                .ForMember(e => e.Restrictions, map => map.MapFrom(e => e.Restrictions.Where(f => f.IsActive).ToList()))
                .ForMember(e => e.InterestGroups, map => map.MapFrom(e => e.InterestGroups.Where(f => f.IsActive).ToList()))
                .ForMember(e => e.Vouchers, map => map.MapFrom(e => e.Vouchers.Where(f => f.IsActive).ToList()));

            CreateMap<StudentManageAccount, StudentManageAccountDTO>()
                .ForMember(d => d.Name, map => map.MapFrom(s => s.Student != null ? s.Student.Name : string.Empty))
                .ForMember(d => d.UserName, map => map.MapFrom(s => s.User != null ? s.User.UserName : string.Empty))
                .ForMember(d => d.FullName, map => map.MapFrom(s => s.User != null ? s.User.FullName: string.Empty))
                .ForMember(d => d.Email, map => map.MapFrom(s => s.User != null ? s.User.Email : string.Empty))
                ;
            CreateMap<StudentManageAccountDTO, StudentManageAccount>();

            CreateMap<UserCardIdDTO, UserCardId>();
            CreateMap<StudentCardDTO, StudentCard>();
            CreateMap<StudentRestriction, StudentRestrictionDTO>()
                .ForMember(d => d.RestrictionCode, map => map.MapFrom(s => s.Restriction != null ? s.Restriction.Code : string.Empty))
                .ForMember(d => d.RestrictionTypeCode, map => map.MapFrom(s => s.Restriction != null && s.Restriction.RestrictionType != null ? s.Restriction.RestrictionType.Code : string.Empty));

            CreateMap<StudentRestrictionDTO, StudentRestriction>();

            CreateMap<StudentVoucher, StudentVoucherDTO>()
                .ForMember(d => d.VoucherCode, map => map.MapFrom(s => s.Voucher != null ? s.Voucher.Code : string.Empty));

            CreateMap<StudentVoucherDTO, StudentVoucher>();

            CreateMap<StudentInterestGroupDTO, StudentInterestGroup>();

            CreateMap<InterestGroup, InterestGroupDTO>();
            CreateMap<InterestGroupDTO, InterestGroup>();

            CreateMap<TokenOrderDTO, TokenOrder>();

            CreateMap<TokenOrder, TokenOrderDTO>()
                .ForMember(e => e.Tokens, map => map.MapFrom(e => e.Tokens.Where(f => f.IsActive).ToList()))
                .ForMember(e => e.MealSessionName, map => map.MapFrom(e => e.Session != null ? e.Session.Name : String.Empty))
                .ForMember(e => e.MealSessionDetailName, map => map.MapFrom(e => e.Session != null ? e.Session.MealSessionName : String.Empty))
                .ForMember(e => e.MealPeriodId, map => map.MapFrom(e => e.Session != null && e.Session.MealSession != null ? e.Session.MealSession.MealPeriodId : null))
                .ForMember(e => e.MealPeriodName, map => map.MapFrom(e => e.Session != null && e.Session.MealSession != null && e.Session.MealSession.MealPeriod != null ? e.Session.MealSession.MealPeriod.Name : null))
                .ForMember(e => e.MealSessionStartDate, map => map.MapFrom(e => e.Session.StartDate))
                .ForMember(e => e.ProfileName, map => map.MapFrom(e => e.Student.Name))
                .ForMember(e => e.PeriodName, map => map.MapFrom(e => e.Period.Name))
                .ForMember(e => e.PaymentTypeName, map => map.MapFrom(e => e.IsFAS ? "FAS" : e.Payment.PaymentType.Name))
                .ForMember(e => e.PaymentNumber, map => map.MapFrom(e => e.Payment != null ? e.Payment.PaymentNumber : string.Empty))
                .ForMember(e => e.InvoiceNumber, map => map.MapFrom(e => e.Payment != null ? e.Payment.InvoiceNumber : string.Empty))
                .ForMember(e => e.VoucherCode, map => map.MapFrom(e => e.Payment != null ? e.Payment.Voucher.Code : string.Empty))
                .ForMember(e => e.Discount, map => map.MapFrom(e => e.Payment != null ? e.Payment.discount : 0))
                .ForMember(e => e.PaymentSubtotal, map => map.MapFrom(e => e.Payment != null ? e.Payment.subtotal : 0))
                .ForMember(e => e.PaymentGst, map => map.MapFrom(e => e.Payment != null ? e.Payment.gst : 0))
                .ForMember(e => e.PaymentTransactionFee, map => map.MapFrom(e => e.Payment != null ? e.Payment.transactionFee : 0))
                .ForMember(e => e.PaymentFixedTransactionFee, map => map.MapFrom(e => e.Payment != null ? e.Payment.fixedTransactionFee : 0))
                .ForMember(e => e.PaymentTotalAmount, map => map.MapFrom(e => e.Payment != null ? e.Payment.total : 0))
                .ForMember(e => e.FomoId, map => map.MapFrom(e => e.Payment != null ? e.Payment.fomoid : string.Empty))
                .ForMember(e => e.ProcessedBy, map => map.MapFrom(e => e.CreatedByUser.UserName))
                .ForMember(e => e.ClassName, map => map.MapFrom(e => e.Student != null && e.Student.Class != null ? e.Student.Class.Name : String.Empty))
                .ForMember(e => e.PaymentStatus, map => map.MapFrom(e => e.Payment.Status))
                .ForMember(e => e.StudentName, map => map.MapFrom(e => e.Student != null && e.Student.Name != null ? e.Student.Name : String.Empty))
                .ForMember(e => e.StudentEmail, map => map.MapFrom(e => e.Student != null && e.Student.Email != null ? e.Student.Email : String.Empty))
                .ForMember(e => e.IsFASDisplay, map => map.MapFrom(e => e.IsFAS ? "Y" : "N"))
                ;

            CreateMap<spSalesOrderReport, TokenOrderDTO>()
                .ForMember(e => e.VoucherCode, map => map.MapFrom(e => e.DiscountCode))
                .ForMember(e => e.ProfileName, map => map.MapFrom(e => e.StudentName))
                .ForMember(e => e.IsFASDisplay, map => map.MapFrom(e => e.IsFAS ? "Y" : "N"))
                .ForMember(e => e.MealSessionName, map => map.MapFrom(e => e.MealSessionDetailName))
                .ForMember(e => e.MealDescription, map => map.MapFrom(e => e.DishLabel))
                .ForMember(e => e.TransactionTime, map => map.MapFrom(e => e.OrderDate))
                .ForMember(e => e.TotalAmount, map => map.MapFrom(e => e.DishPrice));
                //.ForMember(e => e.ProcessedBy, map => map.MapFrom(e => e.ProcessedBy));

            CreateMap<TokensOrderHistoryDTO, TokensOrderHistory>();
            CreateMap<TokensOrderHistory, TokensOrderHistoryDTO>()
                .ForMember(e => e.MealSessionName, map => map.MapFrom(e => e.Session != null ? e.Session.Name : String.Empty))
                .ForMember(e => e.MealSessionDetailName, map => map.MapFrom(e => e.Session != null ? e.Session.MealSessionName : String.Empty))
                .ForMember(e => e.MealPeriodId, map => map.MapFrom(e => e.Session != null && e.Session.MealSession != null ? e.Session.MealSession.MealPeriodId : null))
                .ForMember(e => e.MealSessionStartDate, map => map.MapFrom(e => e.Session.StartDate))
                .ForMember(e => e.ProfileName, map => map.MapFrom(e => e.Student.Name))
                .ForMember(e => e.PeriodName, map => map.MapFrom(e => e.Period.Name))
                .ForMember(e => e.PaymentTypeName, map => map.MapFrom(e => e.Payment.PaymentType.Name))
                .ForMember(e => e.ProcessedBy, map => map.MapFrom(e => e.CreatedByUser.UserName))
                .ForMember(e => e.ClassName, map => map.MapFrom(e => e.Student != null && e.Student.Class != null ? e.Student.Class.Name : String.Empty))
                .ForMember(e => e.PaymentStatus, map => map.MapFrom(e => e.Payment.Status))
                .ForMember(e => e.DishLabel, map => map.MapFrom(e => e.Dish.Label))
                ;

            CreateMap<TokensOrderHistoryDTO, TokenOrder>();
            CreateMap<TokenOrder, TokensOrderHistoryDTO>();

            CreateMap<TokenOrderedDTO, TokenOrdered>();
            CreateMap<TokenOrdered, TokenOrderedDTO>()
                .ForMember(e => e.TokenName, map => map.MapFrom(e => e.Token != null ? e.Token.Name : String.Empty));



            CreateMap<TokenOrderDishDTO, TokenOrderDish>();
            CreateMap<TokenOrderDish, TokenOrderDishDTO>()
                .ForMember(e => e.DishLabel, map => map.MapFrom(e => e.Dish != null ? e.Dish.Label : String.Empty))
                .ForMember(e => e.DishTypeName, map => map.MapFrom(e => e.Dish != null && e.Dish.DishType != null ? e.Dish.DishType.Name : String.Empty))
                .ForMember(e => e.FilePath, map => map.MapFrom(e => e.Dish != null && e.Dish.Icon != null ? e.Dish.Icon.Path : String.Empty))
                .ForMember(e => e.ProductionPicturePath, map => map.MapFrom(e => e.Dish != null && e.Dish.Icon != null ? e.Dish.ProductionPicture.Path : String.Empty));

            CreateMap<TokenAltDishDTO, TokenAltDish>();
            CreateMap<TokenAltDish, TokenAltDishDTO>()
                .ForMember(e => e.DishLabel, map => map.MapFrom(e => e.Dish != null ? e.Dish.Label : String.Empty))
                .ForMember(e => e.FilePath, map => map.MapFrom(e => e.Dish != null && e.Dish.Icon != null ? e.Dish.Icon.Path : String.Empty))
                .ForMember(e => e.ProductionPicturePath, map => map.MapFrom(e => e.Dish != null && e.Dish.Icon != null ? e.Dish.ProductionPicture.Path : String.Empty));

            CreateMap<TokenOrderCombinedDishDTO, TokenOrderCombinedDish>();
            CreateMap<TokenOrderCombinedDish, TokenOrderCombinedDishDTO>();

            CreateMap<TokenLabelDTO, TokenLabel>();
            CreateMap<TokenLabel, TokenLabelDTO>();

            CreateMap<TokenDishLabelDTO, TokenDishLabel>();
            CreateMap<TokenDishLabel, TokenDishLabelDTO>();

            CreateMap<MealAllocationDTO, MealAllocation>();
            CreateMap<MealAllocation, MealAllocationDTO>()
                .ForMember(e => e.routeId, map => map.MapFrom(e => e.MealSessionDetail != null ? e.MealSessionDetail.RouteId : null));

            CreateMap<PackingAllocationDTO, PackingAllocation>();
            CreateMap<PackingAllocation, PackingAllocationDTO>()
                .ForMember(e => e.RouteLabel, map => map.MapFrom(f => f.Route.Label));
                //.ForMember(e => e.ToStoreInfoId, map => map.MapFrom(f => f.));

            CreateMap<DishAllocationDTO, DishAllocation>();
            CreateMap<DishAllocation, DishAllocationDTO>();

            CreateMap<StudentGroupDTO, StudentGroup>();

            CreateMap<StudentGroup, StudentGroupDTO>()
                .ForMember(e => e.MealSessionName, map => map.MapFrom(e => e.MealSession.Name))
                .ForMember(e => e.TermName, map => map.MapFrom(e => e.OutletTerm.Label))
                .ForMember(e => e.Sgdetails, map => map.MapFrom(e => e.Sgdetails.Where(f => f.IsActive).ToList()));

            CreateMap<StudentGroupDetailDTO, StudentGroupDetail>();

            CreateMap<OutletTerm, OutletTermDTO>();
            CreateMap<OutletTermDTO, OutletTerm>();

            CreateMap<Class, ClassDTO>()
                //.ForMember(e => e.ClassLevelName, map => map.MapFrom(e => e.Class.ClassLevel.Name))
                .ForMember(e => e.ClassLevelName, map => map.MapFrom(e => e.ClassLevel.Name));

            CreateMap<StaffType, StaffTypeDTO>();
            CreateMap<StaffTypeDTO, StaffType>();

            //staff
            CreateMap<StaffDTO, Staff>();
            CreateMap<StaffDTO, ApplicationUser>()
                .ForMember(e => e.Id, map => map.MapFrom(e => e.UserId))
                .ForMember(e => e.UserType, map => map.MapFrom(e => e.UserType));

            CreateMap<Staff, StaffDTO>()
                .ForMember(e => e.UserType, map => map.MapFrom(e => e.Account.User.UserType))
                .ForMember(e => e.DepartmentName, map => map.MapFrom(e => e.Department.Name))
                .ForMember(e => e.DepartmentId, map => map.MapFrom(e => e.DepartmentId))
                .ForMember(e => e.StaffTypeName, map => map.MapFrom(e => e.StaffType.Name))
                .ForMember(e => e.StaffTypeId, map => map.MapFrom(e => e.StaffTypeId))
                .ForMember(e => e.UserId, map => map.MapFrom(e => e.Account.User.Id))
                .ForMember(e => e.Username, map => map.MapFrom(e => e.Account.User.UserName))
                .ForMember(e => e.Email, map => map.MapFrom(e => e.Account.User.Email))
                .ForMember(e => e.FullName, map => map.MapFrom(e => e.Account.User.FullName))
                .ForMember(e => e.Cards, map => map.MapFrom(e => e.Account.User.UserCardIds.Where(f => f.IsActive).ToList()));

            CreateMap<StaffCardDTO, UserCardId>();

            CreateMap<RestrictionType, RestrictionTypeDTO>();
            CreateMap<RestrictionTypeDTO, RestrictionType>();

            CreateMap<Restriction, RestrictionDTO>()
                .ForMember(e => e.RestrictionTypeLabel, map => map.MapFrom(f => f.RestrictionType.Label))
                .ForMember(e => e.RestrictionTypeCode, map => map.MapFrom(f => f.RestrictionType.Code));
            CreateMap<RestrictionDTO, Restriction>();

            CreateMap<DishType, DishTypeDTO>();
            CreateMap<DishTypeDTO, DishType>();

            CreateMap<MealPlanOrder, MealPlanOrderDTO>()
                .ForMember(e => e.StudentGroupCode, map => map.MapFrom(f => f.StudentGroup.Code));
            CreateMap<MealPlanOrderDTO, MealPlanOrder>();

            CreateMap<Cuisine, CuisineDTO>();
            CreateMap<CuisineDTO, Cuisine>();

            CreateMap<CatererInfo, CatererInfoDTO>();
            CreateMap<CatererInfoDTO, CatererInfo>();

            CreateMap<OutletProfile, OutletProfileDTO>();
            CreateMap<OutletProfileDTO, OutletProfile>();

            CreateMap<Outlet, OutletDTO>();
                //.ForMember(e => e.OutletProfileName, map => map.MapFrom(e => e.OutletProfile.Label));
            CreateMap<OutletDTO, Outlet>();

            CreateMap<OutletMenuDish, OutletMenuDishDTO>();
            CreateMap<OutletMenuDishDTO, OutletMenuDish>();

            CreateMap<CatererOutlet, CatererOutletDTO>();
                //.ForMember(e => e.OutletProfileId, map => map.MapFrom(e => e.Outlet.OutletProfileId));
            CreateMap<CatererOutletDTO, CatererOutlet>();

            CreateMap<DishByDate, DishByDateDTO>();
            CreateMap<DishByDateDTO, DishByDate>();

            CreateMap<BentoBoxType, BentoBoxTypeDTO>()
                .ForMember(e => e.CatererInfoName, map => map.MapFrom(f => f.CatererInfo != null ? f.CatererInfo.Name : ""));
            CreateMap<BentoBoxTypeDTO, BentoBoxType>();

            CreateMap<BentoBoxType, BentoBoxTypeDetailsDTO>()
                .ForMember(e => e.BentoAssets, map => map.MapFrom(f => f.BentoAssets != null ? f.BentoAssets.Where(d => d.IsActive) : null));
            CreateMap<BentoBoxTypeDetailsDTO, BentoBoxType>();

            CreateMap<CartonType, CartonTypeDTO>()
                .ForMember(e => e.CatererInfoName, map => map.MapFrom(f => f.CatererInfo != null ? f.CatererInfo.Name : ""));
            CreateMap<CartonTypeDTO, CartonType>();

            CreateMap<CartonType, CartonTypeDetailsDTO>()
               .ForMember(e => e.CartonAssets, map => map.MapFrom(f => f.CartonAssets != null ? f.CartonAssets.Where(d => d.IsActive) : null));
            CreateMap<CartonTypeDetailsDTO, CartonType>();

            CreateMap<DeliveryOrder, DeliveryOrderDTO>()
                .ForMember(e => e.CatererInfoName, map => map.MapFrom(f => f.CatererInfo != null ? f.CatererInfo.Name : ""))
                .ForMember(d => d.DeliveryDetails, map => map.MapFrom(s => s.DeliveryDetails != null ? s.DeliveryDetails.Where(z => z.IsActive) : null));
            CreateMap<DeliveryOrderDTO, DeliveryOrder>();

            CreateMap<DeliveryDetail, DeliveryDetailDTO>()
                .ForMember(e => e.CartonAssetCode, map => map.MapFrom(f => f.CartonAsset != null ? f.CartonAsset.Code : ""))
                .ForMember(e => e.Status, map => map.MapFrom(f => f.TrackingStatus != null ? f.TrackingStatus.Status : ""))
                .ForMember(d => d.DeliveryBentos, map => map.MapFrom(s => s.DeliveryBentos != null ? s.DeliveryBentos.Where(z => z.IsActive) : null))
                ;
            CreateMap<DeliveryDetailDTO, DeliveryDetail>();

            CreateMap<DeliveryOrderNew, DeliveryOrderNewDTO>()
                .ForMember(e => e.CatererInfoName, map => map.MapFrom(f => f.CatererInfo != null ? f.CatererInfo.Name : ""))
                .ForMember(e => e.MealSessionName, map => map.MapFrom(f => f.MealSessionDetail != null ? f.MealSessionDetail.MealSession.MealPeriod.Name : ""))
                .ForMember(e => e.RouteName, map => map.MapFrom(f => f.Route != null ? f.Route.Label : ""))
                .ForMember(e => e.PickupTime, map => map.MapFrom(f => f.Route.Pickup))
                .ForMember(d => d.DeliveryDetails, map => map.MapFrom(s => s.DeliveryDetails != null ? s.DeliveryDetails.Where(z => z.IsActive) : null));
            CreateMap<DeliveryOrderNewDTO, DeliveryOrderNew>();

            CreateMap<DeliveryDetailNew, DeliveryDetailNewDTO>()
                .ForMember(e => e.CartonAssetCode, map => map.MapFrom(f => f.CartonAsset != null ? f.CartonAsset.Code : ""))
                .ForMember(e => e.CartonType, map => map.MapFrom(f => f.CartonAsset != null ? f.CartonAsset.CartonType.Code : ""))
                .ForMember(e => e.Status, map => map.MapFrom(f => f.TrackingStatus != null ? f.TrackingStatus.Status : ""))
                .ForMember(d => d.DeliveryBentos, map => map.MapFrom(s => s.DeliveryBentos != null ? s.DeliveryBentos.Where(z => z.IsActive) : null))
                ;
            CreateMap<DeliveryDetailNewDTO, DeliveryDetailNew>();

            CreateMap<StoreInventory, StoreInventoryDTO>()
                .ForMember(d => d.StoreInventoryDetails, map => map.MapFrom(s => s.StoreInventoryDetails != null ? s.StoreInventoryDetails.Where(z => z.IsActive) : null));
            CreateMap<StoreInventoryDTO, StoreInventory>();

            CreateMap<StoreInventoryDetail, StoreInventoryDetailDTO>();
            CreateMap<StoreInventoryDetailDTO, StoreInventoryDetail>();

            CreateMap<DeliveryBento, DeliveryBentoDTO>()
                .ForMember(e => e.BentoAssetCode, map => map.MapFrom(f => f.BentoAsset != null ? f.BentoAsset.Code : ""))
                .ForMember(e => e.DishCode, map => map.MapFrom(f => f.Dish != null ? f.Dish.Code : ""))
                ;
            CreateMap<DeliveryBentoDTO, DeliveryBento>();

            CreateMap<DeliveryBentoNew, DeliveryBentoNewDTO>()
                .ForMember(e => e.BentoAssetCode, map => map.MapFrom(f => f.BentoAsset != null ? f.BentoAsset.Code : ""))
                .ForMember(e => e.BentoType, map => map.MapFrom(f => f.BentoAsset != null ? f.BentoAsset.BentoBoxType.Code : ""))
                .ForMember(e => e.DishCode, map => map.MapFrom(f => f.Dish != null ? f.Dish.Code : ""))
                .ForMember(e => e.DishLabel, map => map.MapFrom(f => f.Dish != null ? f.Dish.Label : ""))
                .ForMember(e => e.DishType, map => map.MapFrom(f => f.Dish != null ? f.Dish.DishType.Name : ""))
                ;
            CreateMap<DeliveryBentoNewDTO, DeliveryBentoNew>();

            CreateMap<TrackingStatus, TrackingStatusDTO>();
            CreateMap<TrackingStatusDTO, TrackingStatus>();

            CreateMap<StoreInfo, StoreInfoDTO>()
                .ForMember(e => e.CatererInfoName, map => map.MapFrom(f => f.CatererInfo != null ? f.CatererInfo.Name : ""));
            CreateMap<StoreInfoDTO, StoreInfo>();

            CreateMap<BentoAsset, BentoAssetDTO>()
                .ForMember(e => e.BentoBoxTypeCode, map => map.MapFrom(f => f.BentoBoxType != null ? f.BentoBoxType.Code : ""))
                .ForMember(e => e.CartonAssetCode, map => map.MapFrom(f => f.CartonAsset != null ? f.CartonAsset.Code : ""))
                .ForMember(e => e.StoreInfoCode, map => map.MapFrom(f => f.StoreInfo != null ? f.StoreInfo.Code : ""))
                .ForMember(e => e.DishCode, map => map.MapFrom(f => f.Dish != null ? f.Dish.Code : ""))
                .ForMember(e => e.DishLabel, map => map.MapFrom(f => f.Dish != null ? f.Dish.Label : ""));
            CreateMap<BentoAssetDTO, BentoAsset>();

            CreateMap<CartonAsset, CartonAssetDTO>()
                .ForMember(e => e.CartonTypeCode, map => map.MapFrom(f => f.CartonType != null ? f.CartonType.Code : ""))
                .ForMember(d => d.BentoAssets, map => map.MapFrom(s => s.BentoAssets != null ? s.BentoAssets.Where(z => z.IsActive) : null))
                .ForMember(d => d.DispoasbleBoxes, map => map.MapFrom(s => s.DisposableBoxes != null ? s.DisposableBoxes.Where(z => z.IsActive) : null))
                .ForMember(e => e.StoreInfoCode, map => map.MapFrom(f => f.StoreInfo != null ? f.StoreInfo.Code : ""))
                .ForMember(e => e.DishCode, map => map.MapFrom(f => f.Dish != null ? f.Dish.Code : ""))
                .ForMember(e => e.DishLabel, map => map.MapFrom(f => f.Dish != null ? f.Dish.Label : ""));
            ;
            CreateMap<CartonAssetDTO, CartonAsset>();

            CreateMap<CartonDisposableBox, CartonDisposableBoxDTO>()
                .ForMember(e => e.CartonAssetCode, map => map.MapFrom(f => f.CartonAsset != null ? f.CartonAsset.Code : ""))
                .ForMember(e => e.DishCode, map => map.MapFrom(f => f.Dish != null ? f.Dish.Code : ""))
                .ForMember(e => e.DishLabel, map => map.MapFrom(f => f.Dish != null ? f.Dish.Label : ""));
            CreateMap<CartonDisposableBoxDTO, CartonDisposableBox>();

            CreateMap<DishTypePeriod, DishTypePeriodDTO>()
                .ForMember(e => e.DishTypeName, map => map.MapFrom(f => f.DishType.Name))
                .ForMember(e => e.PeriodName, map => map.MapFrom(f => f.Period.Name));
            CreateMap<DishTypePeriodDTO, DishTypePeriod>();

            CreateMap<MealType, MealTypeDTO>()
                .ForMember(e => e.CuisineName, map => map.MapFrom(f => f.Cuisine.Name))
                .ForMember(e => e.DishNames, map => map.MapFrom(f => string.Join(",", f.Dishes.Select(x => x.Dish.Label))));
            CreateMap<MealTypeDTO, MealType>();

            CreateMap<MealTypeDish, MealTypeDishDTO>()
                .ForMember(e => e.DishName, map => map.MapFrom(f => f.Dish.Label))
                .ForMember(e => e.MealTypeName, map => map.MapFrom(f => f.MealType.Name));
            CreateMap<MealTypeDishDTO, MealTypeDish>();

            CreateMap<MealPeriod, MealPeriodDTO>()
                .ForMember(e => e.OutletProfileName, map => map.MapFrom(f => f.OutletProfile.Label));
            CreateMap<MealPeriodDTO, MealPeriod>();

            CreateMap<MealSession, MealSessionDTO>()
                .ForMember(e => e.OutletName, map => map.MapFrom(f => f.Outlet.Name))
                .ForMember(e => e.MealPeriodName, map => map.MapFrom(f => f.MealPeriod.Name))
                .ForMember(e => e.Details, map => map.MapFrom(f => f.Details.Where(x => x.IsActive)));
            CreateMap<MealSessionDTO, MealSession>();

            CreateMap<MealSessionMealPeriodDTO, MealSessionMealPeriod>();
            CreateMap<MealSessionMealPeriod, MealSessionMealPeriodDTO>();

            CreateMap<OutletBlockedDate, OutletBlockedDateDTO>();
            CreateMap<OutletBlockedDateDTO, OutletBlockedDate>();

            CreateMap<MealSessionDetail, MealSessionDetailDTO>()
                .ForMember(e => e.MealSessionName, map => map.MapFrom(f => f.MealSession.Name))
                .ForMember(e => e.MealPeriodId, map => map.MapFrom(f => f.MealSession.MealPeriodId))
                .ForMember(e => e.MealPeriodName, map => map.MapFrom(f => f.MealSession.MealPeriod != null ? f.MealSession.MealPeriod.Name : string.Empty))
                .ForMember(e => e.RouteName, map => map.MapFrom(f => f.Route.Label))
                .ForMember(e => e.RouteTime, map => map.MapFrom(f => f.Route.Pickup))
                //.ForMember(e => e.STime, map => map.MapFrom(f => string.Format("{0:HH:mm}", f.StartDate)))
                //.ForMember(e => e.RTime, map => map.MapFrom(f => string.Format("{0:HH:mm}", f.RouteTime)))
                //.ForMember(e => e.OTime, map => map.MapFrom(f => string.Format("{0:HH:mm}", f.OverheadTime)))
                //.ForMember(e => e.CTime, map => map.MapFrom(f => string.Format("{0:HH:mm}", f.CalSourceTime)))
                ;
            CreateMap<MealSessionDetailDTO, MealSessionDetail>()
                //.ForMember(e => e.StartDate, map => map.MapFrom(f => DateTime.Parse(f.STime)))
                //.ForMember(e => e.RouteTime, map => map.MapFrom(f => DateTime.Parse(f.RTime)))
                //.ForMember(e => e.OverheadTime, map => map.MapFrom(f => DateTime.Parse(f.OTime)))
                //.ForMember(e => e.CalSourceTime, map => map.MapFrom(f => DateTime.Parse(f.CTime)))
                ;

            CreateMap<Dish, DishDTO>()
                .ForMember(e => e.DishTypeName, map => map.MapFrom(f => f.DishType.Name))
                .ForMember(e => e.CuisineName, map => map.MapFrom(f => f.Cuisine.Name))
                .ForMember(e => e.StoreInfoCode, map => map.MapFrom(f => f.StoreInfo.Code))
                .ForMember(e => e.StoreInfoName, map => map.MapFrom(f => f.StoreInfo.Name))
                .ForMember(e => e.CatererName, map => map.MapFrom(f => f.Caterer.Name))
                .ForMember(e => e.BentoBoxTypeCode, map => map.MapFrom(f => f.BentoBoxType.Code))
                .ForMember(e => e.BentoBoxTypeRFID, map => map.MapFrom(f => f.BentoBoxType.isRFID))
                .ForMember(e => e.BentoBoxTypeDetails, map => map.MapFrom(f => f.BentoBoxType.Details))
                .ForMember(e => e.BentoBoxTypePicture, map => map.MapFrom(f => f.BentoBoxType.picture))
                .ForMember(e => e.SubDishes, map => map.MapFrom(f => f.SubDishes))
                .ForMember(d => d.FilePath, map => map.MapFrom(s => s.Icon.Path))
                .ForMember(d => d.ProductionPicturePath, map => map.MapFrom(s => s.ProductionPicture.Path))
                .ForMember(e => e.CreatedByName, map => map.MapFrom(f => f.CreatedByUser != null ? f.CreatedByUser.Email : string.Empty))
                .ForMember(e => e.UpdatedByName, map => map.MapFrom(f => f.UpdatedByUser != null ? f.UpdatedByUser.Email : string.Empty));
            CreateMap<DishDTO, Dish>()
                .ForMember(d => d.Icon, map => map.MapFrom(s => new File { Path = s.FilePath, FileName = s.FileName, Type = FileType.Icon.ToString() }))
                .ForMember(d => d.ProductionPicture, map => map.MapFrom(s => new File { Path = s.ProductionPicturePath, FileName = s.ProductionPictureName, Type = FileType.Icon.ToString() }));

            CreateMap<Dish, DishSimple>();

            CreateMap<DishDetailDTO, DishDetail>();
            CreateMap<DishDetail, DishDetailDTO>()
                .ForMember(e => e.Code, map => map.MapFrom(f => f.Dish.Code))
                .ForMember(e => e.Label, map => map.MapFrom(f => f.Dish.Label));

            CreateMap<DishComponentDTO, DishComponent>();
            CreateMap<DishComponent, DishComponentDTO>();

            CreateMap<DishRestrictionDTO, DishRestriction>();
            CreateMap<DishRestriction, DishRestrictionDTO>()
                .ForMember(e => e.RestrictionCode, map => map.MapFrom(f => f.Restriction.Code))
                .ForMember(e => e.RestrictionLabel, map => map.MapFrom(f => f.Restriction.Label));

            CreateMap<DishPeriod, DishPeriodDTO>()
                .ForMember(e => e.DishName, map => map.MapFrom(f => f.Dish.Label))
                .ForMember(e => e.PeriodName, map => map.MapFrom(f => f.Period.Name));
            CreateMap<DishPeriodDTO, DishPeriod>();

            CreateMap<Menu, MenuDTO>()
                .ForMember(e => e.CuisineName, map => map.MapFrom(f => f.Cuisine.Name));
            CreateMap<MenuDTO, Menu>();

            CreateMap<MenuDish, MenuDishDTO>()
                .ForMember(e => e.DishName, map => map.MapFrom(f => f.Dish.Label))
                .ForMember(e => e.MenuName, map => map.MapFrom(f => f.Menu.Label))
                .ForMember(e => e.MealTypeName, map => map.MapFrom(f => f.MealType.Name));
            CreateMap<MenuDishDTO, MenuDish>();

            CreateMap<MenuCycle, MenuCycleDTO>()
                .ForMember(e => e.Days, map => map.MapFrom(f => f.Schedules.Where(x => x.IsActive).Select(s => s.Day).Distinct()))
                .ForMember(e => e.Schedules, map => map.MapFrom(f => f.Schedules.Where(x => x.IsActive).OrderBy(e => e.Day)))
                .ForMember(e => e.OutletProfileName, map => map.MapFrom(f => f.OutletProfile.Label));
            CreateMap<MenuCycleDTO, MenuCycle>();

            CreateMap<MenuCycleBlockedDateDTO, MenuCycleBlockedDate>();
            CreateMap<MenuCycleBlockedDate, MenuCycleBlockedDateDTO>();

            CreateMap<MenuCycleCalendar, MenuCycleCalendarDTO>();
            CreateMap<MenuCycleCalendarDTO, MenuCycleCalendar>();

            CreateMap<MenuCycleCalendarBlockedDate, MenuCycleCalendarBlockedDateDTO>();
            CreateMap<MenuCycleCalendarBlockedDateDTO, MenuCycleCalendarBlockedDate>();

            CreateMap<MenuCycleSchedule, MenuCycleScheduleDTO>()
                .ForMember(e => e.Periods, map => map.MapFrom(f => f.Periods.Where(x => x.IsActive).OrderBy(e => e.MealPeriod.Sequence)));
            CreateMap<MenuCycleScheduleDTO, MenuCycleSchedule>();

            CreateMap<MenuCycleSchedulePeriod, MenuCycleSchedulePeriodDTO>()
                .ForMember(e => e.MealPeriodName, map => map.MapFrom(f => f.MealPeriod.Name));
            CreateMap<MenuCycleSchedulePeriodDTO, MenuCycleSchedulePeriod>();

            CreateMap<MenuCycleSchedulePeriodMenu, MenuCycleSchedulePeriodMenuDTO>()
                .ForMember(e => e.Label, map => map.MapFrom(f => f.Menu.Label))
                .ForMember(e => e.Code, map => map.MapFrom(f => f.Menu.Code));
            CreateMap<MenuCycleSchedulePeriodMenuDTO, MenuCycleSchedulePeriodMenu>();

            CreateMap<OutletMenuCycleSchedulePeriodMenu, OutletMenuCycleSchedulePeriodMenuDTO>()
                .ForMember(e => e.Label, map => map.MapFrom(f => f.Menu.Label));
            CreateMap<OutletMenuCycleSchedulePeriodMenuDTO, OutletMenuCycleSchedulePeriodMenu>();

            CreateMap<Driver, DriverDTO>();
            CreateMap<DriverDTO, Driver>();

            CreateMap<Route, RouteDTO>();
            CreateMap<RouteDTO, Route>();
            CreateMap<RouteNodeDTO, RouteNode>();

            CreateMap<OutletClassRoster, OutletClassRosterDTO>()
                .ForMember(e => e.Days, map => map.MapFrom(f => f.Schedules.Where(x => x.IsActive).Select(s => s.Day).Distinct()))
                .ForMember(e => e.Schedules, map => map.MapFrom(f => f.Schedules.Where(x => x.IsActive).OrderBy(e => e.Day)))
                .ForMember(e => e.OutletProfileName, map => map.MapFrom(f => f.OutletProfile.Label));
            CreateMap<OutletClassRosterDTO, OutletClassRoster>();

            CreateMap<OutletClassRosterSchedule, OutletClassRosterScheduleDTO>();
            CreateMap<OutletClassRosterScheduleDTO, OutletClassRosterSchedule>();

            CreateMap<OutletClassRosterSchedulePeriod, OutletClassRosterSchedulePeriodDTO>()
                .ForMember(e => e.MealSessionDetailName, map => map.MapFrom(f => f.MealSessionDetail.Name))
                .ForMember(e => e.OutletClassRosterStartDate, map => map.MapFrom(f => f.OutletClassRosterSchedule.OutletClassRoster.StartDate))
                .ForMember(e => e.OutletClassRosterEndDate, map => map.MapFrom(f => f.OutletClassRosterSchedule.OutletClassRoster.EndDate))
                .ForMember(e => e.StartDate, map => map.MapFrom(f => f.MealSessionDetail.StartDate))
                .ForMember(e => e.EndDate, map => map.MapFrom(f => f.MealSessionDetail.EndDate))
                .ForMember(e => e.RouteInterval, map => map.MapFrom(f => f.MealSessionDetail.RouteInterval))
                .ForMember(e => e.OverheadInterval, map => map.MapFrom(f => f.MealSessionDetail.OverheadInterval))
                ;

            CreateMap<OutletClassRosterSchedulePeriodDTO, OutletClassRosterSchedulePeriod>();

            CreateMap<OutletClassRosterSchedulePeriodClass, OutletClassRosterSchedulePeriodClassDTO>()
                .ForMember(e => e.Name, map => map.MapFrom(f => f.Class.Name));
            CreateMap<OutletClassRosterSchedulePeriodClassDTO, OutletClassRosterSchedulePeriodClass>();

            CreateMap<MenuDishMealType, MenuDishMealTypeDTO>();
            CreateMap<MenuListCol, MenuListColDTO>();
            CreateMap<MenuListRow, MenuListRowDTO>();
            CreateMap<MenuListCell, MenuListCellDTO>();

            CreateMap<MenuGroup, MenuGroupDTO>()
                .ForMember(e => e.OutletName, map => map.MapFrom(f => f.Outlet.Name))
                .ForMember(e => e.MenuGroupDishCycles, map => map.MapFrom(f => f.MenuGroupDishCycles.Where(m => m.IsActive && m.DishCycle != null && m.DishCycle.IsActive)));
            CreateMap<MenuGroupDTO, MenuGroup>();

            CreateMap<MenuGroupDishCycle, MenuGroupDishCycleDTO>()
                .ForMember(e => e.MenuGroupName, map => map.MapFrom(f => f.MenuGroup.Name))
                .ForMember(e => e.DishCycleLabel, map => map.MapFrom(f => f.DishCycle.Label));
            CreateMap<MenuGroupDishCycleDTO, MenuGroupDishCycle>();

            CreateMap<MenuGroupClass, MenuGroupClassDTO>()
                .ForMember(e => e.MenuGroupName, map => map.MapFrom(f => f.MenuGroup.Name))
                .ForMember(e => e.ClassName, map => map.MapFrom(f => f.Class.Name));
            CreateMap<MenuGroupClassDTO, MenuGroupClass>();

            CreateMap<DishCycle, DishCycleDTO>()
                .ForMember(e => e.OutletProfileName, map => map.MapFrom(f => f.OutletProfile.Label))
                .ForMember(e => e.Schedules, map => map.MapFrom(f => f.Schedules.OrderBy(e => e.Day)))
                .ForMember(e => e.Sets, map => map.MapFrom(f => f.Sets.Where(e => e.IsActive).OrderBy(e => e.Sequence)));
            CreateMap<DishCycleSchedule, DishCycleScheduleDTO>()
                .ForMember(e => e.Details, map => map.MapFrom(f => f.Details.Where(e => e.IsActive).OrderBy(e => e.Sequence)));
            CreateMap<DishCycleScheduleSet, DishCycleScheduleSetDTO>();
            CreateMap<DishCycleScheduleDetail, DishCycleScheduleDetailDTO>()
                .ForMember(e => e.Menus, map => map.MapFrom(f => f.Menus.Where(e => e.IsActive && e.Dish != null && e.Dish.IsActive)));

            CreateMap<DishCycleScheduleDetailMenu, DishCycleScheduleDetailMenuDTO>()
                .ForMember(e => e.DishLabel, map => map.MapFrom(f => f.Dish.Label))
                .ForMember(e => e.DishCode, map => map.MapFrom(f => f.Dish.Code))
                .ForMember(e => e.DishCycleLabel, map => map.MapFrom(f => f.DishCycle.Label))
                .ForMember(e => e.FilePath, map => map.MapFrom(f => f.Dish != null && f.Dish.Icon != null ? f.Dish.Icon.Path : ""))
                .ForMember(e => e.ProductionPicturePath, map => map.MapFrom(f => f.Dish.ProductionPicture.Path))
                .ForMember(e => e.DishObj, map => map.MapFrom(f => f.Dish));


            CreateMap<DishCycleDTO, DishCycle>();
            CreateMap<DishCycleScheduleDTO, DishCycleSchedule>();
            CreateMap<DishCycleScheduleSetDTO, DishCycleScheduleSet>();
            CreateMap<DishCycleScheduleDetailDTO, DishCycleScheduleDetail>();
            CreateMap<DishCycleScheduleDetailMenuDTO, DishCycleScheduleDetailMenu>();


            CreateMap<DishCycleBlockedDateDTO, DishCycleBlockedDate>();
            CreateMap<DishCycleBlockedDate, DishCycleBlockedDateDTO>();

            CreateMap<DishCycleCalendar, DishCycleCalendarDTO>();
            CreateMap<DishCycleCalendarDTO, DishCycleCalendar>();

            CreateMap<DishCycleCalendarBlockedDate, DishCycleCalendarBlockedDateDTO>();
            CreateMap<DishCycleCalendarBlockedDateDTO, DishCycleCalendarBlockedDate>();

            CreateMap<DishCycleScheduleSetMenuDTO, DishCycleScheduleSetDTO>()
                .ForMember(e => e.CycleTypeLabel, map => map.MapFrom(f => f.DishCycleType.Label))
                .ForMember(e => e.MealTypeLabel, map => map.MapFrom(f => f.MealType.Name));

            CreateMap<MealCreditSetMenuDTO, MealCreditSetDTO>();

            CreateMap<OutletDishBlockedDateDTO, OutletDishBlockedDate>();
            CreateMap<OutletDishBlockedDate, OutletDishBlockedDateDTO>();

            CreateMap<DishCyclePeriodDTO, DishCyclePeriod>();
            CreateMap<DishCyclePeriod, DishCyclePeriodDTO>()
                .ForMember(e => e.MealPeriodName, map => map.MapFrom(f => f.MealPeriod.Name));

            CreateMap<OutletDishCyclePeriodMenu, OutletDishCyclePeriodMenuDTO>()
                .ForMember(e => e.Label, map => map.MapFrom(f => f.Dish.Label));
            CreateMap<OutletDishCyclePeriodMenuDTO, OutletDishCyclePeriodMenu>();

            CreateMap<NotificationDTO, Notification>();
            CreateMap<Notification, NotificationDTO>();

            CreateMap<NotificationEventDTO, NotificationEvent>();
            CreateMap<NotificationEvent, NotificationEventDTO>();

            CreateMap<PaymentTypeDTO, PaymentType>();
            CreateMap<PaymentType, PaymentTypeDTO>();

            CreateMap<PaymentDTO, Payment>();
            CreateMap<Payment, PaymentDTO>()
                .ForMember(d => d.TokenOrders, map => map.MapFrom(s => s.TokenOrders != null ? s.TokenOrders.Where(z => z.IsActive) : null))
                .ForMember(d => d.MealPlanOrders, map => map.MapFrom(s => s.MealPlanOrders != null ? s.MealPlanOrders.Where(z => z.IsActive) : null))
                .ForMember(e => e.userName, map => map.MapFrom(f => f.User.FriendlyName))
                .ForMember(e => e.studentName, map => map.MapFrom(f => f.Student.Name));

            CreateMap<TransactionFeeDTO, TransactionFee>();
            CreateMap<TransactionFee, TransactionFeeDTO>()
                .ForMember(e => e.PaymentTypeName, map => map.MapFrom(f => f.PaymentType.Name));

            CreateMap<TransactionFeeDetailDTO, TransactionFeeDetail>();
            CreateMap<TransactionFeeDetail, TransactionFeeDetailDTO>();

            CreateMap<VoucherTypeDTO, VoucherType>();
            CreateMap<VoucherType, VoucherTypeDTO>();

            CreateMap<VoucherMealPeriodDTO, VoucherMealPeriod>();
            CreateMap<VoucherMealPeriod, VoucherMealPeriodDTO>()
                .ForMember(e => e.MealPeriodName, map => map.MapFrom(f => f.MealPeriod.Name));

            CreateMap<VoucherDTO, Voucher>();
            CreateMap<Voucher, VoucherDTO>()
                .ForMember(e => e.VoucherTypeName, map => map.MapFrom(f => f.VoucherType.Name));

            CreateMap<WaiverDTO, Waiver>();
            CreateMap<Waiver, WaiverDTO>()
                .ForMember(e => e.TransactionFeeLabel, map => map.MapFrom(f => f.TransactionFee.Name))
                .ForMember(e => e.TransactionFeePaymentType, map => map.MapFrom(f => f.TransactionFee.PaymentType.Name));

            CreateMap<ContactUsSubject, ContactUsSubjectDTO>();
            CreateMap<ContactUsSubjectDTO, ContactUsSubject>();

            CreateMap<ContactUsDetailDTO, ContactUsDetail>();
            CreateMap<ContactUsDetail, ContactUsDetailDTO>()
                .ForMember(e => e.ContactUsSubjectName, map => map.MapFrom(f => f.ContactUsSubject.Name))
                .ForMember(e => e.ContactUsSubjectDescription, map => map.MapFrom(f => f.ContactUsSubject.Description));

            CreateMap<EmailTemplate, EmailTemplateDTO>();
            CreateMap<EmailTemplateDTO, EmailTemplate>();

            CreateMap<CancelOrderRequest, CancelOrderRequestDTO>()
                .ForMember(e => e.CancellationDate, map => map.MapFrom(f => f.TokenOrder.DeliveryDate))
                .ForMember(e => e.SubmissionDate, map => map.MapFrom(f => f.CreatedDate))
                .ForMember(e => e.UserName, map => map.MapFrom(f => f.User.FriendlyName))
                .ForMember(e => e.IsApproved, map => map.MapFrom(f => f.Status == "Approved"))
                .ForMember(e => e.MealSessionName, map => map.MapFrom(f => f.TokenOrder.Session.MealSessionName))
                .ForMember(e => e.StudentName, map => map.MapFrom(f => f.Student != null ? f.Student.Name : string.Empty))
                .ForMember(e => e.StudentEmail, map => map.MapFrom(f => f.Student != null ? f.Student.Email : string.Empty))
                .ForMember(e => e.IsNotifCancellationRequestStatus, map => map.MapFrom(f => f.Student != null ? f.Student.isNotifCancellationRequestStatus : false))
                .ForMember(e => e.DeliveryDate, map => map.MapFrom(f => f.TokenOrder != null ? f.TokenOrder.DeliveryDate : DateTime.MinValue))
                .ForMember(e => e.Tokens, map => map.MapFrom(f => f.TokenOrder != null ? f.TokenOrder.Tokens.ToList() : new List<TokenOrdered>()));

            CreateMap<CancelOrderRequestDTO, CancelOrderRequest>();
            CreateMap<CreateCancelOrderRequestDTO, CancelOrderRequestDTO>();

            CreateMap<NotificationSettingDTO, NotificationSetting>();
            CreateMap<NotificationSetting, NotificationSettingDTO>()
                .ForMember(e => e.Type, map => map.MapFrom(f => Enum.GetName(f.Type.GetType(), f.Type)))
                .ForMember(e => e.DayEnabled, map => map.MapFrom(f => Enum.GetName(f.DayEnabled.GetType(), f.DayEnabled)));

            CreateMap<UserOrderAlert, UserOrderAlertDTO>();
            CreateMap<UserOrderAlertDTO, UserOrderAlert>();

            CreateMap<StudentGroupMealPlan, StudentGroupMealPlanDTO>();
            CreateMap<StudentGroupMealPlanDTO, StudentGroupMealPlan>();

            CreateMap<OrderPortalContent, OrderPortalContentDTO>();
            CreateMap<OrderPortalContentDTO, OrderPortalContent>();

            CreateMap<OrderPortalBanner, OrderPortalBannerDTO>()
                .ForMember(e => e.FilePath, map => map.MapFrom(e => e.BannerImage != null && e.BannerImage != null ? e.BannerImage.Path : String.Empty));

            CreateMap<OrderPortalBannerDTO, OrderPortalBanner>();
        }
    }
}
