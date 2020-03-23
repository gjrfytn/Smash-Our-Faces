using Sof.Auxiliary;
using Sof.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class GameManager : MonoBehaviour, ITime, XmlScenario.IUnitRegistry
    {
#pragma warning disable 0649
        [SerializeField]
        private UIManager _UIManager;
        [SerializeField]
        private Map _Map;
        [SerializeField]
        private TextAsset _ScenarioFile;

        [SerializeField]
        private Unit[] _UnitPrefabs;
        [SerializeField]
        private Unit _CommanderUnit;
#pragma warning restore 0649

        public IEnumerable<Faction> Factions => _Factions;

        public event System.Action TurnEnded;

        private XmlScenario _Scenario;
        private List<Faction> _Factions;
        private readonly List<Unit> _Units = new List<Unit>(); //TODO unit removement
        private Dictionary<Faction, Color> _FactionColors;

        // TODO temp
        private Unit commanderInstance1;
        private Unit commanderInstance2;

        public Faction CurrentPlayerFaction { get; private set; }
        public IEnumerable<Unit> UnitPrefabs => _UnitPrefabs;

        public Model.Map.IUnitTemplate this[string name]
        {
            get
            {
                switch (name)
                {
                    case "commander1": return commanderInstance1;
                    case "commander2": return commanderInstance2;
                    default: throw new System.ArgumentOutOfRangeException(nameof(name), name);
                }
            }
        }

        private void Awake()
        {
            var palette = new Palette();

            commanderInstance1 = Instantiate(_CommanderUnit, Map.ConvertToWorldPos(new Position(2, 2)), Quaternion.identity, transform);
            commanderInstance2 = Instantiate(_CommanderUnit, Map.ConvertToWorldPos(new Position(7, 7)), Quaternion.identity, transform);

            _Scenario = new XmlScenario(_ScenarioFile.text, this);

            _Factions = _Scenario.Factions.ToList();
            _FactionColors = _Factions.ToDictionary(f1 => f1, f2 => palette.GetNewRandomColor());
        }

        private void Start()
        {
            CurrentPlayerFaction = _Scenario.Factions.First();

            _UIManager.Initialize();
            _Map.Initialize(_Scenario);

            InitializeUnit(commanderInstance1, _Map.ModelMap[new Position(2, 2)].Unit);
            InitializeUnit(commanderInstance2, _Map.ModelMap[new Position(7, 7)].Unit);
        }

        public void DebugSpawnUnit(Unit unitInstance, Model.Tile tile, Faction faction)
        {
            var unit = _Map.ModelMap.Spawn(unitInstance, tile, faction, false);
            InitializeUnit(unitInstance, unit);
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
            InitializeUnit(unitInstance, purchasedUnit);
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

        private void InitializeUnit(Unit unitInstance, Model.Unit modelUnit)
        {
            unitInstance.Initialize(modelUnit, this, _UIManager, _Map);
            _Units.Add(unitInstance);
        }
    }
}