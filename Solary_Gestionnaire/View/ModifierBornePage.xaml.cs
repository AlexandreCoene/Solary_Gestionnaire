using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Solary_Gestionnaire.View
{
    public partial class ModifierBornePage : UserControl
    {
        private ModifierBorneViewModel _viewModel;
        private Borne _borne;

        public ModifierBornePage(Borne borne)
        {
            InitializeComponent();
            _borne = borne;
            _viewModel = new ModifierBorneViewModel(borne);
            DataContext = _viewModel;
        }

        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            AnimatePageTransition(() => {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new DetailBornePage(_borne));
            });
        }

        private void AnnulerButton_Click(object sender, RoutedEventArgs e)
        {
            // Demander confirmation si des modifications ont été apportées
            var result = MessageBox.Show(
                "Voulez-vous vraiment annuler les modifications ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                AnimatePageTransition(() => {
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new DetailBornePage(_borne));
                });
            }
        }

        private async void EnregistrerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Désactiver les boutons pendant l'opération
                EnregistrerButton.IsEnabled = false;
                AnnulerButton.IsEnabled = false;

                bool success = await _viewModel.ModifierBorneAsync();

                if (success)
                {
                    // Attendre 2 secondes pour que l'utilisateur puisse voir le message de succès
                    await System.Threading.Tasks.Task.Delay(2000);

                    // Mettre à jour l'objet borne avec les nouvelles valeurs
                    _borne.name = _viewModel.Name;
                    _borne.address = _viewModel.Address;
                    _borne.city = _viewModel.City;
                    _borne.postal_code = _viewModel.PostalCode;
                    _borne.latitude = _viewModel.Latitude;
                    _borne.longitude = _viewModel.Longitude;
                    _borne.power_output = _viewModel.PowerOutput;
                    _borne.charge_percentage = _viewModel.ChargePercentage;
                    _borne.status = _viewModel.Status;
                    _borne.is_in_maintenance = _viewModel.IsInMaintenance ? 1 : 0;

                    // Retourner à la page de détails avec la borne mise à jour
                    AnimatePageTransition(() => {
                        MainGrid.Children.Clear();
                        MainGrid.Children.Add(new DetailBornePage(_borne));
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Une erreur s'est produite: {ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                // Réactiver les boutons
                EnregistrerButton.IsEnabled = true;
                AnnulerButton.IsEnabled = true;
            }
        }

        private void AnimatePageTransition(Action afterAnimation)
        {
            // Créer une animation de fondu
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            // Définir l'action à exécuter après l'animation
            fadeOut.Completed += (s, e) =>
            {
                // Exécuter l'action fournie
                afterAnimation?.Invoke();

                // Animer le retour en fondu
                DoubleAnimation fadeIn = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                MainGrid.BeginAnimation(OpacityProperty, fadeIn);
            };

            // Démarrer l'animation de sortie
            MainGrid.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
