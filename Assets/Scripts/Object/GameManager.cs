using Sof.Auxiliary;
using Sof.Model;
using Sof.Model.Scenario;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class GameManager : MonoBehaviour, XmlScenario.IUnitRegistry, AI.Player.IGameLoop
    {
        private class HumanPlayer : Game.IPlayer
        {
            public event System.Action Acted;

            public void Act()
            {
            }

            public void OnEndTurnClick() => Acted?.Invoke();
        }

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

        private Dictionary<Faction, HumanPlayer> _HumanPlayers;

        public Game Game { get; private set; }

        private readonly List<Unit> _Units = new List<Unit>(); //TODO unit removement
        private Dictionary<Faction, Color> _FactionColors;

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

        public event System.Action Ticked;

        private void Awake()
        {
            var palette = new Palette();

            CurrentScenario = new XmlScenario(_ScenarioFile.text, this);

            _FactionColors = CurrentScenario.Factions.ToDictionary(f1 => f1, f2 => palette.GetNewRandomColor());
        }

        private void Start()
        {
            const int humanCount = 1;
            _HumanPlayers = CurrentScenario.Factions.Take(humanCount).ToDictionary(f1 => f1, f2 => new HumanPlayer());

            var players = new Dictionary<Faction, Game.IPlayer>();
            foreach (var player in _HumanPlayers)
                players.Add(player.Key, player.Value);

            foreach (var faction in CurrentScenario.Factions.Skip(humanCount))
                players.Add(faction, new AI.Player(this));

            Game = new Game(players);


            _Map.Initialize(Game);

            _Map.ModelMap.UnitSpawned += ModelMap_UnitSpawned;

            _Map.ModelMap.ApplyScenario(CurrentScenario);

            _UIManager.Initialize();

            Game.Start();
        }

        private void Update()
        {
            Ticked?.Invoke();
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

        public void EndTurn()
        {
            _HumanPlayers[Game.CurrentTurnFaction].OnEndTurnClick();
        }

        public Unit GetUnitObject(Model.Unit unit) => _Units.Single(u => u.ModelUnit == unit);

        private void ModelMap_UnitSpawned(Model.Unit unit, Model.Map.IUnitTemplate template)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));
            if (template == null)
                throw new System.ArgumentNullException(nameof(template));

            InstantiateUnit((Unit)template, unit);
        }

        private void Unit_Died(Model.Unit unit) => Game.OnUnitDeath(unit);

        private void InstantiateUnit(Unit prefab, Model.Unit modelUnit)
        {
            var unitInstance = Instantiate(prefab, Map.ConvertToWorldPos(_Map.ModelMap.GetUnitPos(modelUnit)), Quaternion.identity, transform);
            unitInstance.Initialize(modelUnit, this, _UIManager, _Map);
            _Units.Add(unitInstance);
            modelUnit.Died += () => Unit_Died(modelUnit);
        }
    }
}