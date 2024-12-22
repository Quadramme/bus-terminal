using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using DTO = Interfaces.DTO;

namespace UserApp.ViewModel.AppViewModel
{
    public partial class AppViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Rel> Rels { get; set; }
        public ObservableCollection<RelPrice> RelPrices { get; set; }
        public ObservableCollection<Transport> Transports { get; set; }

        public void InitRouteInfoCreate()
        {
            if (Rels == null)
            {
                Rels = new ObservableCollection<Rel>();
            }
            else
            {
                Rels.Clear();
            }

            if (RelPrices == null)
            {
                RelPrices = new ObservableCollection<RelPrice>();
            }
            else
            {
                RelPrices.Clear();
            }

            if (Transports == null)
            {
                Transports = new ObservableCollection<Transport>();
            }
            else
            {
                Transports.Clear();
            }

            var ts = RouteService.GetAllTransports();

            foreach (var t in ts)
            {
                Transports.Add(new Transport()
                {
                    Id = t.Id,
                    MaxSeats = t.MaxSeats,
                    MaxLuggage = t.MaxLuggage,
                    BusCompany = RouteService.GetBusCompanyById(t.BusCompanyId).Name,
                    TransportType = RouteService.GetTransportTypeById(t.TransportTypeId).Name
                });
            }

            Rels.Add(new Rel() { EnableMoveUp = false, EnableMoveDown = true });
            Rels.Add(new Rel() { EnableMoveUp = true, EnableMoveDown = false });
            RelPrices.Add(new RelPrice());
            CanRemove = false;

        }

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

        private int schedule;

        public string ScheduleString
        {
            get
            {
                string s = "";
                for (int i = 0; i < 7; ++i)
                    s += (schedule & (1 << i)) == 0 ? "0" : "1";

                return string.Join("", s.Reverse());
            }
        }

        public bool ScheduleMonday
        {
            get => (schedule & 0b00000001) != 0;
            set
            {
                if (value)
                {
                    schedule |= (char)0b00000001;
                }
                else
                {
                    schedule &= (char)0b11111110;
                }
                OnPropertyChanged("ScheduleString");

            }
        }

        public bool ScheduleTuesday
        {
            get => (schedule & 0b00000010) != 0;
            set
            {
                if (value)
                {
                    schedule |= (char)0b00000010;
                }
                else
                {
                    schedule &= (char)0b11111101;
                }
                OnPropertyChanged("ScheduleString");
            }
        }

        public bool ScheduleWednesday
        {
            get => (schedule & 0b00000100) != 0;
            set
            {
                if (value)
                {
                    schedule |= (char)0b00000100;
                }
                else
                {
                    schedule &= (char)0b11111011;
                }
                OnPropertyChanged("ScheduleString");
            }
        }

        public bool ScheduleThursday
        {
            get => (schedule & 0b00001000) != 0;
            set
            {
                if (value)
                {
                    schedule |= (char)0b00001000;
                }
                else
                {
                    schedule &= (char)0b11110111;
                }
                OnPropertyChanged("ScheduleString");
            }
        }

        public bool ScheduleFriday
        {
            get => (schedule & 0b00010000) != 0;
            set
            {
                if (value)
                {
                    schedule |= (char)0b00010000;
                }
                else
                {
                    schedule &= (char)0b11101111;
                }
                OnPropertyChanged("ScheduleString");
            }
        }

        public bool ScheduleSaturday
        {
            get => (schedule & 0b00100000) != 0;
            set
            {
                if (value)
                {
                    schedule |= (char)0b00100000;
                }
                else
                {
                    schedule &= (char)0b11011111;
                }
                OnPropertyChanged("ScheduleString");
            }
        }

        public bool ScheduleSunday
        {
            get => (schedule & 0b01000000) != 0;
            set
            {
                if (value)
                {
                    schedule |= (char)0b01000000;
                }
                else
                {
                    schedule &= (char)0b10111111;
                }
                OnPropertyChanged("ScheduleString");
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

        private RelayCommand cmdAddRel;
        public RelayCommand CmdAddRel
        {
            get
            {
                return cmdAddRel ??
                  (cmdAddRel = new RelayCommand(obj =>
                  {
                      Rels.Add(new Rel() { EnableMoveUp = true, EnableMoveDown = false });
                      RelPrices.Add(new RelPrice());

                      if (Rels.Count > 2)
                          CanRemove = true;

                      Rels[Rels.Count - 2].EnableMoveDown = true;

                  }));
            }
        }

       

        private RelayCommand cmdMoveRelUp;
        public RelayCommand CmdMoveRelUp
        {
            get
            {
                return cmdMoveRelUp ??
                  (cmdMoveRelUp = new RelayCommand(obj =>
                  {
                      var rel = obj as Rel;
                      int index = Rels.IndexOf(rel);
                      int last = Rels.Count - 1;

                      (Rels[index], Rels[index - 1]) = (Rels[index - 1], Rels[index]);

                      rel.EnableMoveUp = (index - 1) == 0 ? false : true;
                      rel.EnableMoveDown = true;
                      Rels[index].EnableMoveUp = true;
                      Rels[index].EnableMoveDown = last == index ? false : true;
                      
                  }));
            }
        }

        private RelayCommand cmdMoveRelDown;
        public RelayCommand CmdMoveRelDown
        {
            get
            {
                return cmdMoveRelDown ??
                  (cmdMoveRelDown = new RelayCommand(obj =>
                  {
                      var rel = obj as Rel;
                      int index = Rels.IndexOf(rel);
                      int last = Rels.Count - 1;

                      (Rels[index], Rels[index + 1]) = (Rels[index + 1], Rels[index]);

                      rel.EnableMoveUp = true;
                      rel.EnableMoveDown = (index + 1) == last ? false : true;
                      Rels[index].EnableMoveUp = index == 0 ? false : true ;
                      Rels[index].EnableMoveDown = true;

                  }));
            }
        }

        private RelayCommand cmdCreateRoute;
        public RelayCommand CmdCreateRoute
        {
            get
            {
                return cmdCreateRoute ??
                  (cmdCreateRoute = new RelayCommand(obj =>
                  {

                      if (!AskConfirmation("Введённые данные верны?"))
                          return;

                      var routeInfo = new DTO.RouteInfo();

                      if (schedule == 0)
                      {
                          AskOk("Вы не заполнили расписание");
                          return;
                      }

                      if (selectedTransport == null)
                      {
                          AskOk("Вы не выбрали перевозчика");
                          return;
                      }

                      routeInfo.Schedule = schedule;
                      routeInfo.TransportId = selectedTransport.Value;

                      var relsDto = new List<DTO.Rel_RouteInfo_Destination>();

                      decimal totalAdultPrice = 0m;
                      decimal totalUnderagePrice = 0m;


                      for (int i = 0; i < Rels.Count; ++i) 
                      {

                          var rel = Rels[i];

                          if (rel.DestinationId == null)
                          {
                              AskOk("Заданы не все остановки");
                              return;
                          }

                          if (rel.ArrivalTime == null) 
                          {
                              AskOk("У всех остановок должно быть указано время прибытия");
                              return;
                          }

                          if (i != 0)
                          {
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

                      if (totalAdultPrice == 0m)
                      {
                          AskOk("Взрослый билет не может быть бесплатным");
                          return;
                      }

                      if (totalUnderagePrice == 0m)
                      {
                          AskOk("Детский билет не может быть бесплатным");
                          return;
                      }

                      RouteService.CreateRouteInfo(routeInfo, relsDto);

                      InitRouteInfoCreate();
                      AskOk("Новый маршрут успешно добавлен!");

                  }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class Rel : INotifyPropertyChanged
    {

        private bool enableMoveUp;
        public bool EnableMoveUp
        {
            get => enableMoveUp;
            set
            {
                enableMoveUp = value;
                OnPropertyChanged("EnableMoveUp");
            }
        }

        private bool enableMoveDown;
        public bool EnableMoveDown
        {
            get => enableMoveDown;
            set
            {
                enableMoveDown = value;
                OnPropertyChanged("EnableMoveDown");
            }
        }

        private int? destinationId;
        public int? DestinationId 
        {
            get => destinationId;
            set
            {
                destinationId = value;
                OnPropertyChanged("DestinationId");

            }
        }

        private decimal? adultPrice;
        public decimal? AdultPrice 
        {
            get => adultPrice;
            set
            { 
                adultPrice = value; 
                OnPropertyChanged("AdultPrice"); 
            }
        }

        private decimal? underagePrice;
        public decimal? UnderagePrice 
        {
            get => underagePrice;
            set
            {
                underagePrice = value;
                OnPropertyChanged("UnderagePrice");
            }
        }

        private int? arrivalHour;
        public int? ArrivalHour 
        {
            get => arrivalHour;
            set
            {
                arrivalHour = value;
                OnPropertyChanged("ArrivalHour");
                OnPropertyChanged("ArrivalTime");
            }
        }

        private byte? arrivalMinute;
        public byte? ArrivalMinute 
        {
            get => arrivalMinute;
            set
            {
                arrivalMinute = value;
                OnPropertyChanged("ArrivalMinute");
                OnPropertyChanged("ArrivalTime");
            }
        }
        public DateTime? ArrivalTime 
        {
            get
            {
                if (arrivalMinute == null || ArrivalHour == null)
                    return null;

                return new DateTime().AddHours((double)arrivalHour).AddMinutes((double)arrivalMinute); 
            }
            set
            {

                if (value == null) 
                {
                    ArrivalMinute = null;
                    ArrivalHour = null;
                }
                else
                {
                    ArrivalMinute = (byte)value?.Minute;
                    ArrivalHour = (int)value?.Hour;
                }

            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class RelPrice : INotifyPropertyChanged
    {
        private decimal adultPrice;
        public decimal AdulPriceDecimal 
        {

            get => adultPrice;
            set 
            {
                adultPrice = value;
                OnPropertyChanged("AdultPriceDecimal");
                OnPropertyChanged("AdultPrice");
            }
        
        }
        public int AdultPrice 
        {
            get => (int)adultPrice;
            set
            {
                AdulPriceDecimal = value;
            }
        }

        private decimal underagePrice;
        public decimal UnderagePriceDecimal
        {

            get => underagePrice;
            set
            {
                underagePrice = value;
                OnPropertyChanged("UnderagePriceDecimal");
                OnPropertyChanged("UnderagePrice");
            }

        }
        public int UnderagePrice
        {
            get => (int)underagePrice;
            set
            {
                UnderagePriceDecimal = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class Transport
    {
        public int Id { get; set; }
        public int MaxSeats { get; set; }
        public int MaxLuggage { get; set; }
        public string BusCompany { get; set; }
        public string TransportType { get; set; }
        public string Display
        {
            get => $"{BusCompany} // {TransportType} / {MaxSeats} мест, {MaxLuggage} багажных мест";
        }

    }

}
