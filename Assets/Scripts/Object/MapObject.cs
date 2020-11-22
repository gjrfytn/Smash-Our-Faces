using UnityEngine;

namespace Sof.Object
{
    public abstract class MapObject : Auxiliary.SofMonoBehaviour
    {
        public abstract bool OnHover();
        public abstract bool OnLeftClick();
        public abstract bool OnRightClick();
    }
}
