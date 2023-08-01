using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class EmployeeDataViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public int? EmployeeDesignationId { get; set; }
        public string EmployeeDesignationCode { get; set; }
        public string EmployeeDesignationLabel { get; set; }

        public string DisplayName { get; set; }

        public string picture_url { get; set; }
    }
}
