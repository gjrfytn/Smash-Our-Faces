using Sof.Auxiliary;
using Sof.Model.Ground;
using Sof.Model.MapObject;
using Sof.Model.MapObject.Property;
using System.Collections.Generic;
using System.Linq;

namespace Sof.Model
{
    public class Map : Pathfinding.IMap, IMap
    {
        private readonly Pathfinding.Pathfinder _Pathfinder;
        private readonly ITime _Time;

        private readonly Tile[,] _Tiles;

        public PositiveInt Width => new PositiveInt(_Tiles.GetLength(0));
        public PositiveInt Height => new PositiveInt(_Tiles.GetLength(1));

        public Tile this[Position pos] => _Tiles[pos.X, pos.Y];

        public Map(IMapFile mapFile, IScenario scenario, ITime time)
        {
            if (mapFile == null)
                throw new System.ArgumentNullException(nameof(mapFile));
            if (scenario == null)
                throw new System.ArgumentNullException(nameof(scenario));

            _Pathfinder = new Pathfinding.Pathfinder(this);
            _Time = time ?? throw new System.ArgumentNullException(nameof(time));

            _Tiles = ConstructTiles(mapFile.Load());

            foreach (var occupation in scenario.Occupations)
                occupation.Apply((Castle)this[occupation.Position].Object);
        }

        public void Spawn(Unit unit, Tile tile)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));
            if (tile == null)
                throw new System.ArgumentNullException(nameof(tile));

            tile.PlaceUnit(unit); //TODO
        }

        public void Spawn(Unit unit, Castle castle)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));
            if (castle == null)
                throw new System.ArgumentNullException(nameof(castle));

            Spawn(unit, GetMapObjectTile(castle));
        }

        public void Remove(Unit unit)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            this[GetUnitPos(unit)].RemoveUnit();
        }

        public PositiveInt Distance(Unit unit1, Unit unit2)
        {
            if (unit1 == null)
                throw new System.ArgumentNullException(nameof(unit1));
            if (unit2 == null)
                throw new System.ArgumentNullException(nameof(unit2));

            return Distance(GetUnitTile(unit1), GetUnitTile(unit2));
        }

        public IEnumerable<Tile> GetClosestPath(Unit unit, Tile tile)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));
            if (tile == null)
                throw new System.ArgumentNullException(nameof(tile));

            return _Pathfinder.GetClosestPath(GetUnitPos(unit), GetTilePos(tile)).Select(p => this[p]);
        }

        public void MoveUnit(Unit unit, Tile tile)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));
            if (tile == null)
                throw new System.ArgumentNullException(nameof(tile));

            Remove(unit);
            Spawn(unit, tile);
        }

        public IEnumerable<MovePoint> GetMoveRange(Unit unit)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            return _Pathfinder.GetMoveRange(GetUnitPos(unit), unit.MovePoints).Select(p => new MovePoint(this[p.Pos], p.Distance));
        }

        public IEnumerable<Tile> GetTilesInRange(Unit unit, PositiveInt range)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            var unitTile = GetUnitTile(unit);

            var tiles = new List<Tile>();
            for (var y = 0; y < Height.Value; ++y)
                for (var x = 0; x < Width.Value; ++x)
                    tiles.Add(_Tiles[x, y]);

            return tiles.Where(t => Distance(unitTile, t).Value <= range.Value);
        }

        public Tile GetUnitTile(Unit unit)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            return TryGetUnitTile(unit) ?? throw new System.ArgumentException("Map does not contain specified unit.", nameof(unit));
        }

        public Tile TryGetUnitTile(Unit unit)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            for (var y = 0; y < Height.Value; ++y)
                for (var x = 0; x < Width.Value; ++x)
                    if (_Tiles[x, y].Unit == unit)
                        return _Tiles[x, y];

            return null;
        }

        public Position GetMapObjectPos(MapObject.MapObject mapObject)
        {
            if (mapObject == null)
                throw new System.ArgumentNullException(nameof(mapObject));

            return TryGetMapObjectPos(mapObject) ?? throw new System.ArgumentException("Map does not contain specified object.", nameof(mapObject));
        }

        public Position TryGetMapObjectPos(MapObject.MapObject mapObject)
        {
            if (mapObject == null)
                throw new System.ArgumentNullException(nameof(mapObject));

            for (var y = 0; y < Height.Value; ++y)
                for (var x = 0; x < Width.Value; ++x)
                    if (_Tiles[x, y].Object == mapObject)
                        return new Position(x, y);

            return null;
        }

        public bool IsBlocked(Position pos)
        {
            if (pos == null)
                throw new System.ArgumentNullException(nameof(pos));

            return this[pos].Blocked;
        }

        public PositiveInt GetMoveCost(Position pos)
        {
            if (pos == null)
                throw new System.ArgumentNullException(nameof(pos));

            return this[pos].MoveCost;
        }

        public Unit GetUnitIn(Property property)
        {
            if (property == null)
                throw new System.ArgumentNullException(nameof(property));

            return GetMapObjectTile(property).Unit;
        }

        private Position GetUnitPos(Unit unit) => TryGetUnitPos(unit) ?? throw new System.ArgumentException("Map does not contain specified unit.", nameof(unit));

        private Position TryGetUnitPos(Unit unit)
        {
            for (var y = 0; y < Height.Value; ++y)
                for (var x = 0; x < Width.Value; ++x)
                    if (_Tiles[x, y].Unit == unit)
                        return new Position(x, y);

            return null;
        }

        private Position GetTilePos(Tile tile) => TryGetTilePos(tile) ?? throw new System.ArgumentException("Map does not contain specified tile.", nameof(tile));

        private Position TryGetTilePos(Tile tile)
        {
            for (var y = 0; y < Height.Value; ++y)
                for (var x = 0; x < Width.Value; ++x)
                    if (_Tiles[x, y] == tile)
                        return new Position(x, y);

            return null;
        }

        private Tile GetMapObjectTile(MapObject.MapObject mapObject) => TryGetMapObjectTile(mapObject) ?? throw new System.ArgumentException("Map does not contain specified object.", nameof(mapObject));

        private Tile TryGetMapObjectTile(MapObject.MapObject mapObject)
        {
            for (var y = 0; y < Height.Value; ++y)
                for (var x = 0; x < Width.Value; ++x)
                    if (_Tiles[x, y].Object == mapObject)
                        return _Tiles[x, y];

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

        private PositiveInt Distance(Tile tile1, Tile tile2) => new PositiveInt(GetTilePos(tile1).Distance(GetTilePos(tile2)));

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
                    return new Castle(this, _Time, new PositiveInt(10), new PositiveInt(10)); //TODO
                case MapObjectType.House:
                    return new House(_Time, this, new PositiveInt(5), new PositiveInt(10)); //TODO
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