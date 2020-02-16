namespace Sof.Model.MapObject
{
    public abstract class Property : MapObject
    {
        private readonly int _Income;

        public Faction Owner { get; set; }

        public Property(ITime time, int income)
        {
            _Income = income;

            time.TurnEnded += EndTurn;
        }

        private void EndTurn() => Owner?.RecieveIncome(_Income);
    }
}
