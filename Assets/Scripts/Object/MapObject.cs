﻿using UnityEngine;

namespace Sof.Object
{
    public abstract class MapObject : Auxiliary.SofSceneMonoBehaviour
    {
        public abstract bool OnHover();
        public abstract bool OnLeftClick();
        public abstract bool OnRightClick();
    }
}
