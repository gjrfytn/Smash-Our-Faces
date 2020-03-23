using Sof.Model;
using Sof.Model.Scenario;
using System.Collections.Generic;

namespace Sof.Auxiliary
{
    public class XmlScenario : IScenario
    {
        public interface IUnitRegistry
        {
            Map.IUnitTemplate this[string name] { get; }
        }

        private List<Faction> _Factions;

        public IEnumerable<Faction> Factions => _Factions;
        public IEnumerable<Occupation> Occupations => new[] { new Occupation(new Position(2, 2), _Factions[0]), new Occupation(new Position(7, 7), _Factions[1]) };
        public IEnumerable<Model.Scenario.Unit> Units { get; private set; }

        public XmlScenario(IUnitRegistry unitRegistry)
        {
            _Factions = new List<Faction> { new Faction("Faction 1", new PositiveInt(100)), new Faction("Faction 2", new PositiveInt(100)) };
            Units = new[] { new Model.Scenario.Unit(new Position(2, 2), unitRegistry["commander1"], _Factions[0], true), new Model.Scenario.Unit(new Position(7, 7), unitRegistry["commander1"], _Factions[1], true) };
        }
    }
}
