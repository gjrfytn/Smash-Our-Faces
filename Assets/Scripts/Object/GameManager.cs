using Sof.Auxiliary;
using Sof.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class GameManager : MonoBehaviour, ITime, Model.Scenario.IScenario //TODO temp
    {
#pragma warning disable 0649
        [SerializeField]
        private UIManager _UIManager;
        [SerializeField]
        private Map _Map;

        [SerializeField]
        private Unit[] _UnitPrefabs;
        [SerializeField]
        private Unit _CommanderUnit;
#pragma warning restore 0649
        public IEnumerable<Faction> Factions => _Factions;
        public IEnumerable<Model.Scenario.Occupation> Occupations => new[] { new Model.Scenario.Occupation(new Position(2, 2), _Factions[0]), new Model.Scenario.Occupation(new Position(7, 7), _Factions[1]) };
        public IEnumerable<Model.Scenario.Unit> Units { get; private set; }

        public event System.Action TurnEnded;

        private List<Faction> _Factions;
        private readonly List<Unit> _Units = new List<Unit>(); //TODO unit removement
        private Dictionary<Faction, Color> _FactionColors;

        // TODO temp
        private Unit commanderInstance1;
        private Unit commanderInstance2;

        public Faction CurrentPlayerFaction { get; private set; }
        public IEnumerable<Unit> UnitPrefabs => _UnitPrefabs;

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
            CurrentPlayerFaction = _Factions[0];

            _UIManager.Initialize();
            _Map.Initialize();

            commanderInstance1.Initialize(_Map.ModelMap[new Position(2, 2)].Unit, this, _UIManager, _Map);
            commanderInstance2.Initialize(_Map.ModelMap[new Position(7, 7)].Unit, this, _UIManager, _Map);

            _Units.Add(commanderInstance1);
            _Units.Add(commanderInstance2);
        }

        public void DebugSpawnUnit(Unit unitInstance, Model.Tile tile, Faction faction)
        {
            var unit = _Map.ModelMap.Spawn(unitInstance, tile, faction, false);
            unitInstance.Initialize(unit, this, _UIManager, _Map);
            _Units.Add(unitInstance);
        }

        public Color GetFactionColor(Faction faction)
        {
            if (faction == null)
                return Color.gray;

            return _FactionColors[faction];
        }

        public void PurchaseUnitInCastle(Unit unit, Model.MapObject.Property.Castle castle)
        {
            var unitInstance = Instantiate(unit, Map.ConvertToWorldPos(_Map.ModelMap.GetMapObjectPos(castle)), Quaternion.identity, transform);
            var purchasedUnit = castle.PurchaseUnit(unitInstance);
            unitInstance.Initialize(purchasedUnit, this, _UIManager, _Map);
            _Units.Add(unitInstance);
        }

        public void OnCriticalUnitDeath(Faction faction)
        {
            if (faction == null)
                throw new System.ArgumentNullException(nameof(faction));

            if (CurrentPlayerFaction == faction)
                EndTurn();

            _Factions.Remove(faction);
        }

        public void EndTurn()
        {
            if (CurrentPlayerFaction == _Factions.Last())
                CurrentPlayerFaction = _Factions.First();
            else
            {
                var playerIndex = _Factions.IndexOf(CurrentPlayerFaction);
                CurrentPlayerFaction = _Factions[playerIndex + 1];
            }

            TurnEnded?.Invoke();
        }

        public Unit GetUnitObject(Model.Unit unit) => _Units.Single(u => u.ModelUnit == unit);
    }
}