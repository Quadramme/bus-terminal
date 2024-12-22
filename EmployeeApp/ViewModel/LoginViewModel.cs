using BLL.Services;
using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EmployeeApp.ViewModel;
using DTO = Interfaces.DTO;

namespace EmployeeApp.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {

        private readonly IUserService userService;

        public LoginViewModel(Window window, IUserService userService)
        : base(window)
        {

            this.userService = userService;

            // Commands

            CmdLogin = new RelayCommand(obj =>
            {

                while (!userService.CanConnect())
                {
                    (window as MainWindow).DialogOk("Нет подключения к серверу");
                }

                LoginStatus success;
                
                try
                {
                    success = userService.Login(login, password);
                }
                catch
                {
                    (window as MainWindow).DialogOk("Что-то пошло не так");
                    return;
                }

                switch (success)
                {
                    case LoginStatus.Success:
                        Login = null;
                        Password = null;
                        OnLoginSuccessful?.Invoke();
                        break;

                    case LoginStatus.UserDoesNotExist:
                        (window as MainWindow).DialogOk("Такого пользователя не существует");
                        break;

                    case LoginStatus.IncorrectPassword:
                        (window as MainWindow).DialogOk("Неверный пароль");
                        break;
                }
            },
            obj =>
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                    return false;

                return true;
            });

        }

        #region Bindings

        private string login;
        public string Login
        {
            get => login;
            set
            {
                login = value;
                OnPropertyChanged();
            }
        }

        private string password;
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdLogin { get; private set; }

        #endregion

        #region Events

        public delegate void LoginHandler();

        public event LoginHandler OnLoginSuccessful;

        #endregion

    }
}
