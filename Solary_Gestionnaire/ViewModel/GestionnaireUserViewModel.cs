using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.Service;

namespace Solary_Gestionnaire.ViewModel
{
    public class GestionnaireUserViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private ObservableCollection<User> _users;
        private ObservableCollection<User> _filteredUsers;
        private string _searchText;
        private bool _isLoading;
        private string _errorMessage;
        private string _successMessage;

        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { _users = value; OnPropertyChanged(); FilterUsers(); }
        }

        public ObservableCollection<User> FilteredUsers
        {
            get { return _filteredUsers; }
            set { _filteredUsers = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged(); FilterUsers(); }
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

        public GestionnaireUserViewModel()
        {
            _userService = new UserService();
            // Initialiser les collections pour éviter les NullReferenceException
            Users = new ObservableCollection<User>();
            FilteredUsers = new ObservableCollection<User>();
            LoadUsers();
        }

        public async void LoadUsers()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var userList = await _userService.GetUsersAsync();

                // S'assurer que nous sommes sur le thread UI
                if (Application.Current != null && Application.Current.Dispatcher != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Mettre à jour Users avec la nouvelle liste
                        Users.Clear();
                        if (userList != null)
                        {
                            foreach (var user in userList)
                            {
                                Users.Add(user);
                            }
                        }

                        // Appliquer le filtre
                        FilterUsers();
                    });
                }
                else
                {
                    // Fallback si le Dispatcher n'est pas disponible
                    Users = new ObservableCollection<User>(userList ?? new List<User>());
                    FilterUsers();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement des utilisateurs: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            if (userId <= 0)
                return false;

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var success = await _userService.DeleteUserAsync(userId);

                if (success)
                {
                    // Supprimer l'utilisateur de la collection
                    if (Application.Current != null && Application.Current.Dispatcher != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var userToRemove = Users.FirstOrDefault(u => u.UserId == userId);
                            if (userToRemove != null)
                            {
                                Users.Remove(userToRemove);
                                FilterUsers();
                                SuccessMessage = $"L'utilisateur {userToRemove.Email} a été supprimé avec succès.";

                                // Effacer le message après 3 secondes
                                Task.Delay(3000).ContinueWith(_ =>
                                {
                                    Application.Current.Dispatcher.Invoke(() => SuccessMessage = string.Empty);
                                });
                            }
                        });
                    }
                    return true;
                }
                else
                {
                    ErrorMessage = "Erreur lors de la suppression de l'utilisateur.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la suppression de l'utilisateur: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterUsers()
        {
            // Vérifier que les collections ne sont pas null
            if (Users == null || FilteredUsers == null)
                return;

            try
            {
                // Vérifier que le Dispatcher est disponible
                if (Application.Current != null && Application.Current.Dispatcher != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Vider la collection filtrée
                        FilteredUsers.Clear();

                        // Si aucun filtre, ajouter tous les utilisateurs
                        if (string.IsNullOrWhiteSpace(SearchText))
                        {
                            foreach (var user in Users)
                            {
                                FilteredUsers.Add(user);
                            }
                        }
                        else
                        {
                            // Appliquer le filtre
                            string searchLower = SearchText.ToLower();
                            foreach (var user in Users)
                            {
                                // Vérifier que les propriétés ne sont pas null
                                bool emailMatch = user.Email != null && user.Email.ToLower().Contains(searchLower);
                                bool roleMatch = user.Role != null && user.Role.ToLower().Contains(searchLower);

                                if (emailMatch || roleMatch)
                                {
                                    FilteredUsers.Add(user);
                                }
                            }
                        }
                    });
                }
                else
                {
                    // Fallback si le Dispatcher n'est pas disponible
                    FilteredUsers = new ObservableCollection<User>(
                        string.IsNullOrWhiteSpace(SearchText)
                            ? Users
                            : Users.Where(u =>
                                (u.Email != null && u.Email.ToLower().Contains(SearchText.ToLower())) ||
                                (u.Role != null && u.Role.ToLower().Contains(SearchText.ToLower())))
                    );
                }
            }
            catch (Exception ex)
            {
                // Gérer l'exception
                Console.WriteLine($"Erreur dans FilterUsers: {ex.Message}");

                // Essayer de récupérer en affichant tous les utilisateurs
                if (FilteredUsers != null && Users != null)
                {
                    FilteredUsers.Clear();
                    foreach (var user in Users)
                    {
                        FilteredUsers.Add(user);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
