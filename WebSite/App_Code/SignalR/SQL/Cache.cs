﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using Datas.Entities;

namespace SignalR.SQL
{
    public static partial class Sql
    { 
        private static readonly ConcurrentDictionary<int, List<Team>> CompetitionTeams =
            new ConcurrentDictionary<int, List<Team>>();

        private static readonly ConcurrentDictionary<int, string> TeamsName =
            new ConcurrentDictionary<int, string>();

        private static readonly ConcurrentDictionary<string, Team> TeamsValue =
            new ConcurrentDictionary<string, Team>();

        private static readonly ConcurrentDictionary<string, bool> UserUniversesRights =
            new ConcurrentDictionary<string, bool>();

        private static readonly ConcurrentDictionary<string, List<int>> UserCompetitions =
            new ConcurrentDictionary<string, List<int>>();

        private static readonly ConcurrentDictionary<string, int> UniverseCompetitions =
            new ConcurrentDictionary<string, int>();
    }
}