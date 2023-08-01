using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        IEnumerable<Reservation> All();
        Task<List<Reservation>> GetReservationsLoadRelatedAsync(int page, int pageSize, CalendarFilter filter = null);
        Task<BaseOperationResponse> CreateAsync(Reservation reservation, string filePath = null);
        Task<bool> TestCanDeleteAsync(int reservationId);
        Task<BaseOperationResponse> DeleteAsync(int reservationId, int? cancelledBy, string reason, string recurApplyChangesType, bool isCancel = false);
        Task<Reservation> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Reservation reservation, string recurApplyChangesType = null, string filePath = null);
        List<TimeInterval> GetAllTimeIntervals(TimeIntervalFilter filter = null);
        Task<BaseOperationResponse> GetNextBooking(CalendarFilter filter = null);
        List<BookingGridRow> GetBookingGrid(CalendarFilter filter = null);
        Task<BaseOperationResponse> GetCurrentDetail(CalendarFilter filter = null);
        Task<List<SignageBookingDTO>> GetSignageReservations(CalendarFilter filter = null);
        Task<BaseOperationResponse> ValidateCheckIn(string userName, string pin, int reservationId, string name, string company, string designation);
        Task<BaseOperationResponse> ProcessExpiredBookings();
        Task<BaseOperationResponse> MonitorInprogressBookings();
        Task<BaseOperationResponse> CompleteBookings();
        Task<BaseOperationResponse> GetExtensionTimes(int reservationId);
        Task<BaseOperationResponse> ValidateExtend(int reservationId, double totalMinutes);
        Task<BaseOperationResponse> ValidateEnd(int reservationId);
        Task<List<CalendarEvent>> GetCalendarEvents(CalendarFilter filter = null);
        Task<BaseOperationResponse> ValidateAttendanceSignIn(string userName, int reservationId, string name, string company, string designation, string status);
        Task<BaseOperationResponse> ValidateFeedback(int reservationId, int userId, string feedback, string comment);
        Task<byte[]> GenerateAttendanceXlsLayout(int reservationId);
        Task<BaseOperationResponse> Response(int reservationId, string userName, string status);
        Task<LocationApiInformation> GetLocationDetail(CalendarFilter filter = null);
        Task<BaseOperationResponse> UpdateArrival(List<VehicleQueryModel> queryModel);
        Task<BaseOperationResponse> RegisterVehicleToVMS(List<VMSVehiclePostRequestModel> list);
        Task<List<VMSVehicleLog>> GetVehicleLogs(VehicleLogFilter filter = null);
        Task<BaseOperationResponse> SyncSmartRoomSchedules(SMARTRoomXML smartRoom, int? institutionId);
    }
}
