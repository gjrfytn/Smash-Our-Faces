using Gjrfytn.Dim;
using System.Collections.Generic;

namespace Sof.Model
{
    public class Faction
    {
        public interface IUnit
        {
            PositiveInt GoldCost { get; }
        }

        private class Treasury
        {
            public PositiveInt Gold { get; private set; }

            public event System.Action<int> GoldChanged;

            public Treasury(PositiveInt gold)
            {
                Gold = gold;
            }

            public void PurchaseUnit(IUnit unit)
            {
                if (unit == null)
                    throw new System.ArgumentNullException(nameof(unit));

                if (Gold.Value < unit.GoldCost.Value)
                    throw new System.Exception("No gold to buy unit.");

                Gold = new PositiveInt(Gold.Value - unit.GoldCost.Value);

                GoldChanged?.Invoke(-unit.GoldCost.Value);
            }

            public void AddGold(PositiveInt gold)
            {
                Gold = new PositiveInt(Gold.Value + gold.Value);

                GoldChanged?.Invoke(gold.Value);
            }
        }

        private static readonly List<string> _FactionNames = new List<string>();

        private readonly Treasury _Treasury;

        public string Name { get; }
        public PositiveInt Gold => _Treasury.Gold;

        public event System.Action<int> GoldChanged;

        public Faction(string name, PositiveInt gold)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException("Name cannot be empty or whitespace.", nameof(name));

            if (_FactionNames.Contains(name))
                throw new System.ArgumentException($"Faction with name \"{name}\" already exists.", nameof(name));

            _FactionNames.Add(name);

            Name = name;
            _Treasury = new Treasury(gold);
            _Treasury.GoldChanged += Treasury_GoldChanged;
        }

        public void PurchaseUnit(IUnit unit)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            _Treasury.PurchaseUnit(unit);
        }

        public void RecieveIncome(PositiveInt income) => _Treasury.AddGold(income);

        private void Treasury_GoldChanged(int change) => GoldChanged?.Invoke(change);
    }
}
