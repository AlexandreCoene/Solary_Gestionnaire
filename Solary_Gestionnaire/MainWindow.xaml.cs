using Solary_Gestionnaire.View;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Solary_Gestionnaire
{
    public partial class MainWindow : Window
    {
        // Augmentation de la largeur du menu déplié pour éviter les textes coupés
        private const double menuExpandedWidth = 250;
        private const double menuCollapsedWidth = 65;
        private bool isMenuOpen = false;
        private DispatcherTimer dateTimeTimer;

        public MainWindow()
        {
            InitializeComponent();

            // Initialiser et démarrer le timer pour la date/heure
            dateTimeTimer = new DispatcherTimer();
            dateTimeTimer.Interval = TimeSpan.FromSeconds(1);
            dateTimeTimer.Tick += DateTimeTimer_Tick;
            dateTimeTimer.Start();

            // Mettre à jour l'affichage de la date/heure initialement
            UpdateDateTimeDisplay();

            // Charger automatiquement la page d'accueil au démarrage
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new AccueilPage());

            // Mettre à jour les informations système
            SystemInfoDisplay.Text = $"Système connecté | {Environment.UserName}";
        }

        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            UpdateDateTimeDisplay();
        }

        private void UpdateDateTimeDisplay()
        {
            // Récupérer la date et l'heure actuelles
            DateTime now = DateTime.Now;

            // Formater avec première lettre en majuscule seulement
            string jour = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(now.ToString("dddd"));
            string mois = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(now.ToString("MMMM"));

            // Assembler le tout
            DateTimeDisplay.Text = $"{jour}, {now.Day} {mois} {now.Year} {now.ToString("HH:mm:ss")}";
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

            // Animation de la largeur du menu avec easing
            DoubleAnimation widthAnimation = new DoubleAnimation(
                open ? menuCollapsedWidth : menuExpandedWidth,
                open ? menuExpandedWidth : menuCollapsedWidth,
                TimeSpan.FromMilliseconds(300));

            // Ajouter une fonction d'easing pour une animation plus fluide
            widthAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            Sidebar.BeginAnimation(WidthProperty, widthAnimation);

            // Animation de l'opacité du texte "Menu" et des séparateurs
            AnimateMenuElement(MenuTitle, open);
            AnimateMenuElement(MenuSeparator1, open);
            AnimateMenuElement(MenuSeparator2, open);
            AnimateMenuElement(MenuSeparator3, open);
            AnimateMenuElement(VersionText, open);

            // Animation de l'opacité des textes des boutons
            AnimateMenuElement(AccueilText, open);
            AnimateMenuElement(BornesText, open);
            AnimateMenuElement(UserText, open);
            AnimateMenuElement(StatText, open);
            AnimateMenuElement(MaintenanceText, open);
            AnimateMenuElement(ParametresText, open);

            // Mise à jour dynamique de la marge du contenu principal avec animation
            ThicknessAnimation marginAnimation = new ThicknessAnimation(
                new Thickness(open ? menuCollapsedWidth : menuExpandedWidth, 0, 0, 0),
                new Thickness(open ? menuExpandedWidth : menuCollapsedWidth, 0, 0, 0),
                TimeSpan.FromMilliseconds(300));

            marginAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            MainGrid.BeginAnimation(MarginProperty, marginAnimation);
        }

        private void AnimateMenuElement(UIElement element, bool show)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation(
                show ? 0 : 1,
                show ? 1 : 0,
                TimeSpan.FromMilliseconds(300));

            opacityAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            element.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
        }

        private void Accueil_Button_Click(object sender, RoutedEventArgs e)
        {
            AnimatePageTransition(() => {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new AccueilPage());
            });
        }

        private void GestionnaireBornes_Button_Click(object sender, RoutedEventArgs e)
        {
            AnimatePageTransition(() => {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new GestionnaireBornePage());
            });
        }

        private void GestionnaireUser_Click(object sender, RoutedEventArgs e)
        {
            AnimatePageTransition(() => {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new GestionnaireUserPage());
            });
        }

        private void Statistique_Button_Click(object sender, RoutedEventArgs e)
        {
            AnimatePageTransition(() => {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new StatistiquesPage());
            });
        }

        private void PanneMaintenance_Button_Click(object sender, RoutedEventArgs e)
        {
            AnimatePageTransition(() => {
                MainGrid.Children.Clear();
                MainGrid.Children.Add(new PannePage());
            });
        }

        private void AnimatePageTransition(Action changePageAction)
        {
            // Créer une animation de fondu sortant
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            fadeOut.Completed += (s, e) => {
                // Changer le contenu de la page
                changePageAction();

                // Créer une animation de fondu entrant
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                MainGrid.BeginAnimation(OpacityProperty, fadeIn);
            };

            // Démarrer l'animation de fondu sortant
            MainGrid.BeginAnimation(OpacityProperty, fadeOut);
        }

    }
}