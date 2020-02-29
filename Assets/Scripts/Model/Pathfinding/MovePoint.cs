namespace Sof.Model.Pathfinding
{
    public class MovePoint
    {
        public Position Pos { get; }
        public int Distance { get; }

        public MovePoint(Position pos, int distance)
        {
            if (distance < 0)
                throw new System.ArgumentOutOfRangeException(nameof(distance), "Distance cannot be negative.");

            Pos = pos ?? throw new System.ArgumentNullException(nameof(pos));
            Distance = distance;
        }
    }
}
