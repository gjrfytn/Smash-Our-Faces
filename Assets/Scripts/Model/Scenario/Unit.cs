using System;
using Gjrfytn.Dim;

namespace Sof.Model.Scenario
{
    public class Unit
    {
        private readonly Map.IUnitTemplate _Template;
        private readonly Faction _Faction;
        private readonly bool _Critical;

        public Position Position { get; }

        public Unit(Position position, Map.IUnitTemplate template, Faction faction, bool critical)
        {
            Position = position ?? throw new ArgumentNullException(nameof(position));
            _Template = template ?? throw new ArgumentNullException(nameof(template));
            _Faction = faction ?? throw new ArgumentNullException(nameof(faction));
            _Critical = critical;
        }

        public void Spawn(Map map, Tile tile)
        {
            if (map is null)
                throw new ArgumentNullException(nameof(map));
            if (tile is null)
                throw new ArgumentNullException(nameof(tile));

            map.Spawn(_Template, tile, _Faction, _Critical);
        }
    }
}
