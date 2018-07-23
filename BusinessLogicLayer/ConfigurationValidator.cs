using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
   public class ConfigurationValidator
    {

        //EditPanel
        //ConfigWindow

        public bool ValidateConfigWindowFields()
        {
            var input = true;
            return input;
        }

        //private bool ValidateAllFields()
        //{
        //    bool input = false;
        //    var syncBoxValidation = _generalUI.FieldValidation(syncTextBox.Text, syncLabel);
        //    var pathValidation = _generalUI.FieldValidation(_path, pathLabel);
        //    var userName = _generalUI.FieldValidation(userNameTextBox.Text, userNameErrorLabel);
        //    var password = _generalUI.FieldValidation(passwordText.Password, passwordErrorLabel);
        //    var siteUrl = _generalUI.FieldValidation(siteUrlBox.Text, siteErrorLabel);
        //    var listUrl = _generalUI.FieldValidation(listTextBox.Text, listErrorLabel);
        //    var urlColumn = _generalUI.FieldValidation(urlColumnTextBox.Text, urlColumnError);
        //    var userColumn = _generalUI.FieldValidation(userColumnTextBox.Text, userColumnError);
        //    if (syncBoxValidation && pathValidation && userName && password && siteUrl && listUrl && urlColumn && userColumn)
        //    {
        //        input = true;
        //    }
        //    return input;
        //}

    }
}
