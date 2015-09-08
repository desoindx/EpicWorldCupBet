using System;
using System.Collections.Generic;
using System.Linq;
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
                var fountain = context.Fountains.ToList().FirstOrDefault(x =>x.Long == longitude && x.Lat == lattitude);
                if (fountain == null)
                {
                    fountain = new Fountain {Long = longitude, Lat = lattitude};
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
                foreach (var foutain in context.Fountains)
                {
                    fountains.Add(new FountainR(foutain));
                }

                Clients.Client(Context.ConnectionId).displayFountains(fountains);
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