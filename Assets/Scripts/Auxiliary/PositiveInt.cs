
namespace Sof.Auxiliary
{
    public struct PositiveInt
    {
        public int Value { get; }

        public PositiveInt(int value)
        {
            if (value < 0)
                throw new System.ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");

            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}
