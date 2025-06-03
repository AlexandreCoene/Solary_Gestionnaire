using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Solary_Gestionnaire.View
{
    /// <summary>
    /// Logique d'interaction pour AccueilPage.xaml
    /// </summary>
    public partial class AccueilPage : UserControl
    {
        private DispatcherTimer sunRotationTimer;
        private double rotationAngle = 0;

        // URLs des sites web
        private const string Url3D = "https://v0-borne-solary-3-d.vercel.app/";
        private const string UrlSite = "https://v0-site-internet-solary.vercel.app/";

        public AccueilPage()
        {
            InitializeComponent();

            // Initialiser et démarrer l'animation de rotation du soleil
            sunRotationTimer = new DispatcherTimer();
            sunRotationTimer.Interval = TimeSpan.FromMilliseconds(50);
            sunRotationTimer.Tick += SunRotationTimer_Tick;
            sunRotationTimer.Start();
        }

        private void SunRotationTimer_Tick(object sender, EventArgs e)
        {
            rotationAngle = (rotationAngle + 0.2) % 360;
            SunRotation.Angle = rotationAngle;
        }

        // Gestionnaire d'événement pour le bouton 3D
        private void Voir3DButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Url3D,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'ouvrir le lien : {ex.Message}", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Gestionnaire d'événement pour le bouton Site
        private void VoirSiteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = UrlSite,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'ouvrir le lien : {ex.Message}", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
