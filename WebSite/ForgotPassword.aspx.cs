using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Account_ForgotPassword : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected async void ResetPassword(object sender, EventArgs e)
    {
        if (ModelState.IsValid)
        {
            var email = Request.Form["Email"];
            var user = await Master.UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                FailureText.Text = "Adress Mail not valid";
                ErrorMessage.Visible = true;
                return;
            }

            var code = await Master.UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var url = new UriBuilder("http://www.epicsportexchange.com/ResetPassword");
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["Id"] = user.Id;
            parameters["Code"] = code;
            url.Query = parameters.ToString();

            await Master.UserManager.SendEmailAsync(user.Id,
               "Reset your Password",
               "Please reset your password by clicking <a href=\""
                                               + url + "\">here</a>");

            SuccessText.Text = "A reset Email has been correctly send to " + email;
            ErrorMessage.Visible = false;
            SuccessMessage.Visible = true;
        }
    }
}