using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Solary_Gestionnaire.ViewModel
{
    public class ModifierBorneViewModel : INotifyPropertyChanged
    {
        private readonly BorneService _borneService;
        private Borne _originalBorne;

        private int _borneId;
        private string _name;
        private string _address;
        private string _city;
        private string _postalCode;
        private string _latitude;
        private string _longitude;
        private string _powerOutput;
        private int _chargePercentage;
        private string _status;
        private bool _isInMaintenance;
        private bool _isLoading;
        private string _errorMessage;
        private string _successMessage;

        public int BorneId
        {
            get => _borneId;
            set
            {
                _borneId = value;
                OnPropertyChanged(nameof(BorneId));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public string City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged(nameof(City));
            }
        }

        public string PostalCode
        {
            get => _postalCode;
            set
            {
                _postalCode = value;
                OnPropertyChanged(nameof(PostalCode));
            }
        }

        public string Latitude
        {
            get => _latitude;
            set
            {
                _latitude = value;
                OnPropertyChanged(nameof(Latitude));
            }
        }

        public string Longitude
        {
            get => _longitude;
            set
            {
                _longitude = value;
                OnPropertyChanged(nameof(Longitude));
            }
        }

        public string PowerOutput
        {
            get => _powerOutput;
            set
            {
                _powerOutput = value;
                OnPropertyChanged(nameof(PowerOutput));
            }
        }

        public int ChargePercentage
        {
            get => _chargePercentage;
            set
            {
                _chargePercentage = value;
                OnPropertyChanged(nameof(ChargePercentage));
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public bool IsInMaintenance
        {
            get => _isInMaintenance;
            set
            {
                _isInMaintenance = value;
                OnPropertyChanged(nameof(IsInMaintenance));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
                OnPropertyChanged(nameof(LoadingVisibility));
            }
        }

        public Visibility LoadingVisibility => IsLoading ? Visibility.Visible : Visibility.Collapsed;

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(ErrorVisibility));
            }
        }

        public Visibility ErrorVisibility => string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                _successMessage = value;
                OnPropertyChanged(nameof(SuccessMessage));
                OnPropertyChanged(nameof(SuccessVisibility));
            }
        }

        public Visibility SuccessVisibility => string.IsNullOrEmpty(SuccessMessage) ? Visibility.Collapsed : Visibility.Visible;

        // Liste des statuts disponibles
        public string[] StatusOptions => new[] { "disponible", "occupé", "hors service" };

        public ModifierBorneViewModel(Borne borne)
        {
            _borneService = new BorneService();
            _originalBorne = borne;

            // Initialiser les propriétés avec les valeurs de la borne
            BorneId = borne.borne_id;
            Name = borne.name;
            Address = borne.address;
            City = borne.city;
            PostalCode = borne.postal_code;
            Latitude = borne.latitude;
            Longitude = borne.longitude;
            PowerOutput = borne.power_output;
            ChargePercentage = borne.charge_percentage;
            Status = borne.status;
            IsInMaintenance = borne.is_in_maintenance == 1;
        }

        public async Task<bool> ModifierBorneAsync()
        {
            if (string.IsNullOrWhiteSpace(Address))
            {
                ErrorMessage = "L'adresse est obligatoire.";
                return false;
            }

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                var borneModifiee = new Borne
                {
                    borne_id = BorneId,
                    name = Name ?? "",
                    address = Address,
                    city = City ?? "",
                    postal_code = PostalCode ?? "",
                    latitude = Latitude,
                    longitude = Longitude,
                    power_output = PowerOutput,
                    charge_percentage = ChargePercentage,
                    status = Status,
                    is_in_maintenance = IsInMaintenance ? 1 : 0,
                    created_at = _originalBorne.created_at,
                    last_used_at = _originalBorne.last_used_at
                };

                var success = await _borneService.UpdateBorneAsync(borneModifiee);

                if (success)
                {
                    SuccessMessage = "La borne a été modifiée avec succès.";
                    return true;
                }
                else
                {
                    ErrorMessage = "Erreur lors de la modification de la borne.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la modification de la borne: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
