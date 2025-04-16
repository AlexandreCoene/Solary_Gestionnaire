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
        public string location { get; set; }
        public int is_available { get; set; }
        public string created_at { get; set; }
    }
}