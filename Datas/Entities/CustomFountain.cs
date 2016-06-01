using System;
using System.Collections.Generic;

namespace Datas.Entities
{
    public partial class Fountain
    {
        public void Update(Dictionary<string, Tuple<int, int, int>> infos, DateTime referenceDate)
        {
            UpdateNotInZurich(infos, referenceDate);

            if (!InZurich)
            {
                return;
            }

            var minDate = DateTime.MaxValue;
            var users = new List<string>();

            if (Antoine.HasValue)
            {
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

                var info = infos["Antoine"];
                if (Antoine.Value > referenceDate)
                    infos["Antoine"] = new Tuple<int, int, int>(info.Item1, info.Item2 + 1, info.Item3);
                else
                    users.Remove("Antoine");
            }

            if (Camille.HasValue)
            {
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

                var info = infos["Camille"];
                if (Camille.Value > referenceDate)
                    infos["Camille"] = new Tuple<int, int, int>(info.Item1, info.Item2 + 1, info.Item3);
                else
                    users.Remove("Camille");
            }

            if (Loic.HasValue)
            {
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

                var info = infos["Loïc"];
                if (Loic.Value > referenceDate)
                    infos["Loïc"] = new Tuple<int, int, int>(info.Item1, info.Item2 + 1, info.Item3);
                else
                    users.Remove("Loïc");
            }

            if (Rafaela.HasValue)
            {
                if (Rafaela.Value < minDate)
                {
                    users.Clear();
                    users.Add("Rafaela");
                    minDate = Rafaela.Value;
                }
                else if (Rafaela.Value == minDate)
                {
                    users.Add("Rafaela");
                }

                var info = infos["Rafaela"];
                if (Rafaela.Value > referenceDate)
                    infos["Rafaela"] = new Tuple<int, int, int>(info.Item1, info.Item2 + 1, info.Item3);
                else
                    users.Remove("Rafaela");
            }

            if (Xavier.HasValue)
            {
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

                var info = infos["Xavier"];
                if (Xavier.Value > referenceDate)
                    infos["Xavier"] = new Tuple<int, int, int>(info.Item1, info.Item2 + 1, info.Item3);
                else
                    users.Remove("Xavier");
            }

            foreach (var user in users)
            {
                var info = infos[user];
                infos[user] = new Tuple<int, int, int>(info.Item1 + 1, info.Item2, info.Item3);
            }
        }

        private void UpdateNotInZurich(Dictionary<string, Tuple<int, int, int>> infos, DateTime referenceDate)
        {
            if (Antoine.HasValue && Antoine.Value > referenceDate)
            {
                var info = infos["Antoine"];
                infos["Antoine"] = new Tuple<int, int, int>(info.Item1, info.Item2, info.Item3 + 1);
            }
            if (Camille.HasValue && Camille.Value > referenceDate)
            {
                var info = infos["Camille"];
                infos["Camille"] = new Tuple<int, int, int>(info.Item1, info.Item2, info.Item3 + 1);
            }
            if (Loic.HasValue && Loic.Value > referenceDate)
            {
                var info = infos["Loïc"];
                infos["Loïc"] = new Tuple<int, int, int>(info.Item1, info.Item2, info.Item3 + 1);
            }
            if (Rafaela.HasValue && Rafaela.Value > referenceDate)
            {
                var info = infos["Rafaela"];
                infos["Rafaela"] = new Tuple<int, int, int>(info.Item1, info.Item2, info.Item3 + 1);
            }
            if (Xavier.HasValue && Xavier.Value > referenceDate)
            {
                var info = infos["Xavier"];
                infos["Xavier"] = new Tuple<int, int, int>(info.Item1, info.Item2, info.Item3 + 1);
            }
        }

        public Tuple<DateTime, DateTime> Founds
        {
            get
            {
                var maxDate = Antoine ?? DateTime.MinValue;
                var minDate = Antoine ?? DateTime.MaxValue;
                if (Loic.HasValue)
                {
                    if (minDate > Loic.Value)
                        minDate = Loic.Value;
                    if (maxDate < Loic.Value)
                        maxDate = Loic.Value;
                }
                if (Camille.HasValue)
                {
                    if (minDate > Camille.Value)
                        minDate = Camille.Value;
                    if (maxDate < Camille.Value)
                        maxDate = Camille.Value;
                }
                if (Rafaela.HasValue)
                {
                    if (minDate > Rafaela.Value)
                        minDate = Rafaela.Value;
                    if (maxDate < Rafaela.Value)
                        maxDate = Rafaela.Value;
                }
                if (Xavier.HasValue)
                {
                    if (minDate > Xavier.Value)
                        minDate = Xavier.Value;
                    if (maxDate < Xavier.Value)
                        maxDate = Xavier.Value;
                }

                return new Tuple<DateTime, DateTime>(minDate, maxDate);
            }
        }
    }
}