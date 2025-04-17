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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.ViewModel;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Solary_Gestionnaire.View
{
    public partial class AjouterBornePage : UserControl
    {
        private AjouterBorneViewModel _viewModel;

        public AjouterBornePage()
        {
            InitializeComponent();
            _viewModel = new AjouterBorneViewModel();
            DataContext = _viewModel;
        }

        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            AnimatePageTransition(() => {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new GestionnaireBornePage());
            });
        }

        private async void AjouterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Désactiver le bouton pendant l'opération
                AjouterButton.IsEnabled = false;

                bool success = await _viewModel.AjouterBorneAsync();

                // Si l'ajout est réussi, on peut automatiquement retourner à la page de gestion après un délai
                if (success)
                {
                    // Attendre 2 secondes pour que l'utilisateur puisse voir le message de succès
                    await System.Threading.Tasks.Task.Delay(2000);

                    AnimatePageTransition(() => {
                        MainGrid.Children.Clear();
                        MainGrid.Children.Add(new GestionnaireBornePage());
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
                // Réactiver le bouton
                AjouterButton.IsEnabled = true;
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
