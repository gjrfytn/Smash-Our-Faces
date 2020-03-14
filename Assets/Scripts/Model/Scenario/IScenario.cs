using System.Collections.Generic;

namespace Sof.Model.Scenario
{
    public interface IScenario
    {
        IEnumerable<Faction> Factions { get; }
        IEnumerable<Occupation> Occupations { get; }
        IEnumerable<Unit> Units { get; }
    }
}