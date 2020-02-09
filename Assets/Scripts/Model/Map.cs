using Sof.Model.Ground;
using Sof.Model.MapObject;
using System.Collections.Generic;

namespace Sof.Model
{
    public class Map : Pathfinding.IMap
    {
        private readonly Pathfinding.Pathfinder _Pathfinder;
        private readonly ITime _Time;

        private readonly Tile[,] _Tiles;

        public int Width => _Tiles.GetLength(0);
        public int Height => _Tiles.GetLength(1);

        public Tile this[Position pos] => _Tiles[pos.X, pos.Y];

        public event System.Action<Unit> UnitMoved;

        public Map(IMapFile mapFile, IScenario scenario, ITime time)
        {
            _Pathfinder = new Pathfinding.Pathfinder(this);
            _Time = time;

            _Tiles = ConstructTiles(mapFile.Load());

            foreach (var occupation in scenario.Occupations)
                occupation.Apply((Castle)this[occupation.Position].Object);
        }

        public void Spawn(Unit unit, Position pos) => this[pos].PlaceUnit(unit);
        public void Spawn(Unit unit, Castle castle) => Spawn(unit, GetMapObjectPos(castle));

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

        public Position GetMapObjectPos(MapObject.MapObject mapObject) => TryGetMapObjectPos(mapObject) ?? throw new System.ArgumentException("Map does not contain specified object.", nameof(mapObject));

        public Position TryGetMapObjectPos(MapObject.MapObject mapObject)
        {
            for (var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                    if (_Tiles[x, y].Object == mapObject)
                        return new Position(x, y);

            return null;
        }

        public bool IsBlocked(Position pos) => this[pos].Blocked;
        public int GetMoveCost(Position pos) => this[pos].MoveCost;

        private Tile[,] ConstructTiles(TileDefinition[,] definitions)
        {
            var tiles = new Tile[definitions.GetLength(0), definitions.GetLength(1)];

            for (var y = 0; y < definitions.GetLength(1); ++y)
                for (var x = 0; x < definitions.GetLength(0); ++x)
                    tiles[x, y] = new Tile(CreateGround(definitions[x, y].Ground), CreateObject(definitions[x, y].MapObject));

            return tiles;
        }

        private Ground.Ground CreateGround(GroundType type)
        {
            switch (type)
            {
                case GroundType.Water:
                    return new Water();
                case GroundType.Grass:
                    return new Grass();
                case GroundType.Mountain:
                    return new Mountain();
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(type));
            }
        }

        private MapObject.MapObject CreateObject(MapObjectType type)
        {
            switch (type)
            {
                case MapObjectType.None:
                    return null;
                case MapObjectType.Castle:
                    return new Castle(_Time, 10); //TODO
                case MapObjectType.House:
                    return new House();
                case MapObjectType.Bridge:
                    return new Bridge();
                case MapObjectType.Road:
                    return new Road();
                case MapObjectType.Forest:
                    return new Forest();
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}