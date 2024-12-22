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

    public partial class ReportDateCard : UserControl
    {
        public ReportDateCard()
        {
            
            InitializeComponent();
            SearchTypeComboBox.SelectedValuePath = "Key";
            SearchTypeComboBox.DisplayMemberPath = "Value";
            SearchTypeComboBox.ItemsSource = new Dictionary<VM.SearchTypes, string>()
            {
                { VM.SearchTypes.ThisMonth, "За этот месяц" },
                { VM.SearchTypes.ThisYear, "За этот год" },
                { VM.SearchTypes.Overall, "За всё время" },
                { VM.SearchTypes.Custom, "Свой разрез времени" }
            };

        }
    }

    public class ReportCreatedConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "Visible";

            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class IsCustomDateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null)
                return "Collapsed";

            if ((VM.SearchTypes)value == VM.SearchTypes.Custom)
                return "Visible";

            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}