using System.Collections.Generic;
using UnityEngine;

namespace Sof.Auxiliary
{
    public class Palette
    {
        private List<Color> _ColorPool = new List<Color>
        {
            Color.black,
            Color.blue,
            Color.cyan,
            Color.green,
            Color.magenta,
            Color.red,
            Color.yellow
        };

        public Color GetNewRandomColor()
        {
            if (_ColorPool.Count == 0)
                throw new System.InvalidOperationException("Color pool depleted.");

            var color = _ColorPool[Random.Range(0, _ColorPool.Count)];

            _ColorPool.Remove(color);

            return color;
        }
    }
}
