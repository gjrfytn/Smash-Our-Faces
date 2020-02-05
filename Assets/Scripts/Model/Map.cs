using System.Collections.Generic;

namespace Sof.Model
{
    public class Map
    {
        private readonly Pathfinder _Pathfinder;
        private readonly Tile[,] _Tiles;

        public int Width => _Tiles.GetLength(0);
        public int Height => _Tiles.GetLength(1);

        public Tile this[Position pos] => _Tiles[pos.X, pos.Y];

        public event System.Action<Unit> UnitMoved;

        public Map(IMapFile mapFile)
        {
            _Pathfinder = new Pathfinder(this);

            _Tiles = mapFile.Load();
        }

        public void Spawn(Unit unit, Position pos) => this[pos].PlaceUnit(unit);
        public void Remove(Unit unit) => this[GetUnitPos(unit)].RemoveUnit();

        public IEnumerable<Position> GetClosestPath(Unit unit, Position pos) => _Pathfinder.GetClosestPath(GetUnitPos(unit), pos);

        public void MoveUnit(Unit unit, Position pos)
        {
            Remove(unit);
            Spawn(unit, pos);

            UnitMoved?.Invoke(unit);
        }

        public IEnumerable<MovePoint> GetMoveRange(Unit unit) => _Pathfinder.GetMoveRange(GetUnitPos(unit), unit.MovePoints);

        public Position GetUnitPos(Unit unit) => TryGetUnitPos(unit) ?? throw new System.ArgumentException("Map does not contain specified unit.", nameof(unit));

        public Position TryGetUnitPos(Unit unit)
        {
            for (var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                    if (_Tiles[x, y].Unit == unit)
                        return new Position(x, y);

            return null;
        }
    }
}