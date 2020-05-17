using System.Collections.Generic;
using System.Linq;
using Sof.Model;
using UnityEngine;

namespace Sof.AI
{
    public class Player : Game.IPlayer
    {
        public interface IGameLoop
        {
            event System.Action Ticked;
        }

        public interface IGame
        {
            IEnumerable<Model.MapObject.Property.Castle.IUnitTemplate> AvailableUnits { get; }
            IEnumerable<Model.MapObject.Property.Castle> Castles { get; }
            IEnumerable<Model.MapObject.Property.House> Houses { get; }
            IEnumerable<Unit> Units { get; }
        }

        private readonly IGame _Game;
        private readonly Faction _Faction;
        private bool _ShouldAct;

        public event System.Action Acted;

        public Player(IGameLoop gameLoop, IGame game, Faction faction)
        {
            if (gameLoop == null)
                throw new System.ArgumentNullException(nameof(gameLoop));

            gameLoop.Ticked += GameLoop_Ticked;

            _Game = game ?? throw new System.ArgumentNullException(nameof(game));
            _Faction = faction ?? throw new System.ArgumentNullException(nameof(faction));
        }

        public void Act()
        {
            Control();

            _ShouldAct = true;
        }

        private void GameLoop_Ticked()
        {
            if (_ShouldAct)
            {
                _ShouldAct = false;
                Acted?.Invoke();
            }
        }

        private void Control()
        {
            var watchdog = 0;

            while (Recruit() || HasReadyUnits())
            {
                var enemyHouses = _Game.Houses.Where(h => h.Owner != _Faction).ToArray();
                if (enemyHouses.Any())
                    CaptureProperty(enemyHouses.First());

                var enemyCastles = _Game.Castles.Where(h => h.Owner != _Faction).ToArray();
                if (enemyCastles.Any())
                    CaptureProperty(enemyCastles.First());

                AttackEnemyUnits();

                watchdog++;
                if (watchdog == 1000)
                {
                    Debug.LogWarning("Infinite loop detected.");

                    return;
                }
            }
        }

        private bool HasReadyUnits() => _Game.Units.Where(u => u.Faction == _Faction).Any(u => u.MovePoints.Value != 0);

        private bool Recruit()
        {
            foreach (var castle in _Game.Castles.Where(h => h.Owner == _Faction))
            {
                if (castle.Unit == null)
                {
                    var unitToBuy = FindMostExpensiveUnitToBuy();

                    if (unitToBuy != null)
                    {
                        castle.PurchaseUnit(unitToBuy);

                        return true;
                    }
                }
            }

            return false;
        }

        private Model.MapObject.Property.Castle.IUnitTemplate FindMostExpensiveUnitToBuy()
        {
            return _Game.AvailableUnits.OrderByDescending(u => ((Faction.IUnit)u).GoldCost.Value).FirstOrDefault(u => ((Faction.IUnit)u).GoldCost.Value <= _Faction.Gold.Value);
        }

        private void CaptureProperty(Model.MapObject.Property.Property property)
        {
            var myUnits = _Game.Units.Where(u => u.Faction == _Faction);

            foreach (var unit in myUnits)
                unit.Move(property);
        }

        private void AttackEnemyUnits()
        {
            var myUnits = _Game.Units.Where(u => u.Faction == _Faction);
            var enemyUnit = _Game.Units.FirstOrDefault(u => u.Faction != _Faction);

            if (enemyUnit != null)
                foreach (var unit in myUnits)
                {
                    if (unit.CanAttack(enemyUnit))
                        unit.Attack(enemyUnit);
                    else
                        unit.Move(enemyUnit.Tile);
                }
        }
    }
}
