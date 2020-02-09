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

        public void Apply(Castle castle)
        {
            if (castle is null)
                throw new System.ArgumentNullException(nameof(castle));

            castle.Faction = _Faction;
        }
    }
}
