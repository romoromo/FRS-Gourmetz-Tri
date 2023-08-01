
using System;
using System.Collections.Generic;

namespace FRS.ViewModels
{
    public class SignagePublicationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Approved { get; set; }
        public bool Rejected { get; set; }
        public string Remark { get; set; }

        public int? ApprovedBy { get; set; }

        public string ApprovedByName { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public int? RejectedBy { get; set; }

        public string RejectedByName { get; set; }

        public DateTime? RejectedDate { get; set; }

        public int? CreatedBy { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<SignageScheduleViewModel> schedules { get; set; }
    }
}
