using Sof.Model;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Map _Map;

        private int _PlayerCount = 2;
        private int _CurrentPlayerId;
        private Unit _SelectedUnit;

        private void Start()
        {
            Tile.TileLeftClicked += Tile_TileLeftClicked;
            Tile.TileRightClicked += Tile_TileRightClicked;
            Tile.TileHovered += Tile_TileHovered;
        }

        private void Tile_TileLeftClicked(Tile tile)
        {
            if (_SelectedUnit == null)
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
            _Map.ClearPath();
            _SelectedUnit.HideMoveArea();
            _SelectedUnit = null;
        }

        private void Tile_TileHovered(Tile tile)
        {
            if (_SelectedUnit != null)
            {
                var path = _Map.GetBestPath(_SelectedUnit.Pos, new Position((int)tile.transform.position.x, (int)tile.transform.position.y)); //TODO
                _Map.DrawPath(new Position[] { _SelectedUnit.Pos }.Concat(path));
            }
        }

        public void Spawn()
        {

        }

        public void EndTurn()
        {
            if (_CurrentPlayerId == _CurrentPlayerId - 1)
                _CurrentPlayerId = 0;
            else
                _CurrentPlayerId++;
        }
    }
}