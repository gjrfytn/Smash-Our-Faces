using System.Collections.Generic;
using System.Linq;

namespace Sof.Model
{
    public class Unit
    {
        private readonly Map _Map;
        private readonly int _MovePoints;
        private readonly int _Health;
        private readonly int _Damage;

        private int _MovePointsLeft;

        public int FactionId { get; private set; }
        public int MovePoints => _MovePointsLeft;

        public event System.Action<IEnumerable<Position>> UnitMovedAlongPath;

        public Unit(Map map, int movePoints, int health, int damage, int factionId)
        {
            _Map = map;
            _MovePoints = movePoints;
            _Health = health;
            _Damage = damage;
            FactionId = factionId;

            _MovePointsLeft = _MovePoints;
        }

        public void EndTurn()
        {
            _MovePointsLeft = _MovePoints;
        }

        public void Move(Position pos)
        {
            var path = _Map.GetPath(this, pos);

            var traversedPath = new List<Position>();
            foreach (var movePoint in path)
            {
                var movePointsAfterMove = _MovePointsLeft - _Map[movePoint].MoveCost;
                if (movePointsAfterMove >= 0)
                    traversedPath.Add(movePoint);

                _MovePointsLeft = movePointsAfterMove;
            }

            if (traversedPath.Any())
            {
                _Map.MoveUnit(this, traversedPath.Last());

                UnitMovedAlongPath?.Invoke(traversedPath);
            }
        }

        public IEnumerable<MovePoint> GetMoveRange() => _Map.GetMoveRange(this);
    }
}