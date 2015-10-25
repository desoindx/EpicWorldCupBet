using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Datas.Entities;

namespace Pricer
{
    public class SimulationResult
    {
        protected bool Equals(SimulationResult other)
        {
            return Key == other.Key;
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

        public string Key { get; set; }

        [XmlIgnore]
        private SerializableResult[] _serializableResults;

        public SerializableResult[] SerializableResults
        {
            get
            {
                _serializableResults = _result.Select(kv => new SerializableResult { Id = kv.Key, Value = kv.Value })
                    .ToArray();
                return _serializableResults;
            }
            set { _serializableResults = value; }
        }


        [XmlIgnore]
        private Dictionary<Team, double> _result;

        [XmlIgnore]
        public Dictionary<Team, double> Result
        {
            get
            {
                if (_result == null)
                {
                    _result = _serializableResults.ToDictionary(i => i.Id, i => i.Value);
                }
                return _result;
            }
        }

        public SimulationResult()
        {
        }

        public SimulationResult(Dictionary<Team, double> result)
        {
            _result = result;

            Key = string.Empty;
            var orderedResult = result.OrderBy(x => x.Value).ThenBy(x => x.Key.Id);
            foreach (var res in orderedResult)
            {
                Key += res.Key.Id + "-" + res.Value + "/";
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
            return Result.Sum(res => res.Value * positions[res.Key]);
        }

        public class SerializableResult
        {
            public Team Id;
            public double Value;
        }
    }
}
