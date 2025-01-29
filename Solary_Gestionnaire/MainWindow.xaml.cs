using Solary_Gestionnaire.View;
using System.Windows;
using System.Windows.Controls;

namespace Solary_Gestionnaire
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Accueil_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid != null)
            {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new AccueilPage());
            }
        }

        private void GestionnaireBornes_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid != null)
            {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new GestionnaireBornePage());
            }
        }

        private void GestionnaireUser_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid != null)
            {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new GestionnaireUserPage());
            }
        }

        private void Statistique_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid != null)
            {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new StatistiquesPage());
            }
        }

        private void PanneMaintenance_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid != null)
            {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new PannePage());
            }
        }
    }
}