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
    public class ModifierUserViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private User _originalUser;

        private int _userId;
        private string _email;
        private string _role;
        private string _statusCompte;
        private bool _compteVerifie;
        private string _otpCode;
        private string _notes;
        private bool _changerMotDePasse;
        private bool _isLoading;
        private string _errorMessage;
        private string _successMessage;

        public int UserId
        {
            get { return _userId; }
            set { _userId = value; OnPropertyChanged(); }
        }

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

        public bool ChangerMotDePasse
        {
            get { return _changerMotDePasse; }
            set { _changerMotDePasse = value; OnPropertyChanged(); OnPropertyChanged(nameof(PasswordVisibility)); }
        }

        public Visibility PasswordVisibility => ChangerMotDePasse ? Visibility.Visible : Visibility.Collapsed;

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

        public ModifierUserViewModel(User user)
        {
            _userService = new UserService();
            _originalUser = user;

            // Initialiser les propriétés avec les valeurs de l'utilisateur
            UserId = user.UserId;
            Email = user.Email;
            Role = user.Role;
            StatusCompte = user.StatusCompte;
            CompteVerifie = user.CompteVerifie == 1;
            OtpCode = user.OtpCode;
            Notes = string.Empty; // Les notes ne sont pas stockées dans le modèle User actuel
            ChangerMotDePasse = false;
        }

        public async Task<bool> ModifierUserAsync(string nouveauMotDePasse = null)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "L'email est obligatoire.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Role))
            {
                ErrorMessage = "Le rôle est obligatoire.";
                return false;
            }

            if (ChangerMotDePasse && string.IsNullOrWhiteSpace(nouveauMotDePasse))
            {
                ErrorMessage = "Le nouveau mot de passe est obligatoire.";
                return false;
            }

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                var utilisateurModifie = new User
                {
                    UserId = UserId,
                    Email = Email,
                    Role = Role,
                    StatusCompte = StatusCompte ?? "actif",
                    CompteVerifie = CompteVerifie ? 1 : 0,
                    OtpCode = OtpCode,
                    DateCreation = _originalUser.DateCreation,
                    DernierLogin = _originalUser.DernierLogin,
                    PasswordHash = _originalUser.PasswordHash
                };

                var (success, errorMessage) = await _userService.UpdateUserAsync(utilisateurModifie, nouveauMotDePasse);

                if (success)
                {
                    SuccessMessage = "L'utilisateur a été modifié avec succès.";

                    // Mettre à jour l'utilisateur original avec les nouvelles valeurs
                    _originalUser.Email = Email;
                    _originalUser.Role = Role;
                    _originalUser.StatusCompte = StatusCompte;
                    _originalUser.CompteVerifie = CompteVerifie ? 1 : 0;
                    _originalUser.OtpCode = OtpCode;

                    return true;
                }
                else
                {
                    ErrorMessage = $"Erreur lors de la modification de l'utilisateur: {errorMessage}";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la modification de l'utilisateur: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
