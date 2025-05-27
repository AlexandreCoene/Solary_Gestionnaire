using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.Service;
using System.Windows.Media;

namespace Solary_Gestionnaire.ViewModel
{
    public class StatistiquesViewModel : INotifyPropertyChanged
    {
        private readonly MesureService _mesureService;
        private readonly BorneService _borneService;
        private ObservableCollection<MesureEnergie> _mesures;
        private ObservableCollection<Borne> _bornes;
        private Borne _selectedBorne;
        private bool _isLoading;
        private string _errorMessage;
        private bool _hasError;

        // Champs privés pour les collections
        private SeriesCollection _voltageSeriesCollection;
        private SeriesCollection _currentSeriesCollection;
        private SeriesCollection _powerSeriesCollection;
        private SeriesCollection _batteryLevelSeriesCollection;
        private SeriesCollection _energyGeneratedSeriesCollection;
        private SeriesCollection _energyConsumedSeriesCollection;
        private SeriesCollection _solarPowerSeriesCollection;
        private SeriesCollection _totalEnergySeriesCollection;
        private List<string> _labels;

        // Propriétés pour les graphiques avec garantie de non-null
        public SeriesCollection VoltageSeriesCollection
        {
            get => _voltageSeriesCollection ?? (_voltageSeriesCollection = new SeriesCollection());
            set
            {
                _voltageSeriesCollection = value;
                OnPropertyChanged(nameof(VoltageSeriesCollection));
            }
        }

        public SeriesCollection CurrentSeriesCollection
        {
            get => _currentSeriesCollection ?? (_currentSeriesCollection = new SeriesCollection());
            set
            {
                _currentSeriesCollection = value;
                OnPropertyChanged(nameof(CurrentSeriesCollection));
            }
        }

        public SeriesCollection PowerSeriesCollection
        {
            get => _powerSeriesCollection ?? (_powerSeriesCollection = new SeriesCollection());
            set
            {
                _powerSeriesCollection = value;
                OnPropertyChanged(nameof(PowerSeriesCollection));
            }
        }

        public SeriesCollection BatteryLevelSeriesCollection
        {
            get => _batteryLevelSeriesCollection ?? (_batteryLevelSeriesCollection = new SeriesCollection());
            set
            {
                _batteryLevelSeriesCollection = value;
                OnPropertyChanged(nameof(BatteryLevelSeriesCollection));
            }
        }

        public SeriesCollection EnergyGeneratedSeriesCollection
        {
            get => _energyGeneratedSeriesCollection ?? (_energyGeneratedSeriesCollection = new SeriesCollection());
            set
            {
                _energyGeneratedSeriesCollection = value;
                OnPropertyChanged(nameof(EnergyGeneratedSeriesCollection));
            }
        }

        public SeriesCollection EnergyConsumedSeriesCollection
        {
            get => _energyConsumedSeriesCollection ?? (_energyConsumedSeriesCollection = new SeriesCollection());
            set
            {
                _energyConsumedSeriesCollection = value;
                OnPropertyChanged(nameof(EnergyConsumedSeriesCollection));
            }
        }

        public SeriesCollection SolarPowerSeriesCollection
        {
            get => _solarPowerSeriesCollection ?? (_solarPowerSeriesCollection = new SeriesCollection());
            set
            {
                _solarPowerSeriesCollection = value;
                OnPropertyChanged(nameof(SolarPowerSeriesCollection));
            }
        }

        public SeriesCollection TotalEnergySeriesCollection
        {
            get => _totalEnergySeriesCollection ?? (_totalEnergySeriesCollection = new SeriesCollection());
            set
            {
                _totalEnergySeriesCollection = value;
                OnPropertyChanged(nameof(TotalEnergySeriesCollection));
            }
        }

        public List<string> Labels
        {
            get => _labels ?? (_labels = new List<string>());
            set
            {
                _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }

        public Func<double, string> YFormatter { get; set; }

        public StatistiquesViewModel()
        {
            Console.WriteLine("=== INITIALISATION StatistiquesViewModel ===");

            _mesureService = new MesureService();
            _borneService = new BorneService();
            _mesures = new ObservableCollection<MesureEnergie>();
            _bornes = new ObservableCollection<Borne>();

            Labels = new List<string>();
            YFormatter = value => value.ToString("N1");

            // Charger les données
            LoadBornesAsync();
        }

        public ObservableCollection<MesureEnergie> Mesures
        {
            get => _mesures;
            set
            {
                _mesures = value;
                OnPropertyChanged(nameof(Mesures));
            }
        }

        public ObservableCollection<Borne> Bornes
        {
            get => _bornes;
            set
            {
                _bornes = value;
                OnPropertyChanged(nameof(Bornes));
            }
        }

        public Borne SelectedBorne
        {
            get => _selectedBorne;
            set
            {
                Console.WriteLine($"Borne sélectionnée: {value?.name ?? "null"}");
                _selectedBorne = value;
                OnPropertyChanged(nameof(SelectedBorne));
                if (_selectedBorne != null)
                {
                    // Recharger complètement le ViewModel pour cette borne
                    ReloadViewModelForBorne(_selectedBorne.borne_id);
                }
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
                Console.WriteLine($"IsLoading: {value}");
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
                Console.WriteLine($"ErrorMessage: {value}");
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged(nameof(HasError));
                OnPropertyChanged(nameof(ErrorVisibility));
                Console.WriteLine($"HasError: {value}");
            }
        }

        public Visibility ErrorVisibility => HasError ? Visibility.Visible : Visibility.Collapsed;

        private async Task LoadBornesAsync()
        {
            try
            {
                Console.WriteLine("=== CHARGEMENT DES BORNES ===");
                IsLoading = true;
                HasError = false;

                var bornes = await _borneService.GetAllBornesAsync();
                Console.WriteLine($"Bornes récupérées: {bornes?.Count ?? 0}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Bornes.Clear();
                    foreach (var borne in bornes)
                    {
                        Console.WriteLine($"Ajout borne: {borne.name} (ID: {borne.borne_id})");
                        Bornes.Add(borne);
                    }

                    // Sélectionner la première borne par défaut si disponible
                    if (Bornes.Count > 0)
                    {
                        Console.WriteLine($"Sélection automatique de la première borne: {Bornes[0].name}");
                        SelectedBorne = Bornes[0];
                    }
                    else
                    {
                        Console.WriteLine("Aucune borne disponible");
                        HasError = true;
                        ErrorMessage = "Aucune borne disponible";
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR LoadBornesAsync: {ex.Message}");
                HasError = true;
                ErrorMessage = $"Erreur lors du chargement des bornes: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void ReloadViewModelForBorne(int borneId)
        {
            try
            {
                Console.WriteLine($"=== RECHARGEMENT COMPLET POUR BORNE {borneId} ===");
                IsLoading = true;
                HasError = false;
                ErrorMessage = "";

                // Réinitialiser complètement toutes les collections
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Vider les mesures
                    Mesures.Clear();

                    // Recréer toutes les collections de séries
                    _voltageSeriesCollection = new SeriesCollection();
                    _currentSeriesCollection = new SeriesCollection();
                    _powerSeriesCollection = new SeriesCollection();
                    _batteryLevelSeriesCollection = new SeriesCollection();
                    _energyGeneratedSeriesCollection = new SeriesCollection();
                    _energyConsumedSeriesCollection = new SeriesCollection();
                    _solarPowerSeriesCollection = new SeriesCollection();
                    _totalEnergySeriesCollection = new SeriesCollection();
                    _labels = new List<string>();

                    // Notifier tous les changements
                    OnPropertyChanged(nameof(VoltageSeriesCollection));
                    OnPropertyChanged(nameof(CurrentSeriesCollection));
                    OnPropertyChanged(nameof(PowerSeriesCollection));
                    OnPropertyChanged(nameof(BatteryLevelSeriesCollection));
                    OnPropertyChanged(nameof(EnergyGeneratedSeriesCollection));
                    OnPropertyChanged(nameof(EnergyConsumedSeriesCollection));
                    OnPropertyChanged(nameof(SolarPowerSeriesCollection));
                    OnPropertyChanged(nameof(TotalEnergySeriesCollection));
                    OnPropertyChanged(nameof(Labels));
                });

                // Charger les nouvelles données
                var mesures = await _mesureService.GetMesuresEnergieByBorneIdAsync(borneId);
                Console.WriteLine($"Mesures reçues pour borne {borneId}: {mesures?.Count ?? 0}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (mesures != null && mesures.Count > 0)
                    {
                        // Trier les mesures par date
                        mesures = mesures.OrderBy(m => m.measure_date).ToList();
                        Console.WriteLine($"Mesures triées - Première: {mesures[0].measure_date}, Dernière: {mesures[mesures.Count - 1].measure_date}");

                        foreach (var mesure in mesures)
                        {
                            Mesures.Add(mesure);
                        }

                        Console.WriteLine($"Mesures ajoutées à la collection UI: {Mesures.Count}");

                        // Créer les nouveaux graphiques
                        CreateNewCharts();
                    }
                    else
                    {
                        Console.WriteLine($"Aucune mesure trouvée pour la borne {borneId}");
                        HasError = true;
                        ErrorMessage = $"Aucune donnée trouvée pour la borne sélectionnée (ID: {borneId})";
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR ReloadViewModelForBorne: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    HasError = true;
                    ErrorMessage = $"Erreur lors du chargement des mesures: {ex.Message}";
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CreateNewCharts()
        {
            Console.WriteLine($"=== CRÉATION NOUVEAUX GRAPHIQUES - {Mesures.Count} mesures ===");

            if (Mesures.Count == 0)
            {
                Console.WriteLine("Aucune mesure à afficher");
                return;
            }

            // Préparer les données pour les graphiques
            var voltageValues = new ChartValues<double>();
            var currentValues = new ChartValues<double>();
            var powerValues = new ChartValues<double>();
            var batteryLevelValues = new ChartValues<double>();
            var energyGeneratedValues = new ChartValues<double>();
            var energyConsumedValues = new ChartValues<double>();
            var solarPowerValues = new ChartValues<double>();
            var totalEnergyValues = new ChartValues<double>();

            foreach (var mesure in Mesures)
            {
                Labels.Add(mesure.measure_date.ToString("dd/MM HH:mm"));

                voltageValues.Add(mesure.voltage);
                currentValues.Add(mesure.current);
                powerValues.Add(mesure.power);
                batteryLevelValues.Add(mesure.battery_level);

                // Gérer les valeurs nullables
                energyGeneratedValues.Add(mesure.energy_generated_kwh ?? 0);
                energyConsumedValues.Add(mesure.energy_consumed_kwh ?? 0);
                solarPowerValues.Add(mesure.solar_power ?? 0);
                totalEnergyValues.Add(mesure.total_energy ?? 0);
            }

            Console.WriteLine($"Valeurs préparées - Labels: {Labels.Count}, Voltage: {voltageValues.Count}");
            if (voltageValues.Count > 0)
            {
                Console.WriteLine($"Exemple valeurs - Voltage[0]: {voltageValues[0]}, Current[0]: {currentValues[0]}");
            }

            // Créer les nouvelles séries
            VoltageSeriesCollection.Add(new LineSeries
            {
                Title = "Tension (V)",
                Values = voltageValues,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 8,
                Stroke = new SolidColorBrush(Color.FromRgb(63, 81, 181)),
                Fill = Brushes.Transparent
            });

            CurrentSeriesCollection.Add(new LineSeries
            {
                Title = "Courant (A)",
                Values = currentValues,
                PointGeometry = DefaultGeometries.Square,
                PointGeometrySize = 8,
                Stroke = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                Fill = Brushes.Transparent
            });

            PowerSeriesCollection.Add(new LineSeries
            {
                Title = "Puissance (W)",
                Values = powerValues,
                PointGeometry = DefaultGeometries.Diamond,
                PointGeometrySize = 8,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 193, 7)),
                Fill = Brushes.Transparent
            });

            BatteryLevelSeriesCollection.Add(new LineSeries
            {
                Title = "Niveau de batterie (%)",
                Values = batteryLevelValues,
                PointGeometry = DefaultGeometries.Triangle,
                PointGeometrySize = 8,
                Stroke = new SolidColorBrush(Color.FromRgb(156, 39, 176)),
                Fill = Brushes.Transparent
            });

            EnergyGeneratedSeriesCollection.Add(new LineSeries
            {
                Title = "Énergie générée (kWh)",
                Values = energyGeneratedValues,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 8,
                Stroke = new SolidColorBrush(Color.FromRgb(0, 188, 212)),
                Fill = Brushes.Transparent
            });

            EnergyConsumedSeriesCollection.Add(new LineSeries
            {
                Title = "Énergie consommée (kWh)",
                Values = energyConsumedValues,
                PointGeometry = DefaultGeometries.Square,
                PointGeometrySize = 8,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 87, 34)),
                Fill = Brushes.Transparent
            });

            SolarPowerSeriesCollection.Add(new LineSeries
            {
                Title = "Puissance solaire (W)",
                Values = solarPowerValues,
                PointGeometry = DefaultGeometries.Diamond,
                PointGeometrySize = 8,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 235, 59)),
                Fill = Brushes.Transparent
            });

            TotalEnergySeriesCollection.Add(new LineSeries
            {
                Title = "Énergie totale (kWh)",
                Values = totalEnergyValues,
                PointGeometry = DefaultGeometries.Triangle,
                PointGeometrySize = 8,
                Stroke = new SolidColorBrush(Color.FromRgb(121, 85, 72)),
                Fill = Brushes.Transparent
            });

            Console.WriteLine("Nouvelles séries créées et ajoutées aux collections");

            // Notifier tous les changements
            OnPropertyChanged(nameof(VoltageSeriesCollection));
            OnPropertyChanged(nameof(CurrentSeriesCollection));
            OnPropertyChanged(nameof(PowerSeriesCollection));
            OnPropertyChanged(nameof(BatteryLevelSeriesCollection));
            OnPropertyChanged(nameof(EnergyGeneratedSeriesCollection));
            OnPropertyChanged(nameof(EnergyConsumedSeriesCollection));
            OnPropertyChanged(nameof(SolarPowerSeriesCollection));
            OnPropertyChanged(nameof(TotalEnergySeriesCollection));
            OnPropertyChanged(nameof(Labels));

            Console.WriteLine("=== FIN CRÉATION NOUVEAUX GRAPHIQUES ===");
        }

        private void NotifyChartsChanged()
        {
            OnPropertyChanged(nameof(VoltageSeriesCollection));
            OnPropertyChanged(nameof(CurrentSeriesCollection));
            OnPropertyChanged(nameof(PowerSeriesCollection));
            OnPropertyChanged(nameof(BatteryLevelSeriesCollection));
            OnPropertyChanged(nameof(EnergyGeneratedSeriesCollection));
            OnPropertyChanged(nameof(EnergyConsumedSeriesCollection));
            OnPropertyChanged(nameof(SolarPowerSeriesCollection));
            OnPropertyChanged(nameof(TotalEnergySeriesCollection));
            OnPropertyChanged(nameof(Labels));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
