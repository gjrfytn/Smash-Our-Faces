namespace Sof.Model.MapObject
{
    public class Castle : MapObject
    {
        private readonly int _Income;

        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0.75f;

        public Faction Faction { get; set; }

        public Castle(ITime time/*Map map*/, int income)
        {
            time.TurnEnded += EndTurn;
            _Income = income;
        }

        public void PurchaseUnit(Unit unit, Map map) //TODO map
        {
            if (Faction == null)
                throw new System.InvalidOperationException("Cannot purchase unit in neutral castle.");

            Faction.PurchaseUnit(unit);

            map.Spawn(unit, this);
        }

        private void EndTurn() => Faction?.RecieveIncome(_Income);
    }
}
