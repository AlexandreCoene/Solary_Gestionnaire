using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Solary_Gestionnaire.ViewModel
{
    public class GestionnaireLoginViewModel : INotifyPropertyChanged
    {
        #region Propriétés

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                    // Réinitialiser le message d'erreur quand l'utilisateur modifie les champs
                    ErrorMessage = string.Empty;
                    // Mettre à jour l'état de la commande
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        // Pour le PasswordBox, nous utiliserons une méthode spéciale dans le code-behind
        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                    // Réinitialiser le message d'erreur quand l'utilisateur modifie les champs
                    ErrorMessage = string.Empty;
                    // Mettre à jour l'état de la commande
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _rememberMe;
        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                if (_rememberMe != value)
                {
                    _rememberMe = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsErrorVisible));
                }
            }
        }

        public Visibility IsErrorVisible => string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

        #endregion

        #region Commandes

        public ICommand LoginCommand { get; private set; }

        #endregion

        #region Événements

        // Événement pour notifier quand la connexion est réussie
        public event EventHandler<LoginEventArgs> LoginSuccessful;

        // Événement pour demander à la vue de jouer l'animation de secousse
        public event EventHandler ShakeRequested;

        #endregion

        #region Constructeur

        public GestionnaireLoginViewModel()
        {
            // Initialiser les commandes
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);

            // Initialiser les valeurs par défaut
            Username = "admin";
            Password = "admin";
            RememberMe = false;
            ErrorMessage = string.Empty;
        }

        #endregion

        #region Méthodes

        private bool CanExecuteLogin(object parameter)
        {
            // Vérifier si les champs obligatoires sont remplis
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin(object parameter)
        {
            // Vérifier les identifiants
            if (IsValidLogin(Username, Password))
            {
                // Déclencher l'événement de connexion réussie
                LoginSuccessful?.Invoke(this, new LoginEventArgs
                {
                    Username = Username,
                    RememberMe = RememberMe
                });
            }
            else
            {
                // Afficher un message d'erreur
                ErrorMessage = "Identifiants incorrects. Veuillez réessayer.";

                // Demander à la vue de jouer l'animation de secousse
                ShakeRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool IsValidLogin(string username, string password)
        {
            // Logique de validation simple pour démonstration
            // À remplacer par votre propre système d'authentification
            return (username == "admin" && password == "admin") ||
                   (username == "user" && password == "user");
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    // Classe pour les arguments de l'événement de connexion
    public class LoginEventArgs : EventArgs
    {
        public string Username { get; set; }
        public bool RememberMe { get; set; }
    }

    // Classe RelayCommand pour implémenter ICommand
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}