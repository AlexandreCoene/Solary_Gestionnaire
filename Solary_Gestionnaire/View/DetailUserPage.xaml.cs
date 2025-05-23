using Solary_Gestionnaire.Model;
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
using System.Windows.Threading;
using Solary_Gestionnaire.ViewModel;

namespace Solary_Gestionnaire.View
{
    public partial class DetailUserPage : UserControl
    {
        private DetailUserViewModel _viewModel;
        private User _user;

        public DetailUserPage(User user)
        {
            InitializeComponent();
            _user = user;
            _viewModel = new DetailUserViewModel(user);
            DataContext = _viewModel;
        }

        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Trouver le parent direct qui contient ce UserControl
                DependencyObject parent = this.Parent;
                while (parent != null && !(parent is Grid))
                {
                    parent = LogicalTreeHelper.GetParent(parent);
                }

                if (parent is Grid parentGrid)
                {
                    // Utiliser l'animation de transition
                    AnimatePageTransition(parentGrid, () => {
                        // Effacer le contenu actuel et ajouter la nouvelle page
                        parentGrid.Children.Clear();
                        parentGrid.Children.Add(new GestionnaireUserPage());
                    });
                }
                else
                {
                    MessageBox.Show(
                        "Impossible de naviguer vers la page de gestion des utilisateurs.",
                        "Erreur de navigation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de la navigation: {ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void ModifierUser_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "La fonctionnalité de modification sera disponible prochainement.",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "La fonctionnalité de réinitialisation de mot de passe sera disponible prochainement.",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private async void SupprimerUser_Click(object sender, RoutedEventArgs e)
        {
            // Désactiver le bouton pendant l'opération
            var button = sender as Button;
            if (button != null)
                button.IsEnabled = false;

            try
            {
                // Demander confirmation
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer l'utilisateur {_user.Email} ?\n\nCette action est irréversible.",
                    "Confirmation de suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    // Afficher un indicateur de chargement
                    _viewModel.IsLoading = true;

                    // Effectuer la suppression
                    bool success = await _viewModel.DeleteUserAsync();

                    if (success)
                    {
                        MessageBox.Show(
                            $"L'utilisateur {_user.Email} a été supprimé avec succès.",
                            "Suppression réussie",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );

                        // Retourner à la page de gestion
                        RetourButton_Click(sender, e);
                    }
                    else
                    {
                        string errorMessage = string.IsNullOrEmpty(_viewModel.ErrorMessage)
                            ? "La suppression a échoué. Veuillez réessayer."
                            : _viewModel.ErrorMessage;

                        MessageBox.Show(
                            errorMessage,
                            "Erreur",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
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
                // Réactiver le bouton et masquer l'indicateur de chargement
                if (button != null)
                    button.IsEnabled = true;
                _viewModel.IsLoading = false;
            }
        }

        private void AnimatePageTransition(Grid parentGrid, Action afterAnimation)
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

                parentGrid.BeginAnimation(OpacityProperty, fadeIn);
            };

            // Démarrer l'animation de sortie
            parentGrid.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
