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
using System.Dynamic;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using Microsoft.AspNetCore.Http;
using AspNet.Security.OpenIdConnect.Primitives;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using AutoMapper;

namespace DAL.Repositories
{
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        private const string _hrTimeFormat = "hh:mm tt";
        private readonly IHttpContextAccessor _httpAccessor;

        public ReservationRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<Reservation> GetByIdAsync(int id)
        {
            return await _appContext.Reservations
                .Include(e => e.CreatedByUser)
                .Include(e => e.UpdatedByUser)
                .Include(e => e.ReservationTimes)
                .Include(e => e.ReservationInvitees)
                .Include(e => e.Location)
                .ThenInclude(e => e.LocationFacilities)
                .Where(e => e.Id == id).FirstOrDefaultAsync();
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
                .Include(e => e.UpdatedByUser)
                .Include(e => e.ReservationContactGroups)
                .Include(e => e.ReservationInvitees)
                .Include(e => e.Location)
                .ThenInclude(e => e.LocationFacilities)
                .Where(e => e.IsActive);

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Status))
                {
                    if (filter.Status.Equals(BookingStatus.InProgress.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(e => (e.Status == BookingStatus.Pending.ToString() || e.Status == BookingStatus.CheckedIn.ToString() ||
                        string.IsNullOrEmpty(e.Status)) && DateTime.Now >= e.StartDateTime && DateTime.Now <= e.EndDateTime);
                    }
                    else if (filter.Status.Equals(BookingStatus.Completed.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(e => e.Status == BookingStatus.Completed.ToString() ||
                        (e.EndDateTime < DateTime.Now));
                    }
                    else if (filter.Status.Equals(BookingStatus.CheckedIn.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(e => e.Status == BookingStatus.CheckedIn.ToString());
                    }

                    query = query.OrderByDescending(e => e.StartDateTime);
                }

                if (filter.InstitutionId.HasValue)
                {
                    query = query.Where(e => e.InstitutionId == filter.InstitutionId);
                }

                if (filter.capacity > 0)
                {
                    //query = query.Where(e => e.Location.);
                }

                if (filter.IsAllEvents)
                {
                    query = query.Where(e => filter.BookedById == e.CreatedBy ||
                            (e.ReservationInvitees.Any(f => f.IsActive &&
                                                ((f.UserId == filter.CurrentUserId) ||
                                                (!string.IsNullOrEmpty(f.Email) && !string.IsNullOrEmpty(filter.Email) &&
                                                filter.Email.Trim().ToLower().Equals(f.Email.ToLower()))))));
                }
                else
                {
                    if (filter.IsForAttendance)
                    {
                        if (filter.CurrentUserId > 0)
                        {
                            query = query.Where(e => e.ReservationInvitees.Any(f => f.IsActive && f.UserId == filter.CurrentUserId));
                        }
                        else if (!string.IsNullOrEmpty(filter.Email))
                        {
                            query = query.Where(e => e.ReservationInvitees.Any(f => f.IsActive && !string.IsNullOrEmpty(f.Email) &&
                                        filter.Email.Trim().ToLower().Equals(f.Email.ToLower())));
                        }
                    }
                    else
                    {
                        if (filter.BookedById.HasValue)
                        {
                            query = query.Where(e => filter.BookedById == e.CreatedBy);
                        }
                    }
                }

                if (filter.Start.HasValue)
                {
                    query = query.Where(e => e.StartDateTime >= filter.Start.Value);
                }

                if (filter.End.HasValue)
                {
                    if (filter.IsForKiosk)
                    {
                        query = query.Where(e => e.StartDateTime <= filter.End.Value);
                    } else
                    {
                        query = query.Where(e => e.EndDateTime <= filter.End.Value);
                    }
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
                    var q = query.Join(_appContext.LocationFacilities.Where(e => e.IsActive),
                        res => res.LocationId,
                        fac => fac.LocationId,
                        (res, fac) =>
                            new { Reservation = res, Facility = fac });
                    query = q.Where(e => filter.facilityIds.Contains(e.Facility.FacilityId)).Select(e => e.Reservation).Distinct().AsQueryable();

                }

                if (filter.IsForKiosk)
                {
                    query = query.Where(e => e.IsKiosk == true);
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

            var reservations = await query.ToListAsync();

            return reservations.OrderByDescending(r => r.StartDateTime).ToList();
        }

        public async Task<List<CalendarEvent>> GetCalendarEvents(CalendarFilter filter = null)
        {
            IQueryable<Reservation> query = _appContext.Reservations
                .Include(e => e.CreatedByUser)
                .Include(e => e.Location)
                .ThenInclude(e => e.LocationFacilities)
                .Where(e => e.IsActive);

            if (filter != null)
            {
                if (filter.InstitutionId.HasValue)
                {
                    query = query.Where(e => filter.InstitutionId == e.InstitutionId);
                }

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
                    query = query.Where(e => DateTime.Compare(e.StartDateTime, filter.StartTime.Value) >= 0);
                }

                if (filter.EndTime.HasValue)
                {
                    query = query.Where(e => DateTime.Compare(e.EndDateTime, filter.EndTime.Value.AddHours(1)) <= 0);
                }

                if (filter.locationIds != null && filter.locationIds.Count > 0)
                {
                    query = query.Where(e => e.LocationId.HasValue && filter.locationIds.Contains(e.LocationId.Value));
                }

                if (filter.facilityIds != null && filter.facilityIds.Count > 0)
                {
                    var q = query.Join(_appContext.LocationFacilities.Where(e => e.IsActive),
                        res => res.LocationId,
                        fac => fac.LocationId,
                        (res, fac) =>
                            new { Reservation = res, Facility = fac });
                    query = q.Where(e => filter.facilityIds.Contains(e.Facility.FacilityId)).Select(e => e.Reservation).Distinct().AsQueryable();

                }
            }

            var reservationsQuery = await query.Where(e => e.LocationId.HasValue && e.Location.IsActive).ToListAsync();
            var events = reservationsQuery.OrderBy(r => r.StartDateTime).ToList().Select(e => new CalendarEvent
            {
                allDay = e.IsAllDay,
                id = e.Id,
                color = e.Status == BookingStatus.Pending.ToString() ? new EventColor { primary = "#e3bc08", secondary = "#FDF1BA" } :
                              e.Status == BookingStatus.Completed.ToString() ? new EventColor { primary = "#b7b7b7", secondary = "#dadada" } : new EventColor { primary = "#1e90ff", secondary = "#D1E8FF" },
                meta = new Dictionary<string, dynamic>
                {
                    {
                        "location", new
                                    {
                                        Id = e.LocationId.Value,
                                        Name = e.Location.Name,
                                        Description = e.Location.Description,
                                        Capacity = e.Location.Capacity,
                                        Facilities = e.Location.LocationFacilities.Where(a=>a.IsActive && a.Facility.IsActive).Select(f => new { Id = f.FacilityId, Name = f.Facility.Name, Description = f.Facility.Description }).ToList()
                                    }
                    },
                    {
                        "reservation", new
                                    {
                                        Id = e.Id,
                                        ShortDescription = e.ShortDescription,
                                        LongDescription = e.LongDescription,
                                        CreatedById = e.CreatedBy,
                                        ReservationPictures = e.ReservationPictures.Where(a => a.IsActive && a.PictureUrl != null).Select(f => f.PictureUrl).ToList(),
                                        CreatedByName = e.SmartRoomSchedule != null ? e.SmartRoomSchedule.UserName :e.CreatedByUser != null ? e.CreatedByUser.FullName : string.Empty,
                                        DisplayTime = e.DisplayTime
                                    }
                    }
                },
                title = string.Format("{0} booked by {1}", e.ShortDescription, e.SmartRoomSchedule != null ? e.SmartRoomSchedule.UserName : e.CreatedByUser != null ? e.CreatedByUser.FullName : "FRS Admin"),
                start = e.StartDateTime.ToString("yyyy-MM-dd HH:mm"),
                end = e.EndDateTime.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

            return events;
        }

        #region Signage Reservation
        public async Task<List<SignageBookingDTO>> GetSignageReservations(CalendarFilter filter = null)
        {
            IQueryable<Reservation> query = _appContext.Reservations
                .Include(e => e.CreatedByUser)
                .Include(e => e.Location)
                .ThenInclude(e => e.LocationFacilities)
                .Where(e => e.IsActive);

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
            var signageQuery = await query.ToListAsync();
            foreach (var e in signageQuery.OrderBy(r => r.StartDateTime).ToList())
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
        public async Task<BaseOperationResponse> CreateAsync(Reservation reservation, string filePath = null)
        {
            var result = new BaseOperationResponse();
            var currentTime = DateTime.Now;
            if ((!reservation.LocationId.HasValue || (await ValidateReservationTime(reservation))) && reservation.StartDateTime >= currentTime.AddMinutes(-5).AddSeconds(-currentTime.Second))
            {
                var respDuplicate = ValidateDuplicate(reservation);
                if (respDuplicate.IsSuccess)
                {
                    reservation.OriginalEndDateTime = reservation.EndDateTime;
                    if (!string.IsNullOrEmpty(reservation.RepeatType) && reservation.RepeatType.ToString() != RepeatTypes.None.ToString())
                    {
                        result = await GenerateRepeatEvents(reservation, reservation.StartDateTime, reservation.EndDateTime);
                    }
                    else
                    {
                        //if (!string.IsNullOrEmpty(filePath))
                        //{
                        //    reservation.kioskImage = new Models.File
                        //    {
                        //        Path = filePath,
                        //        FileName = System.IO.Path.GetFileName(filePath),
                        //        Type = FileType.PNG.ToString()
                        //    };
                        //}

                        var f = await AddAsync(reservation);
                        if (await _appContext.SaveChangesAsync() > 0)
                        {
                            result.Message = "Successfully saved!";
                            result.IsSuccess = true;
                            result.Data = await GetByIdAsync(f.Id);
                            //result.Data = f;
                        }
                        else
                        {
                            result.Message = "Failed to save reservation!";
                            result.IsSuccess = false;
                        }
                    }

                    if (result.IsSuccess)
                    {
                        //check if there are vehicles
                        string format = "yyyy-MM-dd HH:mm";
                        var res = result.Data as Reservation;
                        DateTime dateToday = new DateTime(res.EndDateTime.Year, res.EndDateTime.Month, res.EndDateTime.Day, 23, 59, 59);

                        var vehicles = res.ReservationInvitees.Where(e => !string.IsNullOrEmpty(e.PlateNumber)).Select(e => new VMSVehiclePostRequestModel
                        {
                            cardType = !string.IsNullOrEmpty(e.CardType) ? e.CardType : (e.UserId.HasValue ? VehicleCardType.STAFF.ToString() : VehicleCardType.VISITOR.ToString()),
                            expiryDate = e.ExpiryDate.HasValue && e.ExpiryDate.Value != DateTime.MinValue ? e.ExpiryDate.Value.ToString(format) : dateToday.ToString(format),
                            issueDate = e.IssueDate.HasValue && e.IssueDate.Value != DateTime.MinValue ? e.IssueDate.Value.ToString(format) : res.StartDateTime.AddHours(-1).ToString(format),
                            personName = !string.IsNullOrEmpty(e.Name) ? e.Name : e.User != null ? e.User.FriendlyName : string.Empty,
                            seasonId = VehicleSeasonType.P.ToString() + "_" + e.Id.ToString(),
                            vehicle = e.PlateNumber
                        }).ToList();

                        if(vehicles != null && vehicles.Any())
                        {
                            await RegisterVehicleToVMS(vehicles);
                        }
                    }
                }
                else
                {
                    result = respDuplicate;
                }
            }
            else
            {
                result.Message = "Please check your start and end times";
                result.IsSuccess = false;
            }

            return result;
        }

        private async Task<BaseOperationResponse> GenerateRepeatEvents(Reservation reservation, DateTime currentStartDateTime, DateTime currentEndDateTime, bool isUpdate = false, List<int> deletedReservationIds = null)
        {
            var result = new BaseOperationResponse();
            Enum.TryParse(reservation.RepeatType, out RepeatTypes repeatType);
            //DateTime currentStartDateTime = reservation.StartDateTime;
            //DateTime currentEndDateTime = reservation.EndDateTime;
            reservation.IsRecurring = true;
            string validationMessage = string.Empty;
            List<Reservation> reservations = new List<Reservation>();
            int count = isUpdate ? 0 : 1;
            while (currentStartDateTime.Date <= reservation.RepeatEndDateTime.Value.Date && currentEndDateTime.Date <= reservation.RepeatEndDateTime.Value.Date)
            {
                if (count > 0)
                {
                    Reservation r = new Reservation();
                    r.CopyFrom(reservation);
                    r.CreatedBy = reservation.CreatedBy;
                    r.UpdatedBy = reservation.UpdatedBy;
                    if (reservation.ReservationContactGroups != null)
                    {
                        r.ReservationContactGroups = reservation.ReservationContactGroups.Select(e => new ReservationContactGroup
                        {
                            ContactGroupId = e.ContactGroupId,
                            Email = e.Email
                        }).ToList();
                    }

                    if (reservation.ReservationInvitees != null)
                    {
                        r.ReservationInvitees = reservation.ReservationInvitees.Select(e => new ReservationInvitee
                        {
                            Company = e.Company,
                            ContactGroupId = e.ContactGroupId,
                            Department = e.Department,
                            Designation = e.Designation,
                            Email = e.Email,
                            Name = e.Name,
                            PhoneNumber = e.PhoneNumber,
                            Status = string.IsNullOrEmpty(e.Status) ? ParticipantStatus.Pending.ToString() : e.Status,
                            UserId = e.UserId,
                            PlateNumber = e.PlateNumber,
                            IssueDate = e.IssueDate,
                            ExpiryDate = e.ExpiryDate,
                            CardType = !string.IsNullOrEmpty(e.CardType) ? e.CardType : VehicleCardType.VISITOR.ToString(),
                            VehicleStatus = e.VehicleStatus
                        }).ToList();
                    }

                    r.StartDateTime = currentStartDateTime;
                    r.EndDateTime = currentEndDateTime;
                    if ((await ValidateReservationTime(r)))
                    {
                        var respDuplicate = ValidateDuplicate(r);
                        if (respDuplicate.IsSuccess)
                        {
                            if (reservations.Any() || isUpdate)
                            {
                                r.Id = 0;
                                if (isUpdate)
                                {
                                    r.Id = 0;
                                    r.ParentReservationId = reservation.Id;
                                }
                                else
                                {
                                    r.ParentReservation = reservation;
                                }

                                reservations.Add(r);
                            }
                            else
                            {
                                reservations.Add(reservation);
                            }
                        }
                        else
                        {
                            validationMessage = respDuplicate.Message;
                        }

                    }
                    else
                    {
                        validationMessage = "Some dates are in conflict with other events.";
                    }
                }
                count++;

                switch (repeatType)
                {
                    case RepeatTypes.Daily:
                        currentStartDateTime = currentStartDateTime.AddDays(1);
                        currentEndDateTime = currentEndDateTime.AddDays(1);
                        break;
                    case RepeatTypes.Weekly:
                        currentStartDateTime = currentStartDateTime.AddDays(7);
                        currentEndDateTime = currentEndDateTime.AddDays(7);
                        break;
                    case RepeatTypes.Monthly:
                        currentStartDateTime = currentStartDateTime.AddMonths(1);
                        currentEndDateTime = currentEndDateTime.AddMonths(1);
                        break;
                    case RepeatTypes.Yearly:
                        currentStartDateTime = currentStartDateTime.AddYears(1);
                        currentEndDateTime = currentEndDateTime.AddYears(1);
                        break;
                }
            }

            //switch (repeatType)
            //{
            //    case RepeatTypes.Daily:
            //        while (currentStartDateTime.Date <= reservation.RepeatEndDateTime.Value.Date && currentEndDateTime.Date <= reservation.RepeatEndDateTime.Value.Date)
            //        {

            //            reservation.StartDateTime = currentStartDateTime;
            //            reservation.EndDateTime = currentEndDateTime;
            //            if (ValidateReservationTime(reservation))
            //            {
            //                Reservation r = new Reservation();
            //                r.CopyFrom(reservation);
            //                r.CreatedBy = reservation.CreatedBy;
            //                r.UpdatedBy = reservation.UpdatedBy;
            //                reservations.Add(r);
            //            }
            //            else
            //            {
            //                validationMessage = "Some dates are in conflict with other events.";
            //            }

            //            currentStartDateTime = currentStartDateTime.AddDays(1);
            //            currentEndDateTime = currentEndDateTime.AddDays(1);
            //        }
            //        break;
            //    case RepeatTypes.Weekly:
            //        //currentStartDateTime = currentStartDateTime.AddDays(7);
            //        //currentEndDateTime = currentEndDateTime.AddDays(7);
            //        while (currentStartDateTime.Date <= reservation.RepeatEndDateTime.Value.Date && currentEndDateTime.Date <= reservation.RepeatEndDateTime.Value.Date)
            //        {

            //            reservation.StartDateTime = currentStartDateTime;
            //            reservation.EndDateTime = currentEndDateTime;
            //            if (ValidateReservationTime(reservation))
            //            {
            //                Reservation r = new Reservation();
            //                r.CopyFrom(reservation);
            //                r.CreatedBy = reservation.CreatedBy;
            //                r.UpdatedBy = reservation.UpdatedBy;
            //                reservations.Add(r);
            //            }
            //            else
            //            {
            //                validationMessage = "Some dates are in conflict with other events.";
            //            }

            //            currentStartDateTime = currentStartDateTime.AddDays(7);
            //            currentEndDateTime = currentEndDateTime.AddDays(7);
            //        }
            //        break;
            //    case RepeatTypes.Monthly:
            //        //currentStartDateTime = currentStartDateTime.AddMonths(1);
            //        //currentEndDateTime = currentEndDateTime.AddMonths(1);
            //        while (currentStartDateTime.Date <= reservation.RepeatEndDateTime.Value.Date && currentEndDateTime.Date <= reservation.RepeatEndDateTime.Value.Date)
            //        {

            //            reservation.StartDateTime = currentStartDateTime;
            //            reservation.EndDateTime = currentEndDateTime;
            //            if (ValidateReservationTime(reservation))
            //            {
            //                Reservation r = new Reservation();
            //                r.CopyFrom(reservation);
            //                r.CreatedBy = reservation.CreatedBy;
            //                r.UpdatedBy = reservation.UpdatedBy;
            //                reservations.Add(r);
            //            }
            //            else
            //            {
            //                validationMessage = "Some dates are in conflict with other events.";
            //            }

            //            currentStartDateTime = currentStartDateTime.AddMonths(1);
            //            currentEndDateTime = currentEndDateTime.AddMonths(1);
            //        }
            //        break;
            //    case RepeatTypes.Yearly:
            //        //currentStartDateTime = currentStartDateTime.AddYears(1);
            //        //currentEndDateTime = currentEndDateTime.AddYears(1);
            //        while (currentStartDateTime.Date <= reservation.RepeatEndDateTime.Value.Date && currentEndDateTime.Date <= reservation.RepeatEndDateTime.Value.Date)
            //        {

            //            reservation.StartDateTime = currentStartDateTime;
            //            reservation.EndDateTime = currentEndDateTime;
            //            if (ValidateReservationTime(reservation))
            //            {
            //                Reservation r = new Reservation();
            //                r.CopyFrom(reservation);
            //                r.CreatedBy = reservation.CreatedBy;
            //                r.UpdatedBy = reservation.UpdatedBy;
            //                reservations.Add(r);
            //            }
            //            else
            //            {
            //                validationMessage = "Some dates are in conflict with other events.";
            //            }

            //            currentStartDateTime = currentStartDateTime.AddYears(1);
            //            currentEndDateTime = currentEndDateTime.AddYears(1);
            //        }
            //        break;
            //}


            AddRange(reservations);
            //reservations = reservations.OrderBy(e => e.StartDateTime).ToList();
            //var reservationToSave = isUpdate ? reservation : reservations.First();
            //reservationToSave.ChildReservations = reservations.Where(e => e.StartDateTime > reservationToSave.StartDateTime).ToList();
            //Add(reservationToSave);
            int savedCount = await _appContext.SaveChangesAsync();
            if (savedCount > 0)
            {
                result.Message = "Successfully saved!" + validationMessage;
                result.IsSuccess = true;
                result.Data = reservation;
            }
            else
            {
                result.Message = "Failed to save reservation!" + validationMessage;
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Reservation reservation, string recurApplyChangesType = null, string filePath = null)
        {
            var result = new BaseOperationResponse();

            string validationMessage = string.Empty;
            var addedInvitees = new List<ReservationInvitee>();
            var addedPictures = new List<ReservationPicture>();
            var currentTime = DateTime.Now;
            bool isGenerateNewEvents = false;
            int? oldParentReservationId = null;

            var picturesToDelete = this._appContext.ReservationPictures.Where(e => e.ReservationId == reservation.Id &&
                            (!reservation.ReservationPictures.Any(r => r.PictureUrl == e.PictureUrl)));

            foreach (var picToDelete in picturesToDelete)
            {
                picToDelete.IsActive = false;
                this._appContext.ReservationPictures.Update(picToDelete);
            }

            reservation.ReservationPictures = reservation.ReservationPictures.GroupBy(x => x.PictureUrl).Select(x => x.FirstOrDefault()).ToList();
            foreach (var picToInsert in reservation.ReservationPictures)
            {
                var ent = this._appContext.ReservationPictures.FirstOrDefault(r => r.ReservationId == reservation.Id && r.IsActive &&
                (r.PictureUrl == picToInsert.PictureUrl));

                if (ent != null)
                {
                    bool isToBeAdded = !ent.IsActive;

                    ent.IsActive = true;
                    ent.PictureUrl = picToInsert.PictureUrl;
                    
                    this._appContext.ReservationPictures.Update(ent);
                    if (addedPictures != null && isToBeAdded)
                    {
                        addedPictures.Add(picToInsert);
                    }
                }
                else
                {
                    picToInsert.ReservationId = reservation.Id;
                    this._appContext.ReservationPictures.Add(picToInsert);
                    if (addedInvitees != null)
                    {
                        addedPictures.Add(picToInsert);
                    }
                }

            }



            if ((!reservation.LocationId.HasValue || (await ValidateReservationTime(reservation)))
                //&& (reservation.Status == BookingStatus.CheckedIn.ToString() ||
                //reservation.Status == BookingStatus.InProgress.ToString() ||
                //reservation.StartDateTime >= currentTime.AddMinutes(-5).AddSeconds(-currentTime.Second))
                )
            {
                var respDuplicate = ValidateDuplicate(reservation);
                if (respDuplicate.IsSuccess)
                {
                    var f = await GetSingleOrDefaultAsync(e => e.Id == reservation.Id);
                    bool isChangeParentReservationId = true;
                    bool isOldRecurring = f.IsRecurring;
                    isGenerateNewEvents = f.RepeatType != reservation.RepeatType ||
                                        (f.RepeatEndDateTime.HasValue && reservation.RepeatEndDateTime.HasValue &&
                                        f.RepeatEndDateTime.Value.Date != reservation.RepeatEndDateTime.Value.Date);
                    oldParentReservationId = isOldRecurring ? (f.ParentReservationId.HasValue ? f.ParentReservationId : f.Id) : null;

                    reservation.OriginalEndDateTime = reservation.EndDateTime;

                    //if (!string.IsNullOrEmpty(filePath))
                    //{
                    //    if (f.kioskImage == null)
                    //    {
                    //        //TODO: check why EF Core is not loading the Icon property; interim solution
                    //        var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == f.FileId);
                    //        if (icon == null)
                    //        {
                    //            f.kioskImage = new Models.File();
                    //        }
                    //        else
                    //        {
                    //            f.kioskImage = icon;
                    //            f.FileId = icon.Id;
                    //        }
                    //    }

                    //    f.kioskImage.Path = filePath;
                    //    f.kioskImage.FileName = System.IO.Path.GetFileName(filePath);
                    //    f.kioskImage.Type = FileType.Icon.ToString();
                    //}

                    //var oldFileId = f.FileId;
                    f.CopyFrom(reservation);
                    //f.FileId = oldFileId;
                    f.IsRecurring = f.RepeatType != RepeatTypes.None.ToString();
                    f.CreatedBy = reservation.CreatedBy;
                    f.UpdatedBy = reservation.UpdatedBy;
                    f.RepeatEndDateTime = f.IsRecurring ? reservation.RepeatEndDateTime : null;
                    UpdateReservation(reservation, ref addedInvitees);
                    Update(f);

                    IQueryable<Reservation> allReservations = null;
                    if (!string.IsNullOrEmpty(recurApplyChangesType) && recurApplyChangesType.ToUpper() == RecurEventType.ALL_EVENTS.ToString())
                    {
                        //this won't happen
                        //get all relative reservations
                        allReservations = this._appContext.Reservations.Where(e => e.IsActive
                        && e.Status != BookingStatus.Completed.ToString()
                        && e.Status != BookingStatus.Cancelled.ToString()
                        && e.Status != BookingStatus.Released.ToString()
                        && ((!e.ParentReservationId.HasValue && e.ParentReservationId == reservation.Id) ||
                            (e.ParentReservationId.HasValue && e.ParentReservationId == oldParentReservationId) ||
                            (e.Id == oldParentReservationId)));
                        isChangeParentReservationId = false;
                    }
                    else if (!string.IsNullOrEmpty(recurApplyChangesType) && recurApplyChangesType.ToUpper() == RecurEventType.OTHER_EVENTS.ToString())
                    {
                        allReservations = this._appContext.Reservations.Where(e => e.IsActive
                        && e.Status != BookingStatus.Completed.ToString()
                        && e.Status != BookingStatus.Cancelled.ToString()
                        && e.Status != BookingStatus.Released.ToString()
                        && ((!e.ParentReservationId.HasValue && e.ParentReservationId == reservation.Id) ||
                            (e.ParentReservationId.HasValue && e.ParentReservationId == oldParentReservationId))
                        && e.StartDateTime > reservation.StartDateTime);
                    }

                    if (allReservations == null || !allReservations.Any())
                    {
                        var dummyInviteeList = new List<ReservationInvitee>();
                        UpdateReservation(reservation, ref dummyInviteeList);
                        if (!string.IsNullOrEmpty(reservation.RepeatType) && f.IsRecurring)
                        {
                            result = await GenerateRepeatEvents(reservation, reservation.StartDateTime, reservation.EndDateTime, true);
                        }
                    }
                    else
                    {
                        if (isGenerateNewEvents && f.IsRecurring)
                        {
                            //soft delete old events
                            //var toDelete = _appContext.Reservations.Where(e => oldParentReservationId.HasValue &&
                            //            e.ParentReservationId == oldParentReservationId);
                            SoftDeleteRange(allReservations);
                            await _appContext.SaveChangesAsync();
                            result = await GenerateRepeatEvents(reservation, reservation.StartDateTime, reservation.EndDateTime, true);
                        }
                        else
                        {
                            if (!f.IsRecurring)
                            {
                                var toDelete = _appContext.Reservations.Where(e => e.Id != f.Id &&
                                        (e.Id == oldParentReservationId || e.ParentReservationId == oldParentReservationId) && e.IsActive
                                        && e.Status != BookingStatus.Completed.ToString()
                                        && e.Status != BookingStatus.Cancelled.ToString()
                                        && e.Status != BookingStatus.Released.ToString());
                                SoftDeleteRange(toDelete);
                            }
                            else
                            {
                                //just reassign the parent reservation id to the reservation being updated
                                var dummyInviteeList = new List<ReservationInvitee>();

                                foreach (var res in allReservations)
                                {
                                    var id = res.Id;
                                    var oldStartDateTime = res.StartDateTime;
                                    var oldEndDateTime = res.EndDateTime;
                                    res.CopyFrom(f);
                                    res.Id = id;
                                    res.StartDateTime = oldStartDateTime;
                                    res.OriginalEndDateTime = res.EndDateTime = oldEndDateTime;
                                    res.ParentReservationId = isChangeParentReservationId ? (f.IsRecurring ? reservation.Id : (int?)null) : oldParentReservationId;
                                    res.ReservationInvitees.Clear();
                                    res.ReservationInvitees = new List<ReservationInvitee>();

                                    res.ReservationContactGroups = new List<ReservationContactGroup>();

                                    if (reservation.ReservationContactGroups != null)
                                    {
                                        res.ReservationContactGroups = reservation.ReservationContactGroups.Select(e => new ReservationContactGroup
                                        {
                                            ContactGroupId = e.ContactGroupId,
                                            Email = e.Email
                                        }).ToList();
                                    }

                                    if (reservation.ReservationInvitees != null)
                                    {
                                        res.ReservationInvitees = reservation.ReservationInvitees.Select(e => new ReservationInvitee
                                        {
                                            Company = e.Company,
                                            ContactGroupId = e.ContactGroupId,
                                            Department = e.Department,
                                            Designation = e.Designation,
                                            Email = e.Email,
                                            Name = e.Name,
                                            PhoneNumber = e.PhoneNumber,
                                            Status = string.IsNullOrEmpty(e.Status) ? ParticipantStatus.Pending.ToString() : e.Status,
                                            UserId = e.UserId,
                                            PlateNumber = e.PlateNumber,
                                            IssueDate = e.IssueDate,
                                            ExpiryDate = e.ExpiryDate,
                                            CardType = !string.IsNullOrEmpty(e.CardType) ? e.CardType : VehicleCardType.VISITOR.ToString(),
                                            VehicleStatus = e.VehicleStatus
                                        }).ToList();
                                    }

                                    UpdateReservation(res, ref dummyInviteeList);
                                    Update(res);

                                }
                            }
                        }
                    }

                    //var inviteesToInsert = this._appContext.ReservationInvitees.Where(e => e.ReservationId == reservation.Id && !reservation.ReservationInvitees.Any(r => r.UserId == e.UserId));
                    if (isOldRecurring && isChangeParentReservationId)
                    {
                        f.ParentReservationId = null;
                        Update(f);
                    }

                    if (result.IsSuccess || await _appContext.SaveChangesAsync() > 0)
                    {
                        result.Message = "Successfully saved!";
                        result.IsSuccess = true;
                        result.Data = new ReservationUpdateResponseData
                        {
                            Reservation = f,
                            Invitees = addedInvitees
                        };

                        //check if there are vehicles
                        string format = "yyyy-MM-dd HH:mm";// "yyyy -mm-dd hh:mm:ss";
                        var res = f;
                        DateTime dateToday = new DateTime(res.EndDateTime.Year, res.EndDateTime.Month, res.EndDateTime.Day, 23, 59, 59);

                        var vehicles = res.ReservationInvitees.Where(e => !string.IsNullOrEmpty(e.PlateNumber)).Select(e => new VMSVehiclePostRequestModel
                        {
                            cardType = !string.IsNullOrEmpty(e.CardType) ? e.CardType : (e.UserId.HasValue ? VehicleCardType.STAFF.ToString() : VehicleCardType.VISITOR.ToString()),
                            expiryDate = e.ExpiryDate.HasValue && e.ExpiryDate.Value != DateTime.MinValue ? e.ExpiryDate.Value.ToString(format) : dateToday.ToString(format),
                            issueDate = e.IssueDate.HasValue && e.IssueDate.Value != DateTime.MinValue ? e.IssueDate.Value.ToString(format) : res.StartDateTime.AddHours(-1).ToString(format),
                            personName = !string.IsNullOrEmpty(e.Name) ? e.Name : e.User != null ? e.User.FriendlyName : string.Empty,
                            seasonId = VehicleSeasonType.P.ToString() + "_" + e.Id.ToString(),
                            vehicle = e.PlateNumber
                        }).ToList();

                        if (vehicles != null && vehicles.Any())
                        {
                            await RegisterVehicleToVMS(vehicles);
                        }
                    }
                    else
                    {
                        result.Message = "Failed to save reservation! " + validationMessage;
                        result.IsSuccess = false;
                    }
                }
                else
                {
                    result = respDuplicate;
                }
            }
            else
            {
                if (reservation.Status == BookingStatus.Released.ToString() ||
                    reservation.Status == BookingStatus.Completed.ToString() ||
                    reservation.Status == BookingStatus.Cancelled.ToString())
                {
                    result.Message = "Booking has already been released, completed or cancelled.";
                }
                else
                {
                    result.Message = "Please check your start and end times.";
                }

                result.IsSuccess = false;
            }

            return result;
        }

        private void UpdateReservation(Reservation reservation, ref List<ReservationInvitee> addedInvitees)
        {
            var inviteesToDelete = this._appContext.ReservationInvitees.Where(e => e.ReservationId == reservation.Id &&
                            (!reservation.ReservationInvitees.Any(r => r.UserId == e.UserId) ||
                            !reservation.ReservationInvitees.Any(r =>
                                (!string.IsNullOrEmpty(r.Email) && !string.IsNullOrEmpty(e.Email) && r.Email.Trim().ToUpper() == e.Email.Trim().ToUpper()))));

            foreach (var invToDelete in inviteesToDelete)
            {
                invToDelete.IsActive = false;
                this._appContext.ReservationInvitees.Update(invToDelete);
            }

            reservation.ReservationInvitees = reservation.ReservationInvitees.GroupBy(x => x.Email).Select(x => x.FirstOrDefault()).ToList();
            foreach (var invToInsert in reservation.ReservationInvitees)
            {
                var ent = this._appContext.ReservationInvitees.FirstOrDefault(r => r.ReservationId == reservation.Id && r.IsActive &&
                ((invToInsert.UserId.HasValue && r.UserId == invToInsert.UserId) || r.Email == invToInsert.Email));

                if (ent != null)
                {
                    bool isToBeAdded = !ent.IsActive;
                    //if (!ent.IsActive)
                    //{
                    ent.IsActive = true;
                    ent.Name = invToInsert.Name;
                    ent.PlateNumber = invToInsert.PlateNumber;
                    ent.Company = invToInsert.Company;
                    ent.Department = invToInsert.Department;
                    ent.Designation = invToInsert.Designation;
                    ent.PhoneNumber = invToInsert.PhoneNumber;
                    ent.Email = invToInsert.Email;
                    ent.IssueDate = invToInsert.IssueDate;
                    ent.ExpiryDate = invToInsert.ExpiryDate;
                    ent.CardType = !string.IsNullOrEmpty(invToInsert.CardType) ? invToInsert.CardType : VehicleCardType.VISITOR.ToString();
                    ent.VehicleStatus = invToInsert.VehicleStatus;

                    this._appContext.ReservationInvitees.Update(ent);
                    if (addedInvitees != null && isToBeAdded)
                    {
                        addedInvitees.Add(invToInsert);
                    }
                    //}
                }
                else
                {
                    invToInsert.ReservationId = reservation.Id;
                    this._appContext.ReservationInvitees.Add(invToInsert);
                    if (addedInvitees != null)
                    {
                        addedInvitees.Add(invToInsert);
                    }
                }

            }

            //contact groups
            var cgToDelete = this._appContext.ReservationContactGroups.Where(e => e.ReservationId == reservation.Id &&
                            (!reservation.ReservationContactGroups.Any(r => r.ContactGroupId == e.ContactGroupId)));

            foreach (var entToDelete in cgToDelete)
            {
                entToDelete.IsActive = false;
                this._appContext.ReservationContactGroups.Update(entToDelete);

                //delete members in the invitees too
                var cgMembersToDelete = this._appContext.ReservationInvitees.Where(e => e.ReservationId == reservation.Id &&
                            entToDelete.ContactGroupId == e.ContactGroupId);

                foreach (var cgMemberToDelete in cgMembersToDelete)
                {
                    cgMemberToDelete.IsActive = false;
                    this._appContext.ReservationInvitees.Update(cgMemberToDelete);
                }
            }

            foreach (var entToInsert in reservation.ReservationContactGroups)
            {
                var ent = this._appContext.ReservationContactGroups.FirstOrDefault(r => r.ReservationId == reservation.Id && r.IsActive &&
                                (entToInsert.ContactGroupId == r.ContactGroupId));
                if (ent != null)
                {
                    if (!ent.IsActive)
                    {
                        ent.IsActive = true;
                        this._appContext.ReservationContactGroups.Update(ent);
                    }
                }
                else
                {
                    entToInsert.ReservationId = reservation.Id;
                    this._appContext.ReservationContactGroups.Add(entToInsert);
                }
            }
        }

        public async Task<bool> TestCanDeleteAsync(int reservationId)
        {
            return true; // !await Exists(r => r.Id == reservationId);
        }


        public async Task<BaseOperationResponse> DeleteAsync(int reservationId, int? cancelledBy, string reason, string recurApplyChangesType, bool isCancel = false)
        {
            var result = new BaseOperationResponse();
            var reservation = await GetSingleOrDefaultAsync(r => r.Id == reservationId);

            if (reservation != null)
            {
                if (isCancel)
                {
                    reservation.CancelReason = reason;
                    reservation.CancelledBy = cancelledBy;
                    reservation.CancelledOn = DateTime.Now;
                    reservation.Status = BookingStatus.Cancelled.ToString();
                }
                return await Delete(reservation, recurApplyChangesType);
            }

            result.IsSuccess = false;
            result.Message = "Reservation not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Reservation reservation, string recurApplyChangesType = null)
        {
            var result = new BaseOperationResponse();
            var oldParentReservationId = reservation.ParentReservationId.HasValue ? reservation.ParentReservationId.Value : reservation.Id;
            if (!string.IsNullOrEmpty(recurApplyChangesType) && recurApplyChangesType != RecurEventType.THIS_EVENT.ToString())
            {
                IQueryable<Reservation> allReservations = null;
                if (!string.IsNullOrEmpty(recurApplyChangesType) && recurApplyChangesType.ToUpper() == RecurEventType.ALL_EVENTS.ToString())
                {
                    //this won't happen
                    //get all relative reservations
                    allReservations = this._appContext.Reservations.Where(e => e.IsActive
                    && e.Status != BookingStatus.Completed.ToString()
                    && e.Status != BookingStatus.Cancelled.ToString()
                    && e.Status != BookingStatus.Released.ToString()
                    && ((!reservation.ParentReservationId.HasValue && e.ParentReservationId == reservation.Id) ||
                        (reservation.ParentReservationId.HasValue && e.ParentReservationId == oldParentReservationId) ||
                        (e.Id == oldParentReservationId)));
                }
                else if (!string.IsNullOrEmpty(recurApplyChangesType) && recurApplyChangesType.ToUpper() == RecurEventType.OTHER_EVENTS.ToString())
                {
                    allReservations = this._appContext.Reservations.Where(e => e.IsActive
                    && e.Status != BookingStatus.Completed.ToString()
                    && e.Status != BookingStatus.Cancelled.ToString()
                    && e.Status != BookingStatus.Released.ToString()
                    && ((!reservation.ParentReservationId.HasValue && e.ParentReservationId == reservation.Id) ||
                        (reservation.ParentReservationId.HasValue && e.ParentReservationId == oldParentReservationId))
                    && e.StartDateTime > reservation.StartDateTime);
                }

                if (allReservations != null)
                {
                    SoftDeleteRange(allReservations);
                    //delete invitees/contact groups
                    var recurInvitees = _appContext.ReservationInvitees.Where(e => allReservations.Any(f => f.Id == e.ReservationId));
                    await recurInvitees.ForEachAsync(e => { e.IsActive = false; });

                    var recurContactGroups = _appContext.ReservationContactGroups.Where(e => allReservations.Any(f => f.Id == e.ReservationId));
                    await recurContactGroups.ForEachAsync(e => { e.IsActive = false; });
                }

            }

            SoftDelete(reservation);

            //delete invitees/contact groups
            var invitees = _appContext.ReservationInvitees.Where(e => e.ReservationId == reservation.Id);
            await invitees.ForEachAsync(e => { e.IsActive = false; });

            var contactGroups = _appContext.ReservationContactGroups.Where(e => e.ReservationId == reservation.Id);
            await contactGroups.ForEachAsync(e => { e.IsActive = false; });

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

        public List<BookingGridRow> GetBookingGrid(CalendarFilter filter = null)
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

            var locReservations = _appContext.Locations.Where(e => e.IsActive).GroupJoin(reservations,
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
                    if (lti.Reservation != null && lti.Reservation.ReservationInvitees != null)
                    {
                        lti.Reservation.ReservationInvitees = lti.Reservation.ReservationInvitees.Where(e => e.IsActive).ToList();
                    }
                    lti.Selected = loc.TimeIntervals.Any(e => e.Hour == timeInterval.Hour && e.Minutes == timeInterval.Minutes);
                    bookingGridRow.LocationTimeIntervals.Add(lti);
                }


                list.Add(bookingGridRow);
            }

            return list;
        }

        public async Task<LocationApiInformation> GetLocationDetail(CalendarFilter filter = null)
        {
            var locationDetail = new LocationApiInformation();
            try
            {
                var currentTime = DateTime.Now;
                var location = await _appContext.Locations
                                .Include(e => e.LocationFacilities)
                                .FirstOrDefaultAsync(e => e.Id == filter.locationIds.FirstOrDefault());
                locationDetail.location_name = location.Name;
                locationDetail.location_capacity = location.Capacity;
                locationDetail.Facilities = location.LocationFacilities.Where(e => e.IsActive).Select(e => new FacilitySimple
                {
                    Description = e.Facility.Description,
                    Link = e.Facility.Link,
                    Name = e.Facility.Name,
                    Path = e.Facility.Icon.Path,
                    Filename = e.Facility.Icon.FileName
                }).ToList();

                var availableTimeSlot = new CurrentAvailableTimeSlot();
                var reservations = _appContext.Reservations.Where(e => e.IsActive);
                if (filter != null)
                {
                    reservations = reservations.Where(e => !filter.Start.HasValue || (e.IsActive && e.StartDateTime.Date == filter.Start.Value.Date));
                    reservations = reservations.Where(e => filter.locationIds.Any(f => f == e.LocationId));
                }

                //check if there is an reservation during currentTime
                var currentRes = await reservations.FirstOrDefaultAsync(e => currentTime >= e.StartDateTime && currentTime <= e.EndDateTime);
                if (currentRes != null)
                {
                    locationDetail.event_id = currentRes.Id;
                    locationDetail.current_reservation_description = currentRes.ShortDescription;
                    if (location != null)
                    {
                        locationDetail.current_reservation_status = currentRes.ActualEndDateTime.HasValue ? BookingStatus.Completed.ToString() : string.IsNullOrEmpty(location.Status) ? "In Progress" : location.Status;
                    }
                    locationDetail.current_reservation_organiser = currentRes.CreatedByUser != null ? currentRes.CreatedByUser.FullName : string.Empty;
                    locationDetail.current_reservation_start_time = currentRes.StartDateTime;
                    locationDetail.current_reservation_end_time = currentRes.EndDateTime;
                    locationDetail.current_reservation_time_display = string.Format("{0} - {1}", currentRes.StartDateTime.ToString(_hrTimeFormat), currentRes.EndDateTime.ToString(_hrTimeFormat));
                }
                else
                {
                    //this means room is free to book
                    availableTimeSlot.AvailableFrom = currentTime.ToString(_hrTimeFormat);

                    if (location != null)
                    {
                        var institution = location.LocationInstitutions.FirstOrDefault(e => e.IsActive);
                        locationDetail.current_reservation_status = string.IsNullOrEmpty(location.Status) ? "Available" : location.Status;

                        TimeSpan result = TimeSpan.FromHours(institution != null && institution.Institution.EndTime > 0 ? institution.Institution.EndTime : 21);
                        DateTime time = DateTime.Today.Add(result);

                        string fromTimeString = time.ToString(_hrTimeFormat);

                        availableTimeSlot.AvailableTo = fromTimeString;

                    }

                    //locationDetail.current_reservation_time_display = string.Format("{0} - {1}", availableTimeSlot.AvailableFrom, availableTimeSlot.AvailableTo);

                }

                var nextReservation = GetNextBooking(filter);
                if (nextReservation.Result != null && nextReservation.Result.Data != null)
                {
                    var nextRes = nextReservation.Result.Data as NextReservation;
                    locationDetail.event_id = nextRes.ReservationId;
                    locationDetail.next_reservation_start_time = nextRes.StartDateTime;
                    locationDetail.next_reservation_end_time = nextRes.EndDateTime;
                    locationDetail.next_reservation_time_display = nextRes.TimeDisplay;
                    locationDetail.next_reservation_organiser = nextRes.BookedBy;
                    locationDetail.next_reservation_description = nextRes.Description;

                    if (string.IsNullOrEmpty(locationDetail.current_reservation_time_display))
                    {
                        locationDetail.current_reservation_start_time = nextRes.StartDateTime;
                        locationDetail.current_reservation_end_time = nextRes.EndDateTime;
                        locationDetail.current_reservation_time_display = string.Format("Until {0}", nextRes.StartDateTime.ToString(_hrTimeFormat));
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(locationDetail.current_reservation_time_display))
                    {
                        locationDetail.current_reservation_time_display = string.Format("Until {0}", availableTimeSlot.AvailableTo);
                    }
                }
            }
            catch (Exception ex)
            {
                locationDetail = null;
            }

            return locationDetail;
        }

        public async Task<BaseOperationResponse> GetCurrentDetail(CalendarFilter filter = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                var currentTime = DateTime.Now;
                var location = await _appContext.Locations.FirstOrDefaultAsync(e => e.Id == filter.locationIds.FirstOrDefault());
                var reservations = _appContext.Reservations.Where(e => e.IsActive);


                if (filter != null)
                {
                    reservations = reservations.Where(e => !filter.Start.HasValue || (e.IsActive && e.StartDateTime.Date == filter.Start.Value.Date));
                    reservations = reservations.Where(e => filter.locationIds.Any(f => f == e.LocationId));
                }

                var currentReservation = new CurrentReservationDTO();

                //check if there is an reservation during currentTime
                var currentRes = reservations.FirstOrDefault(e => currentTime >= e.StartDateTime && currentTime <= e.EndDateTime);
                if (currentRes != null)
                {
                    currentReservation.Title = currentRes.ShortDescription;
                    currentReservation.Description = currentRes.LongDescription;
                    currentReservation.IsAvailable = false;
                    currentReservation.ReservationId = currentRes.Id;
                    currentReservation.InstitutionId = currentRes.InstitutionId;
                    currentReservation.ReservationStatus = currentRes.ActualEndDateTime.HasValue && currentRes.ActualEndDateTime > DateTime.MinValue ?
                        BookingStatus.Completed.ToString() : string.IsNullOrEmpty(currentRes.Status) ? BookingStatus.Pending.ToString() : currentRes.Status;
                    currentReservation.IsCheckedIn = currentRes.Status == BookingStatus.CheckedIn.ToString();
                    currentReservation.CompletedAt = currentRes.ActualEndDateTime;
                    currentReservation.IsCompleted = currentRes.ActualEndDateTime.HasValue;

                    if (location != null)
                    {
                        currentReservation.Status = currentReservation.IsCompleted ? BookingStatus.Completed.ToString() : string.IsNullOrEmpty(location.Status) ? "In Progress" : location.Status;
                    }
                    currentReservation.BookedBy = currentRes.SmartRoomSchedule != null ? currentRes.SmartRoomSchedule.UserName : currentRes.CreatedByUser != null ? currentRes.CreatedByUser.FullName : string.Empty;
                    currentReservation.TimeDisplay = string.Format("{0} - {1}", currentRes.StartDateTime.ToString(_hrTimeFormat), currentRes.EndDateTime.ToString(_hrTimeFormat));
                    response.Data = currentReservation;
                }
                else
                {
                    //this means room is free to book
                    var availableTimeSlot = new CurrentAvailableTimeSlot();
                    availableTimeSlot.AvailableFrom = currentTime.ToString(_hrTimeFormat);

                    if (location != null)
                    {
                        var institution = location.LocationInstitutions.FirstOrDefault(e => e.IsActive);
                        availableTimeSlot.Title = location.Name;
                        availableTimeSlot.IsAvailable = true;
                        availableTimeSlot.Status = string.IsNullOrEmpty(location.Status) ? "Available" : location.Status;
                        availableTimeSlot.IsCompleted = false;

                        TimeSpan result = TimeSpan.FromHours(institution != null && institution.Institution.EndTime > 0 ? institution.Institution.EndTime : 21);
                        DateTime time = DateTime.Today.Add(result);

                        string fromTimeString = time.ToString(_hrTimeFormat);

                        availableTimeSlot.AvailableTo = fromTimeString;
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
                            availableTimeSlot.AvailableTo = nextRes.StartDateTime.ToString(_hrTimeFormat);
                        }
                    }
                    availableTimeSlot.AvailableTimeDisplay = string.Format("{0} - {1}", availableTimeSlot.AvailableFrom, availableTimeSlot.AvailableTo);
                    response.Data = availableTimeSlot;
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

        public async Task<BaseOperationResponse> GetCurrentDetail_Old(CalendarFilter filter = null)
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
                //var reservationTimes = _appContext.ReservationTimes.Where(e => e.IsActive);
                var reservations = GetAll().Where(e => e.IsActive && (
                                    e.Status != BookingStatus.Completed.ToString() ||
                                    e.Status != BookingStatus.Cancelled.ToString() ||
                                    e.Status != BookingStatus.Released.ToString()));


                if (filter != null)
                {
                    reservations = reservations.Where(e => !filter.Start.HasValue || e.StartDateTime.Date == filter.Start.Value.Date);
                    reservations = reservations.Where(e => filter.locationIds.Any(f => f == e.LocationId));
                }

                var nextReservation = reservations.Where(e => e.StartDateTime > currentTime.AddSeconds(-currentTime.Second)).OrderBy(e => e.StartDateTime).FirstOrDefault();

                if (nextReservation != null)
                {
                    var nextResDTO = new NextReservation();
                    //if (nextReservation.ReservationTimes != null)
                    //{
                    nextResDTO.Title = nextReservation.ShortDescription;
                    nextResDTO.Description = nextReservation.LongDescription;
                    nextResDTO.BookedBy = nextReservation.SmartRoomSchedule != null ? nextReservation.SmartRoomSchedule.UserName : nextReservation.CreatedByUser != null ? nextReservation.CreatedByUser.FullName : string.Empty;
                    nextResDTO.InstitutionId = nextReservation.InstitutionId;
                    nextResDTO.ReservationId = nextReservation.Id;
                    nextResDTO.StartDateTime = nextReservation.StartDateTime;
                    nextResDTO.EndDateTime = nextReservation.EndDateTime;
                    //var timings = nextReservation.ReservationTimes.Where(e => e.IsActive).OrderBy(e => e.TimeInterval.Hour).ThenBy(e => e.TimeInterval.Minutes);
                    //nextResDTO.TimeDisplay = string.Format("{0} - {1}", timings.First().TimeInterval.Description, timings.Last().TimeInterval.ToTimeInterval.Description);
                    nextResDTO.TimeDisplay = string.Format("{0} - {1}", nextReservation.StartDateTime.ToString(_hrTimeFormat), nextReservation.EndDateTime.ToString(_hrTimeFormat));
                    response.Data = nextResDTO;
                    //}
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

        public async Task<BaseOperationResponse> ValidateCheckIn(string userName, string pin, int reservationId, string name, string company, string designation)
        {
            var response = new BaseOperationResponse();

            try
            {
                var query = await _appContext.Reservations
                       .Include(e => e.CreatedByUser)
                       .Include(e => e.ReservationInvitees)
                       .Where(e => e.Id == reservationId && e.IsActive && e.Status != BookingStatus.Completed.ToString())
                       .FirstOrDefaultAsync();

                var checkinResult = new CheckInResult()
                {
                    ReservationStatus = query == null ? string.Empty : query.Status,
                    Message = query == null || query.CreatedByUser == null ? "Booking not found" : "Invalid login"
                };

                if (query != null)
                {
                    //check if reservation is checked in already
                    bool isProceedAttendance = false;
                    bool isCheckin = false;
                    if ((query.Status != BookingStatus.CheckedIn.ToString() && query.Status != BookingStatus.InProgress.ToString()) ||
                        (query.Status == BookingStatus.InProgress.ToString() && string.IsNullOrEmpty(query.CheckedInBy)))
                    {
                        //check in
                        bool canCheckin = query.CreatedByUser != null && query.CreatedByUser.Pin == pin;
                        ReservationInvitee invitee = null;
                        if ((!canCheckin && string.IsNullOrEmpty(pin)) || (canCheckin && !string.IsNullOrEmpty(userName)))
                        {
                            if (string.IsNullOrEmpty(userName))
                            {
                                throw new Exception("Please provide the username");
                            }

                            var uName = userName.ToLower();
                            invitee = query.ReservationInvitees.FirstOrDefault(e =>
                                    (e.Email != null && e.Email != "" && e.Email.ToLower() == uName) ||
                                    (e.User != null && e.User.UserName != null && e.User.UserName != "" && e.User.UserName.ToLower() == uName) ||
                                    (e.User != null && e.User.Email != null && e.User.Email != "" && e.User.Email.ToLower() == uName));

                            canCheckin = (query.CreatedByUser != null &&
                                            (string.IsNullOrEmpty(uName) || query.CreatedByUser.UserName.Equals(uName, StringComparison.OrdinalIgnoreCase))) ||
                                            invitee != null;


                        }

                        if (canCheckin)
                        //&& query.CreatedByUser.Pin == pin)
                        {
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
                                        checkinMsg = "Too early to checkin. You are only allowed to checkin an hour before the event.";
                                    }
                                }
                                else
                                {
                                    isAllowCheckin = true; //TODO: temporarily just allow checkin
                                    //if (currentTime.Subtract(query.StartDateTime).TotalMinutes < 16)
                                    //{
                                    //    isAllowCheckin = true;
                                    //}
                                    //else
                                    //{
                                    //    //TODO: temporarily remove; just allow checkin
                                    //    //remove the booking
                                    //    SoftDelete(query);
                                    //    query.Status = BookingStatus.Released.ToString();
                                    //    if (await _appContext.SaveChangesAsync() > 0)
                                    //    {
                                    //        checkinMsg = "Checkin is no longer allowed. You are only allowed to checkin an hour before the event and 15 minutes after your event has started.";
                                    //    }
                                    //}
                                }

                                if (isAllowCheckin)
                                {
                                    if (query.Status != BookingStatus.InProgress.ToString())
                                    {
                                        query.Status = BookingStatus.CheckedIn.ToString();
                                    }

                                    query.CheckedInBy = string.IsNullOrEmpty(userName) ? (query.CreatedByUser != null ? query.CreatedByUser.UserName : string.Empty) : userName;
                                    Update(query);
                                    if (await _appContext.SaveChangesAsync() > 0)
                                    {
                                        isProceedAttendance = invitee != null;
                                        isCheckin = true;
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
                        }
                        else
                        {
                            checkinResult = new CheckInResult
                            {
                                IsUserValid = false,
                                ReservationStatus = BookingStatus.Unknown.ToString(),
                                Message = !string.IsNullOrEmpty(pin) || (!string.IsNullOrEmpty(pin) && !string.IsNullOrEmpty(userName)) ? "Username and pin incorrect." : "Participant not found."
                            };
                        }
                    }
                    else if (query.Status == BookingStatus.CheckedIn.ToString() || query.Status == BookingStatus.InProgress.ToString())
                    {
                        isProceedAttendance = true;
                    }

                    if (isProceedAttendance)
                    {
                        //valid
                        bool isAllowCheckin = false;
                        string checkinMsg = string.Empty;

                        if (string.IsNullOrEmpty(userName))
                        {
                            throw new Exception("Please provide the username");
                        }

                        var uName = userName.ToLower();
                        var invitee = query.ReservationInvitees.FirstOrDefault(e =>
                                    (e.Email != null && e.Email != "" && e.Email.ToLower() == uName) ||
                                    (e.User != null && e.User.UserName != null && e.User.UserName != "" && e.User.UserName.ToLower() == uName) ||
                                    (e.User != null && e.User.Email != null && e.User.Email != "" && e.User.Email.ToLower() == uName));

                        if (invitee != null)
                        {
                            isAllowCheckin = true;
                            //var currentTime = DateTime.Now;

                            //if (currentTime >= query.StartDateTime)
                            //{
                            //    isAllowCheckin = true;
                            //}
                            //else
                            //{
                            //    isAllowCheckin = false;
                            //    checkinMsg = "Event not started. Attendance can't be taken yet.";
                            //}
                        }
                        else
                        {
                            checkinMsg = "Participant not found.";
                        }

                        if (isAllowCheckin)
                        {
                            invitee.Status = ParticipantStatus.Attended.ToString();
                            invitee.AttendedOn = DateTime.Now;
                            invitee.Name = name;
                            invitee.Company = company;
                            invitee.Designation = designation;
                            this._appContext.ReservationInvitees.Update(invitee);
                            if (await _appContext.SaveChangesAsync() > 0)
                            {
                                checkinResult = new CheckInResult
                                {
                                    IsUserValid = true,
                                    ReservationStatus = query.Status,
                                    Message = isCheckin ? "Checkin and attendance successfully saved." : "Attendance successfully saved."
                                };
                            }
                        }
                        else
                        {
                            checkinResult = new CheckInResult
                            {
                                IsUserValid = false,
                                ReservationStatus = query.Status,
                                Message = checkinMsg
                            };
                        }

                    }

                    //if (query.CreatedByUser != null &&
                    //    (string.IsNullOrEmpty(userName) || query.CreatedByUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                    //    && query.CreatedByUser.Pin == pin)
                    //{
                    //    //check if checkin is done 1hr before start and 15mins after start
                    //    var currentTime = DateTime.Now;
                    //    bool isAllowCheckin = false;
                    //    string checkinMsg = string.Empty;
                    //    if (query.StartDateTime >= currentTime)
                    //    {
                    //        if (query.StartDateTime.Subtract(currentTime).TotalMinutes < 61)
                    //        {
                    //            isAllowCheckin = true;
                    //        }
                    //        else
                    //        {
                    //            checkinMsg = "Too early to checkin";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (currentTime.Subtract(query.StartDateTime).TotalMinutes < 16)
                    //        {
                    //            isAllowCheckin = true;
                    //        }
                    //        else
                    //        {
                    //            //remove the booking
                    //            SoftDelete(query);
                    //            if (await _appContext.SaveChangesAsync() > 0)
                    //            {
                    //                checkinMsg = "Checkin is no longer allowed.";
                    //            }
                    //        }
                    //    }

                    //    if (isAllowCheckin)
                    //    {
                    //        query.Status = BookingStatus.CheckedIn.ToString();
                    //        Update(query);
                    //        if (await _appContext.SaveChangesAsync() > 0)
                    //        {
                    //            checkinResult = new CheckInResult
                    //            {
                    //                IsUserValid = true,
                    //                ReservationStatus = query.Status,
                    //                Message = "Successful checkin"
                    //            };
                    //        }
                    //    }
                    //    else
                    //    {
                    //        checkinResult = new CheckInResult
                    //        {
                    //            IsUserValid = false,
                    //            ReservationStatus = BookingStatus.Unknown.ToString(),
                    //            Message = checkinMsg
                    //        };
                    //    }
                    //}
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
                       //.Include(e => e.CreatedByUser)
                       .Where(e => //e.CreatedByUser.IsActive && 
                       e.IsActive &&
                       e.Status != BookingStatus.CheckedIn.ToString() &&
                       e.Status != BookingStatus.Released.ToString() &&
                       e.Status != BookingStatus.Cancelled.ToString() &&
                       e.Status != BookingStatus.Completed.ToString());

                var currentTime = DateTime.Now;
                foreach (var res in query)
                {
                    if (string.IsNullOrEmpty(res.CheckedInBy) &&
                        ((currentTime.Subtract(res.StartDateTime).TotalMinutes >= 16) || (currentTime > res.EndDateTime))
                        )
                    {
                        //remove the booking
                        res.Status = BookingStatus.Released.ToString();
                        SoftDelete(res);
                    }
                }

                await _appContext.SaveChangesAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<BaseOperationResponse> MonitorInprogressBookings()
        {
            var response = new BaseOperationResponse();
            var listOfEvents = new List<Reservation>();
            try
            {
                var query = _appContext.Reservations
                       //.Include(e => e.CreatedByUser)
                       .Where(e => //e.CreatedByUser.IsActive && 
                       e.IsActive &&
                       //e.Status != BookingStatus.CheckedIn.ToString() &&
                       e.Status != BookingStatus.InProgress.ToString() &&
                       e.Status != BookingStatus.Completed.ToString() &&
                       e.Status != BookingStatus.Cancelled.ToString() &&
                       e.Status != BookingStatus.Released.ToString());

                var currentTime = DateTime.Now;
                foreach (var res in query)
                {
                    if (currentTime.Subtract(res.StartDateTime).TotalMinutes >= 1 &&
                        res.EndDateTime > currentTime)
                    {
                        //update the booking
                        res.Status = BookingStatus.InProgress.ToString();
                        Update(res);
                        listOfEvents.Add(res);
                    }
                }

                await _appContext.SaveChangesAsync();
                response.Data = new { data = listOfEvents };
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<BaseOperationResponse> CompleteBookings()
        {
            var response = new BaseOperationResponse();
            var listOfEvents = new List<Reservation>();
            try
            {
                var query = _appContext.Reservations
                       //.Include(e => e.CreatedByUser)
                       .Where(e => //e.CreatedByUser.IsActive && 
                       e.IsActive &&
                       //e.Status != BookingStatus.CheckedIn.ToString() &&
                       //e.Status != BookingStatus.InProgress.ToString() &&
                       e.Status != BookingStatus.Completed.ToString() &&
                       e.Status != BookingStatus.Cancelled.ToString() &&
                       e.Status != BookingStatus.Released.ToString());

                var currentTime = DateTime.Now;
                foreach (var res in query)
                {
                    if (currentTime > res.EndDateTime)
                    {
                        //update the booking
                        res.Status = BookingStatus.Completed.ToString();
                        Update(res);
                        listOfEvents.Add(res);
                    }
                }

                await _appContext.SaveChangesAsync();
                response.Data = new { data = listOfEvents };
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<BaseOperationResponse> ValidateExtend(int reservationId, double totalMinutes)
        {
            var response = new BaseOperationResponse();

            try
            {
                var reservation = await GetByIdAsync(reservationId);
                //var lastTi = reservation.ReservationTimes.Where(e => e.IsActive).OrderBy(e => e.TimeInterval.Hour).ThenBy(e => e.TimeInterval.Minutes).LastOrDefault();
                //var timeIntervals = lastTi != null ? GetNextAvailableTimeIntervals(reservationId, reservation.LocationId, lastTi.TimeInterval).OrderBy(e => e.Hour).ThenBy(e => e.Minutes) : null;

                reservation.EndDateTime = reservation.EndDateTime.AddMinutes(totalMinutes);
                reservation.OriginalEndDateTime = reservation.EndDateTime;
                bool isValid = await ValidateReservationTime(reservation);//timeIntervals.Any(e => e.Id == timeIntervalId);

                var extendResult = new ExtendResult
                {
                    IsExtendValid = isValid,
                    Message = isValid ? "Successfully extended" : "Extension is not allowed"
                };

                if (isValid)
                {
                    //extend logic here
                    //foreach (var ti in timeIntervals)
                    //{
                    //    reservation.ReservationTimes.Add(new ReservationTime
                    //    {
                    //        ReservationId = reservationId,
                    //        TimeIntervalId = ti.Id
                    //    });

                    //    if (ti.Id == timeIntervalId) break;
                    //}

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
                //var reservedTimesToRemove = reservation.ReservationTimes
                //    .Where(e => e.IsActive && (e.TimeInterval.Hour > currentTime.Hour ||
                //    (e.TimeInterval.Hour == currentTime.Hour && e.TimeInterval.Minutes > currentTime.Minute)))
                //    .OrderBy(e => e.TimeInterval.Hour).ThenBy(e => e.TimeInterval.Minutes);

                ////end logic here
                //foreach (var ti in reservedTimesToRemove)
                //{
                //    ti.IsActive = false;
                //    _appContext.ReservationTimes.Update(ti);
                //}


                reservation.OriginalEndDateTime = reservation.EndDateTime;
                reservation.ActualEndDateTime = reservation.EndDateTime = DateTime.Now;
                reservation.Status = BookingStatus.Completed.ToString();

                Update(reservation);
                await _appContext.SaveChangesAsync();

                //Get attendees
                var attendees = _appContext.ReservationInvitees
                    .Include(e => e.User).Where(e => e.ReservationId == reservation.Id && e.IsActive).ToList();
                //.Select(e => new
                //{
                //    Name = e.Name,
                //    Email = e.Email,
                //    PhoneNumber = e.PhoneNumber
                //});

                response.Data = new { Message = "Successfully ended.", Attendees = attendees };
                response.Message = "Successfully ended.";
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<BaseOperationResponse> ValidateAttendanceSignIn(string userName, int reservationId, string name, string company, string designation, string status)
        {
            var response = new BaseOperationResponse();

            try
            {
                var query = await _appContext.Reservations
                       .Include(e => e.CreatedByUser)
                       .Include(e => e.ReservationInvitees)
                       .Where(e => e.Id == reservationId && e.IsActive)
                       .FirstOrDefaultAsync();

                var checkinResult = new CheckInResult()
                {
                    Message = query == null ? "Booking not found" : "Invalid login"
                };

                if (query != null)
                {
                    bool isGoingStatus = !string.IsNullOrEmpty(status);
                    if (isGoingStatus || query.Status == BookingStatus.CheckedIn.ToString() || query.Status == BookingStatus.InProgress.ToString())
                    {
                        //valid
                        bool isAllowCheckin = false;
                        string checkinMsg = string.Empty;

                        var invitee = query.ReservationInvitees.FirstOrDefault(e => e.Email.ToLower() == userName.ToLower()
                            || e.User.UserName.ToLower() == userName.ToLower()
                            || e.User.Email.ToLower() == userName.ToLower());

                        if (invitee != null)
                        {
                            var currentTime = DateTime.Now;

                            if (isGoingStatus || currentTime >= query.StartDateTime)
                            {
                                isAllowCheckin = true;
                            }
                            else
                            {
                                isAllowCheckin = false;
                                checkinMsg = "Event not started. Attendance can't be taken yet.";
                            }

                        }
                        else
                        {
                            checkinMsg = "Participant not found.";
                        }

                        if (isAllowCheckin)
                        {
                            invitee.Status = isGoingStatus ? status : ParticipantStatus.Attended.ToString();
                            invitee.Name = name;
                            invitee.Company = company;
                            invitee.Designation = designation;
                            this._appContext.ReservationInvitees.Update(invitee);
                            if (await _appContext.SaveChangesAsync() > 0)
                            {
                                checkinResult = new CheckInResult
                                {
                                    IsUserValid = true,
                                    Message = isGoingStatus ? "Response successfully saved." : "Attendance successfully saved."
                                };
                            }
                        }
                        else
                        {
                            checkinResult = new CheckInResult
                            {
                                IsUserValid = false,
                                Message = checkinMsg
                            };
                        }

                    }
                    else
                    {
                        checkinResult.Message = query.Status == BookingStatus.Completed.ToString() ? "Event has passed already." : "Event has not been checked in.";
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

        public async Task<BaseOperationResponse> ValidateFeedback(int reservationId, int userId, string feedback, string comment)
        {
            var response = new BaseOperationResponse();

            try
            {
                var query = await _appContext.Reservations
                       .Where(e => e.Id == reservationId && e.IsActive)
                       .FirstOrDefaultAsync();

                if (query == null)
                {
                    response.Message = "Event not found";
                    response.IsSuccess = false;
                }
                else
                {
                    var qFeedback = await _appContext.ReservationFeedbacks
                       .Where(e => e.ReservationId == reservationId && e.IsActive && e.UserId == userId)
                       .FirstOrDefaultAsync();
                    if (qFeedback != null)
                    {
                        response.Message = "You already submitted a feedback for this event.";
                        response.IsSuccess = false;
                    }
                    else
                    {
                        qFeedback = new ReservationFeedback
                        {
                            Comment = comment,
                            Feedback = feedback,
                            UserId = userId,
                            ReservationId = reservationId,
                            IsActive = true
                        };

                        await _appContext.ReservationFeedbacks.AddAsync(qFeedback);
                        if (await _appContext.SaveChangesAsync() > 0)
                        {
                            response.Message = "Feedback successfully saved.";
                            response.IsSuccess = true;
                        }
                    }
                }
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
                var extendTimes = new List<ExtendTime>();
                var reservation = await GetByIdAsync(reservationId);
                var nextReservation = await GetNextBooking(new CalendarFilter { locationIds = new List<int> { reservation.LocationId.Value }, Start = reservation.StartDateTime });
                TimeSpan span;

                if (nextReservation != null && nextReservation.Data != null)
                {
                    NextReservation next = nextReservation.Data as NextReservation;
                    span = next.StartDateTime.Subtract(reservation.EndDateTime);
                }
                else
                {
                    var maxTime = new DateTime(reservation.StartDateTime.Year, reservation.StartDateTime.Month, reservation.StartDateTime.Day,
                        reservation.Institution != null && reservation.Institution.EndTime > 0 ? reservation.Institution.EndTime : 21, 0, 0);
                    span = maxTime.Subtract(reservation.EndDateTime);
                }

                if (span.TotalMinutes >= 30)
                {
                    double currentMinutes = 30;
                    extendTimes.Add(new ExtendTime
                    {
                        ReservationId = reservationId,
                        TotalMinutes = currentMinutes,
                        From = string.Format("{0}", reservation.EndDateTime.ToString(_hrTimeFormat)),
                        To = string.Format("{0}", reservation.EndDateTime.AddMinutes(currentMinutes).ToString(_hrTimeFormat)),
                        TimeDisplay = string.Format("{0} MIN{1}", currentMinutes, "S")
                    });

                    currentMinutes = 60;
                    if (span.TotalMinutes >= currentMinutes)
                    {
                        extendTimes.Add(new ExtendTime
                        {
                            ReservationId = reservationId,
                            TotalMinutes = currentMinutes,
                            From = string.Format("{0}", reservation.EndDateTime.ToString(_hrTimeFormat)),
                            To = string.Format("{0}", reservation.EndDateTime.AddMinutes(currentMinutes).ToString(_hrTimeFormat)),
                            TimeDisplay = string.Format("{0} MIN{1}", currentMinutes, "S")
                        });
                    }

                    currentMinutes = 90;
                    if (span.TotalMinutes >= currentMinutes)
                    {
                        extendTimes.Add(new ExtendTime
                        {
                            ReservationId = reservationId,
                            TotalMinutes = currentMinutes,
                            From = string.Format("{0}", reservation.EndDateTime.ToString(_hrTimeFormat)),
                            To = string.Format("{0}", reservation.EndDateTime.AddMinutes(currentMinutes).ToString(_hrTimeFormat)),
                            TimeDisplay = string.Format("{0} MIN{1}", currentMinutes, "S")
                        });
                    }

                    currentMinutes = 120;
                    if (span.TotalMinutes >= currentMinutes)
                    {
                        extendTimes.Add(new ExtendTime
                        {
                            ReservationId = reservationId,
                            TotalMinutes = currentMinutes,
                            From = string.Format("{0}", reservation.EndDateTime.ToString(_hrTimeFormat)),
                            To = string.Format("{0}", reservation.EndDateTime.AddMinutes(currentMinutes).ToString(_hrTimeFormat)),
                            TimeDisplay = string.Format("{0} MIN{1}", currentMinutes, "S")
                        });
                    }
                }
                else
                {
                    if (span.TotalMinutes > 0)
                    {
                        //actual minutes to extend
                        extendTimes.Add(new ExtendTime
                        {
                            ReservationId = reservationId,
                            TotalMinutes = span.TotalMinutes,
                            From = string.Format("{0}", reservation.EndDateTime.ToString(_hrTimeFormat)),
                            To = string.Format("{0}", reservation.EndDateTime.AddMinutes(span.TotalMinutes).ToString(_hrTimeFormat)),
                            TimeDisplay = string.Format("{0} MIN{1}", span.TotalMinutes, span.TotalMinutes > 1 ? "S" : "")
                        });
                    }
                    else
                    {
                        extendTimes = null;
                    }
                }

                response.Data = extendTimes;
                //var lastTi = reservation.ReservationTimes.Where(e => e.IsActive).OrderBy(e => e.TimeInterval.Hour).ThenBy(e => e.TimeInterval.Minutes).LastOrDefault();
                //var timeIntervals = lastTi != null ? GetNextAvailableTimeIntervals(reservationId, reservation.LocationId, lastTi.TimeInterval).Select(e => new
                //{
                //    TimeIntervalId = e.Id,
                //    ReservationId = reservationId,
                //    From = string.Format("{0}", lastTi.TimeInterval.ToTimeInterval.Value),
                //    To = string.Format("{0}", e.ToTimeInterval.Value),
                //    TimeDisplay = string.Format("{0} MIN{1}", Math.Abs((e.ToTimeInterval.Hour * 60 + e.ToTimeInterval.Minutes) - (lastTi.TimeInterval.ToTimeInterval.Hour * 60 + lastTi.TimeInterval.ToTimeInterval.Minutes)),
                //                        (Math.Abs((e.ToTimeInterval.Hour * 60 + e.ToTimeInterval.Minutes) - (lastTi.TimeInterval.ToTimeInterval.Hour * 60 + lastTi.TimeInterval.ToTimeInterval.Minutes))) > 1 ? "S" : "")
                //}) : null;

                //response.Data = timeIntervals;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        #region Vehicle 
        public async Task<BaseOperationResponse> UpdateArrival(List<VehicleQueryModel> list)
        {
            var response = new BaseOperationResponse() { Data = new { } };

            try
            {
                foreach (var vehicle in list)
                {
                    var vehicleEntry = Mapper.Map<VehicleEntry>(vehicle);

                    if (!string.IsNullOrEmpty(vehicle.SeasonId))
                    {
                        string seasonId = vehicle.SeasonId.Contains("P_") ? vehicle.SeasonId.Substring(vehicle.SeasonId.IndexOf("_") + 1) : vehicle.SeasonId;
                        var invitee = await this._appContext.ReservationInvitees.FirstOrDefaultAsync(e => seasonId == e.Id.ToString());

                        if (invitee != null)
                        {
                            string status = vehicle.Type.Trim().ToUpper();
                            invitee.VehicleStatus = status;
                            if (status == VehicleStatus.ENTRY.ToString())
                            {
                                invitee.EntryDate = vehicle.Timestamp ?? DateTime.Now;
                            }
                            else
                            {
                                invitee.ExitDate = vehicle.Timestamp ?? DateTime.Now;
                            }

                            _appContext.ReservationInvitees.Update(invitee);

                            vehicleEntry.ReservationId = invitee.ReservationId;
                        }
                    }

                    _appContext.VehicleEntries.Add(vehicleEntry);
                }

                await _appContext.SaveChangesAsync();
                response.Message = "Update successful!";
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<BaseOperationResponse> RegisterVehicleToVMS(List<VMSVehiclePostRequestModel> list)
        {
            string logName = "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy");
            var response = new BaseOperationResponse() { Data = new { } };

            try
            {
                string apiUrl = string.Empty;
                var appSetting = await _appContext.ApplicationSettings.FirstOrDefaultAsync(e => e.IsActive && e.Key == "VMS_API_URL");
                if (appSetting != null)
                {
                    apiUrl = appSetting.Value;
                }
                using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    await WriteLog("START REQUEST - " + DateTime.Now.ToString(), logName);
                    var jsonObj = JsonConvert.SerializeObject(new
                    {
                        vehicles = list
                    });
                    await WriteLog("REQUEST BODY: \n" + jsonObj, logName);
                    var stringContent = new StringContent(jsonObj, Encoding.UTF8, "application/json");

                    var resp = await client.PostAsync(apiUrl, stringContent);

                    //var response = client.GetAsync(string.Format("?locationCode={0}", locationCode)).Result;
                    resp.EnsureSuccessStatusCode();
                    var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    response.IsSuccess = true;
                    response.Message = "Successfully registered the vehicle/s";


                    await WriteLog("SUCCESSFUL LOG STARTS HERE", logName);
                    await WriteLog(content, logName);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
                await WriteLog(response.Message, logName);
                await WriteLog(ex.InnerException != null ? ex.InnerException.StackTrace : string.Empty, logName);
            }

            await WriteLog("END REQUEST - " + DateTime.Now.ToString(), logName);
            return response;
        }

        public async Task<List<VMSVehicleLog>> GetVehicleLogs(VehicleLogFilter filter = null)
        {
            IQueryable<VehicleEntry> query = _appContext.VehicleEntries
                .Include(e => e.Reservation)
                .Where(e => e.IsActive && e.Timestamp.HasValue);

            if (filter != null)
            {
                if (filter.Start.HasValue)
                {
                    query = query.Where(e => e.Timestamp >= filter.Start.Value);
                }

                if (filter.End.HasValue)
                {
                    query = query.Where(e => e.Timestamp <= filter.End.Value);
                }
            }

            var logs = Mapper.Map<List<VMSVehicleLog>>(await query.ToListAsync());
            return logs.OrderByDescending(r => r.Timestamp).ToList();
        }

        #endregion
        #region Excel
        public async Task<byte[]> GenerateAttendanceXlsLayout(int reservationId)
        {
            var reservation = await _appContext.Reservations
                       .Include(e => e.CreatedByUser)
                       .Include(e => e.ReservationInvitees)
                       .Where(e => e.Id == reservationId)
                       .FirstOrDefaultAsync();
            if (reservation != null)
            {
                using (var stream = new MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Attendance List");
                    var headers = new string[] { "Email", "Name", "Company", "Designation", "Status", "Attendance Date/Time" };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount);
                    var cell = row.CreateCell(0);
                    cell.SetCellValue("Attendance List Report");
                    cell.CellStyle = headerStyle;
                    var cra = new NPOI.SS.Util.CellRangeAddress(row.RowNum, row.RowNum, 0, headers.Count() - 1);
                    sheet.AddMergedRegion(cra);

                    var subHeaderStyle = wb.CreateCellStyle();
                    subHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;


                    row = sheet.CreateRow(++rowCount);
                    cell = row.CreateCell(0);
                    var headerRoles = "For : ";
                    headerRoles += reservation.ShortDescription;
                    if (reservation.CreatedByUser != null)
                    {
                        headerRoles += " Organised by " + reservation.CreatedByUser.FullName;
                    }

                    cell.SetCellValue(headerRoles);
                    cell.CellStyle = subHeaderStyle;
                    cra = new NPOI.SS.Util.CellRangeAddress(row.RowNum, row.RowNum, 0, headers.Count() - 1);
                    sheet.AddMergedRegion(cra);


                    rowCount++; // intentionally done to move the row count   
                    row = sheet.CreateRow(++rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    for (var i = 0; i < headers.Length; i++)
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = borderedHeaderStyle;
                    }
                    sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();
                    reservation.ReservationInvitees.Where(e => e.IsActive).ToList().ForEach(dt =>
                    {
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(0);
                        cell.SetCellValue(dt.Email);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(1);
                        cell.SetCellValue(dt.Name);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(2);
                        cell.SetCellValue(dt.Company);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(3);
                        cell.SetCellValue(dt.Designation);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(4);
                        cell.SetCellValue(dt.Status);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(5);
                        cell.CellStyle = contentStyle;
                        if (dt.AttendedOn == null)
                        {
                            cell.SetCellValue("");
                        }
                        else
                        {
                            cell.SetCellValue((DateTime)dt.AttendedOn);
                            cell.CellStyle.DataFormat = dataFormatCustom.GetFormat("dd/MM/yyyy HH:mm:ss");
                        }

                    });
                    #endregion

                    for (var i = 0; i < headers.Length; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        public async Task<BaseOperationResponse> Response(int reservationId, string userName, string status)
        {
            var response = new BaseOperationResponse();

            try
            {
                var query = await _appContext.Reservations
                       .Include(e => e.ReservationInvitees)
                       .Where(e => e.Id == reservationId && e.IsActive)
                       .FirstOrDefaultAsync();

                response.Message = query == null ? "Booking not found" : "";

                if (query != null)
                {
                    if (query.Status == BookingStatus.Completed.ToString())
                    {
                        response.Message = "Event already ended.";
                        response.IsSuccess = false;
                    }
                    else
                    {

                        var invitee = query.ReservationInvitees.FirstOrDefault(e => e.IsActive &&
                                ((e.User != null && (e.User.Email.Equals(userName, StringComparison.OrdinalIgnoreCase) || e.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)))
                                || e.Email.Equals(userName, StringComparison.OrdinalIgnoreCase)));

                        if (invitee != null)
                        {
                            invitee.Status = status;
                            this._appContext.ReservationInvitees.Update(invitee);
                            if (await _appContext.SaveChangesAsync() > 0)
                            {
                                response.Message = "Successfully saved. Thank you for responding!";
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.Message = "Failed to save response!";
                                response.IsSuccess = false;
                            }
                        }
                        else
                        {
                            response.Message = "Participant not found!";
                            response.IsSuccess = false;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<BaseOperationResponse> SyncSmartRoomSchedules(SMARTRoomXML smartRoom, int? institutionId)
        {
            var result = new BaseOperationResponse();

            if (smartRoom != null && smartRoom.ApplicationData != null && smartRoom.ApplicationData.Schedules != null)
            {
                string[] roomTypes = new string[] { "trainingroom", "elab", "skilllab" };
                //remove rooms aren't in the list
                var bookingsToDelete = await FindAsync(e => e.IsActive && e.SmartRoomScheduleId.HasValue && !smartRoom.ApplicationData.Schedules.Schedule.Any(f => f.ID == e.SmartRoomSchedule.SmartRoomScheduleID));
                SoftDeleteRange(bookingsToDelete);

                foreach (var schedule in smartRoom.ApplicationData.Schedules.Schedule)
                {
                    var location = await this._appContext.Locations.FirstOrDefaultAsync(e => e.IsActive && e.SmartRoomResource != null && e.SmartRoomResource.SmartRoomResourceID == schedule.RoomID);

                    if(location != null)
                    {
                        var existingSchedule = await this._appContext.SmartRoomSchedules.FirstOrDefaultAsync(e => e.SmartRoomScheduleID == schedule.ID);
                        if (existingSchedule != null)
                        {
                            existingSchedule.CopyFrom(schedule);
                            existingSchedule.SmartRoomScheduleID = schedule.ID;
                            this._appContext.SmartRoomSchedules.Update(existingSchedule);

                            var existingBooking = await GetFirstOrDefaultAsync(e => e.InstitutionId == institutionId && e.SmartRoomScheduleId == existingSchedule.Id);
                            if (existingBooking != null)
                            {
                                existingBooking.ShortDescription = schedule.Name;
                                existingBooking.LongDescription = schedule.Name;
                                existingBooking.LocationId = location?.Id;
                                existingBooking.IsActive = true;
                                existingBooking.RepeatType = RepeatTypes.None.ToString();
                                existingBooking.StartDateTime = schedule.StartTime.GetValueOrDefault();
                                existingBooking.EndDateTime = schedule.EndTime.GetValueOrDefault();
                                existingBooking.ActualEndDateTime = schedule.EndedAt.GetValueOrDefault();
                                existingBooking.CreatedDate = schedule.DateCreated.GetValueOrDefault();
                                existingBooking.Notes = schedule.Remarks;
                                existingBooking.MeetingContactNo = schedule.MeetingContactNo;
                                existingBooking.IsActive = true;
                                existingBooking.Status = schedule.Status;
                                existingBooking.StatusRemark = schedule.StatusRemarks;
                                existingBooking.IsSignage = roomTypes.Any(e => e == schedule.Category);
                                Update(existingBooking);
                            }
                            else
                            {
                                var booking = new Reservation
                                {
                                    InstitutionId = institutionId,
                                    ShortDescription = schedule.Name,
                                    LongDescription = schedule.Category,
                                    LocationId = location?.Id,
                                    RepeatType = RepeatTypes.None.ToString(),
                                    StartDateTime = schedule.StartTime.GetValueOrDefault(),
                                    EndDateTime = schedule.EndTime.GetValueOrDefault(),
                                    ActualEndDateTime = schedule.EndedAt.GetValueOrDefault(),
                                    CreatedDate = schedule.DateCreated.GetValueOrDefault(),
                                    Notes = schedule.Remarks,
                                    MeetingContactNo = schedule.MeetingContactNo,
                                    SmartRoomScheduleId = existingSchedule.Id,
                                    Status = schedule.Status,
                                    StatusRemark = schedule.StatusRemarks,
                                    IsSignage = roomTypes.Any(e => e == schedule.Category)
                                };

                                await AddAsync(booking);
                            }
                        }
                        else
                        {
                            existingSchedule = new SmartRoomSchedule();
                            existingSchedule.CopyFrom(schedule);
                            existingSchedule.SmartRoomScheduleID = schedule.ID;
                            await _appContext.SmartRoomSchedules.AddAsync(existingSchedule);

                            var booking = new Reservation
                            {
                                InstitutionId = institutionId,
                                ShortDescription = schedule.Name,
                                LongDescription = schedule.Category,
                                LocationId = location?.Id,
                                RepeatType = RepeatTypes.None.ToString(),
                                StartDateTime = schedule.StartTime.GetValueOrDefault(),
                                EndDateTime = schedule.EndTime.GetValueOrDefault(),
                                ActualEndDateTime = schedule.EndedAt.GetValueOrDefault(),
                                CreatedDate = schedule.DateCreated.GetValueOrDefault(),
                                Notes = schedule.Remarks,
                                MeetingContactNo = schedule.MeetingContactNo,
                                SmartRoomScheduleId = existingSchedule.Id,
                                Status = existingSchedule.Status,
                                StatusRemark = existingSchedule.StatusRemarks,
                                IsSignage = roomTypes.Any(e => e == schedule.Category)
                            };

                            await AddAsync(booking);
                        }
                    }
                }

                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully synced Smart Room Schedules!";
                    result.IsSuccess = true;
                }
                else
                {
                    result.Message = "Failed to sync Smart Room Schedules!";
                    result.IsSuccess = false;
                }
            }
            else
            {
                result.Message = "No schedules to sync!";
                result.IsSuccess = false;
            }

            return result;
        }

        private List<TimeInterval> GetNextAvailableTimeIntervals(int reservationId, int? locationId, TimeInterval lastTi)
        {
            DateTime currentTime = DateTime.Now;
            var timeIntervals = GetAllTimeIntervals().Where(e => e.Hour > lastTi.ToTimeInterval.Hour ||
            (e.Hour == lastTi.Hour && e.Minutes > lastTi.Minutes) ||
            (e.Hour == lastTi.ToTimeInterval.Hour && e.Minutes >= lastTi.ToTimeInterval.Minutes))
            .OrderBy(e => e.Hour).ThenBy(e => e.Minutes);

            List<TimeInterval> availableTi = new List<TimeInterval>();
            var todayReservations = GetTodayReservations().Where(e => e.LocationId == locationId);
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

        private async Task<bool> ValidateReservationTime(Reservation reservation)
        {
            var valid = await _appContext.Reservations.AnyAsync(e => e.IsActive && e.Id != reservation.Id && e.LocationId == reservation.LocationId &&
                    ((e.StartDateTime < reservation.EndDateTime && e.EndDateTime > reservation.StartDateTime)));
            return !valid;
        }

        private BaseOperationResponse ValidateDuplicate(Reservation reservation, DateTime? dtc = null)
        {
            var response = new BaseOperationResponse();
            response.IsSuccess = true;
            response.Message = string.Empty;

            var institution = _appContext.Institutions.FirstOrDefault(e => e.Id == reservation.InstitutionId);
            if (institution != null && institution.IsRestrictDuplicateBooking)
            {
                int userId = _appContext.CurrentUserId ?? 0;
                ApplicationUser user = _appContext.Users.FirstOrDefault(e => e.Id == userId);
                if (user != null)
                {
                    string frequency = string.Empty;
                    var dateToCheck = dtc ?? DateTime.Now;
                    DateTime dateRangeBegin = dateToCheck;
                    TimeSpan duration = new TimeSpan(0, 0, 0, 0); //One day 
                    DateTime dateRangeEnd = DateTime.Today.Add(duration);

                    if (institution.Restriction == BookingRestrictionFrequency.WEEKLY.ToString())
                    {
                        dateRangeBegin = dateToCheck.AddDays(-(int)dateToCheck.DayOfWeek);
                        dateRangeEnd = dateToCheck.AddDays(6 - (int)dateToCheck.DayOfWeek);
                        frequency = "week";
                    }
                    else if (institution.Restriction == BookingRestrictionFrequency.MONTHLY.ToString())
                    {
                        duration = new TimeSpan(DateTime.DaysInMonth(dateToCheck.Year, dateToCheck.Month) - 1, 0, 0, 0);
                        dateRangeBegin = dateToCheck.AddDays((-1) * dateToCheck.Day + 1);
                        dateRangeEnd = dateRangeBegin.Add(duration);
                        frequency = "month";
                    }
                    else if (institution.Restriction == BookingRestrictionFrequency.YEARLY.ToString())
                    {
                        dateRangeBegin = new DateTime(dateToCheck.Year, 1, 1);
                        dateRangeEnd = new DateTime(dateToCheck.Year, 12, 31);
                        frequency = "year";
                    }
                    else
                    {
                        frequency = "day";
                    }

                    response.IsSuccess = !_appContext.Reservations.Any(e => e.IsActive && e.Id != reservation.Id && e.LocationId == reservation.LocationId &&
                            reservation.StartDateTime.Date >= dateRangeBegin.Date && reservation.StartDateTime.Date <= dateRangeEnd.Date &&
                            reservation.InstitutionId == reservation.InstitutionId &&
                            (e.CreatedByUser != null && !string.IsNullOrEmpty(e.CreatedByUser.UnitNumber) &&
                            e.CreatedByUser.UnitNumber.Equals(user.UnitNumber, StringComparison.OrdinalIgnoreCase)));

                    if (!response.IsSuccess)
                    {
                        response.Message = string.Format("You already accumulated a booking for the {0}.", frequency);
                    }
                }
            }

            return response;
        }

        private async Task WriteLog(string strLog, string logName)
        {
            try
            {
                StreamWriter log;
                FileStream fileStream = null;
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo;

                string logFilePath = string.Empty;
                var appSetting = await _appContext.ApplicationSettings.FirstOrDefaultAsync(e => e.IsActive && e.Key == "VMS_LOG");
                if (appSetting != null)
                {
                    logFilePath = appSetting.Value;
                }

                logFilePath = logFilePath + logName + "." + "txt";
                logFileInfo = new FileInfo(logFilePath);
                logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                if (!logFileInfo.Exists)
                {
                    fileStream = logFileInfo.Create();
                }
                else
                {
                    fileStream = new FileStream(logFilePath, FileMode.Append);
                }
                log = new StreamWriter(fileStream);
                log.WriteLine(strLog);
                log.Close();
            }
            catch (Exception)
            {
            }
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}

public class ReservationUpdateResponseData
{
    public Reservation Reservation { get; set; }
    public List<ReservationInvitee> Invitees { get; set; }
}
