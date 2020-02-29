using Sof.Auxiliary;

namespace Sof.Model.Ground
{
    public class Mountain : Ground
    {
        public override PositiveInt MoveCost => new PositiveInt(6);
        public override float Defence => 0.5f;
    }
}
