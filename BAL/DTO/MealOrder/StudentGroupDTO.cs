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
        public string Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? DeliveryStartDate { get; set; }
        public DateTime? DeliveryEndDate { get; set; }
        public int? MealSessionId { get; set; }
        public string MealSessionName { get; set; }
        public float Price { get; set; }
        public int Term { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public List<StudentGroupDetailDTO> Sgdetails { get; set; }
    }

    public class StudentGroupDetailDTO
    {
        public int Id { get; set; }
        public int StudentGroupId { get; set; }
        public int StudentId { get; set; }

    }
}
