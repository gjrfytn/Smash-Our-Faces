using Sof.Auxiliary;
using Sof.Model;
using Sof.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class GameManager : MonoBehaviour, ITime, Model.Scenario.IScenario //TODO temp
    {
#pragma warning disable 0649
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
        private Unit[] _UnitPrefabs;
        [SerializeField]
        private Unit _CommanderUnit;

        [SerializeField]
        private FactionInfoPanel _FactionInfoPanel;
        [SerializeField]
        private UnitInfoPanel _UnitInfoPanel;
        [SerializeField]
        private UnitPurchasePanel _UnitPurchasePanel;
#pragma warning restore 0649

        public bool DisableUIInteraction { private get; set; } //TODO Make dedicated UIManager

        public IEnumerable<Faction> Factions => _Factions;
        public IEnumerable<Model.Scenario.Occupation> Occupations => new[] { new Model.Scenario.Occupation(new Position(2, 2), _Factions[0]), new Model.Scenario.Occupation(new Position(7, 7), _Factions[1]) };
        public IEnumerable<Model.Scenario.Unit> Units { get; private set; }

        public event System.Action TurnEnded;

        private List<Faction> _Factions;
        private Faction _CurrentPlayerFaction;
        private Model.Unit _SelectedUnit;
        private Faction _SpawnedUnitFaction;
        private Unit _SpawnedUnit;
        private readonly List<Unit> _Units = new List<Unit>(); //TODO unit removement
        private Dictionary<Faction, Color> _FactionColors;

        // TODO temp
        private Unit commanderInstance1;
        private Unit commanderInstance2;

        private void Awake()
        {
            var palette = new Palette();
            _Factions = new List<Faction> { new Faction("Faction 1", new PositiveInt(100)), new Faction("Faction 2", new PositiveInt(100)) };

            commanderInstance1 = Instantiate(_CommanderUnit, Map.ConvertToWorldPos(new Position(2, 2)), Quaternion.identity, transform);
            commanderInstance2 = Instantiate(_CommanderUnit, Map.ConvertToWorldPos(new Position(7, 7)), Quaternion.identity, transform);
            Units = new[] { new Model.Scenario.Unit(new Position(2, 2), commanderInstance1, _Factions[0], true), new Model.Scenario.Unit(new Position(7, 7), commanderInstance2, _Factions[1], true) };

            _FactionColors = _Factions.ToDictionary(f1 => f1, f2 => palette.GetNewRandomColor());
        }

        private void Start()
        {
            _Map.Initialize();

            _CurrentPlayerFaction = _Factions[0];
            _TurnIndicator.SetCurrentPlayer(_CurrentPlayerFaction);
            _FactionInfoPanel.Setup(_CurrentPlayerFaction);

            commanderInstance1.Initialize(_Map.ModelMap[new Position(2, 2)].Unit, this, _Map);
            commanderInstance2.Initialize(_Map.ModelMap[new Position(7, 7)].Unit, this, _Map);

            _Units.Add(commanderInstance1);
            _Units.Add(commanderInstance2);
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
                var unit = _Map.ModelMap.Spawn(_SpawnedUnit, tile.ModelTile, _SpawnedUnitFaction, false);
                _SpawnedUnit.Initialize(unit, this, _Map);
                _Units.Add(_SpawnedUnit);

                _SpawnedUnit = null;
                _SpawnedUnitFaction = null;
            }
            else if (_SelectedUnit == null)
            {
                if (tile.ModelTile.Unit != null && tile.ModelTile.Unit.Faction == _CurrentPlayerFaction)
                {
                    _SelectedUnit = tile.ModelTile.Unit;
                    var unitObject = GetUnitObject(_SelectedUnit);
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

            if (castle.ModelCastle.Owner == _CurrentPlayerFaction)
            {
                _UnitPurchasePanel.gameObject.SetActive(true);
                _UnitPurchasePanel.Setup(_UnitPrefabs, (Unit unit) => PurchaseUnitInCastle(unit, castle.ModelCastle));
            }
        }

        public Color GetFactionColor(Faction faction)
        {
            if (faction == null)
                return Color.gray;

            return _FactionColors[faction];
        }

        private void PurchaseUnitInCastle(Unit unit, Model.MapObject.Property.Castle castle)
        {
            _UnitPurchasePanel.gameObject.SetActive(false);
            var unitInstance = Instantiate(unit, Map.ConvertToWorldPos(_Map.ModelMap.GetMapObjectPos(castle)), Quaternion.identity, transform);
            var purchasedUnit = castle.PurchaseUnit(unitInstance);
            unitInstance.Initialize(purchasedUnit, this, _Map);
            _Units.Add(unitInstance);
        }

        public void DebugCreateUnit(Faction faction)
        {
            if (faction == null)
                throw new System.ArgumentNullException(nameof(faction));

            if (DisableUIInteraction)
                return;

            _SpawnedUnit = Instantiate(_UnitPrefabs[0], Map.ConvertToWorldPos(new Position(_Map.ModelMap.Width.Value / 2, _Map.ModelMap.Height.Value / 2)), Quaternion.identity, transform);
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

            if (_CurrentPlayerFaction == faction)
                EndTurn();

            _Factions.Remove(faction);
        }

        public void EndTurn()
        {
            if (DisableUIInteraction)
                return;

            if (_SelectedUnit != null)
                DeselectUnit();

            if (_CurrentPlayerFaction == _Factions.Last())
                _CurrentPlayerFaction = _Factions.First();
            else
            {
                var playerIndex = _Factions.IndexOf(_CurrentPlayerFaction);
                _CurrentPlayerFaction = _Factions[playerIndex + 1];
            }

            _TurnIndicator.SetCurrentPlayer(_CurrentPlayerFaction);
            _FactionInfoPanel.Setup(_CurrentPlayerFaction);
            _Notifier.ShowNotification($"{_CurrentPlayerFaction.Name} turn");

            TurnEnded?.Invoke();
        }

        private void DeselectUnit()
        {
            _Map.ClearPath();
            GetUnitObject(_SelectedUnit).HideUI();
            _SelectedUnit = null;
            _UnitPurchasePanel.gameObject.SetActive(false);
        }

        private Unit GetUnitObject(Model.Unit unit) => _Units.Single(u => u.ModelUnit == unit);

        private T InstantiateTextAboveUnit<T>(Unit unit, T text) where T : FlyingText
        {
            const float offset = 0.3f;

            return Instantiate(text, Camera.main.WorldToScreenPoint(unit.transform.position + offset * Vector3.up), Quaternion.identity, _Canvas.transform);
        }
    }
}