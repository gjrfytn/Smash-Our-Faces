using Sof.Auxiliary;
using System.Collections.Generic;
using System.Linq;

namespace Sof.Model
{
    public class Unit
    {
        private readonly ITime _Time;
        private readonly Map _Map;
        private readonly PositiveInt _Damage;
        private readonly PositiveInt _AttackRange;

        public Faction Faction { get; private set; }
        public bool Critical { get; private set; }
        public PositiveInt GoldCost { get; private set; }

        public PositiveInt MaxHealth { get; }

        private PositiveInt _Health;
        public PositiveInt Health
        {
            get => _Health;
            set
            {
                _Health = value;

                HealthChanged?.Invoke();
            }
        }

        public event System.Action HealthChanged;

        public PositiveInt MaxMovePoints { get; }

        private PositiveInt _MovePoints;
        public PositiveInt MovePoints
        {
            get => _MovePoints;
            set
            {
                _MovePoints = value;

                MovePointsChanged?.Invoke();
            }
        }

        public event System.Action MovePointsChanged;

        private bool Dead => Health.Value == 0;

        public event System.Action<IEnumerable<Tile>> MovedAlongPath;
        public event System.Action Attacked;
        public event System.Action<PositiveInt> TookHit;
        public event System.Action<PositiveInt> Healed;
        public event System.Action Died;

        public Unit(ITime time, Map map, PositiveInt movePoints, PositiveInt health, PositiveInt damage, PositiveInt attackRange, Faction faction, bool critical, PositiveInt goldCost)
        {
            _Time = time ?? throw new System.ArgumentNullException(nameof(time));
            _Map = map ?? throw new System.ArgumentNullException(nameof(map));
            MaxMovePoints = movePoints;
            MaxHealth = health;
            _Damage = damage;
            _AttackRange = attackRange;
            Faction = faction ?? throw new System.ArgumentNullException(nameof(faction));
            Critical = critical;
            GoldCost = goldCost;
            _MovePoints = MaxMovePoints;
            _Health = MaxHealth;

            _Time.TurnEnded += Time_TurnEnded;
        }

        public void Move(Tile tile)
        {
            if (tile == null)
                throw new System.ArgumentNullException(nameof(tile));

            CheckIsAlive();

            //if (MovePoints == 0) //TODO ??

            var path = _Map.GetClosestPath(this, tile);

            var traversedPath = new List<Tile>();
            foreach (var pathTile in path)
            {
                var movePointsAfterMove = MovePoints.Value - pathTile.MoveCost.Value;
                if (movePointsAfterMove >= 0)
                    traversedPath.Add(pathTile);

                MovePoints = new PositiveInt(UnityEngine.Mathf.Max(0, movePointsAfterMove));
            }

            if (traversedPath.Any())
            {
                _Map.MoveUnit(this, traversedPath.Last());

                MovedAlongPath?.Invoke(traversedPath);
            }
        }

        public bool CanAttack(Unit unit)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            return Faction != unit.Faction && IsInAttackRange(unit) && MovePoints.Value != 0;
        }

        public void Attack(Unit unit)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            CheckIsAlive();

            if (MovePoints.Value == 0) //TODO ??
                throw new System.InvalidOperationException("Unit has no move points.");

            if (!IsInAttackRange(unit))
                throw new System.ArgumentException("Unit is out of attack range.", nameof(unit));

            if (Faction == unit.Faction)
                throw new System.ArgumentException("Cannot attack friendly unit.", nameof(unit));

            unit.TakeHit(_Damage);
            MovePoints = new PositiveInt(0);

            Attacked?.Invoke();
        }

        public void TakeHit(PositiveInt damage)
        {
            CheckIsAlive();

            var actualDamage = new PositiveInt(UnityEngine.Mathf.CeilToInt(damage.Value * (1 - _Map.GetUnitTile(this).Defence)));

            Health = new PositiveInt(UnityEngine.Mathf.Max(0, _Health.Value - actualDamage.Value));

            TookHit?.Invoke(actualDamage);

            if (Dead)
            {
                _Time.TurnEnded -= Time_TurnEnded;

                Died?.Invoke();
            }
        }

        public void Heal(PositiveInt value)
        {
            CheckIsAlive();

            var healthBeforeHeal = _Health.Value;
            Health = new PositiveInt(UnityEngine.Mathf.Min(MaxHealth.Value, _Health.Value + value.Value));

            var actualHeal = _Health.Value - healthBeforeHeal;
            if (actualHeal != 0)
                Healed?.Invoke(new PositiveInt(actualHeal));
        }

        public IEnumerable<MovePoint> GetMoveRange() => _Map.GetMoveRange(this);
        public IEnumerable<Tile> GetAttackArea() => _Map.GetTilesInRange(this, _AttackRange); //TODO temp

        private void Time_TurnEnded() => MovePoints = MaxMovePoints;

        private bool IsInAttackRange(Unit unit) => _Map.Distance(this, unit).Value <= _AttackRange.Value;

        private void CheckIsAlive()
        {
            if (Dead)
                throw new System.InvalidOperationException("Unit is dead.");
        }
    }
}