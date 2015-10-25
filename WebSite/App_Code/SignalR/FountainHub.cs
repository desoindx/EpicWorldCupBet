using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using Datas.Entities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalR
{
    [HubName("Fountain")]
    public class FountainHub : Hub
    {
        public static void InsertFountain(bool antoine, bool camille, bool loic, bool xavier, double longitude, double lattitude)
        {
            if (!antoine && !camille && !loic && !xavier)
                return;

            longitude = Math.Round(longitude, 8);
            lattitude = Math.Round(lattitude, 8);
            var now = TimeZoneInfo.ConvertTime(DateTime.Now,
                TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));
            using (var context = new Entities())
            {
                var fountain = context.Fountains.ToList().FirstOrDefault(x => x.Long == longitude && x.Lat == lattitude);
                if (fountain == null)
                {
                    fountain = new Fountain { Long = longitude, Lat = lattitude };
                    if (antoine)
                    {
                        fountain.Antoine = now;
                    }
                    if (camille)
                    {
                        fountain.Camille = now;
                    }
                    if (loic)
                    {
                        fountain.Loic = now;
                    }
                    if (xavier)
                    {
                        fountain.Xavier = now;
                    }
                    context.Fountains.Add(fountain);
                }
                else
                {
                    if (antoine && fountain.Antoine == null)
                    {
                        fountain.Antoine = now;
                    }
                    if (camille && fountain.Camille == null)
                    {
                        fountain.Camille = now;
                    }
                    if (loic && fountain.Loic == null)
                    {
                        fountain.Loic = now;
                    }
                    if (xavier && fountain.Xavier == null)
                    {
                        fountain.Xavier = now;
                    }
                }

                context.SaveChanges();
            }
        }

        public void getFountains()
        {
            using (var context = new Entities())
            {
                var fountains = new List<FountainR>();
                foreach (var fountain in context.Fountains)
                {
                    var fountainR = new FountainR(fountain);
                    fountains.Add(fountainR);
                }
                Clients.Client(Context.ConnectionId).displayFountains(fountains);

                var zurichImages = "/Zurich/Images";
                var mapPath = HttpContext.Current.Server.MapPath("~");
                var path = HttpContext.Current.Server.MapPath("~" + zurichImages);
                string[] lines = File.ReadAllLines(Path.Combine(path, "LastMail.txt"));
                var lastMailSend = Convert.ToDateTime(lines[0]);

                if (lastMailSend > DateTime.Now.AddDays(-1))
                {
                    return;
                }

                var newImages = new List<string>();
                var oldImages = new List<string>();

                foreach (var fountain in context.Fountains)
                {
                    var founds = fountain.Founds;
                    if (founds.Item2 > lastMailSend)
                    {
                        var fountainR = new FountainR(fountain);
                        foreach (var image in fountainR.Images)
                        {
                            var imagePath = mapPath + image;
                            if (File.GetCreationTime(imagePath) > lastMailSend)
                            {
                                if (founds.Item1 > lastMailSend)
                                {
                                    newImages.Add(imagePath);
                                }
                                else
                                {
                                    oldImages.Add(imagePath);
                                }
                            }
                        }
                    }
                }

                if (newImages.Count + oldImages.Count > 5)
                {
                    File.WriteAllText(Path.Combine(path, "LastMail.txt"), DateTime.Now.ToString());
                    var mail = new MailMessage("Fountain@epicsportexchange.com", "camille.chaperon@gmail.com,lolocic@hotmail.fr,theradis@gmail.com,xavier.desoindre@hotmail.fr")
//                    var mail = new MailMessage("Fountain@epicsportexchange.com", "elfuego95380@aol.com,xavier.desoindre@hotmail.fr")
{
    Subject = "New fountains have been found !",
    Body = "<html><body><p>Check this amazing fountain</p>",
    IsBodyHtml = true
};
                    if (newImages.Any())
                    {
                        mail.Body += "<p>Those fountains have just been found :</p>";
                        foreach (var newImage in newImages)
                        {
                            var fileName = newImage.Split('/').Last();
                            mail.Body += string.Format("<img src=\"cid:{0}\"/>", fileName);
                        }
                    }

                    if (oldImages.Any())
                    {
                        mail.Body += "<p>Those fountains have been found again:</p>";
                        foreach (var oldImage in oldImages)
                        {
                            var fileName = oldImage.Split('/').Last();
                            mail.Body += string.Format("<img src=\"cid:{0}\"/>", fileName);
                        }
                    }

                    mail.Body += "</body></html>";

                    AlternateView altView = AlternateView.CreateAlternateViewFromString(mail.Body, null, MediaTypeNames.Text.Html);
                    if (newImages.Any())
                    {
                        foreach (var newImage in newImages)
                        {
                            var fileName = newImage.Split('/').Last();

                            LinkedResource ressource = new LinkedResource(newImage, MediaTypeNames.Image.Jpeg);
                            ressource.ContentId = fileName;
                            altView.LinkedResources.Add(ressource);
                        }
                    }

                    if (oldImages.Any())
                    {
                        foreach (var oldImage in oldImages)
                        {
                            var fileName = oldImage.Split('/').Last();

                            LinkedResource ressource = new LinkedResource(oldImage, MediaTypeNames.Image.Jpeg);
                            ressource.ContentId = fileName;
                            altView.LinkedResources.Add(ressource);
                        }
                    }

                    mail.AlternateViews.Add(altView);
                    var client = new SmtpClient("mail.epicsportexchange.com")
                    {
                        Credentials = new System.Net.NetworkCredential("Fountain@epicsportexchange.com", "FountainFountain")
                    };
                    client.Send(mail);
                }
            }
        }

        private double Distance(double longitude1, double lattitude1, double longitude2, double lattitude2)
        {
            double rlat1 = Math.PI * lattitude1 / 180;
            double rlat2 = Math.PI * lattitude2 / 180;
            double theta = longitude1 - longitude2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return dist * 1609.344;
        }
    }
}