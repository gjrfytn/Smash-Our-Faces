using Sof.Auxiliary;

namespace Sof.UI
{
    public class HealText : FlyingText
    {
        public PositiveInt Heal
        {
            set => Text = value.ToString();
        }
    }
}
