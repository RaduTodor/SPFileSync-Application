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
            //TODO [CR BT]: check value for null
            var checkIfInteger = int.TryParse(value.ToString(), out intValue);
            //TODO [CR BT]: check in the first place if the value is null. otherwise the values.ToString will crush.
            if (value.ToString().Length == 0 || value == null)
                return new ValidationResult(false, ConfigurationMessages.EmptyField);
            if (checkIfInteger && intValue <= 0)
                return new ValidationResult(false, ConfigurationMessages.InvalidValue);
            if (!checkIfInteger) return new ValidationResult(false, ConfigurationMessages.InvalidValue);
            return ValidationResult.ValidResult;
        }
    }
}