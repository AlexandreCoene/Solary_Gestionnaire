using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Solary_Gestionnaire.View
{
    public partial class GestionnaireBornePage : UserControl
    {
        public GestionnaireBornePage()
        {
            InitializeComponent();
            LoadFakeBornes(); // Charger des bornes fictives pour le test
        }

        private void LoadFakeBornes()
        {
            var bornes = new List<Borne>
            {
                new Borne { Id = 1, Nom = "Borne A1", Statut = "Active", Emplacement = "Parking Nord", DateInstallation = DateTime.Now.AddYears(-1) },
                new Borne { Id = 2, Nom = "Borne B2", Statut = "En maintenance", Emplacement = "Parking Sud", DateInstallation = DateTime.Now.AddYears(-2) },
                new Borne { Id = 3, Nom = "Borne C3", Statut = "Inactive", Emplacement = "Parking Est", DateInstallation = DateTime.Now.AddMonths(-6) }
            };

            DisplayBornes(bornes);
        }

        private void DisplayBornes(List<Borne> bornes)
        {
            BorneListPanel.Children.Clear();

            foreach (var borne in bornes)
            {
                Border borneCard = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(42, 42, 61)), // Couleur sombre
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 15),
                    Child = CreateBorneInfo(borne)
                };

                BorneListPanel.Children.Add(borneCard);
            }
        }

        private StackPanel CreateBorneInfo(Borne borne)
        {
            StackPanel bornePanel = new StackPanel { Orientation = Orientation.Vertical };

            TextBlock nomBlock = new TextBlock
            {
                Text = $"⚡ {borne.Nom}",
                FontSize = 18,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            };

            TextBlock statutBlock = new TextBlock
            {
                Text = $"📌 Statut: {borne.Statut}",
                Foreground = Brushes.LightGray,
                FontSize = 14
            };

            TextBlock emplacementBlock = new TextBlock
            {
                Text = $"📍 Emplacement: {borne.Emplacement}",
                Foreground = Brushes.LightGray,
                FontSize = 14
            };

            TextBlock installationBlock = new TextBlock
            {
                Text = $"🛠️ Installée le: {borne.DateInstallation.ToShortDateString()}",
                Foreground = Brushes.LightGray,
                FontSize = 14
            };

            StackPanel buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };

            buttonPanel.Children.Add(CreateButton("Détails", "#4A90E2", borne, Details_Click));
            buttonPanel.Children.Add(CreateButton("Modifier", "#F5A623", borne, Modifier_Click));
            buttonPanel.Children.Add(CreateButton("Supprimer", "#D0021B", borne, Supprimer_Click));

            bornePanel.Children.Add(nomBlock);
            bornePanel.Children.Add(statutBlock);
            bornePanel.Children.Add(emplacementBlock);
            bornePanel.Children.Add(installationBlock);
            bornePanel.Children.Add(buttonPanel);

            return bornePanel;
        }

        private Button CreateButton(string text, string color, Borne borne, RoutedEventHandler clickHandler)
        {
            return new Button
            {
                Content = text,
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString(color),
                Foreground = Brushes.White,
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(5),
                Tag = borne,
                Cursor = System.Windows.Input.Cursors.Hand,
            };
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Borne borne)
            {
                MessageBox.Show($"Détails de {borne.Nom}:\nStatut: {borne.Statut}\nEmplacement: {borne.Emplacement}", "Détails Borne");
            }
        }

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Borne borne)
            {
                MessageBox.Show($"Modifier la borne : {borne.Nom}", "Modification");
            }
        }

        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Borne borne)
            {
                MessageBoxResult result = MessageBox.Show($"Supprimer {borne.Nom} ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Borne {borne.Nom} supprimée.", "Suppression");
                }
            }
        }

    }

    public class Borne
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Statut { get; set; }
        public string Emplacement { get; set; }
        public DateTime DateInstallation { get; set; }
    }
}
