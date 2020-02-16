namespace Sof.Model.MapObject
{
    public class Castle : MapObject
    {
        private readonly Map _Map;
        private readonly int _Income;

        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0.75f;

        public Faction Faction { get; set; }

        public Castle(Map map, ITime time, int income)
        {
            _Map = map;
            _Income = income;

            time.TurnEnded += EndTurn;
        }

        public void PurchaseUnit(Unit unit)
        {
            if (Faction == null)
                throw new System.InvalidOperationException("Cannot purchase unit in neutral castle.");

            Faction.PurchaseUnit(unit);

            _Map.Spawn(unit, this);
        }

        private void EndTurn() => Faction?.RecieveIncome(_Income);
    }
}
