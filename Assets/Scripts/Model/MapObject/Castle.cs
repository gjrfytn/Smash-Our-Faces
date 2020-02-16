namespace Sof.Model.MapObject
{
    public class Castle : Property
    {
        private readonly Map _Map;

        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0.75f;

        public Castle(Map map, ITime time, int income) : base(time, income)
        {
            _Map = map;
        }

        public void PurchaseUnit(Unit unit)
        {
            if (Owner == null)
                throw new System.InvalidOperationException("Cannot purchase unit in neutral castle.");

            Owner.PurchaseUnit(unit);

            _Map.Spawn(unit, this);
        }
    }
}
