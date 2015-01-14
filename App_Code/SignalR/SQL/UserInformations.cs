using System.Collections.Generic;
using System.Linq;
using Datas.Entities;

namespace SignalR.SQL
{
    public static partial class Sql
    {
        public static int GetMoney(string user)
        {
            using (var context = new Entities())
            {
                var userMoney = context.Moneys.FirstOrDefault(x => x.User == user);
                if (userMoney != null)
                    return userMoney.Money1;
            }
            return 0;
        }

        public static void SetUserMoney(string user, int money)
        {
            using (var context = new Entities())
            {
                var userMoney = context.Moneys.FirstOrDefault(x => x.User == user);
                if (userMoney != null)
                {
                    userMoney.Money1 = money;
                    context.SaveChangesAsync();
                }
            }
        }

        public static List<Datas.Entities.Universe> GetUserUniverses(string user)
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
                return competitions.ToList();
            }
        }
    }
}