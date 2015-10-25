using System;
using System.Collections.Generic;
using System.Linq;
using Datas.Entities;

namespace Pricer
{
    [Serializable]
    public class SimulationResult
    {
        protected bool Equals(SimulationResult other)
        {
            return _key == other._key;
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
            return (_key != null ? _key.GetHashCode() : 0);
        }

        private readonly Dictionary<Team, double> _result;
        private readonly string _key;

        public Dictionary<Team, double> Result { get { return _result; } }

        public SimulationResult(Dictionary<Team, double> result)
        {
            _result = result;

            _key = string.Empty;
            var orderedResult = result.OrderBy(x => x.Value).ThenBy(x => x.Key.Id);
            foreach (var res in orderedResult)
            {
                _key += res.Key.Id + "-" + res.Value + "/";
            }
        }

        public double GetResult(Team team)
        {
            double result;
            if (!_result.TryGetValue(team, out result))
            {
                return 0;
            }

            return result;
        }

        public double GetResult(Dictionary<Team, int> positions)
        {
            return _result.Sum(res => res.Value*positions[res.Key]);
        }
    }
}
