using BLL.Services;
using EmployeeApp.Model;
using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using DTO = Interfaces.DTO;
using Model = EmployeeApp.Model;

namespace EmployeeApp.ViewModel
{
    public partial class CreateRouteViewModel : ViewModelBase
    {

        private readonly IRouteService routeService;
        private readonly IUserService userService;

        public CreateRouteViewModel(Window window, IRouteService routeService, IUserService userService) 
        : base(window)
        {
            this.routeService = routeService;
            this.userService = userService;

            // Commands

            CmdReset = new RelayCommand(obj =>
            {
                Rels.Clear();
                RelPrices.Clear();
                Rels.Add(new Model.Rel() { EnableMoveUp = false, EnableMoveDown = true });
                Rels.Add(new Model.Rel() { EnableMoveUp = true, EnableMoveDown = false });
                RelPrices.Add(new Model.RelPrice());
                CanRemove = false;

                Transports.Clear();
                var transports = routeService.GetAllTransports();
                foreach (var t in transports)
                {
                    var busCompany = routeService.GetBusCompanyById(t.BusCompanyId).Name;
                    var transportType = routeService.GetTransportTypeById(t.TransportTypeId).Name;

                    Transports.Add(new Model.Transport()
                    {
                        Id = t.Id,
                        Name = $"{busCompany} // {transportType} / {t.MaxSeats} мест, {t.MaxLuggage} багажных мест"

                    });
                }

                Destinations.Clear();
                Destinations = new ObservableCollection<DTO.Destination>(routeService.GetAllDestinations());

            });

            CmdAddRel = new RelayCommand(obj =>
            {
                Rels.Add(new Model.Rel() 
                { 
                    EnableMoveUp = true, 
                    EnableMoveDown = false 
                });

                RelPrices.Add(new Model.RelPrice());

                if (Rels.Count > 2)
                    CanRemove = true;

                Rels[Rels.Count - 2].EnableMoveDown = true;
            });
			
			CmdRemoveRel = new RelayCommand(obj =>
			{
				var rel = obj as Model.Rel;
				int index = Rels.IndexOf(rel);
				int last = Rels.Count - 1;

				if (index == 0)
				{
					Rels.RemoveAt(0);
					var first = Rels.ElementAt(0);
					first.EnableMoveUp = false;
					RelPrices.RemoveAt(0);
				}
				else if (index == last)
				{
					Rels.RemoveAt(last);
					var lastRel = Rels.ElementAt(last - 1);
					lastRel.EnableMoveDown = false;
					RelPrices.RemoveAt(RelPrices.Count - 1);
				}
				else
				{
					Rels.RemoveAt(index);
					var prev = RelPrices.ElementAt(index - 1);
					var cur = RelPrices.ElementAt(index);
					RelPrices.Remove(prev);
					cur.AdultPrice = 0;
					cur.UnderagePrice = 0;
				}

				if (Rels.Count == 2)
					CanRemove = false;

			});

            CmdMoveRelUp = new RelayCommand(obj =>
            {
                var rel = obj as Model.Rel;
                int index = Rels.IndexOf(rel);
                int last = Rels.Count - 1;

                (Rels[index], Rels[index - 1]) = (Rels[index - 1], Rels[index]);

                rel.EnableMoveUp = (index - 1) == 0 ? false : true;
                rel.EnableMoveDown = true;
                Rels[index].EnableMoveUp = true;
                Rels[index].EnableMoveDown = last == index ? false : true;
            });

            CmdMoveRelDown = new RelayCommand(obj =>
            {
                var rel = obj as Model.Rel;
                int index = Rels.IndexOf(rel);
                int last = Rels.Count - 1;

                (Rels[index], Rels[index + 1]) = (Rels[index + 1], Rels[index]);

                rel.EnableMoveUp = true;
                rel.EnableMoveDown = (index + 1) == last ? false : true;
                Rels[index].EnableMoveUp = index == 0 ? false : true;
                Rels[index].EnableMoveDown = true;

            });

            CmdCreateRoute = new RelayCommand(obj =>
            {

                var mainWindow = window as MainWindow;

                if (!mainWindow.DialogConfirm("Введённые данные верны?"))
                    return;

                var routeInfo = new DTO.RouteInfo();

                if (schedule.ScheduleMask == 0)
                {
                    mainWindow.DialogOk("Вы не заполнили расписание");
                    return;
                }

                if (selectedTransport == null)
                {
                    mainWindow.DialogOk("Вы не выбрали перевозчика");
                    return;
                }

                routeInfo.Schedule = schedule.ScheduleMask;
                routeInfo.TransportId = selectedTransport.Value;

                var relsDto = new List<DTO.Rel_RouteInfo_Destination>();

                decimal totalAdultPrice = 0m;
                decimal totalUnderagePrice = 0m;


                for (int i = 0; i < Rels.Count; ++i)
                {

                    var rel = Rels[i];

                    if (rel.DestinationId == null)
                    {
                        mainWindow.DialogOk("Заданы не все остановки");
                        return;
                    }

                    if (rel.ArrivalTime == null)
                    {
                        mainWindow.DialogOk("У всех остановок должно быть указано время прибытия");
                        return;
                    }

                    if (i != 0)
                    {

                        if (RelPrices[i - 1].AdultPrice == 0m)
                        {
                            mainWindow.DialogOk("У всех остановок должна быть указана цена взрослого билета");
                            return;
                        }

                        if (RelPrices[i - 1].UnderagePrice == 0m)
                        {
                            mainWindow.DialogOk("У всех остановок должна быть указана цена детского билета");
                            return;
                        }

                        totalAdultPrice += RelPrices[i - 1].AdultPrice;
                        totalUnderagePrice += RelPrices[i - 1].UnderagePrice;
                    }

                    relsDto.Add(new DTO.Rel_RouteInfo_Destination()
                    {
                        DestinationId = (int)rel.DestinationId,
                        OrderNumber = i,
                        AdultPrice = i != 0 ? RelPrices[i - 1].AdulPriceDecimal : 0m,
                        UnderagePrice = i != 0 ? RelPrices[i - 1].UnderagePriceDecimal : 0m,
                        ArrivalHour = (int)rel.ArrivalHour,
                        ArrivalMinute = (byte)rel.ArrivalMinute

                    });

                }

                try
                {
                    routeService.CreateRouteInfo(routeInfo, relsDto);
                }
                catch (Exception ex)
                {
                    (window as MainWindow).DialogOk(ex.Message);
                    return;
                }

                CmdReset.Execute(null);
                mainWindow.DialogOk("Новый маршрут успешно добавлен!");

            });

        }

        #region Bindings

        public ObservableCollection<Model.Rel> Rels { get; set; } = new ObservableCollection<Model.Rel>();
        public ObservableCollection<Model.RelPrice> RelPrices { get; set; } = new ObservableCollection<Model.RelPrice>();
        public ObservableCollection<Model.Transport> Transports { get; set; } = new ObservableCollection<Model.Transport>();
        public ObservableCollection<DTO.Destination> Destinations { get; set; } = new ObservableCollection<DTO.Destination>();

        private bool canRemove;
        public bool CanRemove
        {
            get => canRemove;
            set
            {
                canRemove = value;
                OnPropertyChanged("CanRemove");
            }
        }

        private int? selectedTransport;
        public int? SelectedTransport
        {
            get => selectedTransport;
            set
            {
                selectedTransport = value;
                OnPropertyChanged("SelectedTransport");
            }
        }

        private Model.Schedule schedule = new Model.Schedule();
        public Model.Schedule Schedule
        {
            get => schedule;
            set
            {
                schedule = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdReset { get; private set; }
        public RelayCommand CmdAddRel { get; private set; }
		public RelayCommand CmdRemoveRel { get; private set; }
        public RelayCommand CmdMoveRelUp { get; private set; }
        public RelayCommand CmdMoveRelDown { get; private set; }
        public RelayCommand CmdCreateRoute { get; private set; }

        #endregion

        #region Events

        #endregion

    }

    // TRANSPORT $"{BusCompany} // {TransportType} / {MaxSeats} мест, {MaxLuggage} багажных мест";

}
