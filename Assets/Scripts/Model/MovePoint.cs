namespace Sof.Model
{
    public class MovePoint
    {
        public Tile Tile { get; }
        public int Distance { get; }

        public MovePoint(Tile tile, int distance)
        {
            Tile = tile;
            Distance = distance;
        }
    }
}
