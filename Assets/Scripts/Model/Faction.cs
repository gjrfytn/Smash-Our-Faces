
using System.Collections.Generic;

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

        private static List<string> _FactionNames = new List<string>();

        private readonly Treasury _Treasury;

        public string Name { get; }

        public Faction(string name, int gold)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException("Name cannot be empty or whitespace.", nameof(name));

            if (_FactionNames.Contains(name))
                throw new System.ArgumentException($"Faction with name \"{name}\" already exists.", nameof(name));

            _FactionNames.Add(name);

            Name = name;
            _Treasury = new Treasury(gold);
        }

        public void PurchaseUnit(Unit unit)
        {
            _Treasury.PurchaseUnit(unit);
        }
    }
}
