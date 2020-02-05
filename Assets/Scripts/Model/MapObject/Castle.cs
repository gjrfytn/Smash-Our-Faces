namespace Sof.Model.MapObject
{
    public class Castle : MapObject
    {
        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0.75f;

        public Faction Faction { get; set; }

        public Castle(/*Map map*/)
        {
        }

        public void PurchaseUnit(Unit unit, Map map) //TODO map
        {
            Faction.PurchaseUnit(unit);

            map.Spawn(unit, this);
        }
    }
}
