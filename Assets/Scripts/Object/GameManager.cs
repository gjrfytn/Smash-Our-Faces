using Sof.Auxiliary;
using Sof.Model;
using Sof.Model.Scenario;
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

        private List<Faction> _Factions;
        private readonly List<Unit> _Units = new List<Unit>(); //TODO unit removement
        private Dictionary<Faction, Color> _FactionColors;

        public Faction CurrentPlayerFaction { get; private set; }
        public IEnumerable<Unit> UnitPrefabs => _UnitPrefabs;
        public IScenario CurrentScenario { get; private set; }

        public Model.Map.IUnitTemplate this[string name]
        {
            get
            {
                switch (name)
                {
                    case "commander1": return _CommanderUnit;
                    case "commander2": return _CommanderUnit;
                    default: throw new System.ArgumentOutOfRangeException(nameof(name), name);
                }
            }
        }

        private void Awake()
        {
            var palette = new Palette();

            CurrentScenario = new XmlScenario(_ScenarioFile.text, this);

            _Factions = CurrentScenario.Factions.ToList();
            _FactionColors = _Factions.ToDictionary(f1 => f1, f2 => palette.GetNewRandomColor());
        }

        private void Start()
        {
            CurrentPlayerFaction = CurrentScenario.Factions.First();

            _UIManager.Initialize();
            _Map.Initialize();

            _Map.ModelMap.UnitSpawned += ModelMap_UnitSpawned;

            _Map.ModelMap.ApplyScenario(CurrentScenario);
        }

        private void ModelMap_UnitSpawned(Model.Unit unit, Model.Map.IUnitTemplate template)
        {
            InstantiateUnit((Unit)template, unit);
        }

        public void DebugSpawnUnit(Unit unitInstance, Model.Tile tile, Faction faction)
        {
            _Map.ModelMap.Spawn(unitInstance, tile, faction, false);
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
            castle.PurchaseUnit(unitInstance);
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

        private void InstantiateUnit(Unit prefab, Model.Unit modelUnit)
        {
            var unitInstance = Instantiate(prefab, Map.ConvertToWorldPos(_Map.ModelMap.GetUnitPos(modelUnit)), Quaternion.identity, transform);
            unitInstance.Initialize(modelUnit, this, _UIManager, _Map);
            _Units.Add(unitInstance);
        }
    }
}