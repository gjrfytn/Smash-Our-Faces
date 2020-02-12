using Sof.Model.Ground;
using Sof.Model.MapObject;
using System.Collections.Generic;
using System.Linq;

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

        public void Spawn(Unit unit, Tile tile) => tile.PlaceUnit(unit); //TODO
        public void Spawn(Unit unit, Castle castle) => Spawn(unit, GetMapObjectTile(castle));

        public void Remove(Unit unit) => this[GetUnitPos(unit)].RemoveUnit();

        public IEnumerable<Tile> GetClosestPath(Unit unit, Tile tile) => _Pathfinder.GetClosestPath(GetUnitPos(unit), GetTilePos(tile)).Select(p => this[p]);

        public void MoveUnit(Unit unit, Tile tile)
        {
            Remove(unit);
            Spawn(unit, tile);

            UnitMoved?.Invoke(unit);
        }

        [System.Obsolete("Use MoveUnit.")]
        public void MoveUnitOld(Unit unit, Position pos)
        {
            Remove(unit);
            Spawn(unit, this[pos]);

            UnitMoved?.Invoke(unit);
        }

        public IEnumerable<MovePoint> GetMoveRange(Unit unit) => _Pathfinder.GetMoveRange(GetUnitPos(unit), unit.MovePoints);

        [System.Obsolete("TODO")] //TODO
        public Position GetUnitPos(Unit unit) => TryGetUnitPos(unit) ?? throw new System.ArgumentException("Map does not contain specified unit.", nameof(unit));

        public Position TryGetUnitPos(Unit unit)
        {
            for (var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                    if (_Tiles[x, y].Unit == unit)
                        return new Position(x, y);

            return null;
        }

        public Tile GetUnitTile(Unit unit) => TryGetUnitTile(unit) ?? throw new System.ArgumentException("Map does not contain specified unit.", nameof(unit));

        public Tile TryGetUnitTile(Unit unit)
        {
            for (var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                    if (_Tiles[x, y].Unit == unit)
                        return _Tiles[x, y];

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

        private Tile GetMapObjectTile(MapObject.MapObject mapObject) => TryGetMapObjectTile(mapObject) ?? throw new System.ArgumentException("Map does not contain specified object.", nameof(mapObject));

        private Tile TryGetMapObjectTile(MapObject.MapObject mapObject)
        {
            for (var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                    if (_Tiles[x, y].Object == mapObject)
                        return _Tiles[x, y];

            return null;
        }

        public bool IsBlocked(Position pos) => this[pos].Blocked;
        public int GetMoveCost(Position pos) => this[pos].MoveCost;

        private Position GetTilePos(Tile tile) => TryGetTilePos(tile) ?? throw new System.ArgumentException("Map does not contain specified tile.", nameof(tile));

        private Position TryGetTilePos(Tile tile)
        {
            for (var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                    if (_Tiles[x, y] == tile)
                        return new Position(x, y);

            return null;
        }

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