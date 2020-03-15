using Sof.Auxiliary;
using Sof.Model;
using Sof.UI;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class UIManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private GameManager _GameManager;
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
        private HealText _HealText;

        [SerializeField]
        private FactionInfoPanel _FactionInfoPanel;
        [SerializeField]
        private UnitInfoPanel _UnitInfoPanel;
        [SerializeField]
        private UnitPurchasePanel _UnitPurchasePanel;
#pragma warning restore 0649

        public bool DisableUIInteraction { private get; set; } //TODO Make dedicated UIManager

        private Model.Unit _SelectedUnit;
        private Faction _SpawnedUnitFaction;
        private Unit _SpawnedUnit;

        public void Initialize()
        {
            _GameManager.TurnEnded += OnTurnEnd;

            _TurnIndicator.SetCurrentPlayer(_GameManager.CurrentPlayerFaction);
            _FactionInfoPanel.Setup(_GameManager.CurrentPlayerFaction);
        }

        public void OnTileHover(Tile tile)
        {
            if (tile == null)
                throw new System.ArgumentNullException(nameof(tile));

            if (DisableUIInteraction)
                return;

            if (_SpawnedUnit != null)
            {
                _SpawnedUnit.transform.position = tile.transform.position;
            }
            else if (_SelectedUnit != null)
            {
                var path = _Map.ModelMap.GetClosestPath(_SelectedUnit, tile.ModelTile);
                _Map.DrawPath(new Model.Tile[] { _Map.ModelMap.GetUnitTile(_SelectedUnit) }.Concat(path));
            }
        }

        public void OnTileLeftClick(Tile tile)
        {
            if (tile == null)
                throw new System.ArgumentNullException(nameof(tile));

            if (DisableUIInteraction)
                return;

            if (_SpawnedUnit != null)
            {
                _GameManager.DebugSpawnUnit(_SpawnedUnit, tile.ModelTile, _SpawnedUnitFaction);

                _SpawnedUnit = null;
                _SpawnedUnitFaction = null;
            }
            else if (_SelectedUnit == null)
            {
                if (tile.ModelTile.Unit != null && tile.ModelTile.Unit.Faction == _GameManager.CurrentPlayerFaction)
                {
                    _SelectedUnit = tile.ModelTile.Unit;
                    var unitObject = _GameManager.GetUnitObject(_SelectedUnit);
                    unitObject.ShowUI();
                    _UnitInfoPanel.gameObject.SetActive(true);
                    _UnitInfoPanel.Setup(unitObject);
                }
            }
            else if (_SelectedUnit == tile.ModelTile.Unit)
                DeselectUnit();
            else if (tile.ModelTile.Unit != null)
            {
                if (_SelectedUnit.CanAttack(tile.ModelTile.Unit))
                    _SelectedUnit.Attack(tile.ModelTile.Unit);
            }
            else
            {
                _SelectedUnit.Move(tile.ModelTile);
                _Map.ClearPath();
            }
        }

        public void OnTileRightClick(Tile tile)
        {
            if (tile == null)
                throw new System.ArgumentNullException(nameof(tile));

            if (DisableUIInteraction)
                return;

            if (_SelectedUnit != null)
                DeselectUnit();
        }

        public void OnCastleRightClick(Castle castle)
        {
            if (castle == null)
                throw new System.ArgumentNullException(nameof(castle));

            if (_SelectedUnit != null)
                DeselectUnit();

            if (castle.ModelCastle.Owner == _GameManager.CurrentPlayerFaction)
            {
                _UnitPurchasePanel.gameObject.SetActive(true);
                _UnitPurchasePanel.Setup(_GameManager.UnitPrefabs, (Unit unit) => PurchaseUnitInCastle(unit, castle.ModelCastle));
            }
        }

        private void PurchaseUnitInCastle(Unit unit, Model.MapObject.Property.Castle castle)
        {
            _UnitPurchasePanel.gameObject.SetActive(false);
            _GameManager.PurchaseUnitInCastle(unit, castle);
        }

        public void DebugCreateUnit(Faction faction)
        {
            if (faction == null)
                throw new System.ArgumentNullException(nameof(faction));

            if (DisableUIInteraction)
                return;

            _SpawnedUnit = Instantiate(_GameManager.UnitPrefabs.First(), Map.ConvertToWorldPos(new Position(_Map.ModelMap.Width.Value / 2, _Map.ModelMap.Height.Value / 2)), Quaternion.identity, transform);
            _SpawnedUnitFaction = faction;
        }

        public void OnUnitHit(Unit unit, PositiveInt damage)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            var damageText = InstantiateTextAboveUnit(unit, _DamageText);
            damageText.Damage = damage;
        }

        public void OnUnitHeal(Unit unit, PositiveInt heal)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            var healText = InstantiateTextAboveUnit(unit, _HealText);
            healText.Heal = heal;
        }

        public void OnCriticalUnitDeath(Faction faction)
        {
            if (faction == null)
                throw new System.ArgumentNullException(nameof(faction));

            _Notifier.ShowNotification($"{faction.Name} have lost!");
        }

        public void OnEndTurnClick()
        {
            if (DisableUIInteraction)
                return;

            if (_SelectedUnit != null)
                DeselectUnit();

            _GameManager.EndTurn();
        }

        private void OnTurnEnd()
        {
            _TurnIndicator.SetCurrentPlayer(_GameManager.CurrentPlayerFaction);
            _FactionInfoPanel.Setup(_GameManager.CurrentPlayerFaction);
            _Notifier.ShowNotification($"{_GameManager.CurrentPlayerFaction.Name} turn");
        }

        private void DeselectUnit()
        {
            _Map.ClearPath();
            _GameManager.GetUnitObject(_SelectedUnit).HideUI();
            _SelectedUnit = null;
            _UnitPurchasePanel.gameObject.SetActive(false);
        }

        private T InstantiateTextAboveUnit<T>(Unit unit, T text) where T : FlyingText
        {
            const float offset = 0.3f;

            return Instantiate(text, Camera.main.WorldToScreenPoint(unit.transform.position + offset * Vector3.up), Quaternion.identity, _Canvas.transform);
        }
    }
}