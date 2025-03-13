using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Solary_Gestionnaire.View
{
    public partial class GestionnaireUserPage : UserControl
    {
        public GestionnaireUserPage()
        {
            InitializeComponent();
            LoadFakeUsers(); // Charge les données fictives
        }

        private void LoadFakeUsers()
        {
            var users = new List<User>
            {
                new User { Id = 1, Name = "Alice Dupont", Email = "alice.dupont@email.com", Role = "Admin", CreatedAt = DateTime.Now.AddMonths(-2) },
                new User { Id = 2, Name = "Bob Martin", Email = "bob.martin@email.com", Role = "Utilisateur", CreatedAt = DateTime.Now.AddMonths(-5) },
                new User { Id = 3, Name = "Charlie Durand", Email = "charlie.durand@email.com", Role = "Modérateur", CreatedAt = DateTime.Now.AddMonths(-1) }
            };

            DisplayUsers(users);
        }

        private void DisplayUsers(List<User> users)
        {
            UserListPanel.Children.Clear(); // Nettoie l'affichage avant d'ajouter de nouveaux éléments

            foreach (var user in users)
            {
                Border userCard = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(42, 42, 61)), // Couleur foncée
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 15),
                    Child = CreateUserInfo(user)
                };

                UserListPanel.Children.Add(userCard);
            }
        }

        private StackPanel CreateUserInfo(User user)
        {
            StackPanel userPanel = new StackPanel { Orientation = Orientation.Vertical };

            TextBlock nameBlock = new TextBlock
            {
                Text = $"👤 {user.Name}",
                FontSize = 18,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            };

            TextBlock emailBlock = new TextBlock
            {
                Text = $"📧 {user.Email}",
                Foreground = Brushes.LightGray,
                FontSize = 14
            };

            TextBlock roleBlock = new TextBlock
            {
                Text = $"🔹 Rôle: {user.Role}",
                Foreground = Brushes.LightGray,
                FontSize = 14
            };

            TextBlock createdAtBlock = new TextBlock
            {
                Text = $"📅 Créé le: {user.CreatedAt.ToShortDateString()}",
                Foreground = Brushes.LightGray,
                FontSize = 14
            };

            StackPanel buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };

            buttonPanel.Children.Add(CreateButton("Détails", "#4A90E2", user, Details_Click));
            buttonPanel.Children.Add(CreateButton("Modifier", "#F5A623", user, Modifier_Click));
            buttonPanel.Children.Add(CreateButton("Supprimer", "#D0021B", user, Supprimer_Click));

            userPanel.Children.Add(nameBlock);
            userPanel.Children.Add(emailBlock);
            userPanel.Children.Add(roleBlock);
            userPanel.Children.Add(createdAtBlock);
            userPanel.Children.Add(buttonPanel);

            return userPanel;
        }

        private Button CreateButton(string text, string color, User user, RoutedEventHandler clickHandler)
        {
            return new Button
            {
                Content = text,
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString(color),
                Foreground = Brushes.White,
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(5),
                Tag = user,
                Cursor = System.Windows.Input.Cursors.Hand
            };
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                MessageBox.Show($"Détails de {user.Name}:\nEmail: {user.Email}\nRôle: {user.Role}", "Détails Utilisateur");
            }
        }

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                MessageBox.Show($"Modifier l'utilisateur : {user.Name}", "Modification");
            }
        }

        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                MessageBoxResult result = MessageBox.Show($"Supprimer {user.Name} ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Utilisateur {user.Name} supprimé.", "Suppression");
                }
            }
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
