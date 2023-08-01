using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class KioskSettingsViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Label is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Label must be between 2 and 200 characters")]
        public string Label { get; set; }

        public int screen_save_time { get; set; }

        public int? ss_playlist_id { get; set; }
        public PlaylistViewModel ss_playlist { get; set; }


        //[ForeignKey("ImageFile")]
        public int? bannerId { get; set; }
        public ImageViewModel bannerImage { get; set; }

        //[ForeignKey("Playlist")]
        public int? top_banner_id { get; set; }
        public PlaylistViewModel top_banner { get; set; }

        //[ForeignKey("Playlist")]
        public int? bottom_banner_id { get; set; }
        public PlaylistViewModel bottom_banner { get; set; }

        public float? route_weight { get; set; }
        public float? route_speed { get; set; }

        public float? zoom_level { get; set; }
        public float? floor_interval { get; set; }

        public string arrow_image { get; set; }
        public string arrow_color { get; set; }
        public float? arrow_speed { get; set; }
        public string line_color { get; set; }

        public string route_color { get; set; }

        public string jan { get; set; }
        public string feb { get; set; }
        public string mar { get; set; }
        public string apr { get; set; }
        public string may { get; set; }
        public string jun { get; set; }
        public string jul { get; set; }
        public string aug { get; set; }
        public string sep { get; set; }
        public string oct { get; set; }
        public string nov { get; set; }
        public string dec { get; set; }

        public string dir_color { get; set; }

        public string dir_font_color { get; set; }

        public string dir_font_type { get; set; }

        public string dir_font_type2 { get; set; }

        public float? dir_font_size { get; set; }

        public string baloon_image { get; set; }

        public float? baloon_width { get; set; }

        public float? baloon_height { get; set; }

        public float? anchor_h { get; set; }

        public float? anchor_v { get; set; }

        //[ForeignKey("Playlist")]
        public int? def_event_id { get; set; }
        public PlaylistViewModel def_event { get; set; }
    }
}
