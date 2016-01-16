using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

public partial class SiteMaster : MasterPage
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    public static TextInfo TextInfo { get; private set; }
    protected void Page_Init(object sender, EventArgs e)
    {
        TextInfo = new CultureInfo("en-US", false).TextInfo;
        // The code below helps to protect against XSRF attacks
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // Use the Anti-XSRF token from the cookie
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // Generate a new Anti-XSRF token and save to the cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;
    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Set Anti-XSRF token
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            // Validate the Anti-XSRF token
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut();
    }

    protected IHtmlString GetFolders()
    {
        using (var stringWriter = new StringWriter())
        using (var jsonWriter = new JsonTextWriter(stringWriter))
        {
            var serializer = new JsonSerializer();

            // We don't want quotes around object names
            jsonWriter.QuoteName = false;
            serializer.Serialize(jsonWriter, GetFolders(null));

            //            CopyToTemp();
            //            CopyFromTemp();

            return new HtmlString(stringWriter.ToString());
        }
    }

    private static void CopyToTemp()
    {
        var images = "/Berliner/img/BD/";
        var path = HttpContext.Current.Server.MapPath("~" + images);
        var allFiles = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories).ToList();
        foreach (var photo in allFiles)
        {
            int count = 1;
            var currentPath = photo;
            var fileName = photo.Remove(0, photo.LastIndexOf("\\") + 1);
            var destFileName = "C:\\Temp\\" + fileName;
            while (File.Exists(destFileName))
            {
                currentPath = photo.Replace(fileName, count + "-" + fileName);
                destFileName = "C:\\Temp\\" + count + "-" + fileName;
                count++;
            }
            if (count > 1)
            {
                File.Move(photo, currentPath);
            }

            File.Copy(currentPath, destFileName);
        }
    }

    private static void CopyFromTemp()
    {
        var images = "/Berliner/img/AllThumbnails/";
        var path = HttpContext.Current.Server.MapPath("~" + images);
        var allFiles = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories).ToList();
        var smallImages = "/Berliner/img/Thumbnails/";
        var smallImagePath = HttpContext.Current.Server.MapPath("~" + smallImages);
        foreach (var photo in allFiles)
        {
            var fileName = photo.Remove(0, photo.LastIndexOf("\\") + 1);
            var destFileName = smallImagePath + "tn_" + fileName;
            if (File.Exists(destFileName))
            {
                File.Delete(photo);
                File.Move(destFileName, photo);
            }
        }
    }

    protected object GetFolders(string folder)
    {
        var images = folder == null ? "/Berliner/img/BD/" : "/Berliner/img/BD/" + folder + "/";
        var path = HttpContext.Current.Server.MapPath("~" + images);
        var pathLength = path.Length;
        var folders = new List<object>();
        var directories = Directory.GetDirectories(path);
        if (directories.Any())
        {
            foreach (var directory in directories)
            {
                var name = directory.Remove(0, pathLength);
                var subFolder = folder + "/" + name;
                folders.Add(new { Name = TextInfo.ToTitleCase(name.ToLower()), Directories = GetFolders(subFolder), Link = subFolder.Remove(0, 1) });
            }
            return folders;
        }
        return null;
    }
}