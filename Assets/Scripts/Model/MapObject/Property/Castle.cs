using Gjrfytn.Dim;

namespace Sof.Model.MapObject.Property
{
    public class Castle : Property
    {
        public interface IUnitTemplate : Map.IUnitTemplate, Faction.IUnit
        {
        }

        private readonly Map _Map;

        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0.75f;

        public Castle(Map map, ITime time, PositiveInt income, PositiveInt heal) : base(time, map, income, heal)
        {
            _Map = map ?? throw new System.ArgumentNullException(nameof(map));
        }

        public void PurchaseUnit(IUnitTemplate unitTemplate)
        {
            if (unitTemplate == null)
                throw new System.ArgumentNullException(nameof(unitTemplate));

            if (Owner == null)
                throw new System.InvalidOperationException("Cannot purchase unit in neutral castle.");

            Owner.PurchaseUnit(unitTemplate);

            _Map.Spawn(unitTemplate, this, Owner, false);
        }
    }
}
