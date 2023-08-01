using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Core.DTO
{
    public class CalendarEvent
    {
        public int? id { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string title { get; set; }
        public EventColor color { get; set; }
        public Boolean? allDay { get; set; }
        public dynamic meta { get; set; }
    }

    public class EventColor
    {
        public string primary { get; set; }
        public string secondary { get; set; }
    }
}
