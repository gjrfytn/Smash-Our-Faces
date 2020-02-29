using Sof.Auxiliary;

namespace Sof.Model.Pathfinding
{
    public class MovePoint
    {
        public Position Pos { get; }
        public PositiveInt Distance { get; }

        public MovePoint(Position pos, PositiveInt distance)
        {
            Pos = pos ?? throw new System.ArgumentNullException(nameof(pos));
            Distance = distance;
        }
    }
}
