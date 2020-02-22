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
        public int Health => _HealthLeft;
        public int MovePoints => _MovePointsLeft;

        private bool Dead => _HealthLeft == 0;

        public event System.Action<IEnumerable<Tile>> UnitMovedAlongPath;
        public event System.Action Attacked;
        public event System.Action<int> TookHit;
        public event System.Action<int> Healed;
        public event System.Action Died;

        public Unit(ITime time, Map map, int movePoints, int health, int damage, int attackRange, Faction faction, bool critical, int goldCost)
        {
            _Time = time;
            _Map = map;
            _MovePoints = movePoints;
            _Health = health;
            _Damage = damage;
            _AttackRange = attackRange;
            Faction = faction;
            Critical = critical;
            GoldCost = goldCost;
            _MovePointsLeft = _MovePoints;
            _HealthLeft = _Health;

            _Time.TurnEnded += EndTurn;
        }

        public void Move(Tile tile)
        {
            CheckIsAlive();

            //if (_MovePointsLeft == 0) //TODO ??

            var path = _Map.GetClosestPath(this, tile);

            var traversedPath = new List<Tile>();
            foreach (var pathTile in path)
            {
                var movePointsAfterMove = _MovePointsLeft - pathTile.MoveCost;
                if (movePointsAfterMove >= 0)
                    traversedPath.Add(pathTile);

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
            CheckIsAlive();

            var actualDamage = UnityEngine.Mathf.CeilToInt(damage * (1 - _Map.GetUnitTile(this).Defence));

            _HealthLeft = UnityEngine.Mathf.Max(0, _HealthLeft - actualDamage);

            TookHit?.Invoke(actualDamage);

            if (Dead)
            {
                _Time.TurnEnded -= EndTurn;

                _Map.Remove(this);

                Died?.Invoke();
            }
        }

        public void Heal(int value)
        {
            CheckIsAlive();

            _HealthLeft = UnityEngine.Mathf.Min(_Health, _HealthLeft + value);

            Healed?.Invoke(value);
        }

        public IEnumerable<MovePoint> GetMoveRange() => _Map.GetMoveRange(this);
        public IEnumerable<Tile> GetAttackArea() => _Map.GetTilesInRange(this, _AttackRange); //TODO temp

        private void EndTurn() => _MovePointsLeft = _MovePoints;

        private bool IsInAttackRange(Unit unit) => _Map.Distance(this, unit) <= _AttackRange;

        private void CheckIsAlive()
        {
            if (Dead)
                throw new System.InvalidOperationException("Unit is dead.");
        }
    }
}