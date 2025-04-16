using Solary_Gestionnaire.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Solary_Gestionnaire.View
{
    /// <summary>
    /// Logique d'interaction pour LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : UserControl
    {
        private DispatcherTimer particleTimer;
        private Random random = new Random();
        private const int MaxParticles = 30;

        // Référence au ViewModel
        private GestionnaireLoginViewModel _viewModel;

        // Événement pour notifier quand la connexion est réussie
        public event EventHandler<LoginEventArgs> LoginSuccessful;

        public LoginWindow()
        {
            InitializeComponent();

            // Créer et configurer le ViewModel
            _viewModel = new GestionnaireLoginViewModel();
            _viewModel.LoginSuccessful += ViewModel_LoginSuccessful;
            _viewModel.ShakeRequested += ViewModel_ShakeRequested;

            // Définir le DataContext
            this.DataContext = _viewModel;

            // Initialiser le mot de passe
            PasswordBox.Password = _viewModel.Password;

            // Initialiser l'animation des particules (reste dans la vue car c'est purement visuel)
            InitializeParticles();

            // S'assurer que le contrôle est chargé avant de commencer les animations
            this.Loaded += (s, e) => {
                if (ParticlesCanvas.ActualWidth > 0)
                    CreateInitialParticles();
            };
        }

        // Méthode pour synchroniser le PasswordBox avec le ViewModel
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.Password = PasswordBox.Password;
        }

        // Gestionnaire d'événement pour relayer l'événement LoginSuccessful du ViewModel
        private void ViewModel_LoginSuccessful(object sender, LoginEventArgs e)
        {
            LoginSuccessful?.Invoke(this, e);
        }

        // Gestionnaire d'événement pour jouer l'animation de secousse
        private void ViewModel_ShakeRequested(object sender, EventArgs e)
        {
            Storyboard shakeAnimation = (Storyboard)FindResource("ShakeAnimation");
            shakeAnimation.Begin(LoginPanel);
        }

        #region Animation des particules (reste dans la vue car c'est purement visuel)

        private void InitializeParticles()
        {
            particleTimer = new DispatcherTimer();
            particleTimer.Interval = TimeSpan.FromMilliseconds(50);
            particleTimer.Tick += ParticleTimer_Tick;
            particleTimer.Start();
        }

        private void CreateInitialParticles()
        {
            // Créer les particules initiales
            for (int i = 0; i < MaxParticles; i++)
            {
                CreateParticle();
            }
        }

        private void ParticleTimer_Tick(object sender, EventArgs e)
        {
            // S'assurer que le canvas est initialisé
            if (ParticlesCanvas.ActualWidth <= 0 || ParticlesCanvas.ActualHeight <= 0)
                return;

            // Mettre à jour les particules existantes
            for (int i = ParticlesCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (ParticlesCanvas.Children[i] is Ellipse particle)
                {
                    // Obtenir la position actuelle
                    double top = Canvas.GetTop(particle);
                    double left = Canvas.GetLeft(particle);

                    // Déplacer la particule
                    Canvas.SetTop(particle, top - (double)particle.Tag);

                    // Faire disparaître progressivement
                    particle.Opacity -= 0.005;

                    // Supprimer si hors écran ou invisible
                    if (top < -10 || particle.Opacity <= 0)
                    {
                        ParticlesCanvas.Children.Remove(particle);
                        CreateParticle(); // Remplacer par une nouvelle
                    }
                }
            }
        }

        private void CreateParticle()
        {
            // S'assurer que le canvas est initialisé
            if (ParticlesCanvas.ActualWidth <= 0 || ParticlesCanvas.ActualHeight <= 0)
                return;

            // Créer une particule (petit cercle lumineux)
            Ellipse particle = new Ellipse
            {
                Width = random.Next(2, 6),
                Height = random.Next(2, 6),
                Fill = new SolidColorBrush(Color.FromArgb(
                    (byte)random.Next(100, 200),
                    (byte)255,
                    (byte)random.Next(200, 255),
                    (byte)random.Next(100, 200))),
                Tag = random.NextDouble() * 1.5 + 0.5, // Vitesse
                Opacity = random.NextDouble() * 0.5 + 0.5
            };

            // Positionner aléatoirement
            Canvas.SetLeft(particle, random.Next(0, (int)ParticlesCanvas.ActualWidth));
            Canvas.SetTop(particle, random.Next((int)ParticlesCanvas.ActualHeight - 50, (int)ParticlesCanvas.ActualHeight + 50));

            // Ajouter au canvas
            ParticlesCanvas.Children.Add(particle);
        }

        #endregion
    }
}