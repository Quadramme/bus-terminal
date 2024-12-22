using Interfaces.Services;
using System.Collections.Generic;
using System.Windows;

using System.ComponentModel;
using UserApp.ViewModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Windows.Markup;

namespace UserApp
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public MainWindow(IRouteService routeService, IUserService userService)
        {

            this.routeService = routeService;
            this.userService = userService;

            // ViewModels

            vmLogin = new LoginViewModel(this, userService);
            vmLogin.OnLoginSuccessful += LoginHandler;

            vmMain = new MainViewModel(this, routeService, userService);

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

        private readonly IRouteService routeService;
        private readonly IUserService userService;

        public LoginViewModel vmLogin { get; private set; }
        public MainViewModel vmMain { get; private set; }

        public void LoginHandler()
        {
            DataContext = vmMain;
            vmMain.vmSearchRoutes.ReloadDestinations();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}