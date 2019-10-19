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
        private MapObject _Road;
        [SerializeField]
        private MapObject _Bridge;
        [SerializeField]
        private MapObject _Forest;

        [SerializeField]
        private Tile _InteractionTile;

        public Model.Map ModelMap { get; private set; }

        private LineRenderer _LineRenderer;

        private List<Tile> _Tiles;

        private void Start()
        {
            _LineRenderer = GetComponent<LineRenderer>();

            ModelMap = new Model.Map(_MapFile.text);

            _Tiles = new List<Tile>();
            for (var y = 0; y < ModelMap.Height; ++y)
                for (var x = 0; x < ModelMap.Width; ++x)
                {
                    var pos = new Position(x, y);
                    var tile = Instantiate(_InteractionTile, new Vector3(x, y, 0), Quaternion.identity, transform);

                    tile.Initialize(ModelMap[pos]);

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
                            @object = _Road;
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
            ModelMap.Spawn(unit.ModelUnit, pos);

            _Tiles.Single(t => t.transform.position.x == pos.X && t.transform.position.y == pos.Y).Unit = unit;
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

        public IEnumerable<Position> GetBestPath(Unit unit, Position pos) => ModelMap.GetBestPath(unit.ModelUnit, pos);

        public Position GetUnitPos(Unit unit) => ModelMap.GetUnitPos(unit.ModelUnit);

        public static Vector2 ConvertToWorldPos(Position position) => new Vector2(position.X, position.Y);
    }
}