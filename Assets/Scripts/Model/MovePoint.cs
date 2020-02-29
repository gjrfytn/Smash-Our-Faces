namespace Sof.Model
{
    public class MovePoint
    {
        public Tile Tile { get; }
        public int Distance { get; }

        public MovePoint(Tile tile, int distance)
        {
            if (distance < 0)
                throw new System.ArgumentOutOfRangeException(nameof(distance), "Distance cannot be negative.");

            Tile = tile ?? throw new System.ArgumentNullException(nameof(tile));
            Distance = distance;
        }
    }
}
