using Sof.Auxiliary;

namespace Sof.Model.Pathfinding
{
    public interface IMap
    {
        PositiveInt Width { get; }
        PositiveInt Height { get; }

        bool IsBlocked(Position pos);
        PositiveInt GetMoveCost(Position pos);
    }
}
