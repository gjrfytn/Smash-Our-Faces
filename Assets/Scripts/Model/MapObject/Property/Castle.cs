using Sof.Auxiliary;

namespace Sof.Model.MapObject.Property
{
    public class Castle : Property
    {
        public interface IUnitTemplate : Map.IUnitTemplate, Faction.IUnit
        {
        }

        private readonly Map _Map;
        private readonly ITime _Time;

        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0.75f;

        public Castle(Map map, ITime time, PositiveInt income, PositiveInt heal) : base(time, map, income, heal)
        {
            _Map = map ?? throw new System.ArgumentNullException(nameof(map));
            _Time = time ?? throw new System.ArgumentNullException(nameof(time));
        }

        public Unit PurchaseUnit(IUnitTemplate unitTemplate)
        {
            if (unitTemplate == null)
                throw new System.ArgumentNullException(nameof(unitTemplate));

            if (Owner == null)
                throw new System.InvalidOperationException("Cannot purchase unit in neutral castle.");

            Owner.PurchaseUnit(unitTemplate);

            return _Map.Spawn(unitTemplate, this, _Time, Owner, false);
        }
    }
}
