using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solary_Gestionnaire.Model
{
    public class Borne
    {
        public int borne_id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string postal_code { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string power_output { get; set; }
        public int charge_percentage { get; set; }
        public string status { get; set; }
        public int is_in_maintenance { get; set; }
        public string created_at { get; set; }
        public string last_used_at { get; set; }
    }
}

