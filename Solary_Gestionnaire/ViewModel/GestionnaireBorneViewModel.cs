using Solary_Gestionnaire.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using Solary_Gestionnaire.Model;
using System.Runtime.CompilerServices;

namespace Solary_Gestionnaire.ViewModel
{
    public class GestionnaireBorneViewModel : INotifyPropertyChanged
    {
        private readonly BorneService _borneService;
        private ObservableCollection<Borne> _bornes;
        private bool _isLoading;
        private string _errorMessage;

        public ObservableCollection<Borne> Bornes
        {
            get => _bornes;
            set
            {
                _bornes = value;
                OnPropertyChanged(nameof(Bornes));
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

        public GestionnaireBorneViewModel()
        {
            _borneService = new BorneService();
            Bornes = new ObservableCollection<Borne>();

            // Charger les bornes au démarrage
            Task.Run(async () => await LoadBornesAsync());
        }

        public async Task LoadBornesAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var bornes = await _borneService.GetAllBornesAsync();

                // Mettre à jour l'interface sur le thread UI
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Bornes.Clear();
                    foreach (var borne in bornes)
                    {
                        Bornes.Add(borne);
                    }
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement des bornes: {ex.Message}";
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
