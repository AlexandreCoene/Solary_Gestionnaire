using Solary_Gestionnaire.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Solary_Gestionnaire.ViewModel;

namespace Solary_Gestionnaire.View
{
    public partial class ModifierUserPage : UserControl
    {
        private ModifierUserViewModel _viewModel;
        private User _user;

        public ModifierUserPage(User user)
        {
            InitializeComponent();
            _user = user;
            _viewModel = new ModifierUserViewModel(user);
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
                        // Effacer le contenu actuel et ajouter la page de détails
                        parentGrid.Children.Clear();
                        parentGrid.Children.Add(new DetailUserPage(_user));
                    });
                }
                else
                {
                    MessageBox.Show(
                        "Impossible de naviguer vers la page de détails.",
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
                RetourButton_Click(sender, e);
            }
        }

        private async void EnregistrerButton_Click(object sender, RoutedEventArgs e)
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

                string nouveauMotDePasse = null;

                // Si l'utilisateur veut changer le mot de passe
                if (_viewModel.ChangerMotDePasse)
                {
                    if (string.IsNullOrWhiteSpace(NewPasswordBox.Password))
                    {
                        MessageBox.Show(
                            "Le nouveau mot de passe est obligatoire.",
                            "Erreur",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        return;
                    }

                    if (NewPasswordBox.Password != ConfirmNewPasswordBox.Password)
                    {
                        MessageBox.Show(
                            "Les mots de passe ne correspondent pas.",
                            "Erreur",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        return;
                    }

                    nouveauMotDePasse = NewPasswordBox.Password;
                }

                // Désactiver les boutons pendant l'opération
                EnregistrerButton.IsEnabled = false;
                AnnulerButton.IsEnabled = false;

                bool success = await _viewModel.ModifierUserAsync(nouveauMotDePasse);

                if (success)
                {
                    // Attendre 2 secondes pour que l'utilisateur puisse voir le message de succès
                    await System.Threading.Tasks.Task.Delay(2000);

                    // Retourner à la page de détails avec l'utilisateur mis à jour
                    try
                    {
                        DependencyObject parent = this.Parent;
                        while (parent != null && !(parent is Grid))
                        {
                            parent = LogicalTreeHelper.GetParent(parent);
                        }

                        if (parent is Grid parentGrid)
                        {
                            AnimatePageTransition(parentGrid, () => {
                                parentGrid.Children.Clear();
                                parentGrid.Children.Add(new DetailUserPage(_user));
                            });
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
