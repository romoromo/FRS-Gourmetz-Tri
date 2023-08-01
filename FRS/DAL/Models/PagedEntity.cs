using DAL.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class PagedEntity<T> where T : class
    {
        public int TotalCount { get; set; }
        public BaseFilter Filter { get; set; }

        public IList<T> PagedData { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }

        public PagedEntity()
        {
            PagedData = new List<T>();
        }
    }
}
