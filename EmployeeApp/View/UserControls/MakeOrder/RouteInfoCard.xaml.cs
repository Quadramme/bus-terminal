using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace EmployeeApp.View.UserControls.MakeOrder
{

    public partial class RouteInfoCard : UserControl
    {
        public RouteInfoCard()
        {
            InitializeComponent();
        }
    }

    public class TripConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Model.Destination departure = values[0] as Model.Destination;
            Model.Destination arrival = values[1] as Model.Destination;
            DateTime date = (DateTime)values[2];

            if (departure != null && arrival != null)
            {
                return departure.SettlementName + " — " + arrival.SettlementName + ", " + date.ToString("dd MMMM") + ", " + date.ToString("dddd");
            }

            return null;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class BusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int seats = (int)values[0];
            int luggage = (int)values[1];

            return seats + " пасс. мест,\n" + luggage + " баг. мест";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class DestinationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string name = values[0] as string;
            DateTime arrivalTime  = (DateTime)values[1];

            if (!string.IsNullOrEmpty(name))
                return name + "    |    Прибытие в " + arrivalTime.ToString("t", culture);

            return null;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class IsVisitedConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisited = (bool)value;

            if (isVisited)
                return Colors.Black;
            else
                return Colors.LightGray;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
