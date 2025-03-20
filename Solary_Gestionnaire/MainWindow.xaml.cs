using Solary_Gestionnaire.View;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Solary_Gestionnaire
{
    public partial class MainWindow : Window
    {
        private const double menuExpandedWidth = 200;
        private const double menuCollapsedWidth = 50;
        private bool isMenuOpen = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Sidebar_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isMenuOpen)
            {
                ToggleMenu(true);
            }
        }

        private void Sidebar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isMenuOpen)
            {
                ToggleMenu(false);
            }
        }

        private void ToggleMenu(bool open)
        {
            isMenuOpen = open;

            // Animation de la largeur du menu
            DoubleAnimation widthAnimation = new DoubleAnimation(
                open ? menuCollapsedWidth : menuExpandedWidth,
                open ? menuExpandedWidth : menuCollapsedWidth,
                TimeSpan.FromMilliseconds(300));

            Sidebar.BeginAnimation(WidthProperty, widthAnimation);

            // Animation de l'opacité du texte "Menu"
            DoubleAnimation opacityAnimation = new DoubleAnimation(
                open ? 0 : 1,
                open ? 1 : 0,
                TimeSpan.FromMilliseconds(300));

            MenuTitle.BeginAnimation(OpacityProperty, opacityAnimation);

            // Mise à jour dynamique de la marge du contenu principal
            MainGrid.Margin = new Thickness(open ? menuExpandedWidth : menuCollapsedWidth, 0, 0, 0);

            UpdateMenuText(open);
        }



        private void UpdateMenuText(bool showText)
        {
            AccueilButton.Content = showText ? "🏠 Accueil" : "🏠";
            GestionnaireBornesButton.Content = showText ? "🔌 Gestionnaire Bornes" : "🔌";
            GestionnaireUserButton.Content = showText ? "👤 Gestionnaire User" : "👤";
            StatistiqueButton.Content = showText ? "📊 Statistiques" : "📊";
            PanneMaintenanceButton.Content = showText ? "🔧 Panne et Maintenance" : "🔧";
        }

        private void Accueil_Button_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new AccueilPage());
        }

        private void GestionnaireBornes_Button_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new GestionnaireBornePage());
        }

        private void GestionnaireUser_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new GestionnaireUserPage());
        }

        private void Statistique_Button_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new StatistiquesPage());
        }

        private void PanneMaintenance_Button_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new PannePage());
        }
    }
}
