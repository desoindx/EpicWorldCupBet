using System.IO;
using System.Web;
using Newtonsoft.Json;

public static class JavaScriptSerializer
{
    public static IHtmlString SerializeObject(object value)
    {
        using (var stringWriter = new StringWriter())
        using (var jsonWriter = new JsonTextWriter(stringWriter))
        {
            var serializer = new JsonSerializer();

            // We don't want quotes around object names
            jsonWriter.QuoteName = false;
            serializer.Serialize(jsonWriter, value);

            return new HtmlString(stringWriter.ToString());
        }
    }
}