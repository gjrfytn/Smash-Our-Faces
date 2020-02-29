using Sof.Auxiliary;

namespace Sof.UI
{
    public class DamageText : FlyingText
    {
        public PositiveInt Damage
        {
            set => Text = value.ToString();
        }
    }
}
