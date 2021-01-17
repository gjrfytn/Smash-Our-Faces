using Gjrfytn.Dim;

namespace Sof.Model.Scenario
{
    public class Occupation
    {
        private readonly Faction _Faction;

        public Position Position { get; }

        public Occupation(Position position, Faction faction)
        {
            Position = position ?? throw new System.ArgumentNullException(nameof(position));
            _Faction = faction ?? throw new System.ArgumentNullException(nameof(faction));
        }

        public void Apply(MapObject.Property.Property property)
        {
            if (property == null)
                throw new System.ArgumentNullException(nameof(property));

            property.Owner = _Faction;
        }
    }
}
