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

        public async Task<bool> DeleteBorneAsync(int borneId)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var success = await _borneService.DeleteBorneAsync(borneId);

                if (success)
                {
                    // Supprimer la borne de la collection
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var borneToRemove = Bornes.FirstOrDefault(b => b.borne_id == borneId);
                        if (borneToRemove != null)
                        {
                            Bornes.Remove(borneToRemove);
                        }
                    });
                    return true;
                }
                else
                {
                    ErrorMessage = "Erreur lors de la suppression de la borne.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la suppression de la borne: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Méthode pour récupérer une borne par son ID
        public Borne GetBorneById(int borneId)
        {
            return Bornes.FirstOrDefault(b => b.borne_id == borneId);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
