using Gjrfytn.Dim.Object;

namespace Sof.Object
{
    public abstract class MapObject : SofMonoBehaviour
    {
        public abstract bool OnHover();
        public abstract bool OnLeftClick();
        public abstract bool OnRightClick();
    }
}
