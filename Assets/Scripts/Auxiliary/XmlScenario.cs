using Sof.Model;
using Sof.Model.Scenario;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Sof.Auxiliary
{
    public class XmlScenario : IScenario
    {
        public interface IUnitRegistry
        {
            Map.IUnitTemplate this[string name] { get; }
        }

        private List<Faction> _Factions = new List<Faction>();
        private List<Occupation> _Occupations = new List<Occupation>();
        private List<Model.Scenario.Unit> _Units = new List<Model.Scenario.Unit>();

        public IEnumerable<Faction> Factions => _Factions;
        public IEnumerable<Occupation> Occupations => _Occupations;
        public IEnumerable<Model.Scenario.Unit> Units => _Units;

        public XmlScenario(string xml, IUnitRegistry unitRegistry)
        {
            if (xml == null)
                throw new System.ArgumentNullException(nameof(xml));
            if (unitRegistry == null)
                throw new System.ArgumentNullException(nameof(unitRegistry));

            var doc = XDocument.Parse(xml);

            foreach (var factionElement in doc.Root.Element("Factions").Elements())
            {
                var faction = new Faction(factionElement.Element("Name").Value, new PositiveInt(int.Parse(factionElement.Element("Gold").Value)));
                _Factions.Add(faction);

                var occupations = factionElement.Element("Occupations");
                if (occupations != null)
                    foreach (var occupation in occupations.Elements())
                        _Occupations.Add(new Occupation(new Position(int.Parse(occupation.Attribute("x").Value), int.Parse(occupation.Attribute("y").Value)), faction));

                var units = factionElement.Element("Units");
                if (units != null)
                    foreach (var unit in units.Elements())
                        _Units.Add(new Model.Scenario.Unit(new Position(int.Parse(unit.Attribute("x").Value), int.Parse(unit.Attribute("y").Value)),
                                                                        unitRegistry[unit.Element("Name").Value],
                                                                        faction,
                                                                        unit.Element("Critical") != null));
            }
        }
    }
}
