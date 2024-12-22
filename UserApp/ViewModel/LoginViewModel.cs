using BLL.Services;
using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UserApp.ViewModel;
using DTO = Interfaces.DTO;

namespace UserApp.ViewModel
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

            CmdRegister = new RelayCommand(obj =>
            {

                while (!userService.CanConnect())
                {
                    (window as MainWindow).DialogOk("Нет подключения к серверу");
                }

                RegisterStatus success;

                try
                {
                    success = userService.Register(new DTO.Passenger()
                    {
                        Login = newLogin,
                        Password = newPassword,
                        FirstName = firstName,
                        MiddleName = middleName,
                        LastName = lastName
                    });
                } 
                catch
                {
                    (window as MainWindow).DialogOk("Что-то пошло не так");
                    return;
                }

                if (success == RegisterStatus.Success)
                {
                    NewLogin = null;
                    NewPassword = null;
                    FirstName = null;
                    MiddleName = null;
                    LastName = null;
                    (window as MainWindow).DialogOk("Вы зарегистрированы");
                } 
                else
                {
                    (window as MainWindow).DialogOk("Пользователь с таким логином уже зарегистрирован");
                }
            },
            obj =>
            {
                if (string.IsNullOrEmpty(newLogin) ||
                    string.IsNullOrEmpty(newPassword) ||
                    string.IsNullOrEmpty(newPasswordRepeat) ||
                    string.IsNullOrEmpty(FirstName) ||
                    string.IsNullOrEmpty(LastName))
                    return false;

                if (newPassword != newPasswordRepeat)
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

        private string newLogin;
        public string NewLogin
        {
            get => newLogin;
            set
            {
                newLogin = value;
                OnPropertyChanged();
            }
        }

        private string newPassword;
        public string NewPassword
        {
            get => newPassword;
            set
            {
                newPassword = value;
                OnPropertyChanged();
            }
        }

        private string newPasswordRepeat;
        public string NewPasswordRepeat
        {
            get => newPasswordRepeat;
            set
            {
                newPasswordRepeat = value;
                OnPropertyChanged();
            }
        }

        private string firstName;
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged();
            }
        }

        private string middleName;
        public string MiddleName
        {
            get => middleName;
            set
            {
                middleName = value;
                OnPropertyChanged();
            }
        }

        private string lastName;
        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdLogin { get; private set; }
        public RelayCommand CmdRegister { get; private set; }

        #endregion

        #region

        public delegate void LoginHandler();

        public event LoginHandler OnLoginSuccessful;

        #endregion

    }
}
