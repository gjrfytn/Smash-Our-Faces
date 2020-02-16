using Sof.Model;
using Sof.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class GameManager : MonoBehaviour, ITime, IScenario //TODO temp
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
        private Unit[] _UnitPrefabs;

        [SerializeField]
        private UnitPurchasePanel _UnitPurchasePanel;
#pragma warning restore 0649

        public bool DisableUIInteraction { private get; set; } //TODO Make dedicated UIManager

        public IEnumerable<Faction> Factions => _Factions;
        public IEnumerable<Occupation> Occupations => new[] { new Occupation(new Position(2, 2), _Factions[0]), new Occupation(new Position(7, 7), _Factions[1]) };


        public event System.Action TurnEnded;

        private List<Faction> _Factions;
        private Faction _CurrentPlayerFaction;
        private Model.Unit _SelectedUnit;
        private Unit _SpawnedUnit;
        private readonly List<Unit> _Units = new List<Unit>(); //TODO unit removement
        private Dictionary<Faction, Color> _FactionColors;

        private void Awake()
        {
            _Factions = new List<Faction> { new Faction("Faction 1", 100), new Faction("Faction 2", 100) };
            _FactionColors = _Factions.ToDictionary(f1 => f1, f2 => Random.ColorHSV());
        }

        private void Start()
        {
            _CurrentPlayerFaction = _Factions[0];
            _TurnIndicator.SetCurrentPlayer(_CurrentPlayerFaction);
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
                var path = _Map.ModelMap.GetClosestPath(_SelectedUnit, tile.ModelTile);
                _Map.DrawPath(new Model.Tile[] { _Map.ModelMap.GetUnitTile(_SelectedUnit) }.Concat(path));
            }
        }

        public void OnTileLeftClick(Tile tile)
        {
            if (DisableUIInteraction)
                return;

            if (_SpawnedUnit != null)
            {
                _Map.ModelMap.Spawn(_SpawnedUnit.ModelUnit, tile.ModelTile);
                _SpawnedUnit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

                if (!_Factions.Contains(_SpawnedUnit.ModelUnit.Faction))
                    _Factions.Add(_SpawnedUnit.ModelUnit.Faction);

                _SpawnedUnit = null;
            }
            else if (_SelectedUnit == null)
            {
                if (tile.ModelTile.Unit != null && tile.ModelTile.Unit.Faction == _CurrentPlayerFaction)
                {
                    _SelectedUnit = tile.ModelTile.Unit;
                    GetUnitObject(_SelectedUnit).ShowUI();
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
            if (DisableUIInteraction)
                return;

            if (_SelectedUnit != null)
                DeselectUnit();
        }

        public void OnCastleRightClick(Castle castle)
        {
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

        private void PurchaseUnitInCastle(Unit unit, Model.MapObject.Castle castle)
        {
            _UnitPurchasePanel.gameObject.SetActive(false);
            var unitInstance = Instantiate(unit, Map.ConvertToWorldPos(_Map.ModelMap.GetMapObjectPos(castle)), Quaternion.identity, transform);
            unitInstance.Initialize(this, _Map, _CurrentPlayerFaction);
            _Units.Add(unitInstance);
            castle.PurchaseUnit(unitInstance.ModelUnit);
        }

        public void DebugCreateUnit(Faction faction)
        {
            if (DisableUIInteraction)
                return;

            _SpawnedUnit = Instantiate(_UnitPrefabs[0], Map.ConvertToWorldPos(new Position(_Map.ModelMap.Width / 2, _Map.ModelMap.Height / 2)), Quaternion.identity, transform);
            _SpawnedUnit.Initialize(this, _Map, faction);
            _SpawnedUnit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            _Units.Add(_SpawnedUnit);
        }

        internal void OnUnitHit(Unit unit, int damage)
        {
            const float offset = 0.3f;
            var damageText = Instantiate(_DamageText, Camera.main.WorldToScreenPoint(unit.transform.position + offset * Vector3.up), Quaternion.identity, _Canvas.transform);
            damageText.Damage = damage;
        }

        internal void OnCriticalUnitDeath(Faction faction)
        {
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
            _Notifier.ShowNotification($"{_CurrentPlayerFaction.Name} turn");

            TurnEnded?.Invoke();
        }

        private void DeselectUnit()
        {
            _Map.ClearPath();
            GetUnitObject(_SelectedUnit).HideUI();
            _SelectedUnit = null;
        }

        private Unit GetUnitObject(Model.Unit unit) => _Units.Single(u => u.ModelUnit == unit);
    }
}