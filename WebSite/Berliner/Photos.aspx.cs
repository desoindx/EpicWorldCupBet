using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class Photos : Page
{
    private string _query;
    private bool _search;

    protected void Page_Load(object sender, EventArgs e)
    {
        foreach (string query in Request.QueryString)
        {
            switch (query)
            {
                case "S":
                    _query = Request.QueryString[query].Replace("%20", " ").Replace("*", "");
                    _search = !string.IsNullOrEmpty(_query);
                    break;
                case "B":
                    _query = Request.QueryString[query].Replace("%20", " ");
                    _search = false;
                    break;
            }
        }
    }


    protected List<Photo> GetPhotos()
    {
        if (string.IsNullOrEmpty(_query))
        {
            return ShowLastPhotos();
        }

        if (_search)
        {
            return SearchPhotos();
        }

        var photos = new List<Photo>();
        var images = "/Berliner/img/BD/" + _query + "/";
        var thumbnails = "/Berliner/img/Thumbnails/" + _query + "/";
        var path = HttpContext.Current.Server.MapPath("~" + images);
        foreach (var photo in Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories))
        {
            string comment = string.Empty;
            var commentFile = photo.Replace(".JPG", ".txt");
            if (File.Exists(commentFile))
            {
                comment = File.ReadLines(commentFile).First();
            }
            var photoName = photo.Remove(0, photo.LastIndexOf("\\") + 1);
            photos.Add(new Photo { Path = (images + photo.Remove(0, path.Length)).Replace(" ", "%20"), Thumbnail = (thumbnails + photo.Remove(0, path.Length)).Replace(" ", "%20"), Title = photoName.Remove(photoName.Length - 4), Comment = comment });
        }

        return photos;
    }

    private List<Photo> ShowLastPhotos()
    {
        var photos = new List<Photo>();
        var images = "/Berliner/img/BD/";
        var thumbnails = "/Berliner/img/Thumbnails/";
        var path = HttpContext.Current.Server.MapPath("~" + images);
        foreach (var photo in Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories).OrderByDescending(File.GetCreationTime).Take(60))
        {
            string comment = string.Empty;
            var commentFile = photo.Replace(".JPG", ".txt");
            if (File.Exists(commentFile))
            {
                comment = File.ReadLines(commentFile).First();
            }
            var photoName = photo.Remove(0, photo.LastIndexOf("\\") + 1);
            photos.Add(new Photo { Path = (images + photo.Remove(0, path.Length)).Replace(" ", "%20"), Thumbnail = (thumbnails + photo.Remove(0, path.Length)).Replace(" ", "%20"), Title = photoName.Remove(photoName.Length - 4), Comment = comment });
        }

        return photos;
    }

    private List<Photo> SearchPhotos()
    {
        var paths = new HashSet<string>();
        var photos = new List<Photo>();
        var images = "/Berliner/img/BD/";
        var thumbnails = "/Berliner/img/Thumbnails/";
        var path = HttpContext.Current.Server.MapPath("~" + images);
        foreach (var directory in Directory.GetDirectories(path, string.Format("*{0}*", _query), SearchOption.AllDirectories))
        {
            foreach (var photo in Directory.GetFiles(directory, "*.jpg", SearchOption.AllDirectories))
            {
                if (photos.Count > 400)
                {
                    return photos;
                }

                if (paths.Contains(photo))
                {
                    continue;
                }
                paths.Add(photo);

                string comment = string.Empty;
                var commentFile = photo.Replace(".JPG", ".txt");
                if (File.Exists(commentFile))
                {
                    comment = File.ReadLines(commentFile).First();
                }
                var photoName = photo.Remove(0, photo.LastIndexOf("\\") + 1);
                photos.Add(new Photo
                {
                    Path = (images + photo.Remove(0, path.Length)).Replace(" ", "%20"),
                    Thumbnail = (thumbnails + photo.Remove(0, path.Length)).Replace(" ", "%20"),
                    Title = photoName.Remove(photoName.Length - 4),
                    Comment = comment
                });
            }
        }

        foreach (var photo in Directory.GetFiles(path, string.Format("*{0}*.jpg", _query), SearchOption.AllDirectories))
        {
            if (paths.Contains(photo))
            {
                continue;
            }

            if (photos.Count > 400)
            {
                return photos;
            }

            string comment = string.Empty;
            var commentFile = photo.Replace(".JPG", ".txt");
            if (File.Exists(commentFile))
            {
                comment = File.ReadLines(commentFile).First();
            }
            photos.Add(new Photo { Path = images + photo.Remove(0, path.Length), Thumbnail = thumbnails + photo.Remove(0, path.Length), Title = _query, Comment = comment });
        }

        return photos;
    }

    protected string GetTitle()
    {
        if (!string.IsNullOrEmpty(_query))
        {
            return SiteMaster.TextInfo.ToTitleCase(_query.Replace("/", " - ").ToLower());
        }
        return "News";
    }
}

public class Photo
{
    public string Path { get; set; }
    public string Thumbnail { get; set; }
    public string Title { get; set; }
    public string Comment { get; set; }
}