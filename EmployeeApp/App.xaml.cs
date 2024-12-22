using BLL.Services;
using DAL.Repository;
using Interfaces.Repository;
using Interfaces.Services;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DTO = Interfaces.DTO; 

namespace EmployeeApp
{

    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            connectionString = ConfigurationManager.ConnectionStrings["BusTerminal"].ConnectionString;
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("ru-RU");

            ConfigureContainer();
            ComposeObjects();
            Current.MainWindow.Show();
        }

        private void ConfigureContainer()
        {
            container = new StandardKernel();

            container.Bind<IDbRepository>()
                     .To<DbRepository>()
                     .InSingletonScope()
                     .WithConstructorArgument(connectionString);

            container.Bind<IRouteService>()
                     .To<RouteService>()
                     .InSingletonScope();

            container.Bind<IUserService>()
                     .To<UserService>()
                     .InSingletonScope()
                     .WithConstructorArgument("userType", typeof(DTO.Employee));
        }

        private void ComposeObjects()
        {
            Current.MainWindow = container.Get<MainWindow>();
            Current.MainWindow.Title = "Автовокзал";
        }

        private IKernel container;
        private string connectionString;

    }
}
