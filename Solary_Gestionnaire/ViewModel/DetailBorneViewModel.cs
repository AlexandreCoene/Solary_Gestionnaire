using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Solary_Gestionnaire.ViewModel
{
    public class DetailBorneViewModel : INotifyPropertyChanged
    {
        private readonly BorneService _borneService;
        private Borne _borne;

        // Propriétés avec getters et setters privés
        public int BorneId { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string PostalCode { get; private set; }
        public string Coordinates { get; private set; }
        public string PowerOutput { get; private set; }
        public int ChargePercentage { get; private set; }
        public string Status { get; private set; }
        public bool IsInMaintenance { get; private set; }
        public string CreatedAt { get; private set; }
        public string LastUsedAt { get; private set; }

        // Propriétés calculées
        public string MaintenanceStatus => IsInMaintenance ? "En maintenance" : "Pas en maintenance";
        public Brush MaintenanceColor => IsInMaintenance ? Brushes.Orange : Brushes.LightGreen;

        public DetailBorneViewModel(Borne borne)
        {
            _borneService = new BorneService();
            _borne = borne;

            // Initialiser les propriétés
            BorneId = _borne.borne_id;
            Name = _borne.name;
            Address = _borne.address;
            City = _borne.city;
            PostalCode = _borne.postal_code;
            Coordinates = $"{_borne.latitude}, {_borne.longitude}";
            PowerOutput = _borne.power_output;
            ChargePercentage = _borne.charge_percentage;
            Status = _borne.status;
            IsInMaintenance = _borne.is_in_maintenance == 1;
            CreatedAt = _borne.created_at;
            LastUsedAt = _borne.last_used_at;
        }

        public async Task<bool> DeleteBorneAsync()
        {
            try
            {
                return await _borneService.DeleteBorneAsync(_borne.borne_id);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
