namespace Models
{
    using System.Globalization;
    using System.Windows.Controls;
    using Common.Constants;

    public class ConfigurationWindowModel : ValidationRule
    {
        public string UserName { get; set; } = "username";
        public string Password { get; set; } = "pass123";
        public string SiteUrl { get; set; } = "http://sp2013dc/sites/iship/";
        public string ListName { get; set; } = "SyncList";
        public string UrlColumn { get; set; } = "URL";
        public string UserColumn { get; set; } = "User";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(false, ConfigurationMessages.EmptyField);
            var intValue = 0;
            var checkIfInteger = int.TryParse(value.ToString(), out intValue);
            if (checkIfInteger && intValue <= 0) return new ValidationResult(false, ConfigurationMessages.InvalidValue);

            return ValidationResult.ValidResult;
        }
    }
}