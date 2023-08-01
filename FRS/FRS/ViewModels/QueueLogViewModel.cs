using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class QueueLogViewModel
    {
        public int Id { get; set; }
        public string Queueid { get; set; }
        public string CallAction { get; set; }
        public string QueueNo { get; set; }
        public string StationId { get; set; }
        public string RoomName { get; set; }


        public string MessageId { get; set; }
		public string InstitutionId { get; set; }
		public string ClinicId { get; set; }
		public string TerminalId { get; set; }
		public string TerminalName { get; set; }
		public string QueueNumber { get; set; }
		public string Timestamp { get; set; }

		public string Token { get; set; }
        public bool isT { get; set; }
    }
}
