using System.Windows.Controls;
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
    }
}
