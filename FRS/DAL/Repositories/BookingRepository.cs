using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using DAL.Core.DTO;

namespace DAL.Repositories
{
    public class BookingRepository : Repository<Reservation>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<Reservation> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Reservation> All()
        {
            return GetAll()
                .OrderBy(c => c.StartDateTime)
                .ToList();
        }

        public async Task<List<Reservation>> GetReservationsLoadRelatedAsync(int page, int pageSize, CalendarFilter filter = null)
        {
            IQueryable<Reservation> query = _appContext.Reservations
                .Include(e => e.CreatedByUser)
                .Include(e => e.Location)
                .ThenInclude(e => e.LocationFacilities);

            if (filter != null)
            {
                if (filter.capacity > 0)
                {
                    //query = query.Where(e => e.Location.);
                }

                if (filter.BookedById.HasValue)
                {
                    query = query.Where(e => filter.BookedById == e.CreatedBy);
                }

                if (filter.Start.HasValue)
                {
                    query = query.Where(e => filter.Start.Value.Date == e.StartDateTime.Date);
                }

                if (filter.StartTime.HasValue)
                {
                    query = query.Where(e => DateTime.Compare(filter.StartTime.Value, e.StartDateTime) <= 0);
                }

                if (filter.EndTime.HasValue)
                {
                    query = query.Where(e => DateTime.Compare(filter.EndTime.Value, e.EndDateTime) >= 0);
                }

                if (filter.locationIds != null && filter.locationIds.Count > 0)
                {
                    query = query.Where(e => e.LocationId.HasValue && filter.locationIds.Contains(e.LocationId.Value));
                }

                if (filter.facilityIds != null && filter.facilityIds.Count > 0)
                {
                    var q = query.Join(_appContext.LocationFacilities,
                        res => res.LocationId,
                        fac => fac.LocationId,
                        (res, fac) =>
                            new { Reservation = res, Facility = fac });
                    query = q.Where(e => filter.facilityIds.Contains(e.Facility.FacilityId)).Select(e => e.Reservation).Distinct().AsQueryable();

                }
            }

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.StartDateTime);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var reservations = (await query.ToListAsync())
                .OrderBy(r => r.StartDateTime).ToList();

            return reservations;
        }

        #region Signage Reservation
        public List<SignageBookingDTO> GetSignageReservations(CalendarFilter filter = null)
        {
            IQueryable<Reservation> query = _appContext.Reservations
                .Include(e => e.CreatedByUser)
                .Include(e => e.Location)
                .ThenInclude(e => e.LocationFacilities);

            if (filter != null)
            {
                if (filter.BookedById.HasValue)
                {
                    query = query.Where(e => filter.BookedById == e.CreatedBy);
                }

                if (filter.Start.HasValue)
                {
                    query = query.Where(e => e.StartDateTime.Date >= filter.Start.Value.Date && e.ReservationTimes.Where(a => a.IsActive).Any());
                }

                if (filter.End.HasValue)
                {
                    query = query.Where(e => e.EndDateTime.Date <= filter.End.Value.Date);
                }

                if (filter.StartTime.HasValue)
                {
                    query = query.Where(e => DateTime.Compare(filter.StartTime.Value, e.StartDateTime) <= 0);
                }

                if (filter.EndTime.HasValue)
                {
                    query = query.Where(e => DateTime.Compare(filter.EndTime.Value, e.EndDateTime) >= 0);
                }

                if (filter.UpdatedFrom.HasValue)
                {
                    query = query.Where(e => e.UpdatedDate >= filter.UpdatedFrom);
                }

                if (filter.UpdatedTo.HasValue)
                {
                    query = query.Where(e => new DateTime(e.UpdatedDate.Year, e.UpdatedDate.Month, e.UpdatedDate.Day, e.UpdatedDate.Hour, e.UpdatedDate.Minute, 0)
                                    <= filter.UpdatedTo.Value.AddSeconds(-filter.UpdatedTo.Value.Second).AddMilliseconds(-filter.UpdatedTo.Value.Millisecond));
                }

                if (filter.locationIds != null && filter.locationIds.Count > 0)
                {
                    query = query.Where(e => e.LocationId.HasValue && filter.locationIds.Contains(e.LocationId.Value));
                }

                if (filter.facilityIds != null && filter.facilityIds.Count > 0)
                {
                    var q = query.Join(_appContext.LocationFacilities,
                        res => res.LocationId,
                        fac => fac.LocationId,
                        (res, fac) =>
                            new { Reservation = res, Facility = fac });
                    query = q.Where(e => filter.facilityIds.Contains(e.Facility.FacilityId)).Select(e => e.Reservation).Distinct().AsQueryable();

                }
            }

            var list = new List<SignageBookingDTO>();

            foreach (var e in query)
            {
                var dto = new SignageBookingDTO
                {
                    ID = e.Id,
                    Description = e.LongDescription,
                    BookingBy = e.CreatedByUser != null ? e.CreatedByUser.FullName : string.Empty,
                    InstitutionID = e.InstitutionId,
                    Label = e.ShortDescription,
                    LastUpdateTime = e.UpdatedDate,
                    Status = e.Status,
                    LocationId = e.LocationId
                };

                var start = e.ReservationTimes.Where(f => f.IsActive).OrderBy(f => f.TimeInterval.Hour).ThenBy(f => f.TimeInterval.Minutes).FirstOrDefault().TimeInterval;
                var end = e.ReservationTimes.Where(f => f.IsActive).OrderBy(f => f.TimeInterval.Hour).ThenBy(f => f.TimeInterval.Minutes).LastOrDefault().TimeInterval.ToTimeInterval;
                dto.StartDateTime = dto.EndDateTime = e.StartDateTime.Date;
                dto.StartDateTime = dto.StartDateTime.Value.AddHours(start.Hour).AddMinutes(start.Minutes);
                dto.EndDateTime = dto.EndDateTime.Value.AddHours(end.Hour).AddMinutes(end.Minutes);
                list.Add(dto);
            }

            return list;
        }

        #endregion
        public async Task<BaseOperationResponse> CreateAsync(Reservation reservation)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(reservation);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save reservation!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Reservation reservation)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == reservation.Id);

            f.CopyFrom(reservation);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save reservation!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int reservationId)
        {
            return true; // !await Exists(r => r.Id == reservationId);
        }


        public async Task<BaseOperationResponse> DeleteAsync(int reservationId)
        {
            var result = new BaseOperationResponse();
            var reservation = await GetSingleOrDefaultAsync(r => r.Id == reservationId);

            if (reservation != null)
                return await Delete(reservation);

            result.IsSuccess = false;
            result.Message = "Reservation not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Reservation reservation)
        {
            var result = new BaseOperationResponse();
            SoftDelete(reservation);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete reservation!";
                result.IsSuccess = false;
            }

            return result;
        }

        public List<TimeInterval> GetAllTimeIntervals(TimeIntervalFilter filter = null)
        {
            if (filter != null && filter.IsIncludeReservations)
            {
                var reservations = this.GetAll().AsQueryable();

                if (filter != null)
                {
                    if (filter.LocationId.HasValue)
                    {
                        reservations = reservations.Where(e => e.LocationId == filter.LocationId);
                    }

                    if (filter.StartDate.HasValue)
                    {
                        reservations = reservations.Where(e => e.StartDateTime >= filter.StartDate);
                    }

                    //if (filter.EndDate.HasValue)
                    //{
                    //    reservations = reservations.Where(e => filter.EndDate <= e.EndDateTime);
                    //}
                }

                var reservationTimes = reservations.SelectMany(e => e.ReservationTimes.Where(f => f.IsActive));
                var q = _appContext.TimeIntervals.Where(e => e.IsActive).GroupJoin(reservationTimes,
                            ti => ti.Id,
                            rt => rt.TimeIntervalId,
                            (ti, rt) =>
                                new { TimeInterval = ti, ReservationTimes = rt });

                var timeIntervals = q.Where(e => e.ReservationTimes.Count() == 0).Select(e => e.TimeInterval);

                if (filter != null)
                {
                    if (filter.StartTimeInterval != null)
                    {
                        timeIntervals = timeIntervals.Where(e => e.Hour > filter.StartTimeInterval.Hour ||
                        (e.Hour == filter.StartTimeInterval.Hour && e.Minutes >= filter.StartTimeInterval.Minutes));
                    }

                    if (filter.EndTimeInterval != null)
                    {
                        timeIntervals = timeIntervals.Where(e => e.Hour < filter.EndTimeInterval.Hour ||
                        (e.Hour == filter.EndTimeInterval.Hour && e.Minutes <= filter.EndTimeInterval.Minutes));
                    }
                }

                return timeIntervals.OrderBy(e => e.Hour).ThenBy(e => e.Minutes).ToList();
            }
            else
            {
                return _appContext.TimeIntervals.Where(e => e.IsActive).OrderBy(e => e.Hour).ThenBy(e => e.Minutes).ToList();
            }
        }

        public List<BookingGrid> GetBookingGrid(CalendarFilter filter = null)
        {
            var timeIntervals = GetAllTimeIntervals();
            var reservations = this.GetAll().Where(e => e.IsActive).AsQueryable();

            if (filter != null)
            {
                if (filter.BookedById.HasValue)
                {
                    reservations = reservations.Where(e => filter.BookedById == e.CreatedBy);
                }

                reservations = reservations.Where(e => !filter.Start.HasValue || e.StartDateTime.Date == filter.Start.Value.Date);
            }

            var locReservations = _appContext.Locations.Where(e => e.IsActive && e.LocationType.Name == LocationTypes.Room.ToString()).GroupJoin(reservations,
                            ti => ti.Id,
                            rt => rt.LocationId,
                            (ti, rt) =>
                                new
                                {
                                    Location = ti,
                                    Reservations = rt,
                                    TimeIntervals = rt.SelectMany(f => f.ReservationTimes.Where(e => e.IsActive).Select(x => x.TimeInterval)).Distinct()
                                });

            //var locReservations = reservations.GroupBy(e => e.LocationId)
            //                        .Select(e => new { LocationId = e.Key, Location = e.First().Location,
            //                            TimeIntervals = e.SelectMany(f => f.ReservationTimes.Select(x => x.TimeInterval)).Distinct() });

            if (filter != null)
            {
                if (filter.capacity.HasValue)
                {
                    locReservations = locReservations.Where(e => e.Location.Capacity >= filter.capacity);
                }

                if (filter.locationIds != null && filter.locationIds.Any())
                {
                    locReservations = locReservations.Where(e => filter.locationIds.Any(f => f == e.Location.Id));
                }

                if (filter.facilityIds != null && filter.facilityIds.Any())
                {

                    foreach (var facilityId in filter.facilityIds)
                    {
                        locReservations = locReservations.Where(e => e.Location.LocationFacilities.Any(f => f.FacilityId == facilityId));
                    }

                }
            }

            var list = new List<BookingGridRow>();
            foreach (var loc in locReservations)
            {
                var bookingGridRow = new BookingGridRow();
                bookingGridRow.Location = loc.Location;
                bookingGridRow.LocationTimeIntervals = new List<LocationTimeInterval>();
                foreach (var timeInterval in timeIntervals)
                {
                    var lti = new LocationTimeInterval();
                    lti.TimeInterval = timeInterval;
                    lti.TimeInterval.ToTimeInterval = timeInterval.ToTimeInterval;
                    lti.Reservation = loc.Reservations.FirstOrDefault(e => e.ReservationTimes.Any(f => f.IsActive && f.TimeIntervalId == timeInterval.Id));
                    lti.Selected = loc.TimeIntervals.Any(e => e.Hour == timeInterval.Hour && e.Minutes == timeInterval.Minutes);
                    bookingGridRow.LocationTimeIntervals.Add(lti);
                }


                list.Add(bookingGridRow);
            }

            var grpReservations = list.GroupBy(e => e.Location.ParentLocationId).Select(e => new {
                LocationId = e.Key,
                Rows = e
            });

            var result = new List<BookingGrid>();
            foreach(var grp in grpReservations)
            {
                var bookingGrid = new BookingGrid
                {
                    Rows = grp.Rows.ToList(),
                    Location = grp.Rows.First().Location
                };

                result.Add(bookingGrid);
            }


            return result;

        }

        public async Task<BaseOperationResponse> GetCurrentDetail(CalendarFilter filter = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                var currentTime = DateTime.Now;
                var location = _appContext.Locations.FirstOrDefault(e => e.Id == filter.locationIds.FirstOrDefault());
                var reservationTimes = _appContext.ReservationTimes.Where(e => e.IsActive);


                if (filter != null)
                {
                    reservationTimes = reservationTimes.Where(e => !filter.Start.HasValue || (e.Reservation.IsActive && e.Reservation.StartDateTime.Date == filter.Start.Value.Date));
                    reservationTimes = reservationTimes.Where(e => filter.locationIds.Any(f => f == e.Reservation.LocationId));
                }

                reservationTimes = reservationTimes.OrderBy(e => e.TimeInterval.Hour).ThenBy(e => e.TimeInterval.Minutes);
                var allTimeIntervals = GetAllTimeIntervals();
                var timeSlots = (filter.Start.HasValue && filter.Start.Value.Date > currentTime.Date ? allTimeIntervals :
                        allTimeIntervals.Where(e => (e.Hour >= currentTime.Hour && currentTime.Hour <= e.ToTimeInterval.Hour) //||
                        //((currentTime.Hour == e.Hour && currentTime.Minute >= e.Minutes) && (currentTime.Hour < e.ToTimeInterval.Hour || 
                        //(currentTime.Hour == e.ToTimeInterval.Hour && currentTime.Minute <= e.ToTimeInterval.Minutes)))
                        ))
                        .OrderBy(e => e.Hour).ThenBy(e => e.Minutes);

                bool isAvailable = false;
                bool isReserved = false;
                var availableTimeSlots = new List<TimeInterval>();
                var currentReservation = new CurrentReservationDTO();
                foreach (var timeSlot in timeSlots)
                {
                    var currentTimeSlot = reservationTimes.FirstOrDefault(e => (e.TimeInterval.Hour == timeSlot.Hour && e.TimeInterval.Minutes == timeSlot.Minutes) ||
                                                                                 (e.TimeInterval.ToTimeInterval.Hour == timeSlot.Hour && e.TimeInterval.ToTimeInterval.Minutes == timeSlot.Minutes));
                    if (currentTimeSlot != null)
                    {
                        if (!isAvailable)
                        {
                            if ((currentTime.Hour > currentTimeSlot.TimeInterval.Hour ||
                                (currentTime.Hour == currentTimeSlot.TimeInterval.Hour && currentTime.Minute > currentTimeSlot.TimeInterval.Minutes)) &&
                                ((currentTime.Hour < currentTimeSlot.TimeInterval.ToTimeInterval.Hour ||
                                (currentTime.Hour == currentTimeSlot.TimeInterval.ToTimeInterval.Hour && currentTime.Minute <= currentTimeSlot.TimeInterval.ToTimeInterval.Minutes)))
                                )
                            {
                                currentReservation.Title = currentTimeSlot.Reservation.ShortDescription;
                                currentReservation.Description = currentTimeSlot.Reservation.LongDescription;
                                currentReservation.IsAvailable = false;
                                currentReservation.ReservationId = currentTimeSlot.Reservation.Id;
                                currentReservation.InstitutionId = currentTimeSlot.Reservation.InstitutionId;
                                currentReservation.ReservationStatus = currentTimeSlot.Reservation.ActualEndDateTime.HasValue && currentTimeSlot.Reservation.ActualEndDateTime > DateTime.MinValue ?
                                    BookingStatus.Completed.ToString() : string.IsNullOrEmpty(currentTimeSlot.Reservation.Status) ? BookingStatus.Pending.ToString() : currentTimeSlot.Reservation.Status;
                                currentReservation.IsCheckedIn = currentTimeSlot.Reservation.Status == BookingStatus.CheckedIn.ToString();
                                currentReservation.CompletedAt = currentTimeSlot.Reservation.ActualEndDateTime;
                                currentReservation.IsCompleted = currentTimeSlot.Reservation.ActualEndDateTime.HasValue;

                                if (location != null)
                                {
                                    currentReservation.Status = currentReservation.IsCompleted ? BookingStatus.Completed.ToString() : string.IsNullOrEmpty(location.Status) ? "In Progress" : location.Status;
                                }
                                currentReservation.BookedBy = currentTimeSlot.Reservation.CreatedByUser.FullName;

                                var reservedTimeSlots = currentTimeSlot.Reservation.ReservationTimes.Where(e => e.IsActive).OrderBy(e => e.TimeInterval.Hour).ThenBy(e => e.TimeInterval.Minutes);
                                currentReservation.TimeDisplay = string.Format("{0} - {1}", reservedTimeSlots.FirstOrDefault().TimeInterval.Description, reservedTimeSlots.Last().TimeInterval.ToTimeInterval.Description);
                                isReserved = true;
                                break;
                            }
                            else
                            {
                                if (timeSlot.ToTimeInterval.Hour < currentTime.Hour || (timeSlot.ToTimeInterval.Hour == currentTime.Hour && currentTime.Minute < timeSlot.ToTimeInterval.Minutes)) continue;
                                if (timeSlot.ToTimeInterval.Hour > currentTime.Hour || (timeSlot.ToTimeInterval.Hour == currentTime.Hour && timeSlot.ToTimeInterval.Minutes >= currentTime.Minute))
                                {
                                    if (!reservationTimes.Any(e => (e.TimeInterval.Hour == timeSlot.Hour && e.TimeInterval.Minutes == timeSlot.Minutes)))
                                    {
                                        availableTimeSlots.Add(timeSlot);
                                        isAvailable = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (timeSlot.ToTimeInterval.Hour < currentTime.Hour || (timeSlot.ToTimeInterval.Hour == currentTime.Hour && timeSlot.ToTimeInterval.Minutes < currentTime.Minute)) continue;
                        availableTimeSlots.Add(timeSlot);
                        isAvailable = true;
                    }
                }

                if (isReserved)
                {
                    response.Data = currentReservation;
                }
                else
                {
                    if (availableTimeSlots != null && availableTimeSlots.Any())
                    {
                        var availableTimeSlot = new CurrentAvailableTimeSlot();
                        availableTimeSlot.AvailableFrom = availableTimeSlots.First().Description;
                        availableTimeSlot.AvailableTo = availableTimeSlots.Last().ToTimeInterval.Description;
                        availableTimeSlot.AvailableTimeDisplay = string.Format("{0} - {1}", availableTimeSlot.AvailableFrom, availableTimeSlot.AvailableTo);

                        if (location != null)
                        {
                            availableTimeSlot.Title = location.Name;
                            availableTimeSlot.IsAvailable = true;
                            availableTimeSlot.Status = string.IsNullOrEmpty(location.Status) ? "Available" : location.Status;
                            availableTimeSlot.IsCompleted = false;
                            var nextReservation = GetNextBooking(filter);
                            if (nextReservation.Result != null && nextReservation.Result.Data != null)
                            {
                                var nextRes = nextReservation.Result.Data as NextReservation;
                                availableTimeSlot.InstitutionId = nextRes.InstitutionId;
                                availableTimeSlot.ReservationId = nextRes.ReservationId;
                                availableTimeSlot.TimeDisplay = nextRes.TimeDisplay;
                                availableTimeSlot.BookedBy = nextRes.BookedBy;
                                availableTimeSlot.Description = nextRes.Description;

                                availableTimeSlot.IsCheckedIn = false;
                            }
                        }

                        response.Data = availableTimeSlot;
                    }
                }

                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }


        public async Task<BaseOperationResponse> GetNextBooking(CalendarFilter filter = null)
        {
            var response = new BaseOperationResponse();
            try
            {

                var currentTime = DateTime.Now;
                var reservationTimes = _appContext.ReservationTimes.Where(e => e.IsActive);
                var reservations = GetAll().Where(e => e.IsActive);


                if (filter != null)
                {
                    reservations = reservations.Where(e => !filter.Start.HasValue || e.StartDateTime.Date == filter.Start.Value.Date);
                    reservations = reservations.Where(e => filter.locationIds.Any(f => f == e.LocationId));
                }

                var nextReservation = reservations.Where(e => e.StartDateTime > currentTime && e.ReservationTimes.Where(a => a.IsActive).Any()).OrderBy(e => e.StartDateTime).FirstOrDefault();

                if (nextReservation != null)
                {
                    var nextResDTO = new NextReservation();
                    if (nextReservation.ReservationTimes != null)
                    {
                        nextResDTO.Title = nextReservation.ShortDescription;
                        nextResDTO.Description = nextReservation.LongDescription;
                        nextResDTO.BookedBy = nextReservation.CreatedByUser.FullName;
                        nextResDTO.InstitutionId = nextReservation.InstitutionId;
                        nextResDTO.ReservationId = nextReservation.Id;
                        var timings = nextReservation.ReservationTimes.Where(e => e.IsActive).OrderBy(e => e.TimeInterval.Hour).ThenBy(e => e.TimeInterval.Minutes);
                        nextResDTO.TimeDisplay = string.Format("{0} - {1}", timings.First().TimeInterval.Description, timings.Last().TimeInterval.ToTimeInterval.Description);
                        response.Data = nextResDTO;
                    }
                }

                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        private TimeInterval GetLastTimeInterval(TimeInterval current)
        {
            var allTimings = this.GetAllTimeIntervals();
            var result = allTimings.Where(e => current.Hour > e.Hour || (
            current.Hour == e.Hour && current.Minutes > e.Minutes)).OrderBy(e => e.Hour).ThenBy(e => e.Minutes).LastOrDefault();

            var top2 = allTimings.OrderBy(e => e.Hour).ThenBy(e => e.Minutes).Take(2);
            var intervalInMinutes = (top2.Last().Hour * 60 + top2.Last().Minutes) - (top2.First().Hour * 60 + top2.First().Minutes);
            var nextTiInMinutes = current.Hour * 60 + current.Minutes + intervalInMinutes;
            var ti = new TimeInterval();

            ti.Hour = (int)(nextTiInMinutes / 60);
            ti.Minutes = (nextTiInMinutes % 60);
            ti.Description = string.Format("{0}:{1} {2}", (ti.Hour - 12).ToString().PadLeft(2, '0'), ti.Minutes.ToString().PadLeft(2, '0'), ti.Hour >= 12 ? "PM" : "AM");

            return ti;
        }

        public async Task<BaseOperationResponse> ValidateCheckIn(string userName, string pin, int reservationId)
        {
            var response = new BaseOperationResponse();

            try
            {
                var query = _appContext.Reservations
                       .Include(e => e.CreatedByUser)
                       .Where(e => e.Id == reservationId && e.IsActive && e.CreatedByUser.IsActive)
                       .FirstOrDefault();

                var checkinResult = new CheckInResult()
                {
                    ReservationStatus = query.Status,
                    Message = query == null ? "Booking not found" : "Invalid login"
                };

                if (query != null && query.CreatedByUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                    && query.CreatedByUser.Pin == pin)
                {
                    //check if checkin is done 1hr before start and 15mins after start
                    var currentTime = DateTime.Now;
                    bool isAllowCheckin = false;
                    string checkinMsg = string.Empty;
                    if (query.StartDateTime >= currentTime)
                    {
                        if (query.StartDateTime.Subtract(currentTime).TotalMinutes < 61)
                        {
                            isAllowCheckin = true;
                        }
                        else
                        {
                            checkinMsg = "Too early to checkin";
                        }
                    }
                    else
                    {
                        if (currentTime.Subtract(query.StartDateTime).TotalMinutes < 16)
                        {
                            isAllowCheckin = true;
                        }
                        else
                        {
                            //remove the booking
                            SoftDelete(query);
                            if (await _appContext.SaveChangesAsync() > 0)
                            {
                                checkinMsg = "Checkin is no longer allowed.";
                            }
                        }
                    }

                    if (isAllowCheckin)
                    {
                        query.Status = BookingStatus.CheckedIn.ToString();
                        Update(query);
                        if (await _appContext.SaveChangesAsync() > 0)
                        {
                            checkinResult = new CheckInResult
                            {
                                IsUserValid = true,
                                ReservationStatus = query.Status,
                                Message = "Successful checkin"
                            };
                        }
                    }
                    else
                    {
                        checkinResult = new CheckInResult
                        {
                            IsUserValid = false,
                            ReservationStatus = BookingStatus.Unknown.ToString(),
                            Message = checkinMsg
                        };
                    }
                }

                response.Data = checkinResult;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<BaseOperationResponse> ProcessExpiredBookings()
        {
            var response = new BaseOperationResponse();

            try
            {
                var query = _appContext.Reservations
                       .Include(e => e.CreatedByUser)
                       .Where(e => e.CreatedByUser.IsActive &&
                       e.Status != BookingStatus.CheckedIn.ToString() &&
                       e.Status != BookingStatus.Completed.ToString());

                var currentTime = DateTime.Now;
                foreach (var res in query)
                {
                    if (currentTime.Subtract(res.StartDateTime).TotalMinutes >= 16)
                    {
                        //remove the booking
                        SoftDelete(res);
                    }

                    await _appContext.SaveChangesAsync();
                }

                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<BaseOperationResponse> ValidateExtend(int reservationId, int timeIntervalId)
        {
            var response = new BaseOperationResponse();

            try
            {
                var reservation = await GetByIdAsync(reservationId);
                var lastTi = reservation.ReservationTimes.Where(e => e.IsActive).OrderBy(e => e.TimeInterval.Hour).ThenBy(e => e.TimeInterval.Minutes).LastOrDefault();
                var timeIntervals = lastTi != null ? GetNextAvailableTimeIntervals(lastTi.TimeInterval).OrderBy(e => e.Hour).ThenBy(e => e.Minutes) : null;

                bool isValid = timeIntervals.Any(e => e.Id == timeIntervalId);

                var extendResult = new ExtendResult
                {
                    IsExtendValid = isValid,
                    Message = isValid ? "Successfully extended" : "Extension is not allowed"
                };

                if (isValid)
                {
                    //extend logic here
                    foreach (var ti in timeIntervals)
                    {
                        reservation.ReservationTimes.Add(new ReservationTime
                        {
                            ReservationId = reservationId,
                            TimeIntervalId = ti.Id
                        });

                        if (ti.Id == timeIntervalId) break;
                    }

                    Update(reservation);
                    await _appContext.SaveChangesAsync();
                }

                response.Data = extendResult;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<BaseOperationResponse> ValidateEnd(int reservationId)
        {
            var response = new BaseOperationResponse();

            try
            {
                DateTime currentTime = DateTime.Now;
                var reservation = await GetByIdAsync(reservationId);
                var reservedTimesToRemove = reservation.ReservationTimes
                    .Where(e => e.IsActive && (e.TimeInterval.Hour > currentTime.Hour ||
                    (e.TimeInterval.Hour == currentTime.Hour && e.TimeInterval.Minutes > currentTime.Minute)))
                    .OrderBy(e => e.TimeInterval.Hour).ThenBy(e => e.TimeInterval.Minutes);

                //end logic here
                foreach (var ti in reservedTimesToRemove)
                {
                    ti.IsActive = false;
                    _appContext.ReservationTimes.Update(ti);
                }

                reservation.ActualEndDateTime = DateTime.Now;
                Update(reservation);
                await _appContext.SaveChangesAsync();

                response.Data = new { Message = "Successfully ended." };
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<BaseOperationResponse> GetExtensionTimes(int reservationId)
        {
            var response = new BaseOperationResponse();

            try
            {
                var reservation = await GetByIdAsync(reservationId);
                var lastTi = reservation.ReservationTimes.Where(e => e.IsActive).OrderBy(e => e.TimeInterval.Hour).ThenBy(e => e.TimeInterval.Minutes).LastOrDefault();
                var timeIntervals = lastTi != null ? GetNextAvailableTimeIntervals(lastTi.TimeInterval).Select(e => new
                {
                    TimeIntervalId = e.Id,
                    ReservationId = reservationId,
                    From = string.Format("{0}", lastTi.TimeInterval.ToTimeInterval.Value),
                    To = string.Format("{0}", e.ToTimeInterval.Value),
                    TimeDisplay = string.Format("{0} MIN{1}", Math.Abs((e.ToTimeInterval.Hour * 60 + e.ToTimeInterval.Minutes) - (lastTi.TimeInterval.ToTimeInterval.Hour * 60 + lastTi.TimeInterval.ToTimeInterval.Minutes)),
                                        (Math.Abs((e.ToTimeInterval.Hour * 60 + e.ToTimeInterval.Minutes) - (lastTi.TimeInterval.ToTimeInterval.Hour * 60 + lastTi.TimeInterval.ToTimeInterval.Minutes))) > 1 ? "S" : "")
                }) : null;

                response.Data = timeIntervals;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        private List<TimeInterval> GetNextAvailableTimeIntervals(TimeInterval lastTi)
        {
            DateTime currentTime = DateTime.Now;
            var timeIntervals = GetAllTimeIntervals().Where(e => e.Hour > lastTi.ToTimeInterval.Hour ||
            (e.Hour == lastTi.Hour && e.Minutes > lastTi.Minutes) ||
            (e.Hour == lastTi.ToTimeInterval.Hour && e.Minutes >= lastTi.ToTimeInterval.Minutes))
            .OrderBy(e => e.Hour).ThenBy(e => e.Minutes);

            List<TimeInterval> availableTi = new List<TimeInterval>();
            var todayReservations = GetTodayReservations();
            foreach (var ti in timeIntervals)
            {
                if (todayReservations.SelectMany(e => e.ReservationTimes).Any(e => e.IsActive && e.TimeIntervalId == ti.Id))
                {
                    break;
                }

                availableTi.Add(ti);

                if (Math.Abs((ti.ToTimeInterval.Hour * 60 + ti.ToTimeInterval.Minutes) - (lastTi.ToTimeInterval.Hour * 60 + lastTi.ToTimeInterval.Minutes)) == 120) break; //max 2hrs
            }

            return availableTi;
        }

        private IQueryable<Reservation> GetTodayReservations()
        {
            return _appContext.Reservations.Where(e => e.IsActive && e.StartDateTime.Date >= DateTime.Now.Date);
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
