namespace Datas
{
    using System.Collections.Generic;

    public class Round
    {
        private IEnumerable<string> _teams;

        public void InitTeams(IEnumerable<string> teams)
        {
            _teams = teams;
        }
    }
}
