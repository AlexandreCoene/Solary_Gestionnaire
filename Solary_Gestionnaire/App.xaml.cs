using Solary_Gestionnaire.View;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Solary_Gestionnaire
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Créer et afficher la fenêtre conteneur
            LoginWindowContainer loginWindowContainer = new LoginWindowContainer();
            loginWindowContainer.Show();

            // Définir comme fenêtre principale
            this.MainWindow = loginWindowContainer;
        }
    }

}
