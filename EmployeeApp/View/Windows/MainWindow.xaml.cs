using Interfaces.Services;
using System.Collections.Generic;
using System.Windows;

using System.ComponentModel;
using EmployeeApp.ViewModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Windows.Markup;
using DTO = Interfaces.DTO;

namespace EmployeeApp
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private readonly IRouteService routeService;
        private readonly IUserService userService;

        public MainWindow(IRouteService routeService, IUserService userService)
        {

            this.routeService = routeService;
            this.userService = userService;

            // ViewModels

            vmLogin = new LoginViewModel(this, userService);
            vmLogin.OnLoginSuccessful += LoginHandler;

            vmCashier = new CashierViewModel(this, routeService, userService);
            vmOperator = new OperatorViewModel(this, routeService, userService);

            DataContext = vmLogin;

            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);

            InitializeComponent();
        }

        public bool DialogConfirm(string text)
        {
            var confirmWindow = new View.Windows.ConfirmationWindow(this, text);
            Opacity = 0.4;
            confirmWindow.ShowDialog();
            Opacity = 1;

            return confirmWindow.Success;
        }

        public void DialogOk(string text)
        {
            var okWindow = new View.Windows.OkWindow(this, text);
            Opacity = 0.4;
            okWindow.ShowDialog();
            Opacity = 1;
        }

        public LoginViewModel vmLogin { get; private set; }
        public CashierViewModel vmCashier { get; private set; }
        public OperatorViewModel vmOperator { get; private set; }

        public void LoginHandler()
        {
            var employee = userService.CurrentUser as DTO.Employee;

            if (employee.EmployeeType == 1)
            {
                DialogOk("Интерфейс администратора не поддерживается");
                userService.Logout();
            }
            else if (employee.EmployeeType == 2)
            {
                DataContext = vmCashier;
            }
            else if (employee.EmployeeType == 3)
            {
                DataContext = vmOperator;
                vmOperator.vmCreateRoute.CmdReset.Execute(null);
            }
            else
            {
                DialogOk("Произошла неизвестная ошибка");
                userService.Logout();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

}