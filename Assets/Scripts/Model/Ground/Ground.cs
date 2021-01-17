using Gjrfytn.Dim;

namespace Sof.Model.Ground
{
    public abstract class Ground
    {
        public abstract PositiveInt MoveCost { get; }
        public abstract float Defence { get; }
    }
}
