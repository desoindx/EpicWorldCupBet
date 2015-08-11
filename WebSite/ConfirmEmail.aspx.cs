using System;
using Microsoft.AspNet.Identity;

public partial class Account_ConfirmEmail : System.Web.UI.Page
{
    private string _userId;
    private string _code;
    public bool Success;
    protected void Page_Load(object sender, EventArgs e)
    {
        foreach (string query in Request.QueryString)
        {
            switch (query)
            {
                case "Id":
                    _userId = Request.QueryString[query];
                    break;
                case "Code":
                    _code = Request.QueryString[query];
                    break;
            }
        }

        if (_userId != null || _code == null)
        {
            Response.Redirect("~/Default.aspx");
        }

        var result = Master.UserManager.ConfirmEmail(_userId, _code);
        Success = result.Succeeded;
    }
}