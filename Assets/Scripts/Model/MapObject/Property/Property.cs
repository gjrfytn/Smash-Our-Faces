namespace Sof.Model.MapObject.Property
{
    public abstract class Property : MapObject
    {
        private readonly IMap _Map;
        private readonly int _Income;
        private readonly int _Heal;
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

        public Property(ITime time, IMap map, int income, int heal)
        {
            if (time == null)
                throw new System.ArgumentNullException(nameof(time));
            if (income < 0)
                throw new System.ArgumentOutOfRangeException(nameof(income), "Income cannot be negative.");
            if (heal < 0)
                throw new System.ArgumentOutOfRangeException(nameof(heal), "Heal cannot be negative.");

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
