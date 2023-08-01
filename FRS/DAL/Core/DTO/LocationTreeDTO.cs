using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class LocationTreeDTO
    {
        public LocationTreeDTO()
        {
            Children = new List<LocationTreeDTO>();
        }

        public LocationTreeDTO(int id, string value, string type)
        {
            this.Id = id;
            this.Value = value;
            this.Type = type;
        }

        public int Id { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Settings
        {
            get
            {
                var cssClass = "";
                if (!ParentId.HasValue)
                {
                    cssClass = GetRootSettings();
                }
                else
                {
                    if (Type == LocationTypes.Building.ToString())
                    {
                        cssClass = GetBuildingSettings();
                    }
                    else
                    {
                        cssClass = GetRoomSettings();
                    }
                }
                return cssClass;
            }
        }
        public int? ParentId { get; set; }
        public List<LocationTreeDTO> Children { get; set; }

        private string GetRootSettings()
        {
            return @"{'cssClasses': {
                      'expanded': 'fa fa-caret-down fa-sm',
                      'collapsed': 'fa fa-caret-right fa-sm',
                      'leaf': 'fa fa-sm',
                      'empty': 'fa fa-caret-right disabled'
                    },
                    'templates': {
                      'node': '<i class=""fa fa-bank fa-sm""></i>',
                      'leaf': '<i class=""fa fa-building-o fa-sm"" ></i>',
                      'leftMenu': '<i class=""fa fa-navicon fa-sm"" ></i>'
                    }}";
        }

        private string GetBuildingSettings()
        {
            return @"'{cssClasses': {
                      'expanded': 'fa fa-caret-down fa-sm',
                      'collapsed': 'fa fa-caret-right fa-sm',
                      'leaf': 'fa fa-sm',
                      'empty': 'fa fa-caret-right disabled'
                    },
                    'templates': {
                      'node': '<i class=""fa fa-building-o fa-sm""></i>',
                      'leaf': '<i class=""fa fa-building-o fa-sm"" ></i>',
                      'leftMenu': '<i class=""fa fa-navicon fa-sm"" ></i>'
                    }}";
        }

        private string GetRoomSettings()
        {
            return @"{'cssClasses': {
                      'expanded': 'fa fa-caret-down fa-sm',
                      'collapsed': 'fa fa-caret-right fa-sm',
                      'leaf': 'fa fa-sm',
                      'empty': 'fa fa-caret-right disabled'
                    },
                    'templates': {
                      'leaf': '<i class=""fa fa-hospital-o fa-sm"" ></i>',
                      'leftMenu': '<i class=""fa fa-navicon fa-sm"" ></i>'
                    }}";
        }
    }

    public class LocationTreeFilter
    {
        public int? CurrentUserInstitutionId { get; set; }
        public int? InstitutionId { get; set; }
        public int? ImageReferenceColorId { get; set; }
        public string Keyword { get; set; }
        public bool IsBookingOnly { get; set; }
    }

}
