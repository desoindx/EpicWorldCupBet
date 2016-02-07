using System;

namespace SignalR.SQL
{
    public enum Caches
    {
        All,
        CompetitionTeams,
        TeamsName,
        TeamsValue,
        UserUniversesRights,
        UserCompetitions,
        UniverseCompetitions,
        UserDefaultUniverse,
    }
    public static partial class Sql
    {
        public static void ClearCache(string cacheName)
        {
            ClearCache((Caches) Enum.Parse(typeof(Caches), cacheName));
        }

        public static void ClearCache(Caches cache)
        {
            switch (cache)
            {
                case Caches.All:
                    CompetitionTeams.Clear();
                    Teams.Clear();
                    TeamsValue.Clear();
                    UniverseCompetitions.Clear();
                    UserCompetitions.Clear();
                    UserUniversesRights.Clear();
                    CompetitionTeams.Clear();
                    Pricer.PricerHelper.Clear();
                    UserUniversesRights.Clear();
                    return;
                case Caches.TeamsName:
                    Teams.Clear();
                    return;
                case Caches.TeamsValue:
                    TeamsValue.Clear();
                    return;
                case Caches.UniverseCompetitions:
                    UniverseCompetitions.Clear();
                    return;
                case Caches.UserCompetitions:
                    UserCompetitions.Clear();
                    return;
                case Caches.UserUniversesRights:
                    UserUniversesRights.Clear();
                    return;
                case Caches.UserDefaultUniverse:
                    UserDefaultUniverse.Clear();
                    return;
            }
        }
    }
}