using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Solary_Gestionnaire.ViewModel;

namespace Solary_Gestionnaire.View
{
    public partial class AjouterUserPage : UserControl
    {
        private AjouterUserViewModel _viewModel;

        public AjouterUserPage()
        {
            InitializeComponent();
            _viewModel = new AjouterUserViewModel();
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
                    Console.WriteLine($"Parent type: {this.Parent?.GetType().Name ?? "null"}");
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

        private void AnnulerButton_Click(object sender, RoutedEventArgs e)
        {
            RetourButton_Click(sender, e);
        }

        private async void AjouterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Vérifier que les champs obligatoires sont remplis
                if (string.IsNullOrWhiteSpace(_viewModel.Email))
                {
                    MessageBox.Show(
                        "L'email est obligatoire.",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return;
                }

                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show(
                        "Le mot de passe est obligatoire.",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return;
                }

                // Vérifier que les mots de passe correspondent
                if (PasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show(
                        "Les mots de passe ne correspondent pas.",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return;
                }

                // Désactiver les boutons pendant l'opération
                AjouterButton.IsEnabled = false;
                AnnulerButton.IsEnabled = false;

                bool success = await _viewModel.AjouterUserAsync(PasswordBox.Password);

                if (success)
                {
                    // Attendre 2 secondes pour que l'utilisateur puisse voir le message de succès
                    await System.Threading.Tasks.Task.Delay(2000);

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
                }
                else
                {
                    // Si l'erreur est déjà affichée dans le ViewModel, ne pas afficher de MessageBox supplémentaire
                    if (string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
                    {
                        MessageBox.Show(
                            "Une erreur s'est produite lors de l'ajout de l'utilisateur.",
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
                // Réactiver les boutons
                AjouterButton.IsEnabled = true;
                AnnulerButton.IsEnabled = true;
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
