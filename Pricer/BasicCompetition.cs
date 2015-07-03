using System.Collections.Generic;
using System.Linq;
using Datas.Entities;

namespace Pricer
{
    public class BasicCompetition
    {
        private string _name;
        private readonly Competition _competition;
        private List<Team> _teams;

        public BasicCompetition(string name)
        {
            _name = name;
            using (var context = new Entities())
            {
                _competition = context.Competitions.First(x => x.Name == name);
                _teams = context.Teams.Where(x => x.IdCompetition == _competition.Id).ToList();
            }
        }
    }
}
