using Sof.Model;
using Sof.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Canvas _Canvas;
        [SerializeField]
        private TurnIndicator _TurnIndicator;
        [SerializeField]
        private Notifier _Notifier;

        [SerializeField]
        private Map _Map;

        [SerializeField]
        private DamageText _DamageText;

        [SerializeField]
        private Unit _UnitTemp;

        public bool DisableUIInteraction { private get; set; } //TODO Make dedicated UIManager

        public event System.Action TurnEnded;

        private List<int> _PlayerIds = new List<int> { 0 };
        private int _CurrentPlayerId;
        private Unit _SelectedUnit;
        private Unit _SpawnedUnit;

        private void Start()
        {
            _TurnIndicator.SetCurrentPlayer(_CurrentPlayerId);
        }

        public void OnTileHover(Tile tile)
        {
            if (DisableUIInteraction)
                return;

            if (_SpawnedUnit != null)
            {
                _SpawnedUnit.transform.position = tile.transform.position;
            }
            else if (_SelectedUnit != null)
            {
                var path = _Map.GetClosestPath(_SelectedUnit, new Position((int)tile.transform.position.x, (int)tile.transform.position.y)); //TODO
                _Map.DrawPath(new Position[] { _Map.GetUnitPos(_SelectedUnit) }.Concat(path));
            }
        }

        public void OnTileLeftClick(Tile tile)
        {
            if (DisableUIInteraction)
                return;

            if (_SpawnedUnit != null)
            {
                _Map.Spawn(_SpawnedUnit, new Position((int)tile.transform.position.x, (int)tile.transform.position.y));//TODO
                _SpawnedUnit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

                if (!_PlayerIds.Contains(_SpawnedUnit.ModelUnit.FactionId))
                    _PlayerIds.Add(_SpawnedUnit.ModelUnit.FactionId);

                _SpawnedUnit = null;
            }
            else if (_SelectedUnit == null)
            {
                if (tile.Unit != null && tile.Unit.ModelUnit.FactionId == _CurrentPlayerId)
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
            else if (tile.Unit != null)
            {
                if (tile.Unit.ModelUnit.FactionId != _SelectedUnit.ModelUnit.FactionId && _SelectedUnit.ModelUnit.IsInAttackRange(tile.Unit.ModelUnit))
                    _SelectedUnit.ModelUnit.Attack(tile.Unit.ModelUnit);
            }
            else
            {
                _SelectedUnit.Move(new Position((int)tile.transform.position.x, (int)tile.transform.position.y)); //TODO
                _Map.ClearPath();
            }
        }

        public void OnTileRightClick(Tile tile)
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

        public void Spawn(int playerId)
        {
            if (DisableUIInteraction)
                return;

            _SpawnedUnit = Instantiate(_UnitTemp, Map.ConvertToWorldPos(new Position(_Map.ModelMap.Width / 2, _Map.ModelMap.Height / 2)), Quaternion.identity, transform);
            _SpawnedUnit.Initialize(this, _Map.ModelMap, playerId);
            _SpawnedUnit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }

        internal void OnUnitHit(Unit unit, int damage)
        {
            const float offset = 0.3f;
            var damageText = Instantiate(_DamageText, Camera.main.WorldToScreenPoint(unit.transform.position + offset * Vector3.up), Quaternion.identity, _Canvas.transform);
            damageText.Damage = damage;
        }

        internal void OnCriticalUnitDeath(int factionId)
        {
            _Notifier.ShowNotification($"Player {_CurrentPlayerId} have lost!");

            if (_CurrentPlayerId == factionId)
                EndTurn();

            _PlayerIds.Remove(factionId);
        }

        public void EndTurn()
        {
            if (DisableUIInteraction)
                return;

            if (_CurrentPlayerId == _PlayerIds.Last())
                _CurrentPlayerId = _PlayerIds.First();
            else
            {
                var playerIndex = _PlayerIds.IndexOf(_CurrentPlayerId);
                _CurrentPlayerId = _PlayerIds[playerIndex + 1];
            }

            _TurnIndicator.SetCurrentPlayer(_CurrentPlayerId);
            _Notifier.ShowNotification($"Player {_CurrentPlayerId} turn");

            TurnEnded?.Invoke();
        }
    }
}