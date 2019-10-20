using UnityEngine;

namespace Sof.Object
{
    public class Tile : MonoBehaviour
    {
        private Model.Tile _Tile;
        private GameManager _GameManager;

        public Unit Unit { get; set; }

        public void Initialize(Model.Tile tile, GameManager gameManager)
        {
            _Tile = tile ?? throw new System.ArgumentNullException(nameof(tile));

            if(gameManager == null)
                throw new System.ArgumentNullException(nameof(gameManager));

            _GameManager = gameManager;
        }

        private void OnMouseEnter() => _GameManager.OnTileHover(this);

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonUp(0))
                _GameManager.OnTileLeftClick(this);
            else if (Input.GetMouseButtonUp(1))
                _GameManager.OnTileRightClick(this);
        }
    }
}