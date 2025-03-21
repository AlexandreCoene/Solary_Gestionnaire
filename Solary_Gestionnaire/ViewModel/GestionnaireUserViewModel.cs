using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Solary_Gestionnaire.Model;
using Solary_Gestionnaire.Service;

namespace Solary_Gestionnaire.ViewModel
{
    public class GestionnaireUserViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private ObservableCollection<User> _users;

        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { _users = value; OnPropertyChanged(); }
        }

        public GestionnaireUserViewModel()
        {
            _userService = new UserService();
            Users = new ObservableCollection<User>();
            LoadUsers();
        }

        private async void LoadUsers()
        {
            var userList = await _userService.GetUsersAsync();
            Users = new ObservableCollection<User>(userList);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
