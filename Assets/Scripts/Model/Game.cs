using System.Collections.Generic;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace Sof.Model
{
    public class Game : ITime
    {
        public interface IPlayer
        {
            Task Act();

            event System.Action Acted;
        }

        private readonly IDictionary<Faction, IPlayer> _Factions;
        private readonly IEnumerator<Faction> _FactionTurnEnumerator;
        private readonly List<Faction> _DefeatedFactions = new List<Faction>();

        private IPlayer _ActingPlayer;
        private bool _GameEnded;
        private bool _PlayerIsBeingAsked;

        public Faction CurrentTurnFaction => _FactionTurnEnumerator.Current;
        public IEnumerable<Faction> Factions => _Factions.Keys;

        public event System.Action TurnEnded;
        public event System.Action<Faction> GameEnded;

        public Game(IDictionary<Faction, IPlayer> factions)
        {
            _Factions = factions ?? throw new System.ArgumentNullException(nameof(factions));

            if (_Factions.Count < 2)
                throw new System.ArgumentException("Cannot play game with less than 2 factions.", nameof(factions));

            foreach (var faction in _Factions)
            {
                if (faction.Key == null)
                    throw new System.ArgumentNullException(nameof(faction.Key));

                if (faction.Value == null)
                    throw new System.ArgumentNullException(nameof(faction.Value));

                if (_Factions.Values.Count(p => p == faction.Value) > 1)
                    throw new System.ArgumentException("Player cannot control more than 1 faction.", nameof(factions)); //TODO or can?..

                faction.Value.Acted += () => Player_Acted(faction.Value); //TODO Task
            }

            _FactionTurnEnumerator = Factions.GetEnumerator();
            _FactionTurnEnumerator.MoveNext();
        }

        public Task Start() => AskPlayerToAct();

        private Task Player_Acted(IPlayer player)
        {
            if (_ActingPlayer == null)
                throw new System.InvalidOperationException("Game wasn't started.");

            if (_GameEnded)
                throw new System.InvalidOperationException("Game ended.");

            if (_ActingPlayer != player)
                throw new System.InvalidOperationException("Expected another player to act.");

            if (_DefeatedFactions.Contains(_Factions.Single(f => f.Value == player).Key))
                throw new System.InvalidOperationException("Player faction was defeated.");

            if (_PlayerIsBeingAsked)
                throw new System.InvalidOperationException($"You should not raise '{nameof(IPlayer.Acted)}' event in '{nameof(IPlayer.Act)}' method.");

            return ProcessTurn();
        }

        private Task ProcessTurn()
        {
            EndTurn();
            return AskPlayerToAct();
        }

        private async Task AskPlayerToAct()
        {
            _ActingPlayer = _Factions[CurrentTurnFaction];
            _PlayerIsBeingAsked = true;
            await _ActingPlayer.Act();
            _PlayerIsBeingAsked = false;
        }

        private void EndTurn()
        {
            do
                if (!_FactionTurnEnumerator.MoveNext())
                {
                    _FactionTurnEnumerator.Reset();
                    _FactionTurnEnumerator.MoveNext();
                }
            while (_DefeatedFactions.Contains(CurrentTurnFaction));

            TurnEnded?.Invoke();
        }

        private void EndGame()
        {
            GameEnded?.Invoke(_Factions.Single(f => !_DefeatedFactions.Contains(f.Key)).Key);

            _GameEnded = true;
        }

        public async Task OnUnitDeath(Unit unit) //TODO
        {
            if (unit.Critical)
            {
                _DefeatedFactions.Add(unit.Faction);

                if (_Factions.Count - _DefeatedFactions.Count == 1)
                    EndGame();
                else if (CurrentTurnFaction == unit.Faction)
                    await ProcessTurn();
            }
        }
    }
}
