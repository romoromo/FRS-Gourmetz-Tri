using DAL.Filters;
using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class PagedEntityViewModel<T> where T : class
    {
        public int TotalCount { get; set; }
        public BaseFilter Filter { get; set; }

        public IList<T> PagedData { get; set; }
    }
}
