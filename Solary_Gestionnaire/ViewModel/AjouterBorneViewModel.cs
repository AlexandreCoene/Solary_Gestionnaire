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
        private string _location;
        private bool _isAvailable = true;
        private bool _isLoading;
        private string _errorMessage;
        private string _successMessage;

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                _isAvailable = value;
                OnPropertyChanged(nameof(IsAvailable));
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

        public AjouterBorneViewModel()
        {
            _borneService = new BorneService();
        }

        public async Task<bool> AjouterBorneAsync()
        {
            if (string.IsNullOrWhiteSpace(Location))
            {
                ErrorMessage = "L'emplacement est obligatoire.";
                return false;
            }

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                var nouvelleBorne = new Borne
                {
                    location = Location,
                    is_available = IsAvailable ? 1 : 0,
                    created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                var success = await _borneService.AddBorneAsync(nouvelleBorne);

                if (success)
                {
                    SuccessMessage = "La borne a été ajoutée avec succès.";
                    // Réinitialiser les champs
                    Location = string.Empty;
                    IsAvailable = true;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
