using System.Collections.Generic;
using System.Linq;
using Sof.Model;

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
            Recruit();

            var enemyHouses = _Game.Houses.Where(h => h.Owner != _Faction).ToArray();
            if (enemyHouses.Any())
                CaptureProperty(enemyHouses.First());
            else
            {
                var enemyCastles = _Game.Castles.Where(h => h.Owner != _Faction).ToArray();
                if (enemyCastles.Any())
                    CaptureProperty(enemyCastles.First());
            }
        }

        private void Recruit()
        {
            foreach (var castle in _Game.Castles.Where(h => h.Owner == _Faction))
            {
                if (castle.Unit == null) //TODO move before
                {
                    var unitToBuy = FindMostExpensiveUnitToBuy();

                    if (unitToBuy != null)
                        castle.PurchaseUnit(unitToBuy);
                }
            }
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
    }
}
