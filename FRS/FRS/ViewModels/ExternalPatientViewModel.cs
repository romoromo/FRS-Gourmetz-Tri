using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class PatientInfo
    {
        public object this[string propertyName]
        {
            get { return GetType().GetProperty(propertyName).GetValue(this, null); }
            set { GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

        public long feedback_log_patient_info_id { get; set; }
        public string profile_id { get; set; }
        public string identifier { get; set; }
        public string name { get; set; }
        public string preferred_name { get; set; }
        public string gender { get; set; }
        public bool vip { get; set; }
        public string case_number { get; set; }
        public DateTime? admission_date { get; set; }
        public string status { get; set; }
        public string location_code { get; set; }
        public string location_label { get; set; }
        public string location_alias { get; set; }
        public string attending_doctor { get; set; }
        public string attending_nurse { get; set; }
        public string ongoing_remarks { get; set; }
        public string ahp_remarks { get; set; }
        public string pib_location { get; set; }
        //public string mode_of_feeding { get; set; }
        //public string food_restrictions { get; set; }
        //public string diet_type { get; set; }
        //public string diet_texture { get; set; }
        //public string fluid_restrictions { get; set; }
        //public string function_status { get; set; }
        //public string special_instructions { get; set; }
        public List<Restriction> restrictions { get; set; }
        public List<RestrictionFlatten> restrictions_flatten
        {
            get
            {
                return restrictions != null ?
                    restrictions.GroupBy(e => e.type.restriction_type_id).Select(e => new RestrictionFlatten
                    {
                        restriction_type_id = e.Key,
                        restriction_labels = string.Join(", ", e.Select(f => f.label)),
                        pictures = string.Join(", ", e.Select(f => f.picture))
                    }).ToList() : new List<RestrictionFlatten>();
            }
        }
        public List<Language> languages { get; set; }
        public string languages_flatten
        {
            get
            {
                return languages != null ? string.Join(", ", languages.Select(f => f.label)) : string.Empty;
            }
        }
        public List<DrugAllergy> drug_allergies { get; set; }
        public string drug_allergies_flatten
        {
            get
            {
                return drug_allergies != null ? string.Join(", ", drug_allergies.Select(f => f.name)) : string.Empty;
            }
        }
    }

    public class RestrictionType
    {
        public object this[string propertyName]
        {
            get { return GetType().GetProperty(propertyName).GetValue(this, null); }
            set { GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

        public long restriction_type_id { get; set; }
        public long tenant_id { get; set; }
        public string code { get; set; }
        public string label { get; set; }
        //public bool configurable { get; set; }
        //public bool special_condition { get; set; }
        //public bool carried_forward { get; set; }
        //public bool use_in_meal_filtering { get; set; }
    }

    public class Restriction
    {
        public object this[string propertyName]
        {
            get { return GetType().GetProperty(propertyName).GetValue(this, null); }
            set { GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

        public long restriction_id { get; set; }
        public RestrictionType type { get; set; }
        public string code { get; set; }
        public string label { get; set; }
        public string picture { get; set; }
        //public bool configurable { get; set; }
        //public bool special_condition { get; set; }
        //public bool carried_forward { get; set; }
        //public bool use_in_meal_filtering { get; set; }
    }

    public class RestrictionFlatten
    {
        public long restriction_type_id { get; set; }
        public string restriction_labels { get; set; }
        public string pictures { get; set; }
    }

    public class Language
    {
        public object this[string propertyName]
        {
            get { return GetType().GetProperty(propertyName).GetValue(this, null); }
            set { GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

        public long language_id { get; set; }
        public string code { get; set; }
        public string label { get; set; }
    }

    public class DrugAllergy
    {
        public object this[string propertyName]
        {
            get { return GetType().GetProperty(propertyName).GetValue(this, null); }
            set { GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

        public long drug_allergy_id { get; set; }
        public string name { get; set; }
    }
}
