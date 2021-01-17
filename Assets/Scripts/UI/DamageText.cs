using Gjrfytn.Dim;

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
