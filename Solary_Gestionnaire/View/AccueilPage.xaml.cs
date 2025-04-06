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
    }
}
