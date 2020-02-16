namespace Sof.Model.MapObject
{
    public abstract class Property : MapObject
    {
        private readonly int _Income;
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

        public Property(ITime time, int income)
        {
            _Income = income;

            time.TurnEnded += EndTurn;
        }

        private void EndTurn() => Owner?.RecieveIncome(_Income);
    }
}
