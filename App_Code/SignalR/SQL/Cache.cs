﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using Datas.Entities;

namespace SignalR.SQL
{
    public static partial class Sql
    { 
        private static readonly ConcurrentDictionary<int, List<Team>> Teams =
            new ConcurrentDictionary<int, List<Team>>();

        private static readonly ConcurrentDictionary<string, bool> UserUniverses =
            new ConcurrentDictionary<string, bool>();

        private static readonly ConcurrentDictionary<string, int> UniverseCompetitions =
            new ConcurrentDictionary<string, int>();
    }
}