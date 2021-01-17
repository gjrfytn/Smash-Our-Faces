using Gjrfytn.Dim;

namespace Sof.Model.Ground
{
    public class Grass : Ground
    {
        public override PositiveInt MoveCost => new PositiveInt(3);
        public override float Defence => 0.25f;
    }
}
