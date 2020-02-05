
namespace Sof.Model
{
    public class Tile
    {
        public Ground.Ground Ground { get; }
        public MapObject.MapObject Object { get; }
        public Unit Unit { get; private set; }

        public int MoveCost => Ground.MoveCost + (Object?.MoveCostModificator ?? 0);

        public bool Blocked => Unit != null;

        public Tile(Ground.Ground ground, MapObject.MapObject @object)
        {
            Ground = ground ?? throw new System.ArgumentNullException(nameof(ground));
            Object = @object;
        }

        public void PlaceUnit(Unit unit)
        {
            if (Unit != null)
                throw new System.InvalidOperationException("Tile cannot contain more than one unit.");

            Unit = unit;
        }

        public void RemoveUnit()
        {
            if (Unit == null)
                throw new System.InvalidOperationException("Tile has no unit.");

            Unit = null;
        }
    }
}
