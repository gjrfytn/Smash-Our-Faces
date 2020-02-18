namespace Sof.UI
{
    public class HealText : FlyingText
    {
        public int Heal
        {
            set => Text = value.ToString();
        }
    }
}
