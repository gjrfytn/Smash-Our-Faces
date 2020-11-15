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
        }

        private readonly IDictionary<Faction, IPlayer> _Factions;
        private readonly IEnumerator<Faction> _FactionTurnEnumerator;
        private readonly List<Faction> _DefeatedFactions = new List<Faction>();

        private bool _GameEnded;

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
            }

            _FactionTurnEnumerator = Factions.GetEnumerator();
            _FactionTurnEnumerator.MoveNext();
        }

        public async Task Start()
        {
            while(true)
            {
                await _Factions[CurrentTurnFaction].Act();

                if (_GameEnded)
                    break;

                EndTurn();
            }
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

        public void OnUnitDeath(Unit unit) //TODO
        {
            if (unit.Critical)
            {
                _DefeatedFactions.Add(unit.Faction);

                if (_Factions.Count - _DefeatedFactions.Count == 1)
                    EndGame();
                else if (CurrentTurnFaction == unit.Faction)
                    EndTurn(); //TODO don't work properly (should also stop player turn processing).
            }
        }
    }
}
