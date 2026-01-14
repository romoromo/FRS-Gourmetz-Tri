using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.States;
using Hangfire.Storage;
using System;

namespace FRS.Filters
{
    public class HanfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }

    public class JobExpirationAttribute : JobFilterAttribute, IApplyStateFilter
    {
        private readonly TimeSpan _expiration;

        public JobExpirationAttribute(int days)
        {
            _expiration = TimeSpan.FromDays(days);
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = _expiration;
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
        }
    }
}
