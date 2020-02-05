using Sof.Model;
using UnityEngine;

namespace Sof.Object
{
    public class Tile : MonoBehaviour
    {
        private GameManager _GameManager;

        public Model.Tile ModelTile { get; private set; }
        public Unit Unit { get; set; }

        public void Initialize(Model.Tile tile, GameManager gameManager)
        {
            ModelTile = tile ?? throw new System.ArgumentNullException(nameof(tile));

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