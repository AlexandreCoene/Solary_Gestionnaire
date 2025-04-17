using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Solary_Gestionnaire.ViewModel;
using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.Service;
using System.ComponentModel;

namespace Solary_Gestionnaire.ViewModel
{
    public class AjouterBorneViewModel : INotifyPropertyChanged
    {
        private readonly BorneService _borneService;
        private string _name;
        private string _address;
        private string _city;
        private string _postalCode;
        private string _latitude = "0.0";
        private string _longitude = "0.0";
        private string _powerOutput = "11.00";
        private int _chargePercentage = 100;
        private string _status = "disponible";
        private bool _isInMaintenance;
        private bool _isLoading;
        private string _errorMessage;
        private string _successMessage;

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

        public AjouterBorneViewModel()
        {
            _borneService = new BorneService();
        }

        public async Task<bool> AjouterBorneAsync()
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

                var nouvelleBorne = new Borne
                {
                    name = Name ?? "",
                    address = Address,
                    city = City ?? "",
                    postal_code = PostalCode ?? "",
                    latitude = Latitude,
                    longitude = Longitude,
                    power_output = PowerOutput,
                    charge_percentage = ChargePercentage,
                    status = Status,
                    is_in_maintenance = IsInMaintenance ? 1 : 0
                };

                var success = await _borneService.AddBorneAsync(nouvelleBorne);

                if (success)
                {
                    SuccessMessage = "La borne a été ajoutée avec succès.";
                    // Réinitialiser les champs
                    ResetFields();
                    return true;
                }
                else
                {
                    ErrorMessage = "Erreur lors de l'ajout de la borne.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de l'ajout de la borne: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ResetFields()
        {
            Name = string.Empty;
            Address = string.Empty;
            City = string.Empty;
            PostalCode = string.Empty;
            Latitude = "0.0";
            Longitude = "0.0";
            PowerOutput = "11.00";
            ChargePercentage = 100;
            Status = "disponible";
            IsInMaintenance = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
