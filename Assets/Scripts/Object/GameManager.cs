﻿using Sof.Auxiliary;
using Sof.Model;
using Sof.Model.Scenario;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class GameManager : MonoBehaviour, XmlScenario.IUnitRegistry
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

        private Game _Game;
        private Dictionary<Faction, HumanPlayer> _Players;

        public event System.Action TurnEnded;
        public event System.Action<Faction> GameEnded;

        public IEnumerable<Faction> Factions => _Game.Factions;

        private readonly List<Unit> _Units = new List<Unit>(); //TODO unit removement
        private Dictionary<Faction, Color> _FactionColors;

        public Faction CurrentPlayerFaction => _Game.CurrentTurnFaction;
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

            _FactionColors = CurrentScenario.Factions.ToDictionary(f1 => f1, f2 => palette.GetNewRandomColor());
        }

        private void Start()
        {
            _Players = CurrentScenario.Factions.ToDictionary(f1 => f1, f2 => new HumanPlayer());

            _Game = new Game(_Players.ToDictionary(p1 => p1.Key, p2 => (Game.IPlayer)p2.Value));

            _Game.TurnEnded += Game_TurnEnded;
            _Game.GameEnded += Game_GameEnded;

            _Map.Initialize(_Game);

            _Map.ModelMap.UnitSpawned += ModelMap_UnitSpawned;

            _Map.ModelMap.ApplyScenario(CurrentScenario);

            _UIManager.Initialize();

            _Game.Start();
        }

        private void Game_TurnEnded() => TurnEnded?.Invoke();
        private void Game_GameEnded(Faction winner) => GameEnded?.Invoke(winner);

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
            _Players[CurrentPlayerFaction].OnEndTurnClick();
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

        private void Unit_Died(Model.Unit unit) => _Game.OnUnitDeath(unit);

        private void InstantiateUnit(Unit prefab, Model.Unit modelUnit)
        {
            var unitInstance = Instantiate(prefab, Map.ConvertToWorldPos(_Map.ModelMap.GetUnitPos(modelUnit)), Quaternion.identity, transform);
            unitInstance.Initialize(modelUnit, this, _UIManager, _Map);
            _Units.Add(unitInstance);
            modelUnit.Died += () => Unit_Died(modelUnit);
        }
    }
}