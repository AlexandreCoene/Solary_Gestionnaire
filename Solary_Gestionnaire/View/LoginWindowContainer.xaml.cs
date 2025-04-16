using Solary_Gestionnaire.View;
using Solary_Gestionnaire.ViewModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Solary_Gestionnaire.View
{
    public partial class LoginWindowContainer : Window
    {
        public LoginWindowContainer()
        {
            InitializeComponent();

            // S'abonner à l'événement de connexion réussie
            LoginControl.LoginSuccessful += LoginControl_LoginSuccessful;
        }

        private void LoginControl_LoginSuccessful(object sender, LoginEventArgs e)
        {
            // Créer et afficher la fenêtre principale
            MainWindow mainWindow = new MainWindow();

            // Si vous avez besoin de passer des données à MainWindow, vous pouvez le faire ici
            // Par exemple, vous pourriez vouloir stocker l'utilisateur connecté
            // mainWindow.CurrentUser = e.Username;

            // Afficher la fenêtre principale
            mainWindow.Show();

            // Fermer la fenêtre de login
            this.Close();
        }
    }
}