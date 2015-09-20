using System;
using System.Collections.Generic;
using System.Web.UI;
using Newtonsoft.Json;

namespace Datas.Entities
{
    public partial class Fountain
    {
        public void Update(Dictionary<string, Tuple<int, int>> infos)
        {
            var minDate = DateTime.MaxValue;
            var users = new List<string>();

            if (Antoine.HasValue)
            {
                var info = infos["Antoine"];
                infos["Antoine"] = new Tuple<int, int>(info.Item1, info.Item2 + 1);

                if (Antoine.Value < minDate)
                {
                    users.Clear();
                    users.Add("Antoine");
                    minDate = Antoine.Value;
                }
                else if (Antoine.Value == minDate)
                {
                    users.Add("Antoine");
                }
            }

            if (Camille.HasValue)
            {
                var info = infos["Camille"];
                infos["Camille"] = new Tuple<int, int>(info.Item1, info.Item2 + 1);
                if (Camille.Value < minDate)
                {
                    users.Clear();
                    users.Add("Camille");
                    minDate = Camille.Value;
                }
                else if (Camille.Value == minDate)
                {
                    users.Add("Camille");
                }
            }

            if (Loic.HasValue)
            {
                var info = infos["Loïc"];
                infos["Loïc"] = new Tuple<int, int>(info.Item1, info.Item2 + 1);
                if (Loic.Value < minDate)
                {
                    users.Clear();
                    users.Add("Loïc");
                    minDate = Loic.Value;
                }
                else if (Loic.Value == minDate)
                {
                    users.Add("Loïc");
                }
            }

            if (Xavier.HasValue)
            {
                var info = infos["Xavier"];
                infos["Xavier"] = new Tuple<int, int>(info.Item1, info.Item2 + 1);
                if (Xavier.Value < minDate)
                {
                    users.Clear();
                    users.Add("Xavier");
                    minDate = Xavier.Value;
                }
                else if (Xavier.Value == minDate)
                {
                    users.Add("Xavier");
                }
            }

            foreach (var user in users)
            {
                var info = infos[user];
                infos[user] = new Tuple<int, int>(info.Item1 + 1, info.Item2);
            }
        }
    }
}