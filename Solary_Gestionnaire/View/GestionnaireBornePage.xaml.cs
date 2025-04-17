using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Solary_Gestionnaire.View
{
    public partial class GestionnaireBornePage : UserControl
    {
        private GestionnaireBorneViewModel _viewModel;

        public GestionnaireBornePage()
        {
            InitializeComponent();
            _viewModel = new GestionnaireBorneViewModel();
            DataContext = _viewModel;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadBornesAsync();
        }

        private async void DeleteBorne_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int borneId)
            {
                // Demander confirmation
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer la borne #{borneId} ?",
                    "Confirmation de suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    // Désactiver le bouton pendant l'opération
                    button.IsEnabled = false;

                    try
                    {
                        bool success = await _viewModel.DeleteBorneAsync(borneId);

                        if (success)
                        {
                            // Pas besoin de recharger toute la liste car nous avons déjà supprimé l'élément de la collection
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
                    finally
                    {
                        // Réactiver le bouton
                        button.IsEnabled = true;
                    }
                }
            }
        }

        private void DetailsBorne_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int borneId)
            {
                // Trouver la borne correspondante
                var borne = _viewModel.GetBorneById(borneId);

                if (borne != null)
                {
                    // Naviguer vers la page de détails
                    AnimatePageTransition(() => {
                        MainGrid.Children.Clear();
                        MainGrid.Children.Add(new DetailBornePage(borne));
                    });
                }
                else
                {
                    MessageBox.Show(
                        $"Impossible de trouver la borne #{borneId}.",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private void AjouterBorne_Click(object sender, RoutedEventArgs e)
        {
            AnimatePageTransition(() => {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new AjouterBornePage());
            });
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
