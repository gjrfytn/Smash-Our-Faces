using System.Collections.Generic;
using System.Linq;

namespace Sof.Model
{
    public class Unit
    {
        private readonly ITime _Time;
        private readonly Map _Map;
        private readonly int _MovePoints;
        private readonly int _Health;
        private readonly int _Damage;
        private readonly int _AttackRange;

        private int _MovePointsLeft;
        private int _HealthLeft;

        public Faction Faction { get; private set; }
        public bool Critical { get; private set; }
        public int GoldCost { get; private set; }
        public int MovePoints => _MovePointsLeft;

        private bool Dead => _HealthLeft == 0;

        public event System.Action<IEnumerable<Position>> UnitMovedAlongPath;
        public event System.Action Attacked;
        public event System.Action<int> TookHit;
        public event System.Action Died;

        public Unit(ITime time, Map map, int movePoints, int health, int damage, int attackRange, Faction faction, bool critical)
        {
            _Time = time;
            _Map = map;
            _MovePoints = movePoints;
            _Health = health;
            _Damage = damage;
            _AttackRange = attackRange;
            Faction = faction;
            Critical = critical;

            _MovePointsLeft = _MovePoints;
            _HealthLeft = _Health;

            _Time.TurnEnded += EndTurn;
        }

        public void Move(Position pos)
        {
            CheckIsAlive();

            //if (_MovePointsLeft == 0) //TODO ??

            var path = _Map.GetClosestPath(this, pos);

            var traversedPath = new List<Position>();
            foreach (var movePoint in path)
            {
                var movePointsAfterMove = _MovePointsLeft - _Map[movePoint].MoveCost;
                if (movePointsAfterMove >= 0)
                    traversedPath.Add(movePoint);

                _MovePointsLeft = UnityEngine.Mathf.Max(0, movePointsAfterMove);
            }

            if (traversedPath.Any())
            {
                _Map.MoveUnit(this, traversedPath.Last());

                UnitMovedAlongPath?.Invoke(traversedPath);
            }
        }

        public bool CanAttack(Unit unit) => Faction != unit.Faction && IsInAttackRange(unit) && MovePoints != 0;

        public void Attack(Unit unit)
        {
            CheckIsAlive();

            if (_MovePointsLeft == 0) //TODO ??
                throw new System.InvalidOperationException("Unit has no move points.");

            if (!IsInAttackRange(unit))
                throw new System.ArgumentException("Unit is out of attack range.", nameof(unit));

            if (Faction == unit.Faction)
                throw new System.ArgumentException("Cannot attack friendly unit.", nameof(unit));

            unit.TakeHit(_Damage);
            _MovePointsLeft = 0;

            Attacked?.Invoke();
        }

        public void TakeHit(int damage)
        {
            _HealthLeft = UnityEngine.Mathf.Max(0, _HealthLeft - damage);

            TookHit?.Invoke(damage);

            if (Dead)
            {
                _Time.TurnEnded -= EndTurn;

                _Map.Remove(this);

                Died?.Invoke();
            }
        }

        public IEnumerable<MovePoint> GetMoveRange() => _Map.GetMoveRange(this);

        private void EndTurn()
        {
            _MovePointsLeft = _MovePoints;
        }

        private bool IsInAttackRange(Unit unit)
        {
            var myPos = _Map.GetUnitPos(this);
            var otherPos = _Map.GetUnitPos(unit);

            return myPos.Distance(otherPos) <= _AttackRange;
        }

        private void CheckIsAlive()
        {
            if (Dead)
                throw new System.InvalidOperationException("Unit is dead.");
        }
    }
}