using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IBookingRepository : IRepository<Reservation>
    {
        IEnumerable<Reservation> All();
        Task<List<Reservation>> GetReservationsLoadRelatedAsync(int page, int pageSize, CalendarFilter filter = null);
        Task<BaseOperationResponse> CreateAsync(Reservation reservation);
        Task<bool> TestCanDeleteAsync(int reservationId);
        Task<BaseOperationResponse> DeleteAsync(int reservationId);
        Task<Reservation> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Reservation reservation);
        List<TimeInterval> GetAllTimeIntervals(TimeIntervalFilter filter = null);
        Task<BaseOperationResponse> GetNextBooking(CalendarFilter filter = null);
        List<BookingGrid> GetBookingGrid(CalendarFilter filter = null);
        Task<BaseOperationResponse> GetCurrentDetail(CalendarFilter filter = null);
        List<SignageBookingDTO> GetSignageReservations(CalendarFilter filter = null);
        Task<BaseOperationResponse> ValidateCheckIn(string userName, string pin, int reservationId);
        Task<BaseOperationResponse> ProcessExpiredBookings();
        Task<BaseOperationResponse> GetExtensionTimes(int reservationId);
        Task<BaseOperationResponse> ValidateExtend(int reservationId, int timeIntervalId);
        Task<BaseOperationResponse> ValidateEnd(int reservationId);
    }
}
