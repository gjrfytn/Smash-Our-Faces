﻿namespace Sof.UI
{
    public class DamageText : FlyingText
    {
        public int Damage
        {
            set => Text = value.ToString();
        }
    }
}
