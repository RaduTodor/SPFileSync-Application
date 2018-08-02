namespace Common.Helpers
{
    using System.Globalization;
    using System.Windows.Controls;
    using Constants;

    public class SyncTimeValidator : ValidationRule
    {
        public int SyncTime { get; set; } = 10;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var intValue = 0;
            if(value!=null)
            {
                var checkIfInteger = int.TryParse(value.ToString(), out intValue);              
                if (value.ToString().Length == 0 || value == null)
                    return new ValidationResult(false, ConfigurationMessages.EmptyField);
                if (checkIfInteger && intValue <= 0)
                    return new ValidationResult(false, ConfigurationMessages.InvalidValue);
                if (!checkIfInteger) return new ValidationResult(false, ConfigurationMessages.InvalidValue);
            }
            else
            {
                return new ValidationResult(false, ConfigurationMessages.InvalidValue);
            }
            return ValidationResult.ValidResult;
        }
    }
}