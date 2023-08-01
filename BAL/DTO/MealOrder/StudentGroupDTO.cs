using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class StudentGroupDTO
    {

        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? OutletId { get; set; }
        public List<StudentGroupDetailDTO> Sgdetails { get; set; }
    }

    public class StudentGroupDetailDTO
    {
        public int Id { get; set; }
        public int StudentGroupId { get; set; }
        public int StudentId { get; set; }

    }
}
