using UnityEngine;

namespace Sof.Object
{
    public class Tile : MonoBehaviour
    {
        private Model.Tile _Tile;

        public static event System.Action<Tile> TileLeftClicked;
        public static event System.Action<Tile> TileRightClicked;
        public static event System.Action<Tile> TileHovered;

        public Unit Unit { get; set; }

        public void Initialize(Model.Tile tile)
        {
            _Tile = tile ?? throw new System.ArgumentNullException(nameof(tile));
        }

        private void OnMouseEnter() => TileHovered(this);

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonUp(0))
                TileLeftClicked(this);
            else if (Input.GetMouseButtonUp(1))
                TileRightClicked(this);
        }
    }
}