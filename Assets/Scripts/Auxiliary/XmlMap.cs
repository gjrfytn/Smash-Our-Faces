using Sof.Model;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Sof.Auxiliary
{
    internal class XmlMap : IMapFile
    {
        private readonly string _Xml;

        public XmlMap(string xml)
        {
            _Xml = xml;
        }

        public TileDefinition[,] Load()
        {
            var doc = XDocument.Parse(_Xml);

            var tiles = new List<(int x, int y, TileDefinition definition)>();
            foreach (var tile in doc.Root.Elements())
            {
                var objectElem = tile.Element("Object");
                var objectType = objectElem == null ? MapObjectType.None : ParseEnum<MapObjectType>(objectElem.Value);

                tiles.Add((int.Parse(tile.Attribute("x").Value),
                           int.Parse(tile.Attribute("y").Value),
                           new TileDefinition(ParseEnum<GroundType>(tile.Element("Ground").Value), objectType)));
            }

            var mapWidth = tiles.Max(t => t.x) + 1;
            var mapHeight = tiles.Max(t => t.y) + 1;

            var tilesArray = new TileDefinition[mapWidth, mapHeight];

            foreach (var tile in tiles)
                tilesArray[tile.x, tile.y] = tile.definition;

            return tilesArray;
        }

        private static T ParseEnum<T>(string value) where T : System.Enum => (T)System.Enum.Parse(typeof(T), value);
    }
}
