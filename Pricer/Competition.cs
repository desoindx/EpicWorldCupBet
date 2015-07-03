using Datas.Entities;

namespace Datas
{
    using System.Collections.Generic;
    using System.Linq;

    public class BasicCompetition
    {
        private string _name;

        private readonly List<Team> _teams;
        private readonly Competition _competition;
        private readonly Dictionary<int, List<string>> _rounds;
        private readonly Dictionary<string, List<string>> _prizes;
        private readonly Dictionary<int, List<List<string>>> _results;

        public BasicCompetition(string name)
        {
            _name = name;
           /* using (var context = new Entities.Entities())
            {
                _competition = context.Competitions.First(x => x.Name == name);
                _teams = context.Teams.Where(x => x.IdCompetition == _competition.Id).ToList();
            }*/

            _rounds = new Dictionary<int, List<string>>();
            _prizes = new Dictionary<string, List<string>>();
            _results = new Dictionary<int, List<List<string>>>();
        }

        public void AddRound(int index, IEnumerable<string> teams)
        {
            _rounds.Add(index, teams.ToList());
        }

        public void AddPrize(string prizeName, IEnumerable<string> teams)
        {
            _prizes.Add(prizeName, teams.ToList());
        }

        public void AddResultForRound(int round, IEnumerable<string> result)
        {
            List<List<string>> results;
            if (!_results.TryGetValue(round, out results))
            {
                results = new List<List<string>>();
                _results.Add(round, results);
            }

            results.Add(result.ToList());
        }
    }
}
