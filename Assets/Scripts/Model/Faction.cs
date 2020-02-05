
namespace Sof.Model
{
    public class Faction
    {
        private class Treasury
        {
            private int _Gold;

            public Treasury(int gold)
            {
                if (gold < 0)
                    throw new System.ArgumentOutOfRangeException(nameof(gold), gold, "Treasury gold cannot be negative.");

                _Gold = gold;
            }

            public void PurchaseUnit(Unit unit)
            {
                if (_Gold < unit.GoldCost)
                    throw new System.Exception("No gold to buy unit.");

                _Gold -= unit.GoldCost;
            }
        }

        Treasury _Treasury = new Treasury(500); //TODO temp

        public void PurchaseUnit(Unit unit)
        {
            _Treasury.PurchaseUnit(unit);
        }
    }
}
