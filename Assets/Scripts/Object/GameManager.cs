using Sof.Model;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Map _Map;

        [SerializeField]
        private Unit _UnitTemp;

        public bool DisableUIInteraction { private get; set; } //TODO Make dedicated UIManager

        public event System.Action TurnEnded;

        private int _PlayerCount = 2;
        private int _CurrentPlayerId;
        private Unit _SelectedUnit;
        private Unit _SpawnedUnit;

        private void Start()
        {
            Tile.TileLeftClicked += Tile_TileLeftClicked;
            Tile.TileRightClicked += Tile_TileRightClicked;
            Tile.TileHovered += Tile_TileHovered;
        }

        private void Tile_TileLeftClicked(Tile tile)
        {
            if (DisableUIInteraction)
                return;

            if (_SpawnedUnit != null)
            {
                _Map.Spawn(_SpawnedUnit, new Position((int)tile.transform.position.x, (int)tile.transform.position.y));//TODO
                _SpawnedUnit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                _SpawnedUnit = null;
            }
            else if (_SelectedUnit == null)
            {
                if (tile.Unit != null)
                {
                    _SelectedUnit = tile.Unit;
                    _SelectedUnit.ShowMoveArea();
                }
            }
            else if (_SelectedUnit == tile.Unit)
            {
                _SelectedUnit.HideMoveArea();
                _SelectedUnit = null;
            }
            else
            {
                _SelectedUnit.Move(new Position((int)tile.transform.position.x, (int)tile.transform.position.y)); //TODO
                _Map.ClearPath();
            }
        }

        private void Tile_TileRightClicked(Tile tile)
        {
            if (DisableUIInteraction)
                return;

            if (_SelectedUnit != null)
            {
                _Map.ClearPath();
                _SelectedUnit.HideMoveArea();
                _SelectedUnit = null;
            }
        }

        private void Tile_TileHovered(Tile tile)
        {
            if (DisableUIInteraction)
                return;

            if (_SpawnedUnit != null)
            {
                _SpawnedUnit.transform.position = tile.transform.position;
            }
            else if (_SelectedUnit != null)
            {
                var path = _Map.GetBestPath(_SelectedUnit, new Position((int)tile.transform.position.x, (int)tile.transform.position.y)); //TODO
                _Map.DrawPath(new Position[] { _Map.GetUnitPos(_SelectedUnit) }.Concat(path));
            }
        }

        public void Spawn(int playerId)
        {
            if (DisableUIInteraction)
                return;

            _SpawnedUnit = Instantiate(_UnitTemp, Map.ConvertToWorldPos(new Position(_Map.ModelMap.Width / 2, _Map.ModelMap.Height / 2)), Quaternion.identity, transform);
            _SpawnedUnit.Initialize(this, _Map.ModelMap, playerId);
            _SpawnedUnit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }

        public void EndTurn()
        {
            if (DisableUIInteraction)
                return;

            if (_CurrentPlayerId == _PlayerCount - 1)
                _CurrentPlayerId = 0;
            else
                _CurrentPlayerId++;

            TurnEnded?.Invoke();
        }
    }
}