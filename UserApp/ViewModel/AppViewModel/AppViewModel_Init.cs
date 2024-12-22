using Interfaces.Services;
using System.Windows;

namespace UserApp.ViewModel.AppViewModel
{
    public partial class AppViewModel
    {
        public AppViewModel(Window window, IRouteService routeService) 
        {
            Window = window;
            RouteService = routeService;
            InitRouteInfoCreate();
        }

        public bool AskConfirmation(string text)
        {
            var confirmWindow = new View.Windows.ConfirmationWindow(Window, text);
            Window.Opacity = 0.4;
            confirmWindow.ShowDialog();
            Window.Opacity = 1;

            return confirmWindow.Success;
        }

        public void AskOk(string text)
        {
            var okWindow = new View.Windows.OkWindow(Window, text);
            Window.Opacity = 0.4;
            okWindow.ShowDialog();
            Window.Opacity = 1;
        }

        private readonly IRouteService RouteService;
        private readonly Window Window;

    }

    

}