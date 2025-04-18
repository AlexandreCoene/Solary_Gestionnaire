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
    public partial class DetailBornePage : UserControl
    {
        private DetailBorneViewModel _viewModel;
        private Borne _borne;

        public DetailBornePage(Borne borne)
        {
            InitializeComponent();
            _borne = borne;
            _viewModel = new DetailBorneViewModel(borne);
            DataContext = _viewModel;
        }

        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            AnimatePageTransition(() => {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new GestionnaireBornePage());
            });
        }

        private void ModifierBorne_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la page de modification
            AnimatePageTransition(() => {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new ModifierBornePage(_borne));
            });
        }

        private async void SupprimerBorne_Click(object sender, RoutedEventArgs e)
        {
            // Demander confirmation
            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer la borne #{_borne.borne_id} ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = await _viewModel.DeleteBorneAsync();

                    if (success)
                    {
                        // Retourner à la page de gestion
                        AnimatePageTransition(() => {
                            MainGrid.Children.Clear();
                            MainGrid.Children.Add(new GestionnaireBornePage());
                        });
                    }
                    else
                    {
                        MessageBox.Show(
                            "La suppression a échoué. Veuillez réessayer.",
                            "Erreur",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
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
