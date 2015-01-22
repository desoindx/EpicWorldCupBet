using System;
using System.Linq;
using Microsoft.AspNet.Identity;

public partial class Account_ResetPassword : System.Web.UI.Page
{
    private string _code;
    protected void Page_Load(object sender, EventArgs e)
    {
        foreach (string query in Request.QueryString)
        {
            switch (query)
            {
                case "Id":
                    var userId = Request.QueryString[query];
                    var user = Master.UserManager.FindById(userId);
                    if (user != null)
                        Email.Text = user.Email;
                    break;
                case "Code":
                    _code = Request.QueryString[query];
                    break;
            }
        }
    }

    protected async void ChangePassword(object sender, EventArgs e)
    {
        if (ModelState.IsValid)
        {
            var user = await Master.UserManager.FindByNameAsync(Email.Text);
            if (user == null)
            {
                FailureText.Text = "Adress Mail not valid";
                ErrorMessage.Visible = true;
                return;
            }

            var result = await Master.UserManager.ResetPasswordAsync(user.Id, _code, Password.Text);
            if (result.Succeeded)
            {
                SuccessText.Text = "Your Email has been correctly changed.";
                ErrorMessage.Visible = false;
                SuccessMessage.Visible = true;
                return;
            }
            FailureText.Text = result.Errors.First();
            ErrorMessage.Visible = true;
            return;
        }
        FailureText.Text = "Something went wrong, please verify the informations and try again.";
        ErrorMessage.Visible = true;
    }
}