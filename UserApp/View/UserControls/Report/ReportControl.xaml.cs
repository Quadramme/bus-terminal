using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VM = UserApp.ViewModel.ReportViewModel;

namespace UserApp.View.UserControls.Report
{
    /// <summary>
    /// Interaction logic for ReportControl.xaml
    /// </summary>
    public partial class ReportControl : UserControl
    {
        public ReportControl()
        {
            InitializeComponent();
        }
    }

    public class NoOrdersConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(bool)value)
                return "Collapsed";

            return "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class SearchTypeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values[0] == DependencyProperty.UnsetValue || values[0] == null)
                return DependencyProperty.UnsetValue;

            var type = (VM.SearchTypes)values[0];

            if (type == VM.SearchTypes.ThisMonth)
                return "Отчёт за этот месяц";
            else if (type == VM.SearchTypes.ThisYear)
                return "Отчёт за этот год";
            else if (type == VM.SearchTypes.Overall)
                return "Отчёт за всё время";
            else
            {

                if (values[1] == DependencyProperty.UnsetValue ||
                    values[2] == DependencyProperty.UnsetValue)
                    return DependencyProperty.UnsetValue;
                
                DateTime start = (DateTime)values[1];
                DateTime end = (DateTime)values[2];

                return "Отчёт за период с " + start.ToString("dd/MM/yyyy") + " по " + end.ToString("dd/MM/yyyy");
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}
