namespace Sof.UI
{
    public class HealText : FlyingText
    {
        public int Heal
        {
            set
            {
                if (value < 0)
                    throw new System.ArgumentOutOfRangeException(nameof(value), "Heal cannot be negative.");

                Text = value.ToString();
            }
        }
    }
}
