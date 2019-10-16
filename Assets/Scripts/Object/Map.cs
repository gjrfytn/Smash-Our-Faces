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
        private Unit _UnitTemp;

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

        private LineRenderer _LineRenderer;

        private Model.Map _Map;
        private List<Tile> _Tiles;

        private void Start()
        {
            _LineRenderer = GetComponent<LineRenderer>();

            _Map = new Model.Map(_MapFile.text);

            _Tiles = new List<Tile>();
            for (var y = 0; y < _Map.Height; ++y)
                for (var x = 0; x < _Map.Width; ++x)
                {
                    var pos = new Position(x, y);
                    var tile = Instantiate(_InteractionTile, new Vector3(x, y, 0), Quaternion.identity, transform);

                    tile.Initialize(_Map[pos]);

                    Ground ground;
                    switch (_Map[pos].Ground.Type)
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
                    switch (_Map[pos].Object.Type)
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

        public void Spawn()
        {
            //TODO Делать внутри Object.Unit?
            var pos = new Position(5, 5);
            var unit = new Model.Unit(_Map, _UnitTemp.Speed, _UnitTemp.Health, _UnitTemp.Damage, 0);
            _Map.Spawn(unit, pos);
            var unitObj = Instantiate(_UnitTemp, ConvertToWorldPos(pos), Quaternion.identity, transform);
            unitObj.Initialize(unit);

            _Tiles.Single(t => t.transform.position.x == 5 && t.transform.position.y == 5).Unit = unitObj;
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

        public IEnumerable<Position> GetBestPath(Unit unit, Position pos) => _Map.GetBestPath(unit.ModelUnit, pos);

        public Position GetUnitPos(Unit unit) => _Map.GetUnitPos(unit.ModelUnit);

        private Vector2 ConvertToWorldPos(Position position)
        {
            return new Vector2(position.X, position.Y);
        }
    }
}