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
            _Map = map;
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
