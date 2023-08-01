using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Core.Audit.Configuration;

namespace DAL.Core.Audit.Extensions
{
    public static class EntityTypeConfigurationExtensions
    {
        public static TrackAllResponse<T> TrackAllProperties<T>(this EntityTypeBuilder<T> entityTypeConfig) where T : class
        {
            return EntityTracker.TrackAllProperties<T>();
        }

        public static OverrideTrackingResponse<T> OverrideTracking<T>(this EntityTypeBuilder<T> entityTypeConfig)
            where T : class
        {
            return EntityTracker.OverrideTracking<T>();
        }
    }
}
