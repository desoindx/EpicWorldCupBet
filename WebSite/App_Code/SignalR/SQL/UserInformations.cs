using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Datas.Entities;

namespace SignalR.SQL
{
    public static partial class Sql
    {
        private static readonly ConcurrentDictionary<string, Universe> UserDefaultUniverse =
            new ConcurrentDictionary<string, Universe>();

        public static Universe GetUserSelectedUniverse(string user)
        {
            Universe universe;
            if (UserDefaultUniverse.TryGetValue(user, out universe))
            {
                return universe;
            }

            var universes = GetUserUniverses(user);
            if (universes.Count > 0)
            {
                universe = universes[0];
                UserDefaultUniverse[user] = universe;
                return universe;
            }

            return null;
        }

        public static void SetUserUniverse(string user, Universe universe)
        {
            if (IsUserAuthorizedOn(user, universe.Id))
                UserDefaultUniverse[user] = universe;
        }

        public static int GetMoney(string user, int universeCompetitionId)
        {
            if (universeCompetitionId < 0)
                return 0;

            using (var context = new Entities())
            {
                var userMoney =
                    context.Moneys.FirstOrDefault(
                        x => x.User == user && x.IdUniverseCompetition == universeCompetitionId);

                if (userMoney != null)
                    return userMoney.Money1;
            }
            return 0;
        }

        public static List<Universe> GetUserUniverses(string user)
        {
            using (var context = new Entities())
            {
                var universes = from a in context.UniverseAvailables.Where(x => x.UserName == user)
                                from u in context.Universes.Where(x => x.Id == a.IdUniverse)
                                select u;
                
                return universes.ToList();
            }
        }

        public static List<Competition> GetUniverseCompetitions(string universe)
        {
            using (var context = new Entities())
            {
                var competitions = from u in context.Universes.Where(x => x.Name == universe)
                                   from uc in context.UniverseCompetitions.Where(x => x.IdUniverse == u.Id)
                                   from c in context.Competitions.Where(x => x.Id == uc.IdCompetition)
                                   select c;
                return competitions.Where(x => x.EndDate > DateTime.Now && x.StartDate < DateTime.Now).ToList();
            }
        }

        public static List<int> GetUserAvailableCompetitions(string user)
        {
            List<int> avalaibleCompetitions;
            if (!UserCompetitions.TryGetValue(user, out avalaibleCompetitions))
            {
                using (var context = new Entities())
                {
                    var competitions = from u in context.UniverseAvailables.Where(x => x.UserName == user)
                                       from uc in context.UniverseCompetitions.Where(x => x.IdUniverse == u.IdUniverse)
                                       select uc.Id;
                    avalaibleCompetitions = competitions.ToList();
                }
                UserCompetitions[user] = avalaibleCompetitions;
            }
            return avalaibleCompetitions;
        }

        public static bool IsUserAuthorizedOn(string user, int universeId)
        {
            using (var context = new Entities())
                return IsUserAuthorizedOn(user, universeId, context);
        }

        private static bool IsUserAuthorizedOn(string user, int universeId, Entities context)
        {
            bool isAuthorized;
            var key = string.Format("user:{0}-universe:{1}", user, universeId);
            if (!UserUniversesRights.TryGetValue(key, out isAuthorized))
            {
                isAuthorized = context.UniverseAvailables.Any(x => x.UserName == user && x.IdUniverse == universeId);
                UserUniversesRights[key] = isAuthorized;
            }
            return isAuthorized;
        }
    }
}