﻿using System.Collections.Generic;
using System.Linq;
using Datas.Entities;

namespace Pricer
{
    public class SimulationResult
    {
        protected bool Equals(SimulationResult other)
        {
            return Key.SequenceEqual(other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((SimulationResult)obj);
        }

        public override int GetHashCode()
        {
            return (Key != null ? Key.GetHashCode() : 0);
        }

        public List<short> Key { get; set; }

        private readonly Dictionary<Team, double> _result;

        public Dictionary<Team, double> Result
        {
            get
            {
                return _result;
            }
        }

        public SimulationResult(Dictionary<Team, double> result)
        {
            _result = result;

            Key = new List<short>();
            var orderedResult = result.OrderBy(x => x.Value).ThenBy(x => x.Key.Id);
            foreach (var res in orderedResult)
            {
                Key.Add((short)res.Key.Id);
                Key.Add((short)res.Value);
            }
        }

        public double GetResult(Team team)
        {
            double result;
            if (!Result.TryGetValue(team, out result))
            {
                return 0;
            }

            return result;
        }

        public double GetResult(Dictionary<Team, int> positions)
        {
            return positions.Sum(position =>
            {
                double result;
                if (Result.TryGetValue(position.Key, out result) && result != 0)
                {
                    return result * position.Value;
                }
                return 0;
            });
        }
    }
}
