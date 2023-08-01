using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class ClassDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ClassLevelId { get; set; }

        public string ClassLevelName { get; set; }
    }
}
