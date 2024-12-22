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

namespace EmployeeApp.View.UserControls.MakeOrder
{

    public partial class TicketCard : UserControl
    {
        public TicketCard()
        {
            InitializeComponent();

            ComboBox.DisplayMemberPath = "Value";
            ComboBox.Items.Add(new KeyValuePair<bool, string>(false, "Мужской"));
            ComboBox.Items.Add(new KeyValuePair<bool, string>(true, "Женский"));

        }

    }
    public class RemoveConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;

            if (count > 1)
                return "Visible";

            return "Collapsed";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class ZeroDisableConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((int)value > 1)
                return "Enabled";

            return "Disabled";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
