using System.Collections.Generic;
using System.Linq;
using Sof.Model;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Sof.AI
{
    public class Player : Game.IPlayer
    {
        public interface IGame
        {
            IEnumerable<Model.MapObject.Property.Castle.IUnitTemplate> AvailableUnits { get; }
            IEnumerable<Model.MapObject.Property.Castle> Castles { get; }
            IEnumerable<Model.MapObject.Property.House> Houses { get; }
            IEnumerable<Unit> Units { get; }
        }

        private readonly IGame _Game;
        private readonly Faction _Faction;

        public Player(IGame game, Faction faction)
        {
            _Game = game ?? throw new System.ArgumentNullException(nameof(game));
            _Faction = faction ?? throw new System.ArgumentNullException(nameof(faction));
        }

        public Task Act() => Control();

        private async Task Control()
        {
            var watchdog = 0;

            while (Recruit() || HasReadyUnits())
            {
                var enemyHouses = _Game.Houses.Where(h => h.Owner != _Faction).ToArray();
                if (enemyHouses.Any())
                    await CaptureProperty(enemyHouses.First());

                var enemyCastles = _Game.Castles.Where(h => h.Owner != _Faction).ToArray();
                if (enemyCastles.Any())
                    await CaptureProperty(enemyCastles.First());

                await AttackEnemyUnits();

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

        private async Task CaptureProperty(Model.MapObject.Property.Property property)
        {
            var myUnits = _Game.Units.Where(u => u.Faction == _Faction);

            foreach (var unit in myUnits)
                await unit.Move(property);
        }

        private async Task AttackEnemyUnits()
        {
            var myUnits = _Game.Units.Where(u => u.Faction == _Faction);
            var enemyUnit = _Game.Units.FirstOrDefault(u => u.Faction != _Faction);

            if (enemyUnit != null)
                foreach (var unit in myUnits)
                {
                    if (unit.CanAttack(enemyUnit))
                        await unit.Attack(enemyUnit);
                    else
                        await unit.Move(enemyUnit.Tile);
                }
        }
    }
}
