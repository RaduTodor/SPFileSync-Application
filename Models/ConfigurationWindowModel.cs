using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Models
{
    public class ConfigurationWindowModel:ValidationRule
    {
        public string SyncBoxValidation { get; set; }
        public string PathValidation { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SiteUrl { get; set; }
        public string ListUrl { get; set; }
        public string UrlColumn { get; set; }
        public string UserColumn { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(value == null)
            {
                return new ValidationResult(false, Common.Constants.ConfigurationMessages.EmptyField);
            }

            return ValidationResult.ValidResult;
        }
    }
}
