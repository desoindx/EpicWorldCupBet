using System.Collections.Generic;
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

        public short[] Key { get; set; }

        private readonly Dictionary<int, int> _result;

        public Dictionary<int, int> Result
        {
            get
            {
                return _result;
            }
        }

        public SimulationResult(Dictionary<int, int> result, ushort value = 1)
        {
            Value = value;
            _result = result;
            var orderedResult = result.OrderBy(x => x.Value).ThenBy(x => x.Key).ToList();
            Key = new short[orderedResult.Count];
            int i = 0;
            foreach (var res in orderedResult)
            {
                Key[i++] = (short)res.Key;
            }
        }

        public int GetResult(Team team)
        {
            int result;
            if (!Result.TryGetValue(team.Id, out result))
            {
                return 0;
            }

            return result;
        }

        public ushort Value { get; set; }

        public int GetResult(Dictionary<Team, int> positions)
        {
            return positions.Sum(position =>
            {
                int result;
                if (Result.TryGetValue(position.Key.Id, out result) && result != 0)
                {
                    return result * position.Value;
                }
                return 0;
            });
        }
    }
}
