using Gjrfytn.Dim;

namespace Sof.Model
{
    public class MovePoint
    {
        public Tile Tile { get; }
        public PositiveInt Distance { get; }

        public MovePoint(Tile tile, PositiveInt distance)
        {
            Tile = tile ?? throw new System.ArgumentNullException(nameof(tile));
            Distance = distance;
        }
    }
}
