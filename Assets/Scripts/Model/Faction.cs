using System.Collections.Generic;

namespace Sof.Model
{
    public class Faction
    {
        private class Treasury
        {
            public int Gold { get;private set; }

            public event System.Action<int> GoldChanged;

            public Treasury(int gold)
            {
                if (gold < 0)
                    throw new System.ArgumentOutOfRangeException(nameof(gold), gold, "Treasury gold cannot be negative.");

                Gold = gold;
            }

            public void PurchaseUnit(Unit unit)
            {
                if (unit == null)
                    throw new System.ArgumentNullException(nameof(unit));

                if (Gold < unit.GoldCost)
                    throw new System.Exception("No gold to buy unit.");

                Gold -= unit.GoldCost;

                GoldChanged?.Invoke(-unit.GoldCost);
            }

            public void AddGold(int gold)
            {
                if (gold < 0)
                    throw new System.ArgumentOutOfRangeException(nameof(gold), gold, "Added gold cannot be negative.");

                Gold += gold;

                GoldChanged?.Invoke(gold);
            }
        }

        private static readonly List<string> _FactionNames = new List<string>();

        private readonly Treasury _Treasury;

        public string Name { get; }
        public int Gold => _Treasury.Gold;

        public event System.Action<int> GoldChanged;

        public Faction(string name, int gold)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException("Name cannot be empty or whitespace.", nameof(name));

            if (gold < 0)
                throw new System.ArgumentOutOfRangeException(nameof(gold), "Faction gold cannot be negative.");

            if (_FactionNames.Contains(name))
                throw new System.ArgumentException($"Faction with name \"{name}\" already exists.", nameof(name));

            _FactionNames.Add(name);

            Name = name;
            _Treasury = new Treasury(gold);
            _Treasury.GoldChanged += Treasury_GoldChanged;
        }

        public void PurchaseUnit(Unit unit)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            _Treasury.PurchaseUnit(unit);
        }

        public void RecieveIncome(int income)
        {
            if (income < 0)
                throw new System.ArgumentOutOfRangeException(nameof(income), "Income cannot be negative.");

            _Treasury.AddGold(income);
        }

        private void Treasury_GoldChanged(int change) => GoldChanged?.Invoke(change);
    }
}
