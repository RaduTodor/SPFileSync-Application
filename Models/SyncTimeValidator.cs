
namespace Models
{
    using System.Globalization;
    using System.Windows.Controls;

    public class SyncTimeValidator : ValidationRule
    {
        private int _syncTime;
        public int SyncTime { get; set; } = 10;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int intValue = 0;
            var checkIfInteger = int.TryParse(value.ToString(), out intValue);
            if (value.ToString().Length == 0)
            {
                return new ValidationResult(false, Common.Constants.ConfigurationMessages.EmptyField);
            }
            else if (checkIfInteger && intValue <= 0)
            {
                return new ValidationResult(false, Common.Constants.ConfigurationMessages.InvalidValue);
            }
            else if (!checkIfInteger)
            {
                return new ValidationResult(false, Common.Constants.ConfigurationMessages.InvalidValue);
            }
            return ValidationResult.ValidResult;
        }
    }
}
