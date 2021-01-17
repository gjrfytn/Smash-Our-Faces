using Gjrfytn.Dim;
using Sof.Model;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Sof.Auxiliary
{
    public class XmlMap : XmlParser, IMapFile
    {
        private readonly string _Xml;

        public XmlMap(string xml)
        {
            _Xml = xml ?? throw new System.ArgumentNullException(nameof(xml));
        }

        public TileDefinition[,] Load()
        {
            var doc = XDocument.Parse(_Xml);

            var tiles = new List<(Position pos, TileDefinition definition)>();
            foreach (var tile in doc.Root.Elements())
            {
                var objectElem = tile.Element("Object");
                var objectType = objectElem == null ? MapObjectType.None : ParseEnum<MapObjectType>(objectElem.Value);

                tiles.Add((ExtractPosition(tile), new TileDefinition(ParseEnum<GroundType>(tile.Element("Ground").Value), objectType)));
            }

            var mapWidth = tiles.Max(t => t.pos.X) + 1;
            var mapHeight = tiles.Max(t => t.pos.Y) + 1;

            var tilesArray = new TileDefinition[mapWidth, mapHeight];

            foreach (var tile in tiles)
                tilesArray[tile.pos.X, tile.pos.Y] = tile.definition;

            return tilesArray;
        }
    }
}
