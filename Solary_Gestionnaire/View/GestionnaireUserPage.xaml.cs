using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.ViewModel;

namespace Solary_Gestionnaire.View
{
    public partial class GestionnaireUserPage : UserControl
    {
        private GestionnaireUserViewModel _viewModel;

        public GestionnaireUserPage()
        {
            InitializeComponent();
            _viewModel = new GestionnaireUserViewModel();
            DataContext = _viewModel;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadUsers();
        }

        private void AjouterUser_Click(object sender, RoutedEventArgs e)
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
                        parentGrid.Children.Add(new AjouterUserPage());
                    });
                }
                else
                {
                    MessageBox.Show(
                        "Impossible de naviguer vers la page d'ajout d'utilisateur.",
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

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
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
                            parentGrid.Children.Add(new DetailUserPage(user));
                        });
                    }
                    else
                    {
                        // Fallback: afficher les détails dans une MessageBox
                        MessageBox.Show(
                            $"Détails de l'utilisateur\n\n" +
                            $"ID: {user.UserId}\n" +
                            $"Email: {user.Email ?? "Non défini"}\n" +
                            $"Rôle: {user.Role ?? "Non défini"}\n" +
                            $"Statut: {user.StatusCompte ?? "Non défini"}\n" +
                            $"Date de création: {(user.DateCreation != default(DateTime) ? user.DateCreation.ToString("dd/MM/yyyy HH:mm") : "Non définie")}",
                            "Détails Utilisateur",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Erreur lors de l'affichage des détails: {ex.Message}",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
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
                            // Effacer le contenu actuel et ajouter la page de modification
                            parentGrid.Children.Clear();
                            parentGrid.Children.Add(new ModifierUserPage(user));
                        });
                    }
                    else
                    {
                        MessageBox.Show(
                            "Impossible de naviguer vers la page de modification.",
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
        }

        private async void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                // Désactiver le bouton pendant l'opération
                button.IsEnabled = false;

                try
                {
                    // Demander confirmation
                    var result = MessageBox.Show(
                        $"Êtes-vous sûr de vouloir supprimer l'utilisateur {user.Email} ?",
                        "Confirmation de suppression",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        bool success = await _viewModel.DeleteUserAsync(user.UserId);

                        // Le message de succès est maintenant géré par le ViewModel
                        if (!success)
                        {
                            MessageBox.Show(
                                "La suppression a échoué. Veuillez réessayer.",
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
                    // Réactiver le bouton
                    button.IsEnabled = true;
                }
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
