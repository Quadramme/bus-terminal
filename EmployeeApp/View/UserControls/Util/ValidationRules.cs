using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace EmployeeApp.View.UserControls.ValidationRules
{
    public class RequiredValidationRule : ValidationRule
    {

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            object v = GetBoundValue(value);

            if (v == null)
                return new ValidationResult(false, "Обязательное поле");

            if (v is string s)
            {
                var check = s.Trim();

                if (check.Length == 0)
                    return new ValidationResult(false, "Обязательное поле");
            }

            return ValidationResult.ValidResult;
        }

        private object GetBoundValue(object value)
        {
            if (value is BindingExpression)
            {

                BindingExpression binding = (BindingExpression)value;

                object dataItem = binding.DataItem;
                string propertyName = binding.ParentBinding.Path.Path;

                object propertyValue = dataItem.GetType().GetProperty(propertyName).GetValue(dataItem, null);

                return propertyValue;
            }
            else
            {
                return value;
            }
        }

    }

    public class PasswordValidationRule : ValidationRule
    {

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            if (value is string password)
            {
                var check = password.Trim();

                if (check.Length == 0)
                    return new ValidationResult(false, "Обязательное поле");

            }

            return ValidationResult.ValidResult;
        }

    }

}
