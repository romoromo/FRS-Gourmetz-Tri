using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class StudentGroupTokenOrderSummaryDTO
    {
        public StudentGroupTokenOrderSummaryDTO()
        {
            this.Cols = new List<string>();
            this.Rows = new List<StudentGroupTokenOrderRowDTO>();
        }
        public List<string> Cols { get; set; }
        public List<StudentGroupTokenOrderRowDTO> Rows { get; set; }
        public StudentGroupTokenOrderRowDTO Total { get; set; }
    }

    public class StudentGroupTokenOrderRowDTO
    {
        public StudentGroupTokenOrderRowDTO()
        {
            this.Cells = new List<string>();
        }
        public List<string> Cells { get; set; }
    }
}
