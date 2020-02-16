using Sof.Model.MapObject;

namespace Sof.Model
{
    public class Occupation
    {
        private readonly Faction _Faction;

        public Position Position { get; }

        public Occupation(Position position, Faction faction)
        {
            Position = position;
            _Faction = faction;
        }

        public void Apply(Property property)
        {
            if (property is null)
                throw new System.ArgumentNullException(nameof(property));

            property.Owner = _Faction;
        }
    }
}
