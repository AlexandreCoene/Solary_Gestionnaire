using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solary_Gestionnaire.Model
{
    public class MesureEnergie
    {
        public int mesure_id { get; set; }
        public int borne_id { get; set; }
        public double? energy_generated_kwh { get; set; }
        public double? energy_consumed_kwh { get; set; }

        // Propriété pour recevoir la date en string depuis l'API
        [JsonPropertyName("measure_date")]
        public string measure_date_string { get; set; }

        // Propriété calculée pour convertir en DateTime
        [JsonIgnore]
        public DateTime measure_date
        {
            get
            {
                if (DateTime.TryParse(measure_date_string, out DateTime result))
                {
                    return result;
                }
                return DateTime.Now; // Valeur par défaut si la conversion échoue
            }
        }

        public double voltage { get; set; }
        public double current { get; set; }
        public double power { get; set; }
        public double battery_level { get; set; }
        public double? total_energy { get; set; }
        public double? solar_power { get; set; }
    }
}
