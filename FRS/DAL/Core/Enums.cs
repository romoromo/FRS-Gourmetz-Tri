using System;

namespace DAL.Core
{
    public enum Gender
    {
        None,
        Female,
        Male
    }

    public enum RepeatTypes
    {
        None,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    public enum LocationTypes
    {
        Hospital,
        Building,
        Level,
        Room
    }

    public enum FileType
    {
        Icon,
        PNG,
        JPG,
        Document,
        Excel,
        PDF,
        PPT
    }

    public enum BookingStatus
    {
        Unknown,
        Released,
        Cancelled,
        Pending,
        InProgress,
        CheckedIn,
        Completed
    }

    public enum ColorState
    {
        Red,
        Yellow,
        Green
    }

    public enum ParticipantStatus
    {
        Pending,
        Accepted,
        Declined,
        Attended,
        Maybe
    }

    public enum RecurEventType
    {
        ALL_EVENTS,
        OTHER_EVENTS,
        THIS_EVENT
    }

    public enum EpaperTemplateType
    {
        FRS,
        PIB
    }

    public enum VehicleStatus
    {
        REGISTERED,
        PENDING,
        APPROVED,
        ENTRY,
        EXIT
    }

    public enum CardIdStatus
    {
        ACTIVE,
        INACTIVE
    }

    public enum StudentCardStatus
    {
        ACTIVE,
        INACTIVE
    }

    public enum VehicleSeasonType
    {
        S,  //STAFF
        V,  //VISITOR
        P,  //PARTICIPANT
        O   //OTHERS
    }

    public enum BookingRestrictionFrequency
    {
        DAILY,
        WEEKLY,
        MONTHLY,
        YEARLY
    }

    public enum ConnectionType
    {
        DEVICE,
        PIBDEVICE,
        USER
    }

    public enum UserConnectionStatus
    {
        ONLINE,
        ENGAGED,
        OFFLINE,
        AVAILABLE,
        OFFDUTY
    }

    public enum VehicleCardType
    {
        VISITOR,
        STAFF,
        VIP_VISITOR,
        VIP_STAFF,
        OTHERS
    }

    public enum AuditLogType
    {
        Added = 0,
        Deleted = 1,
        Modified = 2,
        SoftDeleted = 3,
        UnDeleted = 4
    }

    public enum WalletTransactionType
    {
        CREDIT,
        DEBIT
    }

    public enum RewardTransactionType
    {
        CREDIT,
        DEBIT
    }

    public enum CatererOutletStatus
    {
        REQUESTED,
        APPROVED,
        DECLINED,
        CANCELLED
    }

    public enum NotificationSettingType
    {
        NO_ORDER,
        ABANDONED_CART_1,
        ABANDONED_CART_2,
        CANCEL_REQUEST_APPROVED,
        CANCEL_REQUEST_CANCELLED,
        ORDER_NOT_COLLECTED
    }

    public enum UserAlertType
    {
        NO_ORDER,
        ABANDONED_CART_1,
        ABANDONED_CART_2,
        ORDER_NOT_COLLECTED
    }

    public enum UserActivityType
    {
        LOG_IN,
        LOG_OUT,
        ORDER_CREATE,
        ORDER_UPDATE,
        ORDER_DELETE,
        PAYMENT_CREATE,
        PAYMENT_UPDATE,
        PAYMENT_DELETE,
        ROSTER_CREATE,
        ROSTER_UPDATE,
        ROSTER_DELETE,
        ROSTER_MEALSESSION_CREATE,
        ROSTER_MEALSESSION_UPDATE,
        ROSTER_MEALSESSION_DELETE,
        CANCEL_ORDER_CREATE,
        CANCEL_ORDER_UPDATE,
        CANCEL_ORDER_DELETE,
    }
}
