using DAL.Models;
using DAL.Models.MealOrder;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Filters
{
    public class SieveCustomFilterMethods : ISieveCustomFilterMethods
    {
        public IQueryable<Device> FindDeviceWithLocationIdIncludeParent(IQueryable<Device> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            return result; // Must return modified IQueryable<TEntity>
        }

        //public IQueryable<T> Latest<T>(IQueryable<T> source, string op, string[] values) where T : BaseEntity // Generic functions are allowed too
        //{
        //    var result = source.Where(c => c.DateCreated > DateTimeOffset.UtcNow.AddDays(-14));
        //    return result;
        //}

        public IQueryable<AuditLog> AuditDataDateRange(IQueryable<AuditLog> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.EventDateTime.Date >= start);
                }
               

                if(values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.EventDateTime.Date <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }


        public IQueryable<AuthenticationLog> AuditAuthDateRange(IQueryable<AuthenticationLog> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.CreatedDate.Date >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.CreatedDate.Date <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<ExternalAppLoginLog> AuditExternalLoginDateRange(IQueryable<ExternalAppLoginLog> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.EventDateTime.Date >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.EventDateTime.Date <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<TokenOrder> AuditOrderLogTransactionDateRange(IQueryable<TokenOrder> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.TransactionTime.Date >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.TransactionTime.Date <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<TokenOrder> AuditOrderLogDateRange(IQueryable<TokenOrder> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.DeliveryDate.Date >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.DeliveryDate.Date <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<TokenOrder> OrderCancelledDateRange(IQueryable<TokenOrder> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.CancelledOn >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.CancelledOn <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<UpDownTimeLog> UpDownTimeDateRange(IQueryable<UpDownTimeLog> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.CreatedDate.Date >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.CreatedDate.Date <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<ServiceContract> ServiceContractDateRange(IQueryable<ServiceContract> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                DateTime start = DateTime.MinValue;
                DateTime end = DateTime.MaxValue;

                if (!string.IsNullOrEmpty(values[0])) 
                    start = Convert.ToDateTime(values[0]);

                if (values.Length > 1 && !string.IsNullOrEmpty(values[1]))
                    end = Convert.ToDateTime(values[1]);

                result = result.Where(c => c.StartDate.HasValue && c.StartDate.Value.Date <= end);
                result = result.Where(c => c.EndDate.HasValue && c.EndDate.Value.Date >= start);
            }
            return result; // Must return modified IQueryable<TEntity>
        }


        public IQueryable<TokenOrder> TokenOrderDateRange(IQueryable<TokenOrder> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.DeliveryDate >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.DeliveryDate <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<TokenOrder> SalesDDateRange(IQueryable<TokenOrder> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.DeliveryDate >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.DeliveryDate <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<Device> DeviceIsApproval(IQueryable<Device> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    bool isApproval = Convert.ToBoolean(values[0]);
                    if (isApproval)
                    {
                        result = result.Where(c => c.LocationId.HasValue);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<CatererInfo> CatererByOutletProfileId(IQueryable<CatererInfo> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    int outletProfileId = Convert.ToInt32(values[0]);
                    result = result.Where(c => c.OutletProfiles.Any(f => f.Id == outletProfileId));
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<MealType> MealTypeByOutletProfileId(IQueryable<MealType> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    int outletProfileId = Convert.ToInt32(values[0]);
                    result = result.Where(c => c.Caterer.OutletProfiles.Any(f => f.Id == outletProfileId));
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<Student> StudentInterestGroupId(IQueryable<Student> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    int interestGroupId = Convert.ToInt32(values[0]);
                    result = result.Where(c => c.InterestGroups.Any(f => f.Id == interestGroupId));
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<TokenOrder> TokenOrderCancel(IQueryable<TokenOrder> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            return source.Where(e => string.IsNullOrEmpty(e.CancelRequestStatus));
        }

        public IQueryable<ApplicationUser> UserByUserGroupId(IQueryable<ApplicationUser> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    int userGroupId = Convert.ToInt32(values[0]);
                    result = result.Where(c => c.UserGroupMembers.Any(f => f.UserGroupId == userGroupId));
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<ApplicationUser> UserByCreateDateRange(IQueryable<ApplicationUser> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.CreatedDate >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.CreatedDate <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<ApplicationUser> UserByLastLoginDateRange(IQueryable<ApplicationUser> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.LastLoginTime >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.LastLoginTime <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<DishCycle> InOutletId(IQueryable<DishCycle> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    var outletId = Convert.ToInt32(values[0]);
                    result = result.Where(c => c.OutletProfile.Caterer.CatererOutlets.Any(e => e.OutletId == outletId) || c.OutletProfile.Outlets.Any(e => e.Id == outletId));
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<ApplicationRole> RoleByCreatedDateRange(IQueryable<ApplicationRole> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.CreatedDate >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.CreatedDate <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<ApplicationRole> RoleByLastUpdatedDateRange(IQueryable<ApplicationRole> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    DateTime start = Convert.ToDateTime(values[0]);
                    result = result.Where(c => c.UpdatedDate >= start);
                }


                if (values.Length > 1)
                {
                    if (!string.IsNullOrEmpty(values[1]))
                    {
                        DateTime end = Convert.ToDateTime(values[1]);
                        result = result.Where(c => c.UpdatedDate <= end);
                    }
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<ApplicationRole> RoleById(IQueryable<ApplicationRole> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    int id = Convert.ToInt32(values[0]);
                    result = result.Where(c => c.Id == id);
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<Student> StudentPostSearch(IQueryable<Student> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            if (op == "@=" && values != null && values.Length > 0)
            {
                var value = values[0].Replace("^^^",",").Replace("***", "|").Replace("@@@", "(").Replace("###",")");
                var result = source.Where(p => p.Name.Contains(value));
                return result;
            }
            else
            {
                return source;
            }
        }

        public IQueryable<Outlet> OutletByUserId(IQueryable<Outlet> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    int userId = Convert.ToInt32(values[0]);
                    result = result.Where(c => c.UserOutlets.Any(f => f.UserId == userId));
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        public IQueryable<CatererInfo> CatererByUserId(IQueryable<CatererInfo> source, string op, string[] values) // The method is given the {Operator} & {Value}
        {
            var result = source;

            if (values != null && values.Length > 0)
            {
                if (!string.IsNullOrEmpty(values[0]))
                {
                    int userId = Convert.ToInt32(values[0]);
                    result = result.Where(c => c.UserCaterers.Any(f => f.UserId == userId));
                }
            }
            return result; // Must return modified IQueryable<TEntity>
        }

        //public IQueryable<TokenOrder> OrderCurrentSession(IQueryable<TokenOrder> source, string op, string[] values) // The method is given the {Operator} & {Value}
        //{
        //    var result = source;

        //    if (values != null && values.Length > 0)
        //    {
        //        if (!string.IsNullOrEmpty(values[0]))
        //        {
        //            int sessionDetailId = Convert.ToInt32(values[0]);
        //            result = result.Where(c => c.Session.Id == sessionDetailId);
        //        }
        //    }
        //    return result; // Must return modified IQueryable<TEntity>
        //}
    }
}
