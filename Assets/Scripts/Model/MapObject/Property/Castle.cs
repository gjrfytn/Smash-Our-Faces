using Sof.Auxiliary;

namespace Sof.Model.MapObject.Property
{
    public class Castle : Property
    {
        private readonly Map _Map;

        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0.75f;

        public Castle(Map map, ITime time, PositiveInt income, PositiveInt heal) : base(time, map, income, heal)
        {
            _Map = map ?? throw new System.ArgumentNullException(nameof(map));
        }

        public void PurchaseUnit(Unit unit)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            if (Owner == null)
                throw new System.InvalidOperationException("Cannot purchase unit in neutral castle.");

            Owner.PurchaseUnit(unit);

            _Map.Spawn(unit, this);
        }
    }
}
