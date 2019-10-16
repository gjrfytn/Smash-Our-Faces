namespace Sof.Model
{
    public class MovePoint
    {
        public Position Pos { get; }
        public int Distance { get; }

        public MovePoint(Position pos, int distance)
        {
            Pos = pos;
            Distance = distance;
        }
    }
}
