using Gjrfytn.Dim;

namespace Sof.Model.Ground
{
    public class Water : Ground
    {
        public override PositiveInt MoveCost => new PositiveInt(6);
        public override float Defence => 0;
    }
}
