using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using UserApp.Model;
using DTO = Interfaces.DTO;
using System.Windows.Forms;
using iText.Kernel.Pdf;
using System.Security.Cryptography;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Layout.Properties;
using iText.StyledXmlParser.Jsoup.Nodes;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace UserApp.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {

        private readonly IUserService userService;
        private readonly IUserService routeService;

        public enum SearchTypes
        {
            ThisMonth,
            ThisYear,
            Overall,
            Custom
        }

        public enum ReportStatus
        {
            NotCreated,
            Created,
            CreatedEmpty
        }

        public ReportViewModel(Window window, IUserService userService, IRouteService routeService) 
        : base(window)
        {
            this.userService = userService;

            // Commands

            CmdGetReport = new RelayCommand(obj =>
            {
                DateTime? start = null;
                DateTime? end = null;

                

                if (searchType.Value == SearchTypes.ThisMonth)
                {
                    start = new DateTime(DateTime.Now.Year,
                                         DateTime.Now.Month,
                                         1, 0, 0, 0);

                    end = new DateTime(DateTime.Now.Year,
                                       DateTime.Now.Month,
                                       DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month),
                                       23, 59, 59);
                }
                else if (searchType.Value == SearchTypes.ThisYear)
                {
                    start = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
                    end = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 0);
                }
                else if (searchType.Value == SearchTypes.Custom)
                {
                    start = startDate.Value;
                    end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                }

                SearchTypeSelected = searchType;

                Dictionary<DTO.Order, List<DTO.Ticket>> report;

                try {
                    report = userService.GetPersonalReport(start, end);
                } 
                catch (Exception ex)
                {
                    (window as MainWindow).DialogOk(ex.Message);
                    return;
                }

                ReportData = null;
                ReportPositions.Clear();

                if (report.Count == 0)
                {
                    NoOrdersFound = true;
                    ReportCreated = false;
                    return;
                }

                var newReportData = new ReportData();

                var popularity = new Dictionary<int, int>();

                foreach (var position in report)
                {

                    var order = position.Key;
                    var tickets = position.Value;

                    var routeInfo = routeService.GetRouteInfo(order.RouteId);

                    if (popularity.ContainsKey(routeInfo.Id))
                        popularity[routeInfo.Id]++;
                    else
                        popularity.Add(routeInfo.Id, 1);

                    List<DTO.Destination> ds1;
                    decimal adultTicketPrice1;
                    decimal underageTicketPrice1;
                    int travelHours1;
                    byte travelMinutes1;
                    TimeSpan departureTime1;
                    TimeSpan arrivalTime1;

                    try
                    {
                        ds1 = routeService.GetFullRouteInfo(
                              routeInfo.Id,
                              order.DeparturePoint,
                              order.ArrivalPoint,
                              out adultTicketPrice1,
                              out underageTicketPrice1,
                              out travelHours1,
                              out travelMinutes1,
                              out departureTime1,
                              out arrivalTime1);
                    }
                    catch (Exception ex)
                    {
                        (window as MainWindow).DialogOk(ex.Message);
                        return;
                    }

                    var times = routeService.GetRouteTimes(order.RouteId);
                    int departureIndex = ds1.IndexOf(ds1.Find(d => d.Id == order.DeparturePoint));
                    int arrivalIndex = ds1.IndexOf(ds1.Find(d => d.Id == order.ArrivalPoint));

                    var pos = new Model.ReportPositionData()
                    {
                        OrderId = order.Id,
                        OrderDate = order.Date,
                        AdultPrice = adultTicketPrice1,
                        AdultCount = tickets.Where(t => t.Price == adultTicketPrice1).Count(),
                        UnderagePrice = underageTicketPrice1,
                        UnderageCount = tickets.Where(t => t.Price == underageTicketPrice1).Count(),
                        RouteInfoId = routeInfo.Id,
                        RouteId = order.RouteId,
                        DepartureCity = ds1[departureIndex].SettlementName,
                        DepartureDate = times[departureIndex],
                        ArrivalCity = ds1[arrivalIndex].SettlementName,
                        ArrivalDate = times[arrivalIndex],
                        TravelTime = times[arrivalIndex] - times[departureIndex]
                    };

                    ReportPositions.Add(pos);
                    newReportData.OverallPrice += pos.TotalPrice;
                    newReportData.OverallTravelTime += pos.TravelTime;

                }

                int max = 0;
                List<int> candidates = new List<int>();

                foreach (var p in popularity)
                {
                    if (p.Value > max)
                    {
                        max = p.Value;
                        candidates.Clear();
                        candidates.Add(p.Key);
                    }
                    else if (p.Value == max)
                    {
                        candidates.Add(p.Key);
                    }  
                }

                var random = new Random();
                var mostPopularRouteId = candidates[random.Next(candidates.Count)];
                var mostPopularRouteInfo = routeService.GetRouteInfoById(mostPopularRouteId);
                var mostPopularTransport = routeService.GetTransportById(mostPopularRouteInfo.TransportId);

                List<DTO.Destination> ds;
                decimal adultTicketPrice;
                decimal underageTicketPrice;
                int travelHours;
                byte travelMinutes;
                TimeSpan departureTime;
                TimeSpan arrivalTime;

                try
                {
                    ds = routeService.GetFullRouteInfo(
                         mostPopularRouteId,
                         null,
                         null,
                         out adultTicketPrice,
                         out underageTicketPrice,
                         out travelHours,
                         out travelMinutes,
                         out departureTime,
                         out arrivalTime);
                }
                catch (Exception ex)
                {
                    (window as MainWindow).DialogOk(ex.Message);
                    return;
                }

                var mostPopularUserOrders = ReportPositions.Where(p => p.RouteInfoId == mostPopularRouteId);

                newReportData.MostUsedRouteInfoId = mostPopularRouteId;
                newReportData.MostUsedOrderCount = mostPopularUserOrders.Count();
                newReportData.MostUsedSpent = mostPopularUserOrders.Sum(o => o.TotalPrice);
                newReportData.MostUsedDeparture = ds[0].SettlementName;
                newReportData.MostUsedArrival = ds.Last().SettlementName;
                newReportData.MostUsedCompany = routeService.GetBusCompanyById(mostPopularTransport.BusCompanyId).Name;

                ReportData = newReportData;

                NoOrdersFound = false;
                ReportCreated = true;

            },
            obj =>
            {
                if (!searchType.HasValue)
                    return false;

                if (searchType.Value == SearchTypes.ThisMonth || 
                    searchType.Value == SearchTypes.ThisYear ||
                    searchType.Value == SearchTypes.Overall)
                    return true;

                if (searchType.Value == SearchTypes.Custom &&
                    (startDate == null ||
                    endDate == null ||
                    (startDate > endDate)))
                    return false;

                return true;
            });

            CmdExportPDF = new RelayCommand(obj =>
            {

                string filePath;

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "PDF (*.pdf)|";
                saveFileDialog.InitialDirectory = "%HOMEPATH%";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.DefaultExt = "pdf";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = saveFileDialog.FileName;
                }
                else
                {
                    //(window as MainWindow).DialogOk("Выберите папку для сохранения PDF");
                    return;
                }

                PdfDocument pdfDocument;

                try
                {
                    pdfDocument = new PdfDocument(new PdfWriter(filePath));
                } 
                catch (Exception ex)
                {
                    (window as MainWindow).DialogOk(ex.Message);
                    return;
                }

                iText.Layout.Document doc = new iText.Layout.Document(pdfDocument);
                
                PdfFont font = PdfFontFactory.CreateFont("C:/Windows/Fonts/Arial.ttf", PdfEncodings.IDENTITY_H);

                Text text = null;

                switch (searchType.Value)
                {
                    case SearchTypes.ThisMonth:
                        text = new Text("Отчёт за этот месяц");
                        break;

                    case SearchTypes.ThisYear:
                        text = new Text("Отчёт за этот год");
                        break;

                    case SearchTypes.Overall:
                        text = new Text("Отчёт за всё время");
                        break;

                    case SearchTypes.Custom:
                        text = new Text("Отчёт за период с " + startDate?.ToString("dd/MM/yyyy") + " по " + endDate?.ToString("dd/MM/yyyy"));
                        break;
                }

                doc.Add(new Paragraph(text)
                            .SetFont(font)
                            .SetFontSize(25)
                            .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER));

                text = new Text($"Цена всех поездок: {reportData.OverallPrice.ToString("C")}");

                doc.Add(new Paragraph(text)
                            .SetFont(font));

                text = new Text($"Общее время в пути: {reportData.OverallTravelTime.ToString("d'д 'h'ч 'm'м'")}");
                
                doc.Add(new Paragraph(text)
                            .SetFont(font)
                            .SetMarginBottom(30));

                text = new Text("Ваш самый частый маршрут");
                doc.Add(new Paragraph(text)
                            .SetFont(font)
                            .SetFontSize(18));

                text = new Text($"[{reportData.MostUsedRouteInfoId.ToString("D12")}] {reportData.MostUsedDeparture} --> {reportData.MostUsedArrival} | Перевозчик {reportData.MostUsedCompany}");
                doc.Add(new Paragraph(text)
                            .SetFont(font));

                text = new Text($"Количество заказов на этот маршрут: {reportData.MostUsedOrderCount}\n Сумма этих заказов составила {reportData.MostUsedSpent.ToString("C")}");
                doc.Add(new Paragraph(text)
                            .SetFont(font)
                            .SetMarginBottom(30));

                text = new Text("Ваши заказы за этот период");
                doc.Add(new Paragraph(text)
                            .SetFont(font)
                            .SetFontSize(18));

                Table table = null;

                foreach (var pos in reportPositions)
                {

                    table = new Table(UnitValue.CreatePercentArray(new float[] { 0.75f, 1, 1}));
                    table.SetFont(font);
                    table.UseAllAvailableWidth();
                    table.SetMarginBottom(5);
                    table.SetKeepTogether(true);

                    table.AddCell(new Cell(1, 3).Add(new Paragraph($"Номер заказа {pos.OrderId.ToString("D12")}")
                                                         .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                                                         .SimulateBold()
                                                         ));

                    table.AddCell(new Cell(1, 3).Add(new Paragraph($"Дата заказа {pos.OrderDate.ToString("D")}")
                                                         .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                                                         .SimulateBold()
                                                         ));

                    table.AddCell(new Cell(1, 1).Add(new Paragraph("Отправление")));
                    table.AddCell(new Cell(1, 1).Add(new Paragraph(pos.DepartureCity)));
                    table.AddCell(new Cell(1, 1).Add(new Paragraph(pos.DepartureDate.ToString())));

                    table.AddCell(new Cell(1, 1).Add(new Paragraph("Прибытие")));
                    table.AddCell(new Cell(1, 1).Add(new Paragraph(pos.ArrivalCity)));
                    table.AddCell(new Cell(1, 1).Add(new Paragraph(pos.ArrivalDate.ToString())));

                    string prices = "";

                    if (pos.AdultCount != 0)
                        prices += $"Взрослый {pos.AdultCount} x {pos.AdultPrice.ToString("C")}";

                    if (pos.UnderageCount != 0 && !string.IsNullOrWhiteSpace(prices))
                        prices += $"\nДетский {pos.UnderageCount} x {pos.UnderagePrice.ToString("C")}";
                    else if (pos.UnderageCount != 0 && string.IsNullOrWhiteSpace(prices))
                        prices += $"Детский {pos.UnderageCount} x {pos.UnderagePrice.ToString("C")}";

                    table.AddCell(new Cell(1, 3).Add(new Paragraph(prices)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)));

                    //table.AddCell();

                    doc.Add(table);

                    doc.Add(new Paragraph($"ИТОГО: {pos.TotalPrice.ToString("C")}")
                        .SetFont(font)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                        .SimulateBold()
                        .SetMarginBottom(40));

                }

                doc.Close();

            });

        }

        #region Bindings

        private SearchTypes? searchType;
        public SearchTypes? SearchType
        {
            get => searchType;
            set
            {
                searchType = value;
                OnPropertyChanged();
            }
        }

        private SearchTypes? searchTypeSelected;
        public SearchTypes? SearchTypeSelected
        {
            get => searchTypeSelected;
            set
            {
                searchTypeSelected = value;
                OnPropertyChanged();
            }
        }

        private DateTime? startDate;
        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime? endDate;
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                OnPropertyChanged();
            }
        }

        private bool reportCreated = false;
        public bool ReportCreated
        {
            get => reportCreated;
            set
            {
                reportCreated = value;
                OnPropertyChanged();
            }
        }

        private bool noOrdersFound = false;
        public bool NoOrdersFound
        {
            get => noOrdersFound;
            set
            {
                noOrdersFound = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Model.ReportPositionData> reportPositions = new ObservableCollection<Model.ReportPositionData>();
        public ObservableCollection<Model.ReportPositionData> ReportPositions
        {
            get => reportPositions;
            set
            {
                reportPositions = value;
                OnPropertyChanged();
            }
        }

        private Model.ReportData reportData;
        public Model.ReportData ReportData
        {
            get => reportData;
            set
            {
                reportData = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdGetReport { get; private set; }
        public RelayCommand CmdExportPDF { get; private set; }

        #endregion

    }
}
