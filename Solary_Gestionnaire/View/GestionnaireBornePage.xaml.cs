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
