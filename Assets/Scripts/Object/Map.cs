using Sof.Auxiliary;
using Sof.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    [RequireComponent(typeof(LineRenderer))]
    public class Map : MonoBehaviour
    {
#pragma warning disable 0649
        [System.Serializable]
        internal class RoadTiles
        {
            public MapObject Cross;
            public MapObject EndDown;
            public MapObject EndLeft;
            public MapObject EndRight;
            public MapObject EndUp;
            public MapObject Hor;
            public MapObject TDown;
            public MapObject TLeft;
            public MapObject TRight;
            public MapObject TUp;
            public MapObject TurnDownLeft;
            public MapObject TurnDownRight;
            public MapObject TurnLeft;
            public MapObject TurnRight;
            public MapObject Vert;
        }

        [SerializeField]
        private GameManager _GameManager;
        [SerializeField]
        private UIManager _UIManager;

        [SerializeField]
        private TextAsset _MapFile;

        [SerializeField]
        private Ground _Water;
        [SerializeField]
        private Ground _Grass;
        [SerializeField]
        private Ground _Mountain;

        [SerializeField]
        private Castle _Castle;
        [SerializeField]
        private House _House;
        [SerializeField]
        private MapObject _Bridge;
        [SerializeField]
        private MapObject _Forest;
        [SerializeField]
        private RoadTiles _RoadTiles;

        [SerializeField]
        private Tile _InteractionTile;
#pragma warning restore 0649

        public Model.Map ModelMap { get; private set; }

        private LineRenderer _LineRenderer;

        private List<Tile> _Tiles;

        public void Initialize(Model.Scenario.IScenario scenario)
        {
            _LineRenderer = GetComponent<LineRenderer>();

            ModelMap = new Model.Map(new XmlMap(_MapFile.text), scenario, _GameManager);

            _Tiles = new List<Tile>();
            for (var y = 0; y < ModelMap.Height.Value; ++y)
                for (var x = 0; x < ModelMap.Width.Value; ++x)
                {
                    var pos = new Position(x, y);
                    var tile = Instantiate(_InteractionTile, new Vector3(x, y, 0), Quaternion.identity, transform);

                    tile.Initialize(ModelMap[pos], _UIManager);

                    Ground ground;
                    switch (ModelMap[pos].Ground)
                    {
                        case Model.Ground.Water _:
                            ground = _Water;
                            break;
                        case Model.Ground.Grass _:
                            ground = _Grass;
                            break;
                        case Model.Ground.Mountain _:
                            ground = _Mountain;
                            break;
                        default:
                            throw new System.Exception("todo");
                    }

                    Instantiate(ground, new Vector3(x, y, 0), Quaternion.identity, tile.transform);

                    switch (ModelMap[pos].Object)
                    {
                        case null:
                            break;
                        case Model.MapObject.Property.Castle modelCastle:
                            var castle = tile.InstantiateMapObject(_Castle);
                            castle.Initialize(modelCastle, _GameManager, _UIManager);
                            break;
                        case Model.MapObject.Property.House modelHouse:
                            var house = tile.InstantiateMapObject(_House);
                            house.Initialize(modelHouse, _GameManager);
                            break;
                        case Model.MapObject.Bridge _:
                            tile.InstantiateMapObject(_Bridge);
                            break;
                        case Model.MapObject.Road _:
                            tile.InstantiateMapObject(ChooseRoadPiece(pos));
                            break;
                        case Model.MapObject.Forest _:
                            tile.InstantiateMapObject(_Forest);
                            break;
                        default:
                            throw new System.Exception("todo");
                    }

                    _Tiles.Add(tile);
                }
        }

        public void DrawPath(IEnumerable<Model.Tile> pathTiles)
        {
            if (pathTiles == null)
                throw new System.ArgumentNullException(nameof(pathTiles));

            const float zOffset = -0.1f;
            var tArr = pathTiles.ToArray();

            _LineRenderer.positionCount = tArr.Length;
            _LineRenderer.SetPositions(tArr.Select(GetWorldPos).Select(p => new Vector3(p.x, p.y, zOffset)).ToArray());
        }

        public void ClearPath() => _LineRenderer.positionCount = 0;

        public Vector2 GetWorldPos(Model.Tile tile)
        {
            if (tile == null)
                throw new System.ArgumentNullException(nameof(tile));

            return _Tiles.Single(t => t.ModelTile == tile).transform.position;
        }

        public static Vector2 ConvertToWorldPos(Position position)
        {
            if (position == null)
                throw new System.ArgumentNullException(nameof(position));

            return new Vector2(position.X, position.Y);
        }

        private MapObject ChooseRoadPiece(Position pos)
        {
            var hasRoadLeft = pos.X != 0 && CheckIfTileHasRoad(pos.Left);
            var hasRoadUp = pos.Y != ModelMap.Height.Value - 1 && CheckIfTileHasRoad(pos.Above);
            var hasRoadRight = pos.X != ModelMap.Width.Value - 1 && CheckIfTileHasRoad(pos.Right);
            var hasRoadDown = pos.Y != 0 && CheckIfTileHasRoad(pos.Below);

            if (hasRoadLeft && hasRoadUp && hasRoadRight && hasRoadDown)
                return _RoadTiles.Cross;
            else if (hasRoadLeft && hasRoadUp && hasRoadRight)
                return _RoadTiles.TUp;
            else if (hasRoadLeft && hasRoadUp && hasRoadDown)
                return _RoadTiles.TLeft;
            else if (hasRoadLeft && hasRoadRight && hasRoadDown)
                return _RoadTiles.TDown;
            else if (hasRoadUp && hasRoadRight && hasRoadDown)
                return _RoadTiles.TRight;
            else if (hasRoadLeft && hasRoadUp)
                return _RoadTiles.TurnDownLeft;
            else if (hasRoadLeft && hasRoadDown)
                return _RoadTiles.TurnLeft;
            else if (hasRoadRight && hasRoadDown)
                return _RoadTiles.TurnRight;
            else if (hasRoadUp && hasRoadRight)
                return _RoadTiles.TurnDownRight;
            else if (hasRoadUp && hasRoadDown)
                return _RoadTiles.Vert;
            else if (hasRoadLeft && hasRoadRight)
                return _RoadTiles.Hor;
            else if (hasRoadLeft)
                return _RoadTiles.EndRight;
            else if (hasRoadDown)
                return _RoadTiles.EndUp;
            else if (hasRoadRight)
                return _RoadTiles.EndLeft;
            else if (hasRoadUp)
                return _RoadTiles.EndDown;

            throw new System.Exception("todo");
        }

        private bool CheckIfTileHasRoad(Position pos) => ModelMap[pos].Object is Model.MapObject.Road || ModelMap[pos].Object is Model.MapObject.Bridge;
    }
}