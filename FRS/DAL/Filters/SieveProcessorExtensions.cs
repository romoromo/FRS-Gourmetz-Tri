using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Filters
{
    public static class SieveProcessorExtensions
    {
        public static async Task<PagedEntity<T>> GetPagedAsync<T>(this ISieveProcessor sieveProcessor, IQueryable<T> query, SieveModel sieveModel = null) where T : class
        {
            var result = new PagedEntity<T>();

            var (pagedQuery, page, pageSize, rowCount, pageCount) = await GetPagedEntityAsync(sieveProcessor, query, sieveModel);

            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.TotalCount = rowCount;
            result.PageCount = pageCount;

            result.PagedData = await pagedQuery.ToListAsync();
            result.Filter = sieveModel as BaseFilter;

            return result;
        }

        private static async Task<(IQueryable<T> pagedQuery, int page, int pageSize, int rowCount, int pageCount)> GetPagedEntityAsync<T>(ISieveProcessor sieveProcessor, IQueryable<T> query, SieveModel sieveModel = null) where T : class
        {
            //page < 1 or null means all;
            var page = sieveModel?.Page ?? 1;
            var pageSize = sieveModel?.PageSize ?? 20;

            IQueryable<T> pagedQuery = null;
            int pageCount = 1;
            if (sieveModel != null)
            {
                // apply pagination in a later step
                query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
            }

            var rowCount = await query.CountAsync();

            if (sieveModel?.Page > 0 && sieveModel?.PageSize > 0)
            {
                pageCount = (int)Math.Ceiling((double)rowCount / pageSize);

                var skip = (page - 1) * pageSize;
                pagedQuery = query.Skip(skip).Take(pageSize);
            }
            else
            {
                pagedQuery = query;
            }

            return (pagedQuery, page, pageSize, rowCount, pageCount);
        }
    }

}
