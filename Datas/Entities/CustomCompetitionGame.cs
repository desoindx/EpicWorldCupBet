﻿using System;
using System.Linq;

namespace Datas.Entities
{
    public partial class CompetitionGame
    {
        private Team _team;

        public bool HomeAndAway { get { return RoundKey.EndsWith("*"); } }

        public string Key
        {
            get
            {
                return RoundKey.TrimEnd('*');
            }
        }

        public Team Team
        {
            get
            {
                if (_team == null)
                {
                    using (var context = new Entities())
                    {
                        _team = context.Teams.FirstOrDefault(x => x.Id == TeamId && x.IdCompetition == CompetitionId);
                    }
                }
                return _team;
            }
        }
    }
}
