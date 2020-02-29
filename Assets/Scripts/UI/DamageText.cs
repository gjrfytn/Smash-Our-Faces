namespace Sof.UI
{
    public class DamageText : FlyingText
    {
        public int Damage
        {
            set
            {
                if (value < 0)
                    throw new System.ArgumentOutOfRangeException(nameof(value), "Damage cannot be negative.");

                Text = value.ToString();
            }
        }
    }
}
