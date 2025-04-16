using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
    }
}