using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class ReservationPictureViewModel
    {
        public int Id { get; set; }
        public string PictureUrl { get; set; }
        public int? ReservationId { get; set; }
    }
}
