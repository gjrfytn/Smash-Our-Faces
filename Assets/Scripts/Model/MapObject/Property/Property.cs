﻿using Sof.Auxiliary;

namespace Sof.Model.MapObject.Property
{
    public abstract class Property : MapObject
    {
        private readonly IMap _Map;
        private readonly PositiveInt _Income;
        private readonly PositiveInt _Heal;
        private Faction _Owner;

        public Faction Owner
        {
            get => _Owner;
            set
            {
                _Owner = value;

                OwnerChanged?.Invoke();
            }
        }

        public event System.Action OwnerChanged;

        public Property(ITime time, IMap map, PositiveInt income, PositiveInt heal)
        {
            if (time == null)
                throw new System.ArgumentNullException(nameof(time));

            _Map = map ?? throw new System.ArgumentNullException(nameof(map));
            _Income = income;
            _Heal = heal;

            time.TurnEnded += EndTurn;
        }

        private void EndTurn()
        {
            Owner?.RecieveIncome(_Income);
            _Map.GetUnitIn(this)?.Heal(_Heal);
        }
    }
}
