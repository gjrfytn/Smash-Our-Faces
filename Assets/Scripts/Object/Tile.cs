using UnityEngine;

namespace Sof.Object
{
    public class Tile : MonoBehaviour
    {
        private GameManager _GameManager;
        private MapObject _Object;

        public Model.Tile ModelTile { get; private set; }

        public void Initialize(Model.Tile tile, GameManager gameManager)
        {
            ModelTile = tile ?? throw new System.ArgumentNullException(nameof(tile));

            if (gameManager == null)
                throw new System.ArgumentNullException(nameof(gameManager));

            _GameManager = gameManager;
        }

        public T InstantiateMapObject<T>(T @object) where T : MapObject
        {
            if (_Object != null)
                throw new System.InvalidOperationException("Tile already has object.");

            var objectInstance = Instantiate(@object, transform);

            _Object = objectInstance;

            return objectInstance;
        }

        private void OnMouseEnter()
        {
            if (_Object == null || !_Object.OnHover())
                _GameManager.OnTileHover(this);
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (_Object == null || !_Object.OnLeftClick())
                    _GameManager.OnTileLeftClick(this);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                if (_Object == null || !_Object.OnRightClick())
                    _GameManager.OnTileRightClick(this);
            }
        }
    }
}