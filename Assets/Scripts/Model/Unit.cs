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
        private readonly int _AttackRange;

        private int _MovePointsLeft;
        private int _HealthLeft;

        public int FactionId { get; private set; }
        public int MovePoints => _MovePointsLeft;

        public event System.Action<IEnumerable<Position>> UnitMovedAlongPath;
        public event System.Action Attacked;
        public event System.Action<int> TookHit;

        public Unit(Map map, int movePoints, int health, int damage, int attackRange, int factionId)
        {
            _Map = map;
            _MovePoints = movePoints;
            _Health = health;
            _Damage = damage;
            _AttackRange = attackRange;
            FactionId = factionId;

            _MovePointsLeft = _MovePoints;
            _HealthLeft = _Health;
        }

        public void EndTurn()
        {
            _MovePointsLeft = _MovePoints;
        }

        public void Move(Position pos)
        {
            var path = _Map.GetClosestPath(this, pos);

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

        public void Attack(Unit unit)
        {
            if (!IsInAttackRange(unit))
                throw new System.ArgumentException("Unit is out of attack range.", nameof(unit));

            unit.TakeHit(_Damage);
            _MovePointsLeft = 0;

            Attacked?.Invoke();
        }

        public void TakeHit(int damage)
        {
            _HealthLeft -= damage;

            TookHit?.Invoke(damage);
        }

        public IEnumerable<MovePoint> GetMoveRange() => _Map.GetMoveRange(this);

        public bool IsInAttackRange(Unit unit)
        {
            var myPos = _Map.GetUnitPos(this);
            var otherPos = _Map.GetUnitPos(unit);

            return myPos.Distance(otherPos) <= _AttackRange;
        }
    }
}