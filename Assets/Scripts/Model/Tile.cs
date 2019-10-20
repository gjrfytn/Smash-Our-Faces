
namespace Sof.Model
{
    public class Tile
    {
        public Ground.Ground Ground { get; }
        public MapObject.MapObject Object { get; }
        public Unit Unit { get; set; }

        public int MoveCost => Ground.MoveCost + Object.MoveCostModificator;

        public bool Blocked => Unit != null;

        public Tile(Ground.Ground ground, MapObject.MapObject @object)
        {
            Ground = ground;
            Object = @object;
        }
    }
}
