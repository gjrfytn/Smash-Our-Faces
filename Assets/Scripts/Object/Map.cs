using Sof.Model;
using Sof.Model.Ground;
using Sof.Model.MapObject;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    [RequireComponent(typeof(LineRenderer))]
    public class Map : MonoBehaviour
    {
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
        private TextAsset _MapFile;

        [SerializeField]
        private Ground _Water;
        [SerializeField]
        private Ground _Grass;
        [SerializeField]
        private Ground _Mountain;

        [SerializeField]
        private MapObject _Castle;
        [SerializeField]
        private MapObject _House;
        [SerializeField]
        private MapObject _Bridge;
        [SerializeField]
        private MapObject _Forest;
        [SerializeField]
        private RoadTiles _RoadTiles;

        [SerializeField]
        private Tile _InteractionTile;

        public Model.Map ModelMap { get; private set; }

        private LineRenderer _LineRenderer;

        private List<Tile> _Tiles;

        private void Start()
        {
            _LineRenderer = GetComponent<LineRenderer>();

            ModelMap = new Model.Map(new XmlMap(_MapFile.text));
            ModelMap.UnitMoved += ModelMap_UnitMoved;

            _Tiles = new List<Tile>();
            for (var y = 0; y < ModelMap.Height; ++y)
                for (var x = 0; x < ModelMap.Width; ++x)
                {
                    var pos = new Position(x, y);
                    var tile = Instantiate(_InteractionTile, new Vector3(x, y, 0), Quaternion.identity, transform);

                    tile.Initialize(ModelMap[pos], _GameManager);

                    Ground ground;
                    switch (ModelMap[pos].Ground.Type)
                    {
                        case GroundType.Water:
                            ground = _Water;
                            break;
                        case GroundType.Grass:
                            ground = _Grass;
                            break;
                        case GroundType.Mountain:
                            ground = _Mountain;
                            break;
                        default:
                            throw new System.Exception("todo");
                    }

                    Instantiate(ground, new Vector3(x, y, 0), Quaternion.identity, tile.transform);

                    MapObject @object;
                    switch (ModelMap[pos].Object.Type)
                    {
                        case MapObjectType.None:
                            @object = null;
                            break;
                        case MapObjectType.Castle:
                            @object = _Castle;
                            break;
                        case MapObjectType.House:
                            @object = _House;
                            break;
                        case MapObjectType.Bridge:
                            @object = _Bridge;
                            break;
                        case MapObjectType.Road:
                            @object = ChooseRoadPiece(pos);
                            break;
                        case MapObjectType.Forest:
                            @object = _Forest;
                            break;
                        default:
                            throw new System.Exception("todo");
                    }

                    if (@object != null)
                        Instantiate(@object, new Vector3(x, y, 0), Quaternion.identity, tile.transform);

                    _Tiles.Add(tile);
                }
        }

        public void Spawn(Unit unit, Position pos)
        {
            ModelMap.Spawn(unit.ModelUnit, pos); //TODO UnitSpawned event

            _Tiles.Single(t => t.transform.position.x == pos.X && t.transform.position.y == pos.Y).Unit = unit; //TODO ^^^
        }

        public void DrawPath(IEnumerable<Position> points)
        {
            if (points is null)
                throw new System.ArgumentNullException(nameof(points));

            const float zOffset = -0.1f;
            var pArr = points.ToArray();

            _LineRenderer.positionCount = pArr.Length;
            _LineRenderer.SetPositions(pArr.Select(ConvertToWorldPos).Select(v => new Vector3(v.x, v.y, zOffset)).ToArray());
        }

        public void ClearPath()
        {
            _LineRenderer.positionCount = 0;
        }

        public IEnumerable<Position> GetClosestPath(Unit unit, Position pos) => ModelMap.GetClosestPath(unit.ModelUnit, pos);

        public Position GetUnitPos(Unit unit) => ModelMap.GetUnitPos(unit.ModelUnit);

        public static Vector2 ConvertToWorldPos(Position position) => new Vector2(position.X, position.Y);

        private void ModelMap_UnitMoved(Model.Unit unit)
        {
            var fromTile = _Tiles.Single(t => t.Unit != null ? t.Unit.ModelUnit == unit : false);
            var newPos = ModelMap.GetUnitPos(unit);
            var toTile = _Tiles.Single(t => t.transform.position.x == newPos.X && t.transform.position.y == newPos.Y);

            toTile.Unit = fromTile.Unit;
            fromTile.Unit = null;
        }

        private MapObject ChooseRoadPiece(Position pos)
        {
            var hasRoadLeft = pos.X != 0 && CheckIfTileHasRoad(new Position(pos.X - 1, pos.Y));
            var hasRoadUp = pos.Y != ModelMap.Height - 1 && CheckIfTileHasRoad(new Position(pos.X, pos.Y + 1));
            var hasRoadRight = pos.X != ModelMap.Width - 1 && CheckIfTileHasRoad(new Position(pos.X + 1, pos.Y));
            var hasRoadDown = pos.Y != 0 && CheckIfTileHasRoad(new Position(pos.X, pos.Y - 1));

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

        private bool CheckIfTileHasRoad(Position pos)
        {
            var type = ModelMap[pos].Object.Type;

            return type == MapObjectType.Road || type == MapObjectType.Bridge;
        }
    }
}