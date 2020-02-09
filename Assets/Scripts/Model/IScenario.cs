using System.Collections.Generic;

namespace Sof.Model
{
    public interface IScenario
    {

        IEnumerable<Faction> Factions { get; }
        IEnumerable<Occupation> Occupations { get; }
    }
}