using Sof.Model;
using Sof.Model.Ground;
using Sof.Model.MapObject;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Sof
{
    internal class XmlMap : IMapFile
    {
        private readonly string _Xml;

        public XmlMap(string xml)
        {
            _Xml = xml;
        }

        public Tile[,] Load()
        {
            var doc = XDocument.Parse(_Xml);

            var tiles = new List<(int x, int y, Tile tile)>();
            foreach (var tile in doc.Root.Elements())
            {
                var objectElem = tile.Element("Object");
                var objectType = objectElem == null ? MapObjectType.None : ParseEnum<MapObjectType>(objectElem.Value);

                tiles.Add((int.Parse(tile.Attribute("x").Value),
                           int.Parse(tile.Attribute("y").Value),
                           new Tile(new Ground(ParseEnum<GroundType>(tile.Element("Ground").Value)), new MapObject(objectType))));
            }

            var mapWidth = tiles.Max(t => t.x) + 1;
            var mapHeight = tiles.Max(t => t.y) + 1;

            var tilesArray = new Tile[mapWidth, mapHeight];

            foreach (var tile in tiles)
            {
                tilesArray[tile.x, tile.y] = tile.tile;
            }

            return tilesArray;
        }

        private static T ParseEnum<T>(string value) where T : System.Enum => (T)System.Enum.Parse(typeof(T), value);
    }
}
