using System.Windows;
using System.Windows.Controls;
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
            DataContext = _viewModel; // Associer le ViewModel à la vue
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                MessageBox.Show($"Détails de {user.Email}:\nRôle: {user.Role}", "Détails Utilisateur");
            }
        }

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                MessageBox.Show($"Modifier l'utilisateur : {user.Email}", "Modification");
            }
        }

        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                MessageBoxResult result = MessageBox.Show($"Supprimer {user.Email} ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Utilisateur {user.Email} supprimé.", "Suppression");
                }
            }
        }
    }
}
