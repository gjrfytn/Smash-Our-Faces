using System.Collections.Generic;
using System.Linq;

namespace Sof.Model
{
    public class Unit
    {
        private readonly ITime _Time;
        private readonly Map _Map;
        private readonly int _MaxMovePoints;
        private readonly int _MaxHealth;
        private readonly int _Damage;
        private readonly int _AttackRange;

        public Faction Faction { get; private set; }
        public bool Critical { get; private set; }
        public int GoldCost { get; private set; }

        private int _Health;
        public int Health
        {
            get => _Health;
            set
            {
                _Health = value;

                HealthChanged?.Invoke();
            }
        }

        public event System.Action HealthChanged;

        private int _MovePoints;
        public int MovePoints
        {
            get => _MovePoints;
            set
            {
                _MovePoints = value;

                MovePointsChanged?.Invoke();
            }
        }

        public event System.Action MovePointsChanged;

        private bool Dead => Health == 0;

        public event System.Action<IEnumerable<Tile>> MovedAlongPath;
        public event System.Action Attacked;
        public event System.Action<int> TookHit;
        public event System.Action<int> Healed;
        public event System.Action Died;

        public Unit(ITime time, Map map, int movePoints, int health, int damage, int attackRange, Faction faction, bool critical, int goldCost)
        {
            _Time = time;
            _Map = map;
            _MaxMovePoints = movePoints;
            _MaxHealth = health;
            _Damage = damage;
            _AttackRange = attackRange;
            Faction = faction;
            Critical = critical;
            GoldCost = goldCost;
            _MovePoints = _MaxMovePoints;
            _Health = _MaxHealth;

            _Time.TurnEnded += EndTurn;
        }

        public void Move(Tile tile)
        {
            CheckIsAlive();

            //if (MovePoints == 0) //TODO ??

            var path = _Map.GetClosestPath(this, tile);

            var traversedPath = new List<Tile>();
            foreach (var pathTile in path)
            {
                var movePointsAfterMove = MovePoints - pathTile.MoveCost;
                if (movePointsAfterMove >= 0)
                    traversedPath.Add(pathTile);

                MovePoints = UnityEngine.Mathf.Max(0, movePointsAfterMove);
            }

            if (traversedPath.Any())
            {
                _Map.MoveUnit(this, traversedPath.Last());

                MovedAlongPath?.Invoke(traversedPath);
            }
        }

        public bool CanAttack(Unit unit) => Faction != unit.Faction && IsInAttackRange(unit) && MovePoints != 0;

        public void Attack(Unit unit)
        {
            CheckIsAlive();

            if (MovePoints == 0) //TODO ??
                throw new System.InvalidOperationException("Unit has no move points.");

            if (!IsInAttackRange(unit))
                throw new System.ArgumentException("Unit is out of attack range.", nameof(unit));

            if (Faction == unit.Faction)
                throw new System.ArgumentException("Cannot attack friendly unit.", nameof(unit));

            unit.TakeHit(_Damage);
            MovePoints = 0;

            Attacked?.Invoke();
        }

        public void TakeHit(int damage)
        {
            CheckIsAlive();

            var actualDamage = UnityEngine.Mathf.CeilToInt(damage * (1 - _Map.GetUnitTile(this).Defence));

            Health = UnityEngine.Mathf.Max(0, _Health - actualDamage);

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

            Health = UnityEngine.Mathf.Min(_MaxHealth, _Health + value);

            Healed?.Invoke(value);
        }

        public IEnumerable<MovePoint> GetMoveRange() => _Map.GetMoveRange(this);
        public IEnumerable<Tile> GetAttackArea() => _Map.GetTilesInRange(this, _AttackRange); //TODO temp

        private void EndTurn() => MovePoints = _MaxMovePoints;

        private bool IsInAttackRange(Unit unit) => _Map.Distance(this, unit) <= _AttackRange;

        private void CheckIsAlive()
        {
            if (Dead)
                throw new System.InvalidOperationException("Unit is dead.");
        }
    }
}