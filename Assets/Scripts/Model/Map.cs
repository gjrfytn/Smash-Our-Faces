
using Sof.Model.Ground;
using Sof.Model.MapObject;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Sof.Model
{
    public class Map
    {
        private Tile[,] _Tiles;

        public int Width => _Tiles.GetLength(0);
        public int Height => _Tiles.GetLength(1);

        public Tile this[Position pos] => _Tiles[pos.X, pos.Y];

        public Map(string mapXml)
        {
            var doc = XDocument.Parse(mapXml);

            var tiles = new List<(int x, int y, Tile tile)>();
            foreach (var tile in doc.Root.Elements())
            {
                var objectElem = tile.Element("Object");
                var objectType = objectElem == null ? MapObjectType.None : ParseEnum<MapObjectType>(objectElem.Value);

                tiles.Add((int.Parse(tile.Attribute("x").Value),
                           int.Parse(tile.Attribute("y").Value),
                           new Tile(new Ground.Ground(ParseEnum<GroundType>(tile.Element("Ground").Value)), new MapObject.MapObject(objectType))));
            }

            var mapWidth = tiles.Max(t => t.x) + 1;
            var mapHeight = tiles.Max(t => t.y) + 1;

            _Tiles = new Tile[mapWidth, mapHeight];

            foreach (var tile in tiles)
            {
                _Tiles[tile.x, tile.y] = tile.tile;
            }
        }

        public void Spawn(Unit unit, Position pos)
        {
            if (_Tiles[pos.X, pos.Y].Unit != null)
                throw new System.ArgumentException("Target position is occupied.", nameof(pos)); //TODO

            _Tiles[pos.X, pos.Y].Unit = unit;
        }

        public IEnumerable<Position> GetBestPath(Unit unit, Position pos) //TODO
        {
            var currentPos = GetUnitPos(unit);
            if (currentPos == null)
                throw new System.ArgumentException("Map does not contain specified unit.", nameof(unit)); //TODO

            return new Pathfinder(this).GetBestPath(currentPos, pos);
        }

        public void MoveUnit(Unit unit, Position pos)
        {
            if (_Tiles[pos.X, pos.Y].Unit != null)
                throw new System.ArgumentException("Target position is occupied.", nameof(pos)); //TODO

            var oldPos = GetUnitPos(unit);
            if (oldPos == null)
                throw new System.ArgumentException("Map does not contain specified unit.", nameof(unit)); //TODO

            _Tiles[oldPos.X, oldPos.Y].Unit = null;
            _Tiles[pos.X, pos.Y].Unit = unit;
        }

        public IEnumerable<MovePoint> GetMoveRange(Unit unit)
        {
            var pos = GetUnitPos(unit);

            return new MovePoint[]
            {
                new MovePoint(new Position(pos.X-1, pos.Y+1), 2),
                new MovePoint(new Position(pos.X, pos.Y+1), 1),
                new MovePoint(new Position(pos.X+1, pos.Y+1), 2),
                new MovePoint(new Position(pos.X+1, pos.Y), 1),
                new MovePoint(new Position(pos.X+1, pos.Y-1), 2),
                new MovePoint(new Position(pos.X, pos.Y-1), 1),
                new MovePoint(new Position(pos.X-1, pos.Y-1), 2),
                new MovePoint(new Position(pos.X-1, pos.Y), 1),
            }; //TODO
        }

        public Position GetUnitPos(Unit unit)
        {
            for (var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                    if (_Tiles[x, y].Unit == unit)
                        return new Position(x, y);

            return null;
        }

        private static T ParseEnum<T>(string value) where T : System.Enum => (T)System.Enum.Parse(typeof(T), value);
    }
}