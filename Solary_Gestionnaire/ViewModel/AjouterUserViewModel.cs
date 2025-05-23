using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Solary_Gestionnaire.ViewModel
{
    public class AjouterUserViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private string _email;
        private string _role;
        private string _statusCompte;
        private bool _compteVerifie;
        private string _otpCode;
        private string _notes;
        private bool _isLoading;
        private string _errorMessage;
        private string _successMessage;

        public string Email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(); }
        }

        public string Role
        {
            get { return _role; }
            set { _role = value; OnPropertyChanged(); }
        }

        public string StatusCompte
        {
            get { return _statusCompte; }
            set { _statusCompte = value; OnPropertyChanged(); }
        }

        public bool CompteVerifie
        {
            get { return _compteVerifie; }
            set { _compteVerifie = value; OnPropertyChanged(); }
        }

        public string OtpCode
        {
            get { return _otpCode; }
            set { _otpCode = value; OnPropertyChanged(); }
        }

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged(); OnPropertyChanged(nameof(LoadingVisibility)); }
        }

        public Visibility LoadingVisibility => IsLoading ? Visibility.Visible : Visibility.Collapsed;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(ErrorVisibility)); }
        }

        public Visibility ErrorVisibility => string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

        public string SuccessMessage
        {
            get { return _successMessage; }
            set { _successMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(SuccessVisibility)); }
        }

        public Visibility SuccessVisibility => string.IsNullOrEmpty(SuccessMessage) ? Visibility.Collapsed : Visibility.Visible;

        // Liste des rôles disponibles
        public string[] RoleOptions => new[] { "admin", "user" };

        // Liste des statuts disponibles
        public string[] StatusOptions => new[] { "actif", "inactif", "suspendu" };

        public AjouterUserViewModel()
        {
            _userService = new UserService();
            Role = "user"; // Valeur par défaut
            StatusCompte = "actif"; // Valeur par défaut
            CompteVerifie = false; // Valeur par défaut
        }

        public async Task<bool> AjouterUserAsync(string password)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "L'email est obligatoire.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Le mot de passe est obligatoire.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Role))
            {
                ErrorMessage = "Le rôle est obligatoire.";
                return false;
            }

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                var nouvelUtilisateur = new User
                {
                    Email = Email,
                    Role = Role,
                    StatusCompte = StatusCompte ?? "actif",
                    DateCreation = DateTime.Now,
                    CompteVerifie = CompteVerifie ? 1 : 0,
                    OtpCode = OtpCode,
                    OtpCreatedAt = !string.IsNullOrEmpty(OtpCode) ? DateTime.Now : (DateTime?)null
                };

                var (success, errorMessage) = await _userService.AddUserAsync(nouvelUtilisateur, password);

                if (success)
                {
                    SuccessMessage = "L'utilisateur a été ajouté avec succès.";
                    // Réinitialiser les champs
                    ResetFields();
                    return true;
                }
                else
                {
                    ErrorMessage = $"Erreur lors de l'ajout de l'utilisateur: {errorMessage}";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de l'ajout de l'utilisateur: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ResetFields()
        {
            Email = string.Empty;
            Role = "user";
            StatusCompte = "actif";
            CompteVerifie = false;
            OtpCode = string.Empty;
            Notes = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
