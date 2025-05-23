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
    public class DetailUserViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private User _user;
        private bool _isLoading;
        private string _errorMessage;

        public User User
        {
            get { return _user; }
            set { _user = value; OnPropertyChanged(); UpdateDisplayProperties(); }
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

        // Propriétés d'affichage
        public string UserIdDisplay { get; private set; }
        public string EmailDisplay { get; private set; }
        public string RoleDisplay { get; private set; }
        public string StatusDisplay { get; private set; }
        public string DateCreationDisplay { get; private set; }
        public string DernierLoginDisplay { get; private set; }
        public string CompteVerifieDisplay { get; private set; }
        public string OtpCodeDisplay { get; private set; }
        public string OtpCreatedAtDisplay { get; private set; }
        public string PasswordHashDisplay { get; private set; }
        public Visibility DernierLoginVisibility { get; private set; }
        public Visibility OtpCodeVisibility { get; private set; }
        public Visibility OtpCreatedAtVisibility { get; private set; }

        public DetailUserViewModel(User user)
        {
            _userService = new UserService();
            User = user;
        }

        private void UpdateDisplayProperties()
        {
            if (User != null)
            {
                UserIdDisplay = User.UserId.ToString();
                EmailDisplay = User.Email ?? "Non défini";
                RoleDisplay = User.Role ?? "Non défini";
                StatusDisplay = string.IsNullOrEmpty(User.StatusCompte) ? "Non défini" : User.StatusCompte;
                DateCreationDisplay = User.DateCreation != default(DateTime)
                    ? User.DateCreation.ToString("dd/MM/yyyy HH:mm")
                    : "Non définie";
                DernierLoginDisplay = User.DernierLogin.HasValue
                    ? User.DernierLogin.Value.ToString("dd/MM/yyyy HH:mm")
                    : "Jamais connecté";
                CompteVerifieDisplay = User.CompteVerifie == 1 ? "Vérifié" : "Non vérifié";

                OtpCodeDisplay = string.IsNullOrEmpty(User.OtpCode) ? "Aucun code OTP" : User.OtpCode;
                OtpCodeVisibility = Visibility.Visible;

                OtpCreatedAtDisplay = User.OtpCreatedAt.HasValue
                    ? User.OtpCreatedAt.Value.ToString("dd/MM/yyyy HH:mm")
                    : "Non définie";
                OtpCreatedAtVisibility = Visibility.Visible;

                PasswordHashDisplay = string.IsNullOrEmpty(User.PasswordHash) ? "Non défini" : User.PasswordHash;
                DernierLoginVisibility = User.DernierLogin.HasValue ? Visibility.Visible : Visibility.Collapsed;
            }

            OnPropertyChanged(nameof(UserIdDisplay));
            OnPropertyChanged(nameof(EmailDisplay));
            OnPropertyChanged(nameof(RoleDisplay));
            OnPropertyChanged(nameof(StatusDisplay));
            OnPropertyChanged(nameof(DateCreationDisplay));
            OnPropertyChanged(nameof(DernierLoginDisplay));
            OnPropertyChanged(nameof(CompteVerifieDisplay));
            OnPropertyChanged(nameof(OtpCodeDisplay));
            OnPropertyChanged(nameof(OtpCreatedAtDisplay));
            OnPropertyChanged(nameof(PasswordHashDisplay));
            OnPropertyChanged(nameof(DernierLoginVisibility));
            OnPropertyChanged(nameof(OtpCodeVisibility));
            OnPropertyChanged(nameof(OtpCreatedAtVisibility));
        }

        public async Task<bool> DeleteUserAsync()
        {
            if (User == null || User.UserId <= 0)
            {
                ErrorMessage = "ID utilisateur invalide";
                return false;
            }

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                bool success = await _userService.DeleteUserAsync(User.UserId);

                if (!success)
                {
                    ErrorMessage = "Échec de la suppression";
                }

                return success;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur: {ex.Message}";
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
