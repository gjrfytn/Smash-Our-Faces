
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
        private List<Unit> _Units = new List<Unit>();

        public int Width => _Tiles.GetLength(0);
        public int Height => _Tiles.GetLength(1);

        public Tile this[int x, int y] => _Tiles[x, y];

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

        public void Spawn(Unit unit)
        {
            _Units.Add(unit);
        }

        public IEnumerable<Position> GetBestPath(Position from, Position to) //TODO
        {
            return new Pathfinder(this).GetBestPath(from, to);
        }

        public IEnumerable<MovePoint> GetMoveRange(Position pos, int movePoints)
        {
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

        private static T ParseEnum<T>(string value) where T : System.Enum => (T)System.Enum.Parse(typeof(T), value);
    }
}