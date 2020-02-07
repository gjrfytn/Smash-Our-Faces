namespace Sof.Model
{
    public class TileDefinition
    {
        public GroundType Ground { get; }
        public MapObjectType MapObject { get; }

        public TileDefinition(GroundType ground, MapObjectType mapObject)
        {
            Ground = ground;
            MapObject = mapObject;
        }
    }
}
