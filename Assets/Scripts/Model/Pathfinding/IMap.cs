namespace Sof.Model.Pathfinding
{
    public interface IMap
    {
        int Width { get; }
        int Height { get; }

        bool IsBlocked(Position pos);
        int GetMoveCost(Position pos);
    }
}
